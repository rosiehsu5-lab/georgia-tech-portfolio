using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class TutorialSceneSetup
{
    [MenuItem("Tools/Pengu Delivery/Setup Tutorial Scene")]
    public static void Setup()
    {
        var active = SceneManager.GetActiveScene();
        if (!active.name.Equals("Tutorial"))
        {
            EditorUtility.DisplayDialog(
                "Tutorial scene not active",
                "Open Assets/Scenes/Tutorial.unity first, then run this menu.\n\nActive scene: " + active.name,
                "OK");
            return;
        }

        TutorialManager tm = Object.FindFirstObjectByType<TutorialManager>();
        if (tm == null)
        {
            Debug.LogError("[Tutorial Setup] No TutorialManager in scene. Aborting.");
            return;
        }

        Canvas canvas = null;
        if (tm.tutorialText != null)
            canvas = tm.tutorialText.GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[Tutorial Setup] No Canvas in scene.");
            return;
        }

        RectTransform panelRT = FindChild<RectTransform>(canvas.transform, "InstructionPanel");
        Image panelImg;
        if (panelRT == null)
        {
            GameObject go = new GameObject("InstructionPanel",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(canvas.transform, false);
            panelRT = go.GetComponent<RectTransform>();
            panelImg = go.GetComponent<Image>();
        }
        else
        {
            panelImg = panelRT.GetComponent<Image>();
        }
        panelRT.anchorMin = new Vector2(0.5f, 1f);
        panelRT.anchorMax = new Vector2(0.5f, 1f);
        panelRT.pivot = new Vector2(0.5f, 1f);
        panelRT.anchoredPosition = new Vector2(0f, -30f);
        panelRT.sizeDelta = new Vector2(560f, 170f);
        panelImg.color = new Color(0f, 0f, 0f, 0.55f);
        panelImg.raycastTarget = false;
        panelImg.transform.SetAsFirstSibling();

        if (tm.tutorialText != null)
        {
            tm.tutorialText.transform.SetParent(panelRT, false);
            RectTransform rt = tm.tutorialText.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.55f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = new Vector2(12f, 0f);
            rt.offsetMax = new Vector2(-12f, -8f);
            tm.tutorialText.alignment = TextAlignmentOptions.Center;
            tm.tutorialText.fontSize = 40;
            tm.tutorialText.color = Color.white;
            tm.tutorialText.enableWordWrapping = true;
        }

        TextMeshProUGUI keyHint = FindChild<TextMeshProUGUI>(panelRT, "KeyHint");
        if (keyHint == null)
        {
            GameObject go = new GameObject("KeyHint",
                typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(panelRT, false);
            keyHint = go.AddComponent<TextMeshProUGUI>();
        }
        {
            RectTransform rt = keyHint.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.15f);
            rt.anchorMax = new Vector2(1f, 0.5f);
            rt.offsetMin = new Vector2(12f, 0f);
            rt.offsetMax = new Vector2(-12f, 0f);
            keyHint.alignment = TextAlignmentOptions.Center;
            keyHint.fontSize = 28;
            keyHint.color = new Color(1f, 0.88f, 0.25f, 1f);
            keyHint.text = "[W]  [A]  [S]  [D]";
            keyHint.enableWordWrapping = false;
        }

        TextMeshProUGUI stepCounter = FindChild<TextMeshProUGUI>(panelRT, "StepCounter");
        if (stepCounter == null)
        {
            GameObject go = new GameObject("StepCounter",
                typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(panelRT, false);
            stepCounter = go.AddComponent<TextMeshProUGUI>();
        }
        {
            RectTransform rt = stepCounter.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0.18f);
            rt.offsetMin = new Vector2(12f, 4f);
            rt.offsetMax = new Vector2(-12f, 0f);
            stepCounter.alignment = TextAlignmentOptions.Center;
            stepCounter.fontSize = 18;
            stepCounter.color = new Color(0.85f, 0.85f, 0.85f, 0.85f);
            stepCounter.text = "Step 1 / 4";
            stepCounter.enableWordWrapping = false;
        }

        AudioSource audio = tm.GetComponent<AudioSource>();
        if (audio == null) audio = tm.gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.volume = 0.8f;

        tm.stepCounter = stepCounter;
        tm.keyHint = keyHint;
        tm.instructionPanel = panelRT;
        tm.audioSource = audio;

        string chimePath = "Assets/Sounds/freesound_community-chime-sound-7143.mp3";
        AudioClip chime = AssetDatabase.LoadAssetAtPath<AudioClip>(chimePath);
        if (chime != null && tm.stepCompleteClip == null) tm.stepCompleteClip = chime;

        EditorUtility.SetDirty(tm);
        EditorSceneManager.MarkSceneDirty(active);

        Selection.activeGameObject = tm.gameObject;
        EditorGUIUtility.PingObject(tm.gameObject);

        Debug.Log("[Tutorial Setup] Done. Panel, KeyHint, StepCounter added. " +
                  "Step-complete chime auto-wired if the sound file exists. " +
                  "Save the scene (Cmd+S).");
    }

    static T FindChild<T>(Transform parent, string name) where T : Component
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var c = parent.GetChild(i);
            if (c.name == name)
            {
                var comp = c.GetComponent<T>();
                if (comp != null) return comp;
            }
        }
        return null;
    }
}
