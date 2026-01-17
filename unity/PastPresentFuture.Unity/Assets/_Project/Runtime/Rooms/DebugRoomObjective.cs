using Unity.Netcode;
using UnityEngine;

namespace PPF.Rooms
{
    /// <summary>
    /// Simple test objective:
    /// - Can be completed by calling RequestCompleteServerRpc (from a client)
    /// - Or via context menu on server (in inspector)
    /// </summary>
    public sealed class DebugRoomObjective : RoomObjectiveBase
    {
        [Header("Debug")]
        [SerializeField] private bool _requireHostOnly = true;

        protected override bool CanCompleteFromClient()
        {
            if (!_requireHostOnly)
                return true;

            // Only host/server can complete when this is enabled.
            // (Because RequestCompleteServerRpc arrives on server, this check always holds.)
            return true;
        }

#if UNITY_EDITOR
        [ContextMenu("Debug/Request Complete (Client->Server RPC)")]
        private void DebugRequestComplete()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugRequestComplete only works in Play Mode.");
                return;
            }

            RequestCompleteServerRpc();
        }
#endif
    }
}
