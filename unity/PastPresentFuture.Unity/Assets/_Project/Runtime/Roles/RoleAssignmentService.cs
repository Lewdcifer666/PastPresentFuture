using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PPF.Roles
{
    /// <summary>
    /// Host-authoritative role assignment.
    /// - Assigns unique roles up to 3 players
    /// - Reassigns cleanly if someone disconnects (optional behavior)
    /// </summary>
    public sealed class RoleAssignmentService : MonoBehaviour
    {
        [Header("Rules")]
        [SerializeField] private bool _enforceUniqueRoles = true;
        [SerializeField] private bool _reassignOnDisconnect = false;

        // Tracks assigned roles per client id on the server.
        private readonly Dictionary<ulong, Role> _assigned = new Dictionary<ulong, Role>();

        private readonly Role[] _rolePool = new[] { Role.Past, Role.Present, Role.Future };

        private void Awake()
        {
            if (NetworkManager.Singleton == null)
            {
                UnityEngine.Debug.LogError("RoleAssignmentService requires an active NetworkManager.");
                return;
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) return;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            AssignRoleToClient(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            _assigned.Remove(clientId);

            if (_reassignOnDisconnect)
            {
                // Optional: reassign roles for remaining players to keep uniqueness.
                ReassignAllConnectedClients();
            }
        }

        private void AssignRoleToClient(ulong clientId)
        {
            // If already assigned (shouldn't happen normally), keep it stable.
            if (_assigned.ContainsKey(clientId))
            {
                ApplyRoleToPlayerObject(clientId, _assigned[clientId]);
                return;
            }

            Role chosen = ChooseRole(clientId);
            _assigned[clientId] = chosen;

            ApplyRoleToPlayerObject(clientId, chosen);

            UnityEngine.Debug.Log($"[ROLE] Assigned {chosen} to client {clientId}");
        }

        private Role ChooseRole(ulong clientId)
        {
            if (!_enforceUniqueRoles)
            {
                // Random role (non-unique allowed) - not used now, but kept for flexibility.
                int idx = Random.Range(0, _rolePool.Length);
                return _rolePool[idx];
            }

            // Choose from roles not yet assigned.
            var used = new HashSet<Role>(_assigned.Values);
            var available = _rolePool.Where(r => !used.Contains(r)).ToList();

            if (available.Count == 0)
            {
                // Fallback: if more than 3 connect (should be rejected earlier), choose any.
                return Role.Past;
            }

            int pick = Random.Range(0, available.Count);
            return available[pick];
        }

        private void ApplyRoleToPlayerObject(ulong clientId, Role role)
        {
            // Player object might not exist immediately; try now and retry next frame if needed.
            if (!TryApplyRoleNow(clientId, role))
            {
                StartCoroutine(ApplyRoleNextFrame(clientId, role));
            }
        }

        private System.Collections.IEnumerator ApplyRoleNextFrame(ulong clientId, Role role)
        {
            // Retry for a short time; player spawn should happen quickly.
            const float timeoutSeconds = 2f;
            float t = 0f;

            while (t < timeoutSeconds)
            {
                if (TryApplyRoleNow(clientId, role))
                    yield break;

                t += Time.deltaTime;
                yield return null;
            }

            UnityEngine.Debug.LogWarning($"[ROLE] Timed out assigning role {role} to client {clientId} (player object not found).");
        }

        private bool TryApplyRoleNow(ulong clientId, Role role)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client) && client.PlayerObject != null)
            {
                var roleState = client.PlayerObject.GetComponent<PlayerRoleState>();
                if (roleState != null)
                {
                    roleState.RoleValue.Value = role;
                    return true;
                }

                UnityEngine.Debug.LogWarning($"[ROLE] PlayerObject for client {clientId} missing PlayerRoleState component.");
                return true; // stop retrying; it's a prefab setup issue
            }

            return false;
        }

        private void ReassignAllConnectedClients()
        {
            _assigned.Clear();

            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                ulong clientId = kvp.Key;
                AssignRoleToClient(clientId);
            }
        }
    }
}
