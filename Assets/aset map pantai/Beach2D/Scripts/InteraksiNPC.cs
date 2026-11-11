
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteraksiNPC : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject panelPenjelasanNPC;
    public Image backgroundGelap;

    [Header("Gambar Penjelasan")]
    public RectTransform gambarPenjelasan;

    [Header("Tombol")]
    public RectTransform buttonBantu;
    public RectTransform buttonPeringatkan;
    public RectTransform buttonPanggilBantuan;

    [Header("Pengaturan Animasi")]
    public float durasiAnimasi = 0.5f;
    public float jarakTurun = 250f;

    [Range(0f, 1f)]
    public float tingkatGelap = 0.65f;

    private Vector2 posisiGambar;
    private Vector2 posisiBantu;
    private Vector2 posisiPeringatkan;
    private Vector2 posisiPanggilBantuan;

    private bool panelTerbuka = false;
    private Coroutine animasiPanel;

    void Awake()
    {
        // Simpan posisi akhir semua elemen
        posisiGambar = gambarPenjelasan.anchoredPosition;
        posisiBantu = buttonBantu.anchoredPosition;
        posisiPeringatkan = buttonPeringatkan.anchoredPosition;
        posisiPanggilBantuan = buttonPanggilBantuan.anchoredPosition;
    }

    void Start()
    {
        panelPenjelasanNPC.SetActive(false);

        Color warna = backgroundGelap.color;
        warna.a = 0f;
        backgroundGelap.color = warna;
    }

    void OnMouseDown()
    {
        BukaPanel();
    }

    public void BukaPanel()
    {
        if (panelTerbuka)
            return;

        panelTerbuka = true;
        panelPenjelasanNPC.SetActive(true);

        animasiPanel = StartCoroutine(AnimasiPanel());
    }

    IEnumerator AnimasiPanel()
    {
        // Tentukan posisi awal: semua elemen berada di atas
        Vector2 awalGambar = posisiGambar + Vector2.up * jarakTurun;
        Vector2 awalBantu = posisiBantu + Vector2.up * jarakTurun;
        Vector2 awalPeringatkan = posisiPeringatkan + Vector2.up * jarakTurun;
        Vector2 awalPanggil = posisiPanggilBantuan + Vector2.up * jarakTurun;

        gambarPenjelasan.anchoredPosition = awalGambar;
        buttonBantu.anchoredPosition = awalBantu;
        buttonPeringatkan.anchoredPosition = awalPeringatkan;
        buttonPanggilBantuan.anchoredPosition = awalPanggil;

        // Background mulai transparan
        Color warna = backgroundGelap.color;
        warna.a = 0f;
        backgroundGelap.color = warna;

        float waktu = 0f;

        // Semua elemen bergerak secara bersamaan
        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasiAnimasi > 0f
                ? Mathf.Clamp01(waktu / durasiAnimasi)
                : 1f;

            float smooth = Mathf.SmoothStep(0f, 1f, t);

            gambarPenjelasan.anchoredPosition =
                Vector2.Lerp(awalGambar, posisiGambar, smooth);

            buttonBantu.anchoredPosition =
                Vector2.Lerp(awalBantu, posisiBantu, smooth);

            buttonPeringatkan.anchoredPosition =
                Vector2.Lerp(awalPeringatkan, posisiPeringatkan, smooth);

            buttonPanggilBantuan.anchoredPosition =
                Vector2.Lerp(awalPanggil, posisiPanggilBantuan, smooth);

            // Background ikut menggelap
            warna.a = Mathf.Lerp(0f, tingkatGelap, smooth);
            backgroundGelap.color = warna;

            yield return null;
        }

        // Pastikan semua posisi tepat di posisi akhir
        gambarPenjelasan.anchoredPosition = posisiGambar;
        buttonBantu.anchoredPosition = posisiBantu;
        buttonPeringatkan.anchoredPosition = posisiPeringatkan;
        buttonPanggilBantuan.anchoredPosition = posisiPanggilBantuan;

        warna.a = tingkatGelap;
        backgroundGelap.color = warna;

        animasiPanel = null;
    }

    // Pilihan tombol

    public void PilihBantu()
    {
        Debug.Log("Player memilih BANTU");
        TutupPanel();
    }

    public void PilihPeringatkan()
    {
        Debug.Log("Player memilih PERINGATKAN");
        TutupPanel();
    }

    public void PilihPanggilBantuan()
    {
        Debug.Log("Player memilih PANGGIL BANTUAN");
        TutupPanel();
    }

    public void TutupPanel()
    {
        if (animasiPanel != null)
        {
            StopCoroutine(animasiPanel);
            animasiPanel = null;
        }

        panelTerbuka = false;
        panelPenjelasanNPC.SetActive(false);
    }
}
