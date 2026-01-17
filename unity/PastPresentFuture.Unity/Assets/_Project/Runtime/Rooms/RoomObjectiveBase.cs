using Unity.Netcode;
using UnityEngine;

namespace PPF.Rooms
{
    /// <summary>
    /// Base class for room objectives.
    /// Server-authoritative completion:
    /// - Server writes IsComplete
    /// - Everyone reads IsComplete
    ///
    /// Derived objectives can complete themselves on the server, or allow clients to request completion.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public abstract class RoomObjectiveBase : NetworkBehaviour
    {
        [Header("Objective")]
        [SerializeField] private string _objectiveId = "OBJ";
        [SerializeField] private string _displayName = "Objective";

        public string ObjectiveId => _objectiveId;
        public string DisplayName => _displayName;

        public NetworkVariable<bool> IsComplete { get; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public override void OnNetworkSpawn()
        {
            IsComplete.OnValueChanged += OnCompleteChanged;
            OnCompleteChanged(false, IsComplete.Value);
        }

        public override void OnNetworkDespawn()
        {
            IsComplete.OnValueChanged -= OnCompleteChanged;
        }

        private void OnCompleteChanged(bool prev, bool now)
        {
            OnObjectiveCompletionChanged(prev, now);
        }

        /// <summary>
        /// Optional hook for derived classes to react to completion changes (VFX, SFX, etc.).
        /// Runs on all clients (and server) because it is driven by the replicated NetworkVariable.
        /// </summary>
        protected virtual void OnObjectiveCompletionChanged(bool previous, bool current) { }

        /// <summary>
        /// Server-only: mark this objective complete/incomplete.
        /// </summary>
        protected void SetCompleteServer(bool complete)
        {
            if (!IsServer)
                return;

            IsComplete.Value = complete;
        }

        /// <summary>
        /// Client -> Server request to complete this objective.
        /// You can call this from a local interaction script, trigger, etc.
        /// Server decides whether to accept in CanCompleteFromClient.
        /// </summary>
        [Rpc(SendTo.Server)]
        public void RequestCompleteServerRpc()
        {
            if (!IsServer)
                return;

            if (IsComplete.Value)
                return;

            if (!CanCompleteFromClient())
                return;

            IsComplete.Value = true;
        }

        /// <summary>
        /// Override to restrict when a client request is accepted.
        /// Default: accept.
        /// </summary>
        protected virtual bool CanCompleteFromClient() => true;

#if UNITY_EDITOR
        [ContextMenu("Debug/Mark Complete (Server Only)")]
        private void DebugMarkComplete()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugMarkComplete only works in Play Mode.");
                return;
            }

            if (!IsServer)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugMarkComplete ignored: not server.");
                return;
            }

            IsComplete.Value = true;
        }

        [ContextMenu("Debug/Reset Complete (Server Only)")]
        private void DebugResetComplete()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugResetComplete only works in Play Mode.");
                return;
            }

            if (!IsServer)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugResetComplete ignored: not server.");
                return;
            }

            IsComplete.Value = false;
        }
#endif
    }
}
