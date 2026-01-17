using Unity.Netcode;
using UnityEngine;

namespace PPF.Roles
{
    /// <summary>
    /// Network-synchronized role state owned by the server.
    /// Attached to the networked player object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class PlayerRoleState : NetworkBehaviour
    {
        // Server writes; everyone reads.
        public NetworkVariable<Role> RoleValue { get; } = new NetworkVariable<Role>(
            Role.None,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public override void OnNetworkSpawn()
        {
            // Optional: Debug to verify role arrives.
            RoleValue.OnValueChanged += OnRoleChanged;

            if (IsSpawned)
            {
                // Trigger initial display in case it already has a value.
                OnRoleChanged(Role.None, RoleValue.Value);
            }
        }

        public override void OnNetworkDespawn()
        {
            RoleValue.OnValueChanged -= OnRoleChanged;
        }

        private void OnRoleChanged(Role previous, Role current)
        {
            if (IsOwner)
            {
                UnityEngine.Debug.Log($"[ROLE] Local role updated: {current}");
            }
        }
    }
}
