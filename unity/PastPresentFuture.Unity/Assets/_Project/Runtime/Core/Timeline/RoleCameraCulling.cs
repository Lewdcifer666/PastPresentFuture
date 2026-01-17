using UnityEngine;
using Unity.Netcode;
using PPF.Roles;

namespace PPF.Core.Timeline
{
    /// <summary>
    /// Sets camera culling mask based on local player's role.
    /// Past sees only proxy layer, others see only real layer.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class RoleCameraCulling : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private string _realLayerName = "WorldReal";
        [SerializeField] private string _proxyLayerName = "WorldProxy";

        private Camera _cam;
        private int _realLayer;
        private int _proxyLayer;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _realLayer = LayerMask.NameToLayer(_realLayerName);
            _proxyLayer = LayerMask.NameToLayer(_proxyLayerName);

            if (_realLayer < 0 || _proxyLayer < 0)
            {
                UnityEngine.Debug.LogError("RoleCameraCulling: Missing layers. Create 'WorldReal' and 'WorldProxy' in Project Settings.");
            }
        }

        private void LateUpdate()
        {
            // If not connected yet, show real world (Lobby UI will still show because UI is separate layer).
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                SetMaskRealOnly();
                return;
            }

            var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (localPlayer == null)
            {
                SetMaskRealOnly();
                return;
            }

            var roleState = localPlayer.GetComponent<PlayerRoleState>();
            if (roleState == null)
            {
                SetMaskRealOnly();
                return;
            }

            Role role = roleState.RoleValue.Value;
            if (role == Role.Past)
                SetMaskProxyOnly();
            else
                SetMaskRealOnly();
        }

        private void SetMaskRealOnly()
        {
            // Render only the real layer + UI layers (UI is normally separate and rendered by Canvas).
            _cam.cullingMask = (1 << _realLayer) | (1 << LayerMask.NameToLayer("UI"));
        }

        private void SetMaskProxyOnly()
        {
            _cam.cullingMask = (1 << _proxyLayer) | (1 << LayerMask.NameToLayer("UI"));
        }
    }
}
