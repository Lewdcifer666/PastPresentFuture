using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PPF.Roles;

namespace PPF.Core.Timeline
{
    /// <summary>
    /// Creates and updates visual-only proxies for TimelineTrackable objects.
    /// Only active for the local player when role == Past.
    /// </summary>
    public sealed class PastViewProxyService : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private string _realLayerName = "WorldReal";
        [SerializeField] private string _proxyLayerName = "WorldProxy";

        [Header("Proxy Settings")]
        [Tooltip("If true, proxies are created for all trackables as they appear.")]
        [SerializeField] private bool _autoCreateProxies = true;

        [Tooltip("If true, proxy objects are hidden (inactive) when not Past.")]
        [SerializeField] private bool _disableWhenNotPast = true;

        private int _realLayer;
        private int _proxyLayer;

        private TimelineService _timeline;

        // Maps trackables to their proxy GameObject
        private readonly Dictionary<TimelineTrackable, GameObject> _proxies = new();

        private bool _isPastActive;

        private void Awake()
        {
            _realLayer = LayerMask.NameToLayer(_realLayerName);
            _proxyLayer = LayerMask.NameToLayer(_proxyLayerName);

            if (_realLayer < 0 || _proxyLayer < 0)
            {
                UnityEngine.Debug.LogError("PastViewProxyService: Missing layers. Create 'WorldReal' and 'WorldProxy'.");
            }

            _timeline = FindFirstObjectByType<TimelineService>();
            if (_timeline == null)
            {
                UnityEngine.Debug.LogError("PastViewProxyService: TimelineService not found.");
            }
        }

        private void Update()
        {
            bool shouldBePast = IsLocalRolePast();

            if (shouldBePast != _isPastActive)
            {
                _isPastActive = shouldBePast;
                OnPastActiveChanged(_isPastActive);
            }

            if (!_isPastActive)
                return;

            if (_timeline == null)
                return;

            // Ensure proxies exist for trackables (supports spawned objects).
            if (_autoCreateProxies)
            {
                EnsureProxiesForAllTrackables();
            }

            // Update proxy transforms to past time
            float now = Time.time;
            float targetTime = _timeline.GetPastViewTime(now);

            foreach (var kv in _proxies)
            {
                var trackable = kv.Key;
                var proxy = kv.Value;

                if (trackable == null || proxy == null)
                    continue;

                if (_timeline.TryGetSnapshot(trackable, targetTime, out var snap))
                {
                    // Smooth update: Lerp toward snapshot to avoid jitter.
                    proxy.transform.position = Vector3.Lerp(proxy.transform.position, snap.Position, 0.5f);
                    proxy.transform.rotation = Quaternion.Slerp(proxy.transform.rotation, snap.Rotation, 0.5f);
                }
            }
        }

        private void OnPastActiveChanged(bool active)
        {
            if (!_disableWhenNotPast)
                return;

            foreach (var kv in _proxies)
            {
                if (kv.Value != null)
                    kv.Value.SetActive(active);
            }
        }

        private bool IsLocalRolePast()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
                return false;

            var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (localPlayer == null)
                return false;

            var roleState = localPlayer.GetComponent<PlayerRoleState>();
            if (roleState == null)
                return false;

            return roleState.RoleValue.Value == Role.Past;
        }

        private void EnsureProxiesForAllTrackables()
        {
            var all = FindObjectsByType<TimelineTrackable>(FindObjectsSortMode.None);
            foreach (var t in all)
            {
                if (t == null)
                    continue;

                // Only proxy real objects (avoid proxying proxies)
                if (t.gameObject.layer == _proxyLayer)
                    continue;

                if (!_proxies.ContainsKey(t))
                {
                    CreateProxyFor(t);
                }
            }
        }

        private void CreateProxyFor(TimelineTrackable trackable)
        {
            // Clone the object visually, then strip components we don't want.
            GameObject proxy = Instantiate(trackable.gameObject);
            proxy.name = $"{trackable.gameObject.name}_PROXY";

            // Put proxy on proxy layer recursively.
            SetLayerRecursively(proxy, _proxyLayer);

            // Strip everything that can interfere (networking/physics/scripts).
            StripUnwantedComponents(proxy);

            // Ensure active state matches role
            if (_disableWhenNotPast)
                proxy.SetActive(_isPastActive);

            _proxies.Add(trackable, proxy);
        }

        private void StripUnwantedComponents(GameObject root)
        {
            // Remove all NetworkBehaviours + NetworkObject
            foreach (var nb in root.GetComponentsInChildren<Unity.Netcode.NetworkBehaviour>(true))
            {
                Destroy(nb);
            }

            foreach (var no in root.GetComponentsInChildren<Unity.Netcode.NetworkObject>(true))
            {
                Destroy(no);
            }

            // Remove colliders and rigidbodies (visual-only)
            foreach (var rb in root.GetComponentsInChildren<Rigidbody>(true))
            {
                Destroy(rb);
            }

            foreach (var col in root.GetComponentsInChildren<Collider>(true))
            {
                Destroy(col);
            }

            // Remove AudioSources (optional)
            foreach (var audio in root.GetComponentsInChildren<AudioSource>(true))
            {
                Destroy(audio);
            }

            // Remove TimelineTrackable on proxy to avoid proxy-of-proxy recursion
            foreach (var tt in root.GetComponentsInChildren<TimelineTrackable>(true))
            {
                Destroy(tt);
            }

            // Remove any scripts you don't want on proxies (conservative approach):
            // We remove all MonoBehaviours except those needed for rendering (none).
            foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                // We already handled some specific types above; this catches the rest.
                Destroy(mb);
            }
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
