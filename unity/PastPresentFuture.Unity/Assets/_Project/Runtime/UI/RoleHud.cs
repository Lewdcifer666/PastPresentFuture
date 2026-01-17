using TMPro;
using Unity.Netcode;
using UnityEngine;
using PPF.Roles;

namespace PPF.UI
{
    /// <summary>
    /// Displays the local player's assigned role.
    /// </summary>
    public sealed class RoleHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roleText;

        private PlayerRoleState _localRoleState;

        private void Awake()
        {
            if (_roleText == null)
            {
                Debug.LogError("RoleHud: Role text is not assigned.");
            }
        }

        private void Update()
        {
            // Wait until networking is active and local player exists.
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                SetText("Role: (not connected)");
                return;
            }

            if (_localRoleState == null)
            {
                TryBindLocalPlayer();
                SetText("Role: (connecting...)");
                return;
            }

            SetText($"Role: {_localRoleState.RoleValue.Value}");
        }

        private void TryBindLocalPlayer()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null) return;

            var localPlayer = nm.LocalClient?.PlayerObject;
            if (localPlayer == null) return;

            _localRoleState = localPlayer.GetComponent<PlayerRoleState>();
        }

        private void SetText(string value)
        {
            if (_roleText != null && _roleText.text != value)
                _roleText.text = value;
        }
    }
}
