
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelPeringatanController : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject panelPilihanNPC;

    [Header("Panel Peringatkan")]
    public GameObject panelPeringatkan;
    public Image backgroundGelapPeringatkan;
    public RectTransform gambarPeringatan;
    public RectTransform buttonCobaLagiPeringatkan;

    [Header("Panel Panggil Bantuan")]
    public GameObject panelPanggilBantuan;
    public Image backgroundGelapBantuan;
    public RectTransform gambarBantuan;
    public RectTransform buttonCobaLagiBantuan;

    [Header("Audio Panel")]
    public AudioSource audioSource;
    public AudioClip suaraPilihanNPC;
    public AudioClip suaraPeringatan;
    public AudioClip suaraBantuan;

    [Header("Pengaturan Animasi")]
    public float durasiAnimasi = 0.5f;
    public float jarakTurun = 250f;

    [Range(0f, 1f)]
    public float tingkatGelap = 0.65f;

    private Vector2 posisiGambarPeringatan;
    private Vector2 posisiTombolPeringatan;

    private Vector2 posisiGambarBantuan;
    private Vector2 posisiTombolBantuan;

    private Coroutine animasiPanel;

    private void Awake()
    {
        posisiGambarPeringatan = gambarPeringatan.anchoredPosition;
        posisiTombolPeringatan = buttonCobaLagiPeringatkan.anchoredPosition;

        posisiGambarBantuan = gambarBantuan.anchoredPosition;
        posisiTombolBantuan = buttonCobaLagiBantuan.anchoredPosition;
    }

    private void Start()
    {
        // Panel peringatan dan bantuan disembunyikan saat game dimulai.
        panelPeringatkan.SetActive(false);
        panelPanggilBantuan.SetActive(false);

        // Panel utama tidak dibuka otomatis.
        // Panel utama akan dibuka melalui tombol Quest.
        AturTransparansi(backgroundGelapPeringatkan);
        AturTransparansi(backgroundGelapBantuan);
    }

    private void AturTransparansi(Image background)
    {
        Color warna = background.color;
        warna.a = 0f;
        background.color = warna;
    }

    // =========================
    // TAMPILKAN PANEL UTAMA
    // =========================

    public void TampilkanPilihanNPC()
    {
        panelPilihanNPC.SetActive(true);

        if (audioSource != null && suaraPilihanNPC != null)
        {
            audioSource.PlayOneShot(suaraPilihanNPC);
        }
    }

    // =========================
    // TAMPILKAN PANEL PERINGATAN
    // =========================

    public void TampilkanPeringatan()
    {
        if (panelPeringatkan.activeSelf ||
            panelPanggilBantuan.activeSelf)
            return;

        panelPilihanNPC.SetActive(false);
        panelPeringatkan.SetActive(true);

        if (audioSource != null && suaraPeringatan != null)
        {
            audioSource.PlayOneShot(suaraPeringatan);
        }

        animasiPanel = StartCoroutine(
            AnimasiPanel(
                backgroundGelapPeringatkan,
                gambarPeringatan,
                buttonCobaLagiPeringatkan,
                posisiGambarPeringatan,
                posisiTombolPeringatan
            )
        );
    }

    // =========================
    // TAMPILKAN PANEL BANTUAN
    // =========================

    public void TampilkanPanggilBantuan()
    {
        if (panelPeringatkan.activeSelf ||
            panelPanggilBantuan.activeSelf)
            return;

        panelPilihanNPC.SetActive(false);
        panelPanggilBantuan.SetActive(true);

        if (audioSource != null && suaraBantuan != null)
        {
            audioSource.PlayOneShot(suaraBantuan);
        }

        animasiPanel = StartCoroutine(
            AnimasiPanel(
                backgroundGelapBantuan,
                gambarBantuan,
                buttonCobaLagiBantuan,
                posisiGambarBantuan,
                posisiTombolBantuan
            )
        );
    }

    // =========================
    // ANIMASI BERSAMA
    // =========================

    private IEnumerator AnimasiPanel(
        Image background,
        RectTransform gambar,
        RectTransform tombol,
        Vector2 posisiAkhirGambar,
        Vector2 posisiAkhirTombol)
    {
        Vector2 awalGambar =
            posisiAkhirGambar + Vector2.up * jarakTurun;

        Vector2 awalTombol =
            posisiAkhirTombol + Vector2.up * jarakTurun;

        gambar.anchoredPosition = awalGambar;
        tombol.anchoredPosition = awalTombol;

        Color warna = background.color;
        warna.a = 0f;
        background.color = warna;

        float waktu = 0f;

        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasiAnimasi > 0f
                ? Mathf.Clamp01(waktu / durasiAnimasi)
                : 1f;

            float smooth = Mathf.SmoothStep(0f, 1f, t);

            gambar.anchoredPosition = Vector2.Lerp(
                awalGambar,
                posisiAkhirGambar,
                smooth
            );

            tombol.anchoredPosition = Vector2.Lerp(
                awalTombol,
                posisiAkhirTombol,
                smooth
            );

            warna.a = Mathf.Lerp(0f, tingkatGelap, smooth);
            background.color = warna;

            yield return null;
        }

        gambar.anchoredPosition = posisiAkhirGambar;
        tombol.anchoredPosition = posisiAkhirTombol;

        warna.a = tingkatGelap;
        background.color = warna;

        animasiPanel = null;
    }

    // =========================
    // COBA LAGI
    // =========================

    public void CobaLagi()
    {
        if (animasiPanel != null)
        {
            StopCoroutine(animasiPanel);
            animasiPanel = null;
        }

        panelPeringatkan.SetActive(false);
        panelPanggilBantuan.SetActive(false);

        TampilkanPilihanNPC();
    }
}
