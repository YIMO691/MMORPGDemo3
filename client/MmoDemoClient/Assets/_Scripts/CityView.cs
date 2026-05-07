using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    public class CityView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text statusText;

        private NetworkManager _network;
        private ChatPanel _chatPanel;
        private QuestTracker _questTracker;

        public void Init(NetworkManager network, string roleId)
        {
            _network = network;
            statusText.text = "Entering city...";

            ConfigureOverlayCanvas();
            EnsureChatPanel();
            EnsureQuestTracker();

            StartCoroutine(_network.EnterCity(roleId, result => OnCityEntered(result, roleId), OnError));
        }

        private void ConfigureOverlayCanvas()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null) return;

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
            transform.SetAsLastSibling();
        }

        private void EnsureChatPanel()
        {
            var existing = KeepSingleChild("ChatPanel");
            if (existing != null)
            {
                Stretch(existing.gameObject);
                LayoutChatPanel(existing);
                existing.SetAsLastSibling();
                _chatPanel = existing.GetComponent<ChatPanel>() ?? existing.gameObject.AddComponent<ChatPanel>();
                _chatPanel.SetUI(
                    existing.Find("ChatLog")?.GetComponent<Text>(),
                    existing.Find("ChatInput")?.GetComponent<InputField>(),
                    existing.Find("SendBtn")?.GetComponent<Button>());
                return;
            }

            // Container fills parent canvas
            var chatGo = NewUIChild("ChatPanel", transform);
            Stretch(chatGo);
            chatGo.transform.SetAsLastSibling();

            // Chat log: bottom-left, 250x130
            var logGo = NewUIChild("ChatLog", chatGo.transform);
            var logText = logGo.AddComponent<Text>();
            logText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            logText.fontSize = 14;
            logText.color = Color.white;
            logText.alignment = TextAnchor.LowerLeft;
            AnchorToCorner(logGo, new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 56), new Vector2(300, 150));

            // Input background: bottom-left, 180x28
            var inputGo = NewUIChild("ChatInput", chatGo.transform);
            inputGo.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
            var inputField = inputGo.AddComponent<InputField>();
            AnchorToCorner(inputGo, new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 16), new Vector2(220, 30));

            // Input text
            var inputTxtGo = NewUIChild("Text", inputGo.transform);
            var inputTxt = inputTxtGo.AddComponent<Text>();
            inputTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            inputTxt.fontSize = 14;
            inputTxt.color = Color.white;
            inputTxt.alignment = TextAnchor.MiddleLeft;
            FillParent(inputTxtGo);
            inputField.textComponent = inputTxt;

            // Send button
            var sendGo = NewUIChild("SendBtn", chatGo.transform);
            sendGo.AddComponent<Image>().color = new Color(0.2f, 0.6f, 0.2f);
            var sendBtn = sendGo.AddComponent<Button>();
            AnchorToCorner(sendGo, new Vector2(0, 0), new Vector2(0, 0), new Vector2(244, 16), new Vector2(64, 30));

            var sendTxtGo = NewUIChild("Label", sendGo.transform);
            var sendTxt = sendTxtGo.AddComponent<Text>();
            sendTxt.text = "Send";
            sendTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            sendTxt.fontSize = 13;
            sendTxt.color = Color.white;
            sendTxt.alignment = TextAnchor.MiddleCenter;
            FillParent(sendTxtGo);

            _chatPanel = chatGo.AddComponent<ChatPanel>();
            _chatPanel.SetUI(logText, inputField, sendBtn);
        }

        private void EnsureQuestTracker()
        {
            var existing = KeepSingleChild("QuestTracker");
            if (existing != null)
            {
                Stretch(existing.gameObject);
                LayoutQuestTracker(existing);
                existing.SetAsLastSibling();
                _questTracker = existing.GetComponent<QuestTracker>() ?? existing.gameObject.AddComponent<QuestTracker>();
                _questTracker.SetUI(
                    existing.Find("QuestStatus")?.GetComponent<Text>(),
                    FindButton(existing, "Quest1Btn", 0),
                    FindButton(existing, "Quest2Btn", 1),
                    FindButton(existing, "Quest3Btn", 2));
                return;
            }

            var questGo = NewUIChild("QuestTracker", transform);
            Stretch(questGo);
            questGo.transform.SetAsLastSibling();

            // Status text: top-right
            var statusGo = NewUIChild("QuestStatus", questGo.transform);
            var questText = statusGo.AddComponent<Text>();
            questText.text = "Select a quest:";
            questText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            questText.fontSize = 15;
            questText.color = Color.yellow;
            questText.alignment = TextAnchor.UpperRight;
            AnchorToCorner(statusGo, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, -16), new Vector2(240, 28));

            // Quest buttons stacked top-right
            var q1 = MakeQuestBtn(questGo.transform, "Quest1Btn", "Slime x3", -48);
            var q2 = MakeQuestBtn(questGo.transform, "Quest2Btn", "Goblins x2", -82);
            var q3 = MakeQuestBtn(questGo.transform, "Quest3Btn", "Wolf x1", -116);

            _questTracker = questGo.AddComponent<QuestTracker>();
            _questTracker.SetUI(questText, q1, q2, q3);
        }

        private static void LayoutChatPanel(Transform panel)
        {
            var log = panel.Find("ChatLog");
            if (log != null)
            {
                AnchorToCorner(log.gameObject, new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 56), new Vector2(300, 150));
                var text = log.GetComponent<Text>();
                if (text != null) text.alignment = TextAnchor.LowerLeft;
            }

            var input = panel.Find("ChatInput");
            if (input != null)
                AnchorToCorner(input.gameObject, new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 16), new Vector2(220, 30));

            var send = panel.Find("SendBtn");
            if (send != null)
                AnchorToCorner(send.gameObject, new Vector2(0, 0), new Vector2(0, 0), new Vector2(244, 16), new Vector2(64, 30));
        }

        private static void LayoutQuestTracker(Transform panel)
        {
            var status = panel.Find("QuestStatus");
            if (status != null)
            {
                AnchorToCorner(status.gameObject, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, -16), new Vector2(240, 28));
                var text = status.GetComponent<Text>();
                if (text != null) text.alignment = TextAnchor.UpperRight;
            }

            var buttons = panel.GetComponentsInChildren<Button>(true);
            for (var i = 0; i < buttons.Length; i++)
                AnchorToCorner(buttons[i].gameObject, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, -48 - i * 34), new Vector2(160, 28));
        }

        private Transform KeepSingleChild(string childName)
        {
            Transform keep = null;
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child.name != childName) continue;

                if (keep == null)
                {
                    keep = child;
                    continue;
                }

                DestroyUiObject(child.gameObject);
            }

            return keep;
        }

        private static Button FindButton(Transform parent, string preferredName, int fallbackIndex)
        {
            var named = parent.Find(preferredName)?.GetComponent<Button>();
            if (named != null) return named;

            var buttons = parent.GetComponentsInChildren<Button>(true);
            return fallbackIndex >= 0 && fallbackIndex < buttons.Length ? buttons[fallbackIndex] : null;
        }

        private static void DestroyUiObject(GameObject go)
        {
            if (Application.isPlaying)
                Destroy(go);
            else
                DestroyImmediate(go);
        }

        private Button MakeQuestBtn(Transform parent, string name, string label, float y)
        {
            var go = NewUIChild(name, parent);
            go.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.5f);
            var btn = go.AddComponent<Button>();
            AnchorToCorner(go, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, y), new Vector2(160, 28));

            var txtGo = NewUIChild("Label", go.transform);
            var txt = txtGo.AddComponent<Text>();
            txt.text = label;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 13;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            FillParent(txtGo);
            return btn;
        }

        // ── UI helpers ──

        private static GameObject NewUIChild(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void AnchorToCorner(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private static void Stretch(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        private static void FillParent(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        private void OnCityEntered(EnterCityResult result, string roleId)
        {
            if (result.role != null)
            {
                nameText.text = result.role.name;
                levelText.text = $"Level {result.role.level}";
                goldText.text = $"Gold: {result.role.gold:N0}";
                statusText.text = "Connecting to game server...";

                var gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.Connect(_network.PlayerId, _network.Token, roleId);
                }
                else
                {
                    statusText.text = "Welcome to the city! (WebSocket not available)";
                }
            }
        }

        private void OnError(string error)
        {
            statusText.text = $"Error: {error}";
        }
    }
}
