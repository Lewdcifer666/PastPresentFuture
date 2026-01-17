using Unity.Netcode;
using UnityEngine;

namespace PPF.Networking.Netcode
{
    /// <summary>
    /// Host-authoritative connection approval.
    /// Enforces player limit and can later enforce role assignment rules.
    /// </summary>
    public sealed class ConnectionApprovalService : MonoBehaviour
    {
        [Header("Rules")]
        [SerializeField] private int _maxPlayers = 3;

        private void Awake()
        {
            // This component is expected to live on the NetworkManager GameObject.
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("ConnectionApprovalService requires an active NetworkManager.");
                return;
            }

            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            }
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // ConnectedClientsList includes host if hosting.
            int currentPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;

            // Approve if room available
            bool hasRoom = currentPlayers < _maxPlayers;

            response.Approved = hasRoom;
            response.CreatePlayerObject = hasRoom;
            response.PlayerPrefabHash = null; // use NetworkManager's PlayerPrefab
            response.Pending = false;

            if (!hasRoom)
            {
                response.Reason = $"Room is full (max {_maxPlayers}).";
            }
        }
    }
}
