
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
        // Simpan posisi akhir panel peringatan
        posisiGambarPeringatan = gambarPeringatan.anchoredPosition;
        posisiTombolPeringatan = buttonCobaLagiPeringatkan.anchoredPosition;

        // Simpan posisi akhir panel bantuan
        posisiGambarBantuan = gambarBantuan.anchoredPosition;
        posisiTombolBantuan = buttonCobaLagiBantuan.anchoredPosition;
    }

    private void Start()
    {
        panelPeringatkan.SetActive(false);
        panelPanggilBantuan.SetActive(false);

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
    // TAMPILKAN PANEL PERINGATAN
    // =========================

    public void TampilkanPeringatan()
    {
        if (panelPeringatkan.activeSelf ||
            panelPanggilBantuan.activeSelf)
            return;

        panelPilihanNPC.SetActive(false);
        panelPeringatkan.SetActive(true);

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

            // Gambar dan tombol turun bersamaan
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

            // Background menggelap
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

        // Kembalikan panel pilihan
        panelPilihanNPC.SetActive(true);
    }
}
