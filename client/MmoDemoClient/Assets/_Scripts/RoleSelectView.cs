using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    public class RoleSelectView : MonoBehaviour
    {
        [SerializeField] private Transform roleListContainer;
        [SerializeField] private GameObject roleButtonPrefab;
        [SerializeField] private GameObject createRolePanel;
        [SerializeField] private InputField nameInput;
        [SerializeField] private Button warriorButton;
        [SerializeField] private Button mageButton;
        [SerializeField] private Button archerButton;
        [SerializeField] private Button createButton;
        [SerializeField] private Text statusText;

        private NetworkManager _network;
        private GameLauncher _launcher;
        private int _selectedClassId = 1;

        public void Init(NetworkManager network)
        {
            _network = network;
            _launcher = FindObjectOfType<GameLauncher>();

            createButton.onClick.AddListener(OnCreateClick);
            warriorButton.onClick.AddListener(() => SelectClass(1));
            mageButton.onClick.AddListener(() => SelectClass(2));
            archerButton.onClick.AddListener(() => SelectClass(3));

            SelectClass(1);
            RefreshRoleList();
        }

        private void SelectClass(int classId)
        {
            _selectedClassId = classId;
            warriorButton.interactable = classId != 1;
            mageButton.interactable = classId != 2;
            archerButton.interactable = classId != 3;
        }

        private void RefreshRoleList()
        {
            statusText.text = "Loading roles...";
            StartCoroutine(_network.GetRoleList(OnRoleListLoaded, OnError));
        }

        private void OnRoleListLoaded(RoleListResult result)
        {
            foreach (Transform child in roleListContainer)
                Destroy(child.gameObject);

            if (result.roles == null || result.roles.Length == 0)
            {
                createRolePanel.SetActive(true);
                statusText.text = "Create your first character";
            }
            else
            {
                createRolePanel.SetActive(false);
                statusText.text = "Select a character";

                foreach (var role in result.roles)
                {
                    var btnObj = Instantiate(roleButtonPrefab, roleListContainer);
                    var label = btnObj.GetComponentInChildren<Text>();
                    if (label != null)
                        label.text = $"{role.name}  Lv.{role.level}  Gold:{role.gold}";
                    var capturedRoleId = role.roleId;
                    btnObj.GetComponent<Button>().onClick.AddListener(() => OnSelectRole(capturedRoleId));
                }
            }
        }

        private void OnSelectRole(string roleId)
        {
            statusText.text = "Selecting...";
            StartCoroutine(_network.SelectRole(roleId,
                _ => _launcher.OnRoleSelected(roleId),
                OnError));
        }

        private void OnCreateClick()
        {
            var name = (nameInput.text ?? "").Trim();
            if (name.Length < 1 || name.Length > 12)
            {
                statusText.text = "Name must be 1-12 characters";
                return;
            }

            statusText.text = "Creating...";
            StartCoroutine(_network.CreateRole(name, _selectedClassId, OnCreateSuccess, OnError));
        }

        private void OnCreateSuccess(CreateRoleResult result)
        {
            if (result.role != null)
                _launcher.OnRoleSelected(result.role.roleId);
            else
                statusText.text = "Create failed: no role returned";
        }

        private void OnError(string error)
        {
            statusText.text = $"Error: {error}";
        }
    }
}
