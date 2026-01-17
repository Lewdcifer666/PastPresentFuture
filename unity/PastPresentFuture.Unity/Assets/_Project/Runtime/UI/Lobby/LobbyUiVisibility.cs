using Unity.Netcode;
using UnityEngine;

namespace PPF.UI.Lobby
{
    /// <summary>
    /// Simple lobby UI visibility controller:
    /// - Shows lobby buttons when not connected
    /// - Hides them once we are Host/Client
    /// </summary>
    public sealed class LobbyUiVisibility : MonoBehaviour
    {
        [Header("Assign the UI root that contains Host/Join/Leave buttons")]
        [SerializeField] private GameObject _buttonsRoot;

        [Header("Optional: keep visible even after connect (debug labels etc.)")]
        [SerializeField] private bool _keepVisibleWhenConnected = false;

        private void Awake()
        {
            if (_buttonsRoot == null)
            {
                UnityEngine.Debug.LogError("LobbyUiVisibility: Buttons Root is not assigned.");
            }
        }

        private void Update()
        {
            bool connected = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

            if (_buttonsRoot != null)
            {
                // If keepVisibleWhenConnected is true, leave it on.
                _buttonsRoot.SetActive(!connected || _keepVisibleWhenConnected);
            }
        }
    }
}
