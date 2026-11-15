
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Panel Loading")]
    public RectTransform panelTransition;
    public CanvasGroup canvasGroupLoading;

    [Header("Progress Bar")]
    public Image progressFill;

    [Header("Audio Transisi")]
    public AudioSource audioSource;
    public AudioClip suaraTransisi;

    [Header("Pengaturan Animasi")]
    public float durasiFadeIn = 0.5f;
    public float durasiLoadingMinimum = 2f;
    public float durasiGerakKeAtas = 0.8f;

    private bool sedangTransisi;
    private Vector2 posisiAwalPanel;
    private Coroutine coroutineTransisi;

    private void Awake()
    {
        // Mencegah manager duplikat.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Validasi komponen.
        if (panelTransition == null ||
            canvasGroupLoading == null ||
            progressFill == null)
        {
            Debug.LogError("Referensi panel loading belum lengkap!", this);
            enabled = false;
            return;
        }

        posisiAwalPanel = panelTransition.anchoredPosition;
        ResetTampilanTransisi();
    }

    // =========================================
    // PINDAH SCENE
    // =========================================

    public void PindahScene(int nomorScene)
    {
        Debug.Log("PindahScene dipanggil: " + nomorScene);

        if (sedangTransisi)
        {
            Debug.LogWarning("Transisi sedang berlangsung.");
            return;
        }

        if (nomorScene < 0 ||
            nomorScene >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Build Index tidak valid: " + nomorScene);
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == nomorScene)
        {
            Debug.LogWarning("Scene tujuan sama dengan scene saat ini.");
            return;
        }

        if (panelTransition == null ||
            canvasGroupLoading == null ||
            progressFill == null)
        {
            Debug.LogError("Komponen transisi belum lengkap!");
            return;
        }

        // Kunci status sebelum coroutine dimulai.
        sedangTransisi = true;

        coroutineTransisi = StartCoroutine(
            ProsesTransisi(nomorScene)
        );
    }

    private IEnumerator ProsesTransisi(int nomorScene)
    {
        panelTransition.gameObject.SetActive(true);
        panelTransition.anchoredPosition = posisiAwalPanel;

        canvasGroupLoading.alpha = 0f;
        canvasGroupLoading.interactable = false;
        canvasGroupLoading.blocksRaycasts = true;

        progressFill.fillAmount = 0f;

        // Memainkan suara transisi.
        if (audioSource != null && suaraTransisi != null)
        {
            audioSource.PlayOneShot(suaraTransisi);
        }

        yield return FadeIn();

        // Memulai loading scene.
        AsyncOperation operasi =
            SceneManager.LoadSceneAsync(nomorScene);

        if (operasi == null)
        {
            Debug.LogError("Gagal memuat scene.");
            ResetStatusTransisi();
            yield break;
        }

        operasi.allowSceneActivation = false;

        float waktuLoading = 0f;

        while (progressFill.fillAmount < 1f)
        {
            waktuLoading += Time.unscaledDeltaTime;

            float progressAsli = Mathf.Clamp01(
                operasi.progress / 0.9f
            );

            float progressWaktu = durasiLoadingMinimum > 0f
                ? Mathf.Clamp01(waktuLoading / durasiLoadingMinimum)
                : 1f;

            float target = Mathf.Min(
                progressAsli,
                progressWaktu
            );

            progressFill.fillAmount = Mathf.MoveTowards(
                progressFill.fillAmount,
                target,
                Time.unscaledDeltaTime * 0.8f
            );

            if (operasi.progress >= 0.9f &&
                waktuLoading >= durasiLoadingMinimum)
            {
                progressFill.fillAmount = Mathf.MoveTowards(
                    progressFill.fillAmount,
                    1f,
                    Time.unscaledDeltaTime * 1.5f
                );
            }

            yield return null;
        }

        progressFill.fillAmount = 1f;

        // Mengaktifkan scene tujuan.
        operasi.allowSceneActivation = true;

        while (!operasi.isDone)
        {
            yield return null;
        }

        yield return null;

        // Animasi panel keluar.
        yield return GerakkanPanelKeAtas();

        // Membersihkan status setelah transisi selesai.
        ResetStatusTransisi();

        Debug.Log("Transisi scene selesai.");
    }

    // =========================================
    // FADE IN
    // =========================================

    private IEnumerator FadeIn()
    {
        float waktu = 0f;

        while (waktu < durasiFadeIn)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasiFadeIn > 0f
                ? Mathf.Clamp01(waktu / durasiFadeIn)
                : 1f;

            canvasGroupLoading.alpha =
                Mathf.SmoothStep(0f, 1f, t);

            yield return null;
        }

        canvasGroupLoading.alpha = 1f;
    }

    // =========================================
    // GERAK PANEL KE ATAS
    // =========================================

    private IEnumerator GerakkanPanelKeAtas()
    {
        float waktu = 0f;

        Vector2 posisiAkhir = posisiAwalPanel +
            Vector2.up * (panelTransition.rect.height + 200f);

        while (waktu < durasiGerakKeAtas)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasiGerakKeAtas > 0f
                ? Mathf.Clamp01(waktu / durasiGerakKeAtas)
                : 1f;

            float smooth = Mathf.SmoothStep(0f, 1f, t);

            panelTransition.anchoredPosition = Vector2.Lerp(
                posisiAwalPanel,
                posisiAkhir,
                smooth
            );

            yield return null;
        }

        panelTransition.anchoredPosition = posisiAkhir;
    }

    // =========================================
    // RESET UNIVERSAL
    // =========================================

    public void ResetTransisi()
    {
        // Hentikan coroutine jika masih berjalan.
        if (coroutineTransisi != null)
        {
            StopCoroutine(coroutineTransisi);
            coroutineTransisi = null;
        }

        ResetStatusTransisi();

        Debug.Log("Transisi berhasil di-reset.");
    }

    // Kompatibilitas dengan script lama.
    public void ResetUntukKembaliKeMenu()
    {
        ResetTransisi();
    }

    private void ResetStatusTransisi()
    {
        sedangTransisi = false;
        coroutineTransisi = null;

        ResetTampilanTransisi();
    }

    private void ResetTampilanTransisi()
    {
        if (panelTransition != null)
        {
            panelTransition.anchoredPosition = posisiAwalPanel;
            panelTransition.gameObject.SetActive(false);
        }

        if (canvasGroupLoading != null)
        {
            canvasGroupLoading.alpha = 0f;
            canvasGroupLoading.interactable = false;
            canvasGroupLoading.blocksRaycasts = false;
        }

        if (progressFill != null)
        {
            progressFill.fillAmount = 0f;
        }
    }

    // =========================================
    // MEMBERSIHKAN INSTANCE
    // =========================================

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
