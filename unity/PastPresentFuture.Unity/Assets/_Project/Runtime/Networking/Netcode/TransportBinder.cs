using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace PPF.Networking.Netcode
{
    /// <summary>
    /// Unity 6 + NGO can sometimes fail to auto-bind a transport in-editor/runtime,
    /// especially when NetworkManager is spawned from a prefab.
    ///
    /// This component guarantees a transport is assigned.
    /// Attach it to the same GameObject as NetworkManager + UnityTransport.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public sealed class TransportBinder : MonoBehaviour
    {
        private void Awake()
        {
            var nm = GetComponent<NetworkManager>();
            if (nm == null)
            {
                Debug.LogError("TransportBinder: NetworkManager missing.");
                return;
            }

            // If already set, do nothing.
            if (nm.NetworkConfig != null && nm.NetworkConfig.NetworkTransport != null)
                return;

            var utp = GetComponent<UnityTransport>();
            if (utp == null)
            {
                Debug.LogError("TransportBinder: UnityTransport missing. Add UnityTransport to the NetworkManager prefab.");
                return;
            }

            nm.NetworkConfig.NetworkTransport = utp;
            Debug.Log("TransportBinder: Bound UnityTransport to NetworkManager.NetworkConfig.NetworkTransport.");
        }
    }
}
