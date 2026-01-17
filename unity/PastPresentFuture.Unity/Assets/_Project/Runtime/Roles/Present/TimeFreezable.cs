using Unity.Netcode;
using UnityEngine;

namespace PPF.Roles.Present
{
    /// <summary>
    /// A networked object that can be frozen/unfrozen by the Present role.
    /// Server-authoritative: server sets IsFrozen.
    ///
    /// FX-020 addition:
    /// - Optional visual feedback by swapping materials while frozen.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class TimeFreezable : NetworkBehaviour
    {
        [Header("Visual Feedback (optional)")]
        [Tooltip("Renderer whose materials will swap when frozen. If null, auto-finds in children.")]
        [SerializeField] private Renderer _renderer;

        [Tooltip("Material applied while frozen. If null, visuals won't change.")]
        [SerializeField] private Material _frozenMaterial;

        public NetworkVariable<bool> IsFrozen { get; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private Rigidbody _rb;

        // Physics state
        private Vector3 _savedVelocity;
        private Vector3 _savedAngularVelocity;
        private bool _savedIsKinematic;
        private bool _cachedOriginalState;

        // Visual state
        private Material[] _originalMaterials;

        private void Awake()
        {
            // Rigidbody on same object or in children (prefab setups vary)
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
                _rb = GetComponentInChildren<Rigidbody>(true);

            // Renderer on same object or in children
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>(true);

            CacheOriginalVisuals();
        }

        public override void OnNetworkSpawn()
        {
            IsFrozen.OnValueChanged += OnFrozenChanged;
            OnFrozenChanged(false, IsFrozen.Value);
        }

        public override void OnNetworkDespawn()
        {
            IsFrozen.OnValueChanged -= OnFrozenChanged;
        }

        private void CacheOriginalVisuals()
        {
            if (_renderer != null)
                _originalMaterials = _renderer.materials;
        }

        private void OnFrozenChanged(bool prev, bool now)
        {
            ApplyFreezeState(now);
            ApplyVisualState(now);
        }

        private void ApplyFreezeState(bool frozen)
        {
            if (_rb == null)
                return;

            // Cache original state once (first time we ever freeze/unfreeze),
            // so we restore correctly for objects that start kinematic.
            if (!_cachedOriginalState)
            {
                _savedIsKinematic = _rb.isKinematic;
                _cachedOriginalState = true;
            }

            if (frozen)
            {
                _savedVelocity = _rb.linearVelocity;
                _savedAngularVelocity = _rb.angularVelocity;

                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.isKinematic = true;
            }
            else
            {
                _rb.isKinematic = _savedIsKinematic;

                // Only restore velocity if it will simulate again.
                // (If original was kinematic, restoring velocities is meaningless.)
                if (!_rb.isKinematic)
                {
                    _rb.linearVelocity = _savedVelocity;
                    _rb.angularVelocity = _savedAngularVelocity;
                }
            }
        }

        private void ApplyVisualState(bool frozen)
        {
            if (_renderer == null)
                return;

            if (_frozenMaterial == null)
                return;

            if (_originalMaterials == null || _originalMaterials.Length == 0)
                _originalMaterials = _renderer.materials;

            if (frozen)
            {
                // Swap all slots to frozen material
                var mats = _renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = _frozenMaterial;
                _renderer.materials = mats;
            }
            else
            {
                // Restore original materials
                if (_originalMaterials != null && _originalMaterials.Length > 0)
                    _renderer.materials = _originalMaterials;
            }
        }

        // New NGO style: explicitly allow any client to send this RPC to the server.
        [Rpc(SendTo.Server)]
        public void SetFrozenServerRpc(bool frozen)
        {
            if (!IsServer)
                return;

            UnityEngine.Debug.Log($"[FREEZE][SERVER] SetFrozenServerRpc({frozen}) on {name}");
            IsFrozen.Value = frozen;
        }
    }
}
