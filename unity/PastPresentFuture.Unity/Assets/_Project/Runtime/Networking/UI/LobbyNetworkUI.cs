using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace PPF.Networking.UI
{
    /// <summary>
    /// Minimal Host/Join/Leave UI for NET-010.
    /// - Host starts NGO host
    /// - Join(Local) connects to 127.0.0.1
    /// - Leave disconnects
    /// </summary>
    public sealed class LobbyNetworkUI : MonoBehaviour
    {
        [Header("Optional Status Text")]
        [SerializeField] private TMP_Text _statusText;

        [Header("Local Join Settings")]
        [SerializeField] private string _localAddress = "127.0.0.1";
        [SerializeField] private ushort _port = 7777;

        private void Update()
        {
            if (_statusText == null || NetworkManager.Singleton == null)
                return;

            if (NetworkManager.Singleton.IsHost)
                _statusText.text = $"Status: HOST (Clients: {NetworkManager.Singleton.ConnectedClientsList.Count})";
            else if (NetworkManager.Singleton.IsClient)
                _statusText.text = $"Status: CLIENT (ClientId: {NetworkManager.Singleton.LocalClientId})";
            else
                _statusText.text = "Status: DISCONNECTED";
        }

        public void Host()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                UnityEngine.Debug.LogError("No NetworkManager.Singleton found. Ensure PF_NetworkManager is in the scene.");
                return;
            }

            ApplyLocalTransportSettingsIfPossible();

            if (nm.IsListening)
            {
                UnityEngine.Debug.Log("Already running.");
                return;
            }

            bool ok = nm.StartHost();
            UnityEngine.Debug.Log(ok ? "Host started." : "Failed to start host.");
        }

        public void JoinLocal()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                UnityEngine.Debug.LogError("No NetworkManager.Singleton found. Ensure PF_NetworkManager is in the scene.");
                return;
            }

            ApplyLocalTransportSettingsIfPossible();

            if (nm.IsListening)
            {
                UnityEngine.Debug.Log("Already running.");
                return;
            }

            bool ok = nm.StartClient();
            UnityEngine.Debug.Log(ok ? "Client started (joining local)." : "Failed to start client.");
        }

        public void Leave()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
                return;

            if (!nm.IsListening)
                return;

            nm.Shutdown();
            UnityEngine.Debug.Log("Network shutdown.");
        }

        private void ApplyLocalTransportSettingsIfPossible()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
                return;

            // Configure UnityTransport for local testing.
            var utp = nm.GetComponent<UnityTransport>();
            if (utp != null)
            {
                utp.SetConnectionData(_localAddress, _port);
            }
        }
    }
}
