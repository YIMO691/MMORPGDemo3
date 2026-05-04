using UnityEngine;

namespace MmoDemo.Client
{
    /// <summary>
    /// Simple UI stack for Phase 1. Full-screen views only.
    /// </summary>
    public class UIManager
    {
        private readonly GameObject _loginPrefab;
        private readonly GameObject _roleSelectPrefab;
        private readonly GameObject _cityPrefab;
        private readonly NetworkManager _network;
        private readonly GameLauncher _launcher;

        private GameObject _currentView;

        public UIManager(GameObject loginPrefab, GameObject roleSelectPrefab, GameObject cityPrefab,
            NetworkManager network, GameLauncher launcher)
        {
            _loginPrefab = loginPrefab;
            _roleSelectPrefab = roleSelectPrefab;
            _cityPrefab = cityPrefab;
            _network = network;
            _launcher = launcher;
        }

        public void ShowLogin()
        {
            SwitchView(_loginPrefab, go =>
            {
                var view = go.GetComponent<LoginView>();
                if (view != null) view.Init(_network, _launcher);
            });
        }

        public void ShowRoleSelect()
        {
            SwitchView(_roleSelectPrefab, go =>
            {
                var view = go.GetComponent<RoleSelectView>();
                if (view != null) view.Init(_network);
            });
        }

        public void ShowCity(string roleId)
        {
            SwitchView(_cityPrefab, go =>
            {
                var view = go.GetComponent<CityView>();
                if (view != null) view.Init(_network, roleId);
            });
        }

        private void SwitchView(GameObject prefab, System.Action<GameObject> onCreated)
        {
            if (_currentView != null)
                Object.Destroy(_currentView);

            if (prefab != null)
            {
                var go = Object.Instantiate(prefab);
                _currentView = go;
                onCreated?.Invoke(go);
            }
        }
    }
}
