using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    /// <summary>
    /// Phase 4 quest tracker overlay. Shows active quest progress and accept buttons.
    /// Attach to a child of CityView canvas.
    /// </summary>
    public class QuestTracker : MonoBehaviour
    {
        [SerializeField] private Text statusText;
        [SerializeField] private Button acceptQuest1Btn;
        [SerializeField] private Button acceptQuest2Btn;
        [SerializeField] private Button acceptQuest3Btn;

        private GameManager _gm;

        private void Start()
        {
            _gm = FindObjectOfType<GameManager>();
            if (_gm != null)
            {
                _gm.OnQuestUpdated += OnUpdated;
                _gm.OnQuestCompleted += OnCompleted;
            }

            acceptQuest1Btn?.onClick.AddListener(() => _gm?.SendAcceptQuest(1));
            acceptQuest2Btn?.onClick.AddListener(() => _gm?.SendAcceptQuest(2));
            acceptQuest3Btn?.onClick.AddListener(() => _gm?.SendAcceptQuest(3));
        }

        private void OnUpdated(string info)
        {
            statusText.text = info;
            SetButtons(false);
        }

        private void OnCompleted(string info)
        {
            statusText.text = info;
            SetButtons(true);
        }

        private void SetButtons(bool visible)
        {
            if (acceptQuest1Btn) acceptQuest1Btn.gameObject.SetActive(visible);
            if (acceptQuest2Btn) acceptQuest2Btn.gameObject.SetActive(visible);
            if (acceptQuest3Btn) acceptQuest3Btn.gameObject.SetActive(visible);
        }

        private void OnDestroy()
        {
            if (_gm != null)
            {
                _gm.OnQuestUpdated -= OnUpdated;
                _gm.OnQuestCompleted -= OnCompleted;
            }
        }
    }
}
