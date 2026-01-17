using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PPF.Rooms
{
    /// <summary>
    /// ROOM-010:
    /// - Owns a set of objectives
    /// - Evaluates completion server-side
    /// - Marks room solved and opens door when all objectives are complete
    ///
    /// Intended to be placed as a scene NetworkObject (server authoritative).
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class RoomManager : NetworkBehaviour
    {
        [Header("Room")]
        [SerializeField] private string _roomId = "ROOM";

        [Tooltip("If empty, will auto-find objectives in children at runtime.")]
        [SerializeField] private List<RoomObjectiveBase> _objectives = new List<RoomObjectiveBase>();

        [Tooltip("Door to open when room is solved.")]
        [SerializeField] private RoomDoor _door;

        [Header("Behavior")]
        [Tooltip("If true, room is considered solved when ALL objectives are complete.")]
        [SerializeField] private bool _requireAllObjectives = true;

        [Tooltip("If false and there are 0 objectives, the room will never auto-solve.")]
        [SerializeField] private bool _autoSolveIfNoObjectives = false;

        [Header("Debug")]
        [SerializeField] private bool _debugLogs = true;

        public string RoomId => _roomId;

        // Server writes; everyone reads.
        public NetworkVariable<bool> IsSolved { get; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                EnsureWired();
                EvaluateAndApplyServer();
            }

            IsSolved.OnValueChanged += OnSolvedChanged;
            OnSolvedChanged(false, IsSolved.Value);
        }

        public override void OnNetworkDespawn()
        {
            IsSolved.OnValueChanged -= OnSolvedChanged;

            if (IsServer)
                UnsubscribeObjectivesServer();
        }

        private void EnsureWired()
        {
            if (_door == null)
                _door = GetComponentInChildren<RoomDoor>(true);

            if (_objectives == null)
                _objectives = new List<RoomObjectiveBase>();

            if (_objectives.Count == 0)
            {
                // Auto-find objectives in children (including inactive)
                var found = GetComponentsInChildren<RoomObjectiveBase>(true);
                _objectives.AddRange(found);
            }

            SubscribeObjectivesServer();
        }

        private void SubscribeObjectivesServer()
        {
            // Server evaluates when any objective changes.
            for (int i = 0; i < _objectives.Count; i++)
            {
                var obj = _objectives[i];
                if (obj == null)
                    continue;

                obj.IsComplete.OnValueChanged += OnObjectiveCompleteChangedServer;
            }
        }

        private void UnsubscribeObjectivesServer()
        {
            for (int i = 0; i < _objectives.Count; i++)
            {
                var obj = _objectives[i];
                if (obj == null)
                    continue;

                obj.IsComplete.OnValueChanged -= OnObjectiveCompleteChangedServer;
            }
        }

        private void OnObjectiveCompleteChangedServer(bool prev, bool now)
        {
            if (!IsServer)
                return;

            EvaluateAndApplyServer();
        }

        private void EvaluateAndApplyServer()
        {
            if (!IsServer)
                return;

            if (IsSolved.Value)
                return;

            int total = 0;
            int complete = 0;

            for (int i = 0; i < _objectives.Count; i++)
            {
                var obj = _objectives[i];
                if (obj == null)
                    continue;

                total++;
                if (obj.IsComplete.Value)
                    complete++;
            }

            bool solved;
            if (total == 0)
            {
                solved = _autoSolveIfNoObjectives;
            }
            else if (_requireAllObjectives)
            {
                solved = (complete == total);
            }
            else
            {
                solved = (complete > 0);
            }

            if (_debugLogs)
                UnityEngine.Debug.Log($"[ROOM] {_roomId} evaluate: {complete}/{total} complete -> solved={solved}");

            if (solved)
                SolveServer();
        }

        private void SolveServer()
        {
            if (!IsServer)
                return;

            if (IsSolved.Value)
                return;

            IsSolved.Value = true;

            if (_door != null)
                _door.OpenServer();

            if (_debugLogs)
                UnityEngine.Debug.Log($"[ROOM] {_roomId} SOLVED. Door opened.");
        }

        private void OnSolvedChanged(bool prev, bool now)
        {
            if (_debugLogs)
                UnityEngine.Debug.Log($"[ROOM] {_roomId} IsSolved changed {prev} -> {now}");

            // Door state is already driven by its own NetworkVariable.
            // But if you ever want additional solved VFX/HUD later, this is the hook.
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Editor-only guardrails (won't run in builds)
            if (_objectives == null)
                _objectives = new List<RoomObjectiveBase>();

            if (_door == null)
                _door = GetComponentInChildren<RoomDoor>(true);
        }
#endif


#if UNITY_EDITOR
        [ContextMenu("Debug/Solve Room (Server Only)")]
        private void DebugSolveRoom()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugSolveRoom only works in Play Mode.");
                return;
            }

            if (!IsServer)
            {
                UnityEngine.Debug.LogWarning("[ROOM] DebugSolveRoom ignored: not server.");
                return;
            }

            SolveServer();
        }
#endif
    }
}
