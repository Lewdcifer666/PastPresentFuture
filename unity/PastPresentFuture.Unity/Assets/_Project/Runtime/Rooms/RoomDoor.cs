using Unity.Netcode;
using UnityEngine;

namespace PPF.Rooms
{
    /// <summary>
    /// Networked door that opens/closes when the server sets IsOpen.
    /// Visual motion is driven locally on each client from replicated state.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class RoomDoor : NetworkBehaviour
    {
        [Header("Door Transform")]
        [Tooltip("If null, uses this transform.")]
        [SerializeField] private Transform _doorTransform;

        [Header("Open Pose (Local Space)")]
        [Tooltip("Local position offset applied when door is open.")]
        [SerializeField] private Vector3 _openLocalPositionOffset = new Vector3(0f, 2f, 0f);

        [Tooltip("Local rotation offset (Euler) applied when door is open.")]
        [SerializeField] private Vector3 _openLocalEulerOffset = new Vector3(0f, 90f, 0f);

        [Header("Animation")]
        [Tooltip("Seconds to fully open/close.")]
        [SerializeField] private float _openDurationSeconds = 0.5f;

        [Tooltip("If true, snap instantly instead of animating.")]
        [SerializeField] private bool _snapInstantly = false;

        [Header("Debug")]
        [SerializeField] private bool _debugLogs = false;

        public NetworkVariable<bool> IsOpen { get; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private Vector3 _closedLocalPos;
        private Quaternion _closedLocalRot;

        private Vector3 _openLocalPos;
        private Quaternion _openLocalRot;

        private float _t; // 0 closed, 1 open

        private void Awake()
        {
            if (_doorTransform == null)
                _doorTransform = transform;

            _closedLocalPos = _doorTransform.localPosition;
            _closedLocalRot = _doorTransform.localRotation;

            _openLocalPos = _closedLocalPos + _openLocalPositionOffset;
            _openLocalRot = _closedLocalRot * Quaternion.Euler(_openLocalEulerOffset);
        }

        public override void OnNetworkSpawn()
        {
            IsOpen.OnValueChanged += OnOpenChanged;
            OnOpenChanged(false, IsOpen.Value);
        }

        public override void OnNetworkDespawn()
        {
            IsOpen.OnValueChanged -= OnOpenChanged;
        }

        private void Update()
        {
            if (!IsSpawned)
                return;

            if (_snapInstantly)
                return;

            float target = IsOpen.Value ? 1f : 0f;

            if (_openDurationSeconds <= 0.0001f)
            {
                _t = target;
            }
            else
            {
                float speed = 1f / _openDurationSeconds;
                _t = Mathf.MoveTowards(_t, target, speed * Time.deltaTime);
            }

            ApplyPose(_t);
        }

        private void OnOpenChanged(bool prev, bool now)
        {
            if (_debugLogs)
                UnityEngine.Debug.Log($"[ROOM][DOOR] IsOpen changed {prev} -> {now} on {name}");

            if (_snapInstantly)
            {
                _t = now ? 1f : 0f;
                ApplyPose(_t);
                return;
            }

            // If we just spawned, initialize t to current state to avoid visible "travel from default"
            if (Mathf.Approximately(_t, 0f) && now)
                _t = 1f;
            else if (Mathf.Approximately(_t, 1f) && !now)
                _t = 0f;

            ApplyPose(_t);
        }

        private void ApplyPose(float t01)
        {
            if (_doorTransform == null)
                return;

            _doorTransform.localPosition = Vector3.Lerp(_closedLocalPos, _openLocalPos, t01);
            _doorTransform.localRotation = Quaternion.Slerp(_closedLocalRot, _openLocalRot, t01);
        }

        /// <summary>
        /// Server-only open.
        /// </summary>
        public void OpenServer()
        {
            if (!IsServer)
                return;

            IsOpen.Value = true;
        }

        /// <summary>
        /// Server-only close.
        /// </summary>
        public void CloseServer()
        {
            if (!IsServer)
                return;

            IsOpen.Value = false;
        }
    }
}
