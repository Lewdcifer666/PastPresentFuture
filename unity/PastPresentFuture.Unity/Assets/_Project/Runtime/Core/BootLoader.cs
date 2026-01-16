using UnityEngine;
using UnityEngine.SceneManagement;

namespace PastPresentFuture.Core
{
    /// <summary>
    /// Automatically loads the Lobby scene when Boot scene starts.
    /// Attach this to an empty GameObject in the Boot scene.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private string _lobbySceneName = "Lobby";
        [SerializeField] private float _delaySeconds = 0f;

        private void Start()
        {
            if (_delaySeconds > 0f)
            {
                Invoke(nameof(LoadLobby), _delaySeconds);
            }
            else
            {
                LoadLobby();
            }
        }

        private void LoadLobby()
        {
            Debug.Log($"[BootLoader] Loading scene: {_lobbySceneName}");
            SceneManager.LoadScene(_lobbySceneName);
        }
    }
}