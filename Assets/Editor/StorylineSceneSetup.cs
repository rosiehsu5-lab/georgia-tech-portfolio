using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class StorylineSceneSetup
{
    [MenuItem("Tools/Pengu Delivery/Setup Storyline Scene")]
    public static void Setup()
    {
        var active = SceneManager.GetActiveScene();
        if (!active.name.Equals("Storyline"))
        {
            if (!EditorUtility.DisplayDialog(
                "Storyline scene not active",
                "Open Assets/Scenes/Storyline.unity first, then run this menu.\n\nActive scene: " + active.name,
                "OK"))
                return;
            return;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[Storyline Setup] No Canvas in scene. Create one first.");
            return;
        }

        RectTransform canvasRT = canvas.GetComponent<RectTransform>();

        Image tint = FindChildComponent<Image>(canvas.transform, "BackgroundTint");
        if (tint == null)
        {
            GameObject go = new GameObject("BackgroundTint", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(canvas.transform, false);
            go.transform.SetAsFirstSibling();
            tint = go.GetComponent<Image>();
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            tint.color = new Color(0.02f, 0.02f, 0.05f, 1f);
            tint.raycastTarget = false;
        }

        TextMeshProUGUI body = null;
        Transform existingStoryline = canvas.transform.Find("storyline");
        if (existingStoryline != null)
        {
            existingStoryline.name = "BodyText";
            body = existingStoryline.GetComponent<TextMeshProUGUI>();
            if (body != null) body.text = "";
        }
        if (body == null) body = FindChildComponent<TextMeshProUGUI>(canvas.transform, "BodyText");
        if (body == null)
        {
            GameObject go = new GameObject("BodyText", typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(canvas.transform, false);
            body = go.AddComponent<TextMeshProUGUI>();
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.3f);
            rt.anchorMax = new Vector2(0.9f, 0.8f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            body.text = "";
            body.alignment = TextAlignmentOptions.Center;
            body.fontSize = 38;
            body.color = Color.white;
            body.enableWordWrapping = true;
        }

        TextMeshProUGUI prompt = FindChildComponent<TextMeshProUGUI>(canvas.transform, "ContinuePrompt");
        if (prompt == null)
        {
            GameObject go = new GameObject("ContinuePrompt", typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(canvas.transform, false);
            prompt = go.AddComponent<TextMeshProUGUI>();
        }
        {
            RectTransform rt = prompt.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-30f, 30f);
            rt.sizeDelta = new Vector2(420f, 40f);
            prompt.text = "Press Space / Click to continue";
            prompt.alignment = TextAlignmentOptions.MidlineRight;
            prompt.fontSize = 20;
            prompt.color = new Color(0.9f, 0.9f, 0.9f, 0.6f);
            prompt.enableWordWrapping = false;
            prompt.gameObject.SetActive(false);
        }

        GameObject mgr = GameObject.Find("StorylineManager");
        if (mgr == null)
        {
            mgr = new GameObject("StorylineManager");
        }

        var controller = mgr.GetComponent<StorylineController>();
        if (controller == null) controller = mgr.AddComponent<StorylineController>();

        var sources = mgr.GetComponents<AudioSource>();
        AudioSource music = sources.Length > 0 ? sources[0] : mgr.AddComponent<AudioSource>();
        AudioSource sfx   = sources.Length > 1 ? sources[1] : mgr.AddComponent<AudioSource>();
        music.playOnAwake = false; music.loop = true; music.volume = 0.5f;
        sfx.playOnAwake = false; sfx.loop = false; sfx.volume = 1f;

        controller.bodyText = body;
        controller.continuePrompt = prompt;
        controller.backgroundTint = tint;
        controller.musicSource = music;
        controller.sfxSource = sfx;
        controller.typeDelay = 0.035f;
        controller.punctuationPause = 0.18f;
        controller.tintLerpSpeed = 2f;
        controller.typeTickVolume = 0.15f;
        controller.typeTickEveryNChars = 3;
        controller.nextSceneName = "Tutorial";
        controller.continuePromptText = "Press Space / Click to continue";

        controller.slides = new[]
        {
            new StorylineController.Slide {
                text = "Q1 targets are slipping. Leadership is escalating.",
                backgroundTint = new Color(0.05f, 0.08f, 0.12f, 1f)
            },
            new StorylineController.Slide {
                text = "You are Pengu, formerly part of a larger delivery team. Due to recent \"organisational restructuring,\" you are now the entire team.",
                backgroundTint = new Color(0.02f, 0.02f, 0.05f, 1f)
            },
            new StorylineController.Slide {
                text = "Deliver as many fish as you can to the log house before time runs out.",
                backgroundTint = new Color(0.05f, 0.10f, 0.15f, 1f)
            },
            new StorylineController.Slide {
                text = "Seals are on patrol. If they see or hear you, they will chase and catch you. The faster you go, the louder you are... and the easier you are to find.",
                backgroundTint = new Color(0.12f, 0.04f, 0.04f, 1f)
            },
            new StorylineController.Slide {
                text = "No pressure. We'll be monitoring your performance in real time... and adjusting headcount as needed.",
                backgroundTint = new Color(0.20f, 0.02f, 0.02f, 1f)
            },
        };

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(mgr);
        EditorSceneManager.MarkSceneDirty(active);

        Selection.activeGameObject = mgr;
        EditorGUIUtility.PingObject(mgr);

        Debug.Log("[Storyline Setup] Done. Now manually: " +
                  "(1) drag BGM AudioClip into the Music AudioSource (first AudioSource on StorylineManager). " +
                  "(2) rewire the SkipButton OnClick to StorylineController.SkipStoryline(). " +
                  "(3) save the scene.");
    }

    static T FindChildComponent<T>(Transform parent, string name) where T : Component
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
