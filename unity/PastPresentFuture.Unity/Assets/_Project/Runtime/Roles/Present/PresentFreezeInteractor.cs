using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using PPF.Roles;

namespace PPF.Roles.Present
{
    /// <summary>
    /// FX-010: Present can freeze/unfreeze TimeFreezable objects.
    /// - Uses new Input System
    /// - Raycasts from camera forward
    /// - Ignores all Player-layer colliders
    /// </summary>
    public sealed class PresentFreezeInteractor : NetworkBehaviour
    {
        [Header("Input (New Input System)")]
        [SerializeField] private Key _toggleFreezeKey = Key.F;

        [Header("Raycast")]
        [SerializeField] private float _range = 25f;

        [Tooltip("Layers to include in raycast. If left as Everything, we will still auto-exclude Player layer.")]
        [SerializeField] private LayerMask _raycastMask = ~0;

        [Header("Camera (optional)")]
        [SerializeField] private Camera _cameraOverride;

        [Header("Debug")]
        [SerializeField] private bool _debugLogs = true;
        [SerializeField] private bool _debugDrawRay = true;

        private PlayerRoleState _roleState;
        private Camera _cam;

        private int _playerLayerMask;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            _roleState = GetComponent<PlayerRoleState>();
            ResolveCamera();

            int playerLayer = LayerMask.NameToLayer("Player");
            _playerLayerMask = (playerLayer >= 0) ? (1 << playerLayer) : 0;

            if (_debugLogs && playerLayer < 0)
                UnityEngine.Debug.LogWarning("[FREEZE] Layer 'Player' not found. Raycast may hit player colliders.");
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
                return;

            if (_roleState == null || _roleState.RoleValue.Value != Role.Present)
                return;

            if (_cam == null)
            {
                ResolveCamera();
                if (_cam == null)
                {
                    if (_debugLogs)
                        UnityEngine.Debug.LogWarning("[FREEZE] No camera found. Tag your camera as MainCamera or assign Camera Override.");
                    return;
                }
            }

            if (_debugDrawRay)
                Debug.DrawRay(_cam.transform.position, _cam.transform.forward * _range, Color.yellow);

            if (Keyboard.current != null && Keyboard.current[_toggleFreezeKey].wasPressedThisFrame)
            {
                TryToggleFreeze();
            }
        }

        private void ResolveCamera()
        {
            if (_cameraOverride != null)
            {
                _cam = _cameraOverride;
                return;
            }

            _cam = Camera.main;
            if (_cam != null)
                return;

            _cam = FindFirstObjectByType<Camera>();
        }

        private void TryToggleFreeze()
        {
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);

            // Exclude Player layer automatically
            int mask = _raycastMask;
            if (_playerLayerMask != 0)
                mask &= ~_playerLayerMask;

            if (!Physics.Raycast(ray, out RaycastHit hit, _range, mask, QueryTriggerInteraction.Ignore))
            {
                if (_debugLogs)
                    UnityEngine.Debug.Log("[FREEZE] Raycast hit nothing. Aim at the cube and try again.");
                return;
            }

            if (_debugLogs)
                UnityEngine.Debug.Log($"[FREEZE] Raycast hit: {hit.collider.name}");

            var freezable = hit.collider.GetComponentInParent<TimeFreezable>();
            if (freezable == null)
            {
                if (_debugLogs)
                    UnityEngine.Debug.Log("[FREEZE] Hit object is not TimeFreezable.");
                return;
            }

            bool next = !freezable.IsFrozen.Value;
            freezable.SetFrozenServerRpc(next);

            if (_debugLogs)
                UnityEngine.Debug.Log($"[FREEZE] Request toggle -> {next} on {freezable.name}");
        }
    }
}
