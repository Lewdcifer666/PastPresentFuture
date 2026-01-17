using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace PPF.Debug
{
    /// <summary>
    /// DEV ONLY: Press F5 on the Host/Server to load the gameplay scene for everyone.
    /// Uses the NEW Input System safely (no Key indexing).
    /// </summary>
    public sealed class DevSceneLoader : MonoBehaviour
    {
        [SerializeField] private string _gameplaySceneName = "Room_001";

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            var nm = NetworkManager.Singleton;
            if (nm == null)
                return;

            if (!nm.IsServer)
                return;

            var keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard.f5Key != null && keyboard.f5Key.wasPressedThisFrame)
            {
                UnityEngine.Debug.Log($"[DEV] Loading scene '{_gameplaySceneName}' for all clients...");
                nm.SceneManager.LoadScene(_gameplaySceneName, LoadSceneMode.Single);
            }
        }
    }
}
