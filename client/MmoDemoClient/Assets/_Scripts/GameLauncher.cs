using System.Collections;
using UnityEngine;

namespace MmoDemo.Client
{
    /// <summary>
    /// Application entry point. Initializes network, UI, and Lua VM,
    /// then opens the Login screen.
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private string serverBaseUrl = "http://localhost:5000";
        [SerializeField] private GameObject loginViewPrefab;
        [SerializeField] private GameObject roleSelectViewPrefab;
        [SerializeField] private GameObject cityViewPrefab;

        private NetworkManager _network;
        private LuaManager _lua;
        private UIManager _ui;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _network = new NetworkManager(serverBaseUrl);
            _lua = new LuaManager();
            _ui = new UIManager(loginViewPrefab, roleSelectViewPrefab, cityViewPrefab, _network, this);

            _lua.RegisterBridge("network", _network);
            _lua.RegisterBridge("ui", _ui);
        }

        private IEnumerator Start()
        {
            _lua.Start();

            bool healthOk = false;
            yield return _network.CheckHealth(ok => healthOk = ok);

            if (healthOk)
                _ui.ShowLogin();
            else
                Debug.LogError("[Launcher] Cannot reach server at " + serverBaseUrl);
        }

        public void OnLoginSuccess()
        {
            _ui.ShowRoleSelect();
        }

        public void OnRoleSelected(string roleId)
        {
            _ui.ShowCity(roleId);
        }

        private void OnDestroy()
        {
            _lua?.Dispose();
        }
    }
}
