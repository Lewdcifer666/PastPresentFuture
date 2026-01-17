using Unity.Netcode;
using UnityEngine;
using PPF.Roles;

namespace PPF.Rooms
{
    /// <summary>
    /// Objective that can only be completed by a specific Role.
    /// Server-authoritative:
    /// - Client requests completion
    /// - Server checks the sender's PlayerRoleState.RoleValue
    /// - Server marks objective complete if allowed
    /// </summary>
    public abstract class RoleLockedObjective : RoomObjectiveBase
    {
        [Header("Role Lock")]
        [SerializeField] private Role _requiredRole = Role.Present;

        /// <summary>
        /// We intentionally block the base RequestCompleteServerRpc path.
        /// Use RequestCompleteWithRoleCheckServerRpc instead.
        /// </summary>
        protected override bool CanCompleteFromClient() => false;

        /// <summary>
        /// Call this from client interactions to complete the objective.
        /// Server validates the sender's role.
        /// </summary>
        [Rpc(SendTo.Server)]
        public void RequestCompleteWithRoleCheckServerRpc(RpcParams rpcParams = default)
        {
            if (!IsServer)
                return;

            if (IsComplete.Value)
                return;

            ulong senderClientId = rpcParams.Receive.SenderClientId;

            if (!IsClientAllowed(senderClientId))
                return;

            IsComplete.Value = true;
        }

        private bool IsClientAllowed(ulong clientId)
        {
            PlayerRoleState prs = FindRoleStateForClient(clientId);
            if (prs == null)
                return false;

            return prs.RoleValue.Value == _requiredRole;
        }

        private static PlayerRoleState FindRoleStateForClient(ulong clientId)
        {
#if UNITY_2023_1_OR_NEWER
            var all = Object.FindObjectsByType<PlayerRoleState>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
#else
            // Older Unity versions: includeInactive=true to find disabled objects too
            var all = Object.FindObjectsOfType<PlayerRoleState>(true);
#endif
            for (int i = 0; i < all.Length; i++)
            {
                var prs = all[i];
                if (prs == null || !prs.IsSpawned)
                    continue;

                if (prs.OwnerClientId == clientId)
                    return prs;
            }

            return null;
        }
    }
}
