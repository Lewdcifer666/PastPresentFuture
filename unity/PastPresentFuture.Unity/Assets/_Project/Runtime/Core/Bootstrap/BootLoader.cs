using UnityEngine;
using UnityEngine.SceneManagement;

namespace PPF.Core.Bootstrap
{
    /// <summary>
    /// Boot scene responsibility:
    /// - Ensure a single NetworkManager exists (spawn from prefab)
    /// - Persist it across scene loads
    /// - Load Lobby
    /// </summary>
    public sealed class BootLoader : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string _lobbySceneName = "Lobby";

        [Header("Networking")]
        [Tooltip("Drag PF_NetworkManager prefab here from Assets/_Project/Prefabs/Networking/")]
        [SerializeField] private GameObject _networkManagerPrefab;

        [Tooltip("If true, the NetworkManager prefab is marked DontDestroyOnLoad.")]
        [SerializeField] private bool _persistNetworkManager = true;

        private static bool s_networkSpawned;

        private void Awake()
        {
            EnsureNetworkManager();
            LoadLobby();
        }

        private void EnsureNetworkManager()
        {
            // Prevent duplicates when returning to Boot or domain reload quirks.
            if (s_networkSpawned)
                return;

            if (_networkManagerPrefab == null)
            {
                Debug.LogError(
                    "BootLoader: NetworkManager prefab is not assigned. " +
                    "Assign PF_NetworkManager in the Boot scene inspector.");
                return;
            }

            // Instantiate the prefab.
            GameObject nm = Instantiate(_networkManagerPrefab);
            Debug.Log("BootLoader: Spawned NetworkManager prefab.");
            nm.name = "NET";

            if (_persistNetworkManager)
                DontDestroyOnLoad(nm);

            s_networkSpawned = true;
        }

        private void LoadLobby()
        {
            if (SceneManager.GetActiveScene().name != _lobbySceneName)
                SceneManager.LoadScene(_lobbySceneName, LoadSceneMode.Single);
        }
    }
}
