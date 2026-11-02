using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class CPRSceneGenerator
{
    private const string ScenePath = "Assets/Scenes/Scene_CPR.unity";

    [MenuItem("Tools/CPR/Generate Starter Scene")]
    public static void GenerateScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EnsureFolder("Assets/Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "Scene_CPR";

        // World placeholders
        CreateWorldPlaceholder("Environment", new Vector3(0, 0, 0));
        CreateSpritePlaceholder("Background_Pantai", new Vector3(0, 1.2f, 5), new Vector2(16f, 5f), new Color(0.45f, 0.78f, 0.91f));
        CreateSpritePlaceholder("Pasir", new Vector3(0, -2.2f, 4), new Vector2(16f, 3.5f), new Color(0.91f, 0.81f, 0.59f));
        CreateSpritePlaceholder("Laut", new Vector3(-4.5f, -0.1f, 3), new Vector2(7f, 2.5f), new Color(0.20f, 0.61f, 0.79f));

        var characters = new GameObject("Characters");
        CreateCharacterPlaceholder("Lifeguard_Player", characters.transform, new Vector3(-1.6f, -0.7f, 0), new Color(1f, 0.48f, 0.18f));
        CreateCharacterPlaceholder("NPC_Korban", characters.transform, new Vector3(1.0f, -1.05f, 0), new Color(0.96f, 0.78f, 0.65f));

        // Camera
        var cam = Camera.main;
        if (cam == null)
        {
            var cameraObject = new GameObject("Main Camera");
            cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.backgroundColor = new Color(0.70f, 0.86f, 0.91f);

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var missionPanel = MakePanel(canvasGO.transform, "Panel_Misi", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -25), new Vector2(900, 115), new Color(0.05f, 0.22f, 0.32f, 0.88f));
        MakeText(missionPanel.transform, "Text_Judul", "MISI PENYELAMATAN", 34, new Vector2(0, 20), new Vector2(820, 48), Color.white, FontStyles.Bold);
        MakeText(missionPanel.transform, "Text_Progress", "Tahap 1 dari 5  •  Pastikan area aman", 23, new Vector2(0, -27), new Vector2(820, 38), Color.white, FontStyles.Normal);

        var dialog = MakePanel(canvasGO.transform, "Panel_Dialog", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0, 35), new Vector2(1250, 220), new Color(0.04f, 0.20f, 0.30f, 0.94f));
        MakeText(dialog.transform, "Text_Instruksi",
            "Selamat datang di simulasi penyelamatan. Ikuti panduan keselamatan dan arahan petugas/layanan darurat.",
            27, new Vector2(-100, 20), new Vector2(1000, 120), Color.white, FontStyles.Normal);
        MakeButton(dialog.transform, "Button_Lanjut", "LANJUT", new Vector2(480, -70), new Vector2(190, 62));

        var cprPanel = MakePanel(canvasGO.transform, "Panel_CPR", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(900, 500), new Color(0.95f, 0.98f, 1f, 0.97f));
        MakeText(cprPanel.transform, "Text_CPRJudul", "SIMULASI CPR", 36, new Vector2(0, 170), new Vector2(780, 60), new Color(0.04f, 0.20f, 0.30f), FontStyles.Bold);
        MakeText(cprPanel.transform, "Text_Pertanyaan",
            "Ikuti panduan pertolongan pertama yang benar. Dalam keadaan darurat nyata, hubungi layanan darurat dan ikuti instruksi operator.",
            25, new Vector2(0, 70), new Vector2(760, 130), new Color(0.10f, 0.18f, 0.22f), FontStyles.Normal);
        MakeButton(cprPanel.transform, "Button_Aksi1", "AKSI 1", new Vector2(-190, -95), new Vector2(250, 70));
        MakeButton(cprPanel.transform, "Button_Aksi2", "AKSI 2", new Vector2(190, -95), new Vector2(250, 70));
        MakeText(cprPanel.transform, "Text_Feedback", "Feedback akan muncul di sini.", 22, new Vector2(0, -190), new Vector2(760, 45), new Color(0.05f, 0.35f, 0.25f), FontStyles.Bold);

        var resultPanel = MakePanel(canvasGO.transform, "Panel_Hasil", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(760, 390), new Color(0.05f, 0.22f, 0.32f, 0.96f));
        MakeText(resultPanel.transform, "Text_HasilJudul", "MISI SELESAI", 42, new Vector2(0, 95), new Vector2(680, 70), Color.white, FontStyles.Bold);
        MakeText(resultPanel.transform, "Text_HasilDetail", "Ringkasan dan skor misi akan ditampilkan di sini.", 25, new Vector2(0, 5), new Vector2(650, 90), Color.white, FontStyles.Normal);
        MakeButton(resultPanel.transform, "Button_Ulangi", "ULANGI", new Vector2(-150, -115), new Vector2(220, 65));
        MakeButton(resultPanel.transform, "Button_Kembali", "KEMBALI", new Vector2(150, -115), new Vector2(220, 65));

        // A simple scene controller placeholder
        var manager = new GameObject("GameManager");
        manager.AddComponent<CPRSceneReferences>();

        // Initial visibility
        cprPanel.SetActive(false);
        resultPanel.SetActive(false);

        // EventSystem
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        EditorSceneManager.SaveScene(scene, ScenePath);
        Selection.activeGameObject = canvasGO;
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath));
        Debug.Log("Template Scene_CPR berhasil dibuat di " + ScenePath);
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
    }

    private static void CreateWorldPlaceholder(string name, Vector3 position)
    {
        var go = new GameObject(name);
        go.transform.position = position;
    }

    private static void CreateSpritePlaceholder(string name, Vector3 position, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        sr.color = color;
        go.transform.localScale = new Vector3(size.x / 100f, size.y / 100f, 1f);
        sr.sortingOrder = -5;
    }

    private static void CreateCharacterPlaceholder(string name, Transform parent, Vector3 position, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = position;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = color;
        go.transform.localScale = new Vector3(1.2f, 1.8f, 1f);
        sr.sortingOrder = 1;
    }

    private static GameObject MakePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPosition, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = color;
        return go;
    }

    private static void MakeText(Transform parent, string name, string value, float fontSize,
        Vector2 position, Vector2 size, Color color, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        var text = go.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = style;
        text.textWrappingMode = TextWrappingModes.Normal;
    }

    private static void MakeButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = new Color(1f, 0.48f, 0.18f, 1f);
        MakeText(go.transform, "Text", label, 23, Vector2.zero, size, Color.white, FontStyles.Bold);
    }
}

public class CPRSceneReferences : MonoBehaviour
{
    // Placeholder component: use this GameObject to attach your future CPR gameplay manager.
}
