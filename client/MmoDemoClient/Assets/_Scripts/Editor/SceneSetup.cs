using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MmoDemo.Client.Editor
{
    /// <summary>
    /// Creates the Bootstrap scene with GameLauncher and basic UI canvases.
    /// Run from CLI: Unity -batchmode -quit -projectPath . -executeMethod MmoDemo.Client.Editor.SceneSetup.CreateAll
    /// </summary>
    public static class SceneSetup
    {
        [MenuItem("MmoDemo/Setup All Scenes")]
        public static void CreateAll()
        {
            CreatePlayerPrefabs();
            CreatePhase3Prefabs();
            CreateUIPrefabs();
            AssetDatabase.Refresh();
            CreateBootstrapScene();
            AssetDatabase.Refresh();
            Debug.Log("[SceneSetup] All scenes and prefabs created.");
        }

        public static void CreatePlayerPrefabs()
        {
            var lp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            lp.name = "LocalPlayer";
            SavePrefab(lp, "LocalPlayer");

            var op = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            op.name = "OtherPlayer";
            SavePrefab(op, "OtherPlayer");
        }

        public static void CreatePhase3Prefabs()
        {
            // Monster prefab (green cube)
            var monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.name = "Monster";
            SavePrefab(monster, "Monster");

            // Drop prefab (yellow sphere)
            var drop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            drop.name = "Drop";
            drop.transform.localScale = Vector3.one * 0.3f;
            SavePrefab(drop, "Drop");

            // Damage text prefab
            var dt = new GameObject("DamageText");
            var tm = dt.AddComponent<TextMesh>();
            tm.text = "0";
            tm.fontSize = 36;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;
            SavePrefab(dt, "DamageText");
        }

        public static void CreateBootstrapScene()
        {
            var loginPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/LoginView.prefab");
            var rolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/RoleSelectView.prefab");
            var cityPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/CityView.prefab");
            var localPlayerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/LocalPlayer.prefab");
            var otherPlayerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/OtherPlayer.prefab");
            var monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Monster.prefab");
            var dropPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Drop.prefab");
            var damagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/DamageText.prefab");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // GameLauncher
            var launcherObj = new GameObject("GameLauncher");
            var launcher = launcherObj.AddComponent<GameLauncher>();
            var launcherSo = new SerializedObject(launcher);
            launcherSo.FindProperty("loginViewPrefab").objectReferenceValue = loginPrefab;
            launcherSo.FindProperty("roleSelectViewPrefab").objectReferenceValue = rolePrefab;
            launcherSo.FindProperty("cityViewPrefab").objectReferenceValue = cityPrefab;
            launcherSo.ApplyModifiedProperties();

            // GameManager (Phase 2)
            var gmObj = new GameObject("GameManager");
            var gm = gmObj.AddComponent<GameManager>();
            var gmSo = new SerializedObject(gm);
            gmSo.FindProperty("localPlayerPrefab").objectReferenceValue = localPlayerPrefab;
            gmSo.FindProperty("otherPlayerPrefab").objectReferenceValue = otherPlayerPrefab;
            gmSo.FindProperty("monsterPrefab").objectReferenceValue = monsterPrefab;
            gmSo.FindProperty("dropPrefab").objectReferenceValue = dropPrefab;
            gmSo.FindProperty("damageTextPrefab").objectReferenceValue = damagePrefab;
            gmSo.ApplyModifiedProperties();

            // EventSystem
            var eventSys = new GameObject("EventSystem");
            eventSys.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSys.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            EditorSceneManager.SaveScene(scene, "Assets/_Scenes/Bootstrap.unity", false);
            Debug.Log("[SceneSetup] Bootstrap scene created with Phase 2 GameManager.");
        }

        public static void CreateUIPrefabs()
        {
            CreateLoginViewPrefab();
            CreateRoleSelectViewPrefab();
            CreateCityViewPrefab();
        }

        private static void CreateLoginViewPrefab()
        {
            var go = CreateCanvas("LoginView");
            go.AddComponent<LoginView>();

            // Login button
            var btnGo = CreateUIElement("LoginButton", go.transform, new Vector2(0, 0), new Vector2(200, 60));
            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = new Color(0.3f, 0.6f, 1f);
            var btn = btnGo.AddComponent<Button>();
            var btnTxt = CreateText("Login", btnGo.transform, "Login");

            // Status text
            var status = CreateText("Connecting...", go.transform, "StatusText");
            status.rectTransform.anchoredPosition = new Vector2(0, 60);

            // Wire up references
            var view = go.GetComponent<LoginView>();
            var so = new SerializedObject(view);
            so.FindProperty("loginButton").objectReferenceValue = btn;
            so.FindProperty("statusText").objectReferenceValue = status;
            so.ApplyModifiedProperties();

            SavePrefab(go, "LoginView");
        }

        private static void CreateRoleSelectViewPrefab()
        {
            var go = CreateCanvas("RoleSelectView");
            go.AddComponent<RoleSelectView>();

            var container = new GameObject("RoleListContainer", typeof(RectTransform));
            container.transform.SetParent(go.transform, false);

            // Role button template
            var btnTpl = CreateUIElement("RoleButton", container.transform, new Vector2(0, 0), new Vector2(280, 50));
            btnTpl.AddComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f);
            btnTpl.AddComponent<Button>();
            var tplTxt = CreateText("", btnTpl.transform, "");
            tplTxt.resizeTextForBestFit = true;

            // Create panel
            var createPanel = new GameObject("CreateRolePanel", typeof(RectTransform));
            createPanel.transform.SetParent(go.transform, false);

            var nameInput = CreateInputField("NameInput", createPanel.transform, new Vector2(0, 60));

            // Class selection buttons instead of Dropdown
            var warriorBtn = CreateButton("WarriorBtn", createPanel.transform, new Vector2(-80, 0), "Warrior");
            var mageBtn = CreateButton("MageBtn", createPanel.transform, new Vector2(0, 0), "Mage");
            var archerBtn = CreateButton("ArcherBtn", createPanel.transform, new Vector2(80, 0), "Archer");
            warriorBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 40);
            mageBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 40);
            archerBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 40);

            var createBtn = CreateButton("CreateButton", createPanel.transform, new Vector2(0, -60), "Create");

            var status = CreateText("", go.transform, "StatusText");
            status.rectTransform.anchoredPosition = new Vector2(0, 140);

            var view = go.GetComponent<RoleSelectView>();
            var so = new SerializedObject(view);
            so.FindProperty("roleListContainer").objectReferenceValue = container.transform;
            so.FindProperty("roleButtonPrefab").objectReferenceValue = btnTpl;
            so.FindProperty("createRolePanel").objectReferenceValue = createPanel;
            so.FindProperty("nameInput").objectReferenceValue = nameInput;
            so.FindProperty("warriorButton").objectReferenceValue = warriorBtn;
            so.FindProperty("mageButton").objectReferenceValue = mageBtn;
            so.FindProperty("archerButton").objectReferenceValue = archerBtn;
            so.FindProperty("createButton").objectReferenceValue = createBtn;
            so.FindProperty("statusText").objectReferenceValue = status;
            so.ApplyModifiedProperties();

            SavePrefab(go, "RoleSelectView");
        }

        private static void CreateCityViewPrefab()
        {
            var go = CreateCanvas("CityView");
            go.AddComponent<CityView>();

            var nameTxt = CreateText("PlayerName", go.transform, "NameText");
            nameTxt.rectTransform.anchoredPosition = new Vector2(0, 100);
            nameTxt.fontSize = 32;

            var levelTxt = CreateText("Level 1", go.transform, "LevelText");
            levelTxt.rectTransform.anchoredPosition = new Vector2(0, 40);
            levelTxt.fontSize = 24;

            var goldTxt = CreateText("Gold: 0", go.transform, "GoldText");
            goldTxt.rectTransform.anchoredPosition = new Vector2(0, 0);
            goldTxt.fontSize = 24;

            var statusTxt = CreateText("Entering city...", go.transform, "StatusText");
            statusTxt.rectTransform.anchoredPosition = new Vector2(0, -60);

            // Phase 4: Chat panel (bottom-left overlay)
            var chatGo = new GameObject("ChatPanel", typeof(RectTransform));
            chatGo.transform.SetParent(go.transform, false);
            var chatPanel = chatGo.AddComponent<ChatPanel>();
            var chatSo = new SerializedObject(chatPanel);

            var chatLog = CreateText("", chatGo.transform, "ChatLog");
            chatLog.rectTransform.anchoredPosition = new Vector2(-300, -200);
            chatLog.rectTransform.sizeDelta = new Vector2(280, 160);
            chatLog.alignment = TextAnchor.LowerLeft;

            var inputGo = CreateUIElement("ChatInput", chatGo.transform, new Vector2(-300, -280), new Vector2(200, 30));
            inputGo.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
            var chatInput = inputGo.AddComponent<InputField>();
            var inputTxtGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            inputTxtGo.transform.SetParent(inputGo.transform, false);
            var inputTxt = inputTxtGo.GetComponent<Text>();
            inputTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            inputTxt.fontSize = 14;
            inputTxt.color = Color.white;
            inputTxt.alignment = TextAnchor.MiddleLeft;
            inputTxt.rectTransform.sizeDelta = new Vector2(190, 26);
            chatInput.textComponent = inputTxt;

            var sendBtnGo = CreateUIElement("SendBtn", chatGo.transform, new Vector2(-70, -280), new Vector2(50, 30));
            sendBtnGo.AddComponent<Image>().color = new Color(0.2f, 0.6f, 0.2f);
            var sendBtn = sendBtnGo.AddComponent<Button>();
            CreateText("Send", sendBtnGo.transform, "Label");

            chatSo.FindProperty("chatLog").objectReferenceValue = chatLog;
            chatSo.FindProperty("inputField").objectReferenceValue = chatInput;
            chatSo.FindProperty("sendButton").objectReferenceValue = sendBtn;
            chatSo.ApplyModifiedProperties();

            // Phase 4: Quest tracker (top-right overlay)
            var questGo = new GameObject("QuestTracker", typeof(RectTransform));
            questGo.transform.SetParent(go.transform, false);
            var questTracker = questGo.AddComponent<QuestTracker>();
            var questSo = new SerializedObject(questTracker);

            var questStatus = CreateText("Press a quest button to begin", questGo.transform, "QuestStatus");
            questStatus.rectTransform.anchoredPosition = new Vector2(300, 260);
            questStatus.rectTransform.sizeDelta = new Vector2(280, 40);
            questStatus.fontSize = 16;
            questStatus.alignment = TextAnchor.UpperRight;

            var q1Btn = CreateButton("Quest1Btn", questGo.transform, new Vector2(300, 220), "Slime x3");
            q1Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            var q2Btn = CreateButton("Quest2Btn", questGo.transform, new Vector2(300, 185), "Goblins x2");
            q2Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            var q3Btn = CreateButton("Quest3Btn", questGo.transform, new Vector2(300, 150), "Wolf x1");
            q3Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);

            questSo.FindProperty("statusText").objectReferenceValue = questStatus;
            questSo.FindProperty("acceptQuest1Btn").objectReferenceValue = q1Btn;
            questSo.FindProperty("acceptQuest2Btn").objectReferenceValue = q2Btn;
            questSo.FindProperty("acceptQuest3Btn").objectReferenceValue = q3Btn;
            questSo.ApplyModifiedProperties();

            var view = go.GetComponent<CityView>();
            var so = new SerializedObject(view);
            so.FindProperty("nameText").objectReferenceValue = nameTxt;
            so.FindProperty("levelText").objectReferenceValue = levelTxt;
            so.FindProperty("goldText").objectReferenceValue = goldTxt;
            so.FindProperty("statusText").objectReferenceValue = statusTxt;
            so.ApplyModifiedProperties();

            SavePrefab(go, "CityView");
        }

        // ── Helpers ──

        private static GameObject CreateCanvas(string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(800, 600);
            return go;
        }

        private static GameObject CreateUIElement(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return go;
        }

        private static Text CreateText(string text, Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.rectTransform.sizeDelta = new Vector2(300, 40);
            return txt;
        }

        private static Button CreateButton(string name, Transform parent, Vector2 pos, string label)
        {
            var go = CreateUIElement(name, parent, pos, new Vector2(200, 50));
            go.AddComponent<Image>().color = new Color(0.3f, 0.6f, 1f);
            var btn = go.AddComponent<Button>();
            CreateText(label, go.transform, "Label").text = label;
            return btn;
        }

        private static InputField CreateInputField(string name, Transform parent, Vector2 pos)
        {
            var go = CreateUIElement(name, parent, pos, new Vector2(200, 40));
            go.AddComponent<Image>().color = Color.white;
            var ifield = go.AddComponent<InputField>();

            // Placeholder text
            var placeholder = new GameObject("Placeholder", typeof(RectTransform), typeof(Text));
            placeholder.transform.SetParent(go.transform, false);
            var pt = placeholder.GetComponent<Text>();
            pt.text = "Enter name...";
            pt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            pt.fontSize = 16;
            pt.color = Color.gray;
            pt.alignment = TextAnchor.MiddleLeft;
            ifield.placeholder = pt;

            // Text component
            var text = new GameObject("Text", typeof(RectTransform), typeof(Text));
            text.transform.SetParent(go.transform, false);
            var tt = text.GetComponent<Text>();
            tt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tt.fontSize = 16;
            tt.color = Color.black;
            tt.alignment = TextAnchor.MiddleLeft;
            tt.rectTransform.sizeDelta = new Vector2(190, 30);
            ifield.textComponent = tt;

            return ifield;
        }

        private static void SavePrefab(GameObject go, string name)
        {
            var path = $"Assets/Resources/Prefabs/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }
    }
}
