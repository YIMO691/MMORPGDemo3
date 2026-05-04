using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    public class LoginView : MonoBehaviour
    {
        [SerializeField] private Button loginButton;
        [SerializeField] private Text statusText;

        private NetworkManager _network;
        private GameLauncher _launcher;

        public void Init(NetworkManager network, GameLauncher launcher)
        {
            _network = network;
            _launcher = launcher;
            loginButton.onClick.AddListener(OnLoginClick);
            statusText.text = "Tap Login to begin";
        }

        private void OnLoginClick()
        {
            loginButton.interactable = false;
            statusText.text = "Connecting...";

            StartCoroutine(_network.GuestLogin(
                SystemInfo.deviceUniqueIdentifier,
                Application.isEditor ? "editor" : "windows",
                Application.version,
                OnLoginSuccess,
                OnLoginError
            ));
        }

        private void OnLoginSuccess(LoginResult result)
        {
            statusText.text = $"Welcome!";
            _launcher.OnLoginSuccess();
        }

        private void OnLoginError(string error)
        {
            statusText.text = $"Error: {error}";
            loginButton.interactable = true;
        }
    }
}
