using Unity.Netcode;
using UnityEngine;

namespace PPF.Networking.Netcode
{
    /// <summary>
    /// Minimal network player used for NET-010 validation.
    /// Later this becomes your role-based player controller with per-role rendering.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class SimpleNetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private void Reset()
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        public override void OnNetworkSpawn()
        {
            // Visual feedback so it's obvious which object belongs to the local client.
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer != null)
            {
                // Avoid specifying exact colors as a "style". We'll just toggle material property lightly.
                // Local player: brighten emission a bit; remote: leave default.
                var mat = _renderer.material;
                if (IsOwner)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", mat.color * 0.5f);
                    }
                    transform.name = $"Player(Local) [{OwnerClientId}]";
                }
                else
                {
                    transform.name = $"Player(Remote) [{OwnerClientId}]";
                }
            }
        }
    }
}
