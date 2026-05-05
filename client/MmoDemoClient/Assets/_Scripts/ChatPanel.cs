using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client
{
    /// <summary>
    /// Phase 4 chat overlay. Hooks GameManager events for real-time chat display.
    /// Attach to a child of CityView canvas.
    /// </summary>
    public class ChatPanel : MonoBehaviour
    {
        [SerializeField] private Text chatLog;
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;

        private GameManager _gm;
        private readonly List<string> _messages = new();
        private const int MaxMessages = 20;

        private void Start()
        {
            _gm = FindObjectOfType<GameManager>();
            if (_gm != null) _gm.OnChatReceived += OnChat;

            sendButton.onClick.AddListener(Send);
        }

        public void Send()
        {
            var text = inputField.text?.Trim();
            if (string.IsNullOrEmpty(text) || _gm == null) return;
            _gm.SendChat(text);
            inputField.text = "";
        }

        private void OnChat(string sender, string text)
        {
            _messages.Add($"[{sender}]: {text}");
            while (_messages.Count > MaxMessages) _messages.RemoveAt(0);
            chatLog.text = string.Join("\n", _messages);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused)
                Send();
        }

        private void OnDestroy()
        {
            if (_gm != null) _gm.OnChatReceived -= OnChat;
        }
    }
}
