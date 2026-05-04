using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    /// <summary>
    /// Phase 1 empty city UI. Displays role name, level, and gold.
    /// </summary>
    public class CityView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text statusText;

        private NetworkManager _network;

        public void Init(NetworkManager network, string roleId)
        {
            _network = network;
            statusText.text = "Entering city...";
            StartCoroutine(_network.EnterCity(roleId, OnCityEntered, OnError));
        }

        private void OnCityEntered(EnterCityResult result)
        {
            if (result.role != null)
            {
                nameText.text = result.role.name;
                levelText.text = $"Level {result.role.level}";
                goldText.text = $"Gold: {result.role.gold:N0}";
                statusText.text = "Welcome to the city!";
            }
        }

        private void OnError(string error)
        {
            statusText.text = $"Error: {error}";
        }
    }
}
