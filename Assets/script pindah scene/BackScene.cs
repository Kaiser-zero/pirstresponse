
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackScene : MonoBehaviour
{
    [Header("Scene Tujuan")]
    public int nomorSceneTujuan = 0;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip suaraKlik;

    [Header("Animasi Fade")]
    public float durasiFade = 0.35f;

    private bool sedangPindah;
    private CanvasGroup fadeGroup;

    private void Awake()
    {
        // Membuat Canvas fade secara otomatis.
        GameObject canvasObj = new GameObject("BackFadeCanvas");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        fadeGroup = panel.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    public void Kembali()
    {
        if (sedangPindah) return;

        if (nomorSceneTujuan < 0 ||
            nomorSceneTujuan >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Build Index tidak valid!");
            return;
        }

        sedangPindah = true;

        StartCoroutine(ProsesKembali());
    }

    private IEnumerator ProsesKembali()
    {
        // Memainkan audio klik.
        if (audioSource != null && suaraKlik != null)
        {
            audioSource.PlayOneShot(suaraKlik);
        }

        // Fade menjadi hitam.
        float waktu = 0f;

        while (waktu < durasiFade)
        {
            waktu += Time.unscaledDeltaTime;

            fadeGroup.alpha = durasiFade > 0f
                ? Mathf.Clamp01(waktu / durasiFade)
                : 1f;

            yield return null;
        }

        fadeGroup.alpha = 1f;

        // Kembali langsung ke scene tujuan.
        SceneManager.LoadScene(nomorSceneTujuan);
    }
}
