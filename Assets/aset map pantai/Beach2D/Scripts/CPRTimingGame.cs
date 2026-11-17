
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CPRTimingGame : MonoBehaviour
{
    [Header("Panel Gameplay")]
    public GameObject panelGameplay;

    [Header("Panel Hasil Menang")]
    public GameObject panelHasilMenang;
    public TMP_Text teksSkorMenang;
    public TMP_Text teksBerhasilMenang;
    public TMP_Text teksKesalahanMenang;
    public TMP_Text teksApresiasiMenang;

    [Header("Panel Hasil Kalah")]
    public GameObject panelHasilKalah;
    public TMP_Text teksSkorKalah;
    public TMP_Text teksBerhasilKalah;
    public TMP_Text teksKesalahanKalah;
    public TMP_Text teksApresiasiKalah;

    [Header("Animasi Panel Hasil")]
    public float jarakJatuh = 500f;
    public float durasiJatuh = 0.6f;

    [Header("Timing Bar")]
    public RectTransform timingBar;
    public RectTransform indikatorBergerak;
    public RectTransform areaTargetMerah;
    public RectTransform ikonJantung;

    [Header("Teks UI Gameplay")]
    public TMP_Text teksInstruksi;
    public TMP_Text teksProgress;
    public TMP_Text teksKesalahan;

    [Header("Pengaturan Game")]
    public float kecepatanIndikator = 450f;
    public int targetBerhasil = 10;
    public int batasKesalahan = 3;
    public int poinPerKeberhasilan = 100;

    [Header("Feedback Salah")]
    public GameObject frameSalah;
    public float durasiFrameSalah = 0.5f;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip suaraSalah;
    public AudioClip suaraBenar;
    public AudioClip suaraPanelHasil;

    [Header("Karakter CPR")]
    public GameObject objekKorban;
    public GameObject objekPenyelamat;

    [Header("Animasi CPR")]
    public Animator animatorKorban;
    public Animator animatorPenyelamat;
    public float durasiBerhenti = 0.5f;

    private bool sedangBerhenti = false;
    private int jumlahBerhasil = 0;
    private int jumlahKesalahan = 0;

    private float waktu = 0f;
    private float batasGerak = 0f;

    private bool gameSelesai = false;

    private Vector2 posisiAsliPanelMenang;
    private Vector2 posisiAsliPanelKalah;

    private Coroutine coroutineFrameSalah;
    private Coroutine coroutinePanelHasil;

    void Awake()
    {
        // Simpan posisi asli kedua panel sebelum disembunyikan.
        if (panelHasilMenang != null)
        {
            RectTransform rect =
                panelHasilMenang.GetComponent<RectTransform>();

            if (rect != null)
                posisiAsliPanelMenang = rect.anchoredPosition;
        }

        if (panelHasilKalah != null)
        {
            RectTransform rect =
                panelHasilKalah.GetComponent<RectTransform>();

            if (rect != null)
                posisiAsliPanelKalah = rect.anchoredPosition;
        }
    }

    void Start()
    {
        if (panelGameplay == null ||
            panelHasilMenang == null ||
            panelHasilKalah == null ||
            timingBar == null ||
            indikatorBergerak == null ||
            areaTargetMerah == null ||
            teksProgress == null ||
            teksKesalahan == null)
        {
            Debug.LogError("CPRTimingGame: Referensi UI belum lengkap!");
            enabled = false;
            return;
        }

        // Tampilkan gameplay dan sembunyikan kedua panel hasil.
        panelGameplay.SetActive(true);
        panelHasilMenang.SetActive(false);
        panelHasilKalah.SetActive(false);

        if (frameSalah != null)
            frameSalah.SetActive(false);

        if (objekKorban != null)
            objekKorban.SetActive(true);

        if (objekPenyelamat != null)
            objekPenyelamat.SetActive(true);

        AturKecepatanAnimasi(1f);

        HitungBatasGerak();
        PindahkanTarget();
        PerbaruiUI();
    }

    void Update()
    {
        if (gameSelesai || sedangBerhenti)
            return;

        // Gerakkan indikator putih.
        waktu += Time.deltaTime;

        float posisiX = Mathf.PingPong(
            waktu * kecepatanIndikator,
            batasGerak * 2f
        ) - batasGerak;

        Vector2 posisi = indikatorBergerak.anchoredPosition;
        posisi.x = posisiX;
        indikatorBergerak.anchoredPosition = posisi;

        // Ikon jantung mengikuti target merah.
        if (ikonJantung != null)
        {
            Vector2 posisiJantung = ikonJantung.anchoredPosition;
            posisiJantung.x = areaTargetMerah.anchoredPosition.x;
            ikonJantung.anchoredPosition = posisiJantung;
        }

        // Tekan SPACE untuk mengecek timing.
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CekTiming();
        }
    }

    void HitungBatasGerak()
    {
        float lebarBar = timingBar.rect.width;
        float lebarIndikator = indikatorBergerak.rect.width;

        batasGerak = (lebarBar - lebarIndikator) / 2f;
        batasGerak = Mathf.Max(0f, batasGerak);
    }

    void CekTiming()
    {
        if (gameSelesai || sedangBerhenti)
            return;

        float posisiIndikator =
            indikatorBergerak.anchoredPosition.x;

        float posisiTarget =
            areaTargetMerah.anchoredPosition.x;

        float toleransi =
            (areaTargetMerah.rect.width +
             indikatorBergerak.rect.width) / 2f;

        if (Mathf.Abs(posisiIndikator - posisiTarget) <= toleransi)
        {
            jumlahBerhasil++;

            PutarSuara(suaraBenar);

            Debug.Log("Timing berhasil!");
        }
        else
        {
            jumlahKesalahan++;

            PutarSuara(suaraSalah);

            Debug.Log("Timing meleset!");

            if (frameSalah != null)
            {
                if (coroutineFrameSalah != null)
                    StopCoroutine(coroutineFrameSalah);

                coroutineFrameSalah =
                    StartCoroutine(TampilkanFrameSalah());
            }

            StartCoroutine(BerhentiSementara());
        }

        PerbaruiUI();

        if (jumlahBerhasil >= targetBerhasil)
        {
            SelesaikanGame(true);
            return;
        }

        if (jumlahKesalahan >= batasKesalahan)
        {
            SelesaikanGame(false);
            return;
        }

        PindahkanTarget();
    }

    void PutarSuara(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    IEnumerator TampilkanFrameSalah()
    {
        frameSalah.SetActive(true);

        yield return new WaitForSeconds(durasiFrameSalah);

        frameSalah.SetActive(false);
        coroutineFrameSalah = null;
    }

    IEnumerator BerhentiSementara()
    {
        sedangBerhenti = true;

        AturKecepatanAnimasi(0f);

        yield return new WaitForSeconds(durasiBerhenti);

        if (!gameSelesai)
            AturKecepatanAnimasi(1f);

        sedangBerhenti = false;
    }

    void PindahkanTarget()
    {
        float setengahLebarBar = timingBar.rect.width / 2f;
        float setengahLebarTarget = areaTargetMerah.rect.width / 2f;

        float jarakAman = 40f;

        float batasKiri =
            -setengahLebarBar + jarakAman + setengahLebarTarget;

        float batasKanan =
            setengahLebarBar - jarakAman - setengahLebarTarget;

        if (batasKiri > batasKanan)
        {
            Debug.LogWarning(
                "Area target terlalu lebar untuk timing bar!"
            );
            return;
        }

        float posisiAcak = Random.Range(batasKiri, batasKanan);

        Vector2 posisi = areaTargetMerah.anchoredPosition;
        posisi.x = posisiAcak;
        areaTargetMerah.anchoredPosition = posisi;
    }

    void PerbaruiUI()
    {
        teksProgress.text =
            "Berhasil: " + jumlahBerhasil +
            " / " + targetBerhasil;

        teksKesalahan.text =
            "Kesalahan: " + jumlahKesalahan +
            " / " + batasKesalahan;
    }

    void SelesaikanGame(bool menang)
    {
        gameSelesai = true;

        AturKecepatanAnimasi(0f);

        // Hentikan feedback salah jika masih berjalan.
        if (coroutineFrameSalah != null)
        {
            StopCoroutine(coroutineFrameSalah);
            coroutineFrameSalah = null;
        }

        if (frameSalah != null)
            frameSalah.SetActive(false);

        // Sembunyikan karakter gameplay.
        if (objekKorban != null)
            objekKorban.SetActive(false);

        if (objekPenyelamat != null)
            objekPenyelamat.SetActive(false);

        // Sembunyikan kedua panel hasil terlebih dahulu.
        panelHasilMenang.SetActive(false);
        panelHasilKalah.SetActive(false);

        // Hitung skor.
        int skorAkhir = jumlahBerhasil * poinPerKeberhasilan;

        RectTransform panelTerpilih;
        Vector2 posisiAsliPanel;

        if (menang)
        {
            panelTerpilih =
                panelHasilMenang.GetComponent<RectTransform>();

            posisiAsliPanel = posisiAsliPanelMenang;

            IsiTeksHasil(
                teksSkorMenang,
                teksBerhasilMenang,
                teksKesalahanMenang,
                teksApresiasiMenang,
                skorAkhir,
                true
            );

            panelHasilMenang.SetActive(true);
        }
        else
        {
            panelTerpilih =
                panelHasilKalah.GetComponent<RectTransform>();

            posisiAsliPanel = posisiAsliPanelKalah;

            IsiTeksHasil(
                teksSkorKalah,
                teksBerhasilKalah,
                teksKesalahanKalah,
                teksApresiasiKalah,
                skorAkhir,
                false
            );

            panelHasilKalah.SetActive(true);
        }

        // Putar suara panel hasil.
        PutarSuara(suaraPanelHasil);

        // Jalankan animasi panel yang terpilih.
        if (panelTerpilih != null)
        {
            coroutinePanelHasil = StartCoroutine(
                AnimasiPanelHasil(panelTerpilih, posisiAsliPanel)
            );
        }
        else
        {
            Debug.LogWarning(
                "Panel hasil belum memiliki RectTransform!"
            );
        }

        Debug.Log(
            (menang ? "CPR BERHASIL!" : "CPR GAGAL!") +
            " Skor akhir: " + skorAkhir
        );
    }

    void IsiTeksHasil(
        TMP_Text teksSkor,
        TMP_Text teksBerhasil,
        TMP_Text teksKesalahan,
        TMP_Text teksApresiasi,
        int skor,
        bool menang)
    {
        if (teksSkor != null)
            teksSkor.text = skor.ToString();

        if (teksBerhasil != null)
        {
            teksBerhasil.text =
                " " + jumlahBerhasil + " / " + targetBerhasil;
        }

        if (teksKesalahan != null)
        {
            teksKesalahan.text =
                " " + jumlahKesalahan + " / " + batasKesalahan;
        }

        if (teksApresiasi != null)
        {
            teksApresiasi.text = menang
                ? "Luar biasa! Korban berhasil diselamatkan!"
                : "Jangan menyerah! Ayo coba lagi!";
        }
    }

    IEnumerator AnimasiPanelHasil(
        RectTransform panel,
        Vector2 posisiAsli)
    {
        Vector2 posisiAwal =
            posisiAsli + Vector2.up * jarakJatuh;

        panel.anchoredPosition = posisiAwal;

        float waktuAnimasi = 0f;

        while (waktuAnimasi < durasiJatuh)
        {
            waktuAnimasi += Time.unscaledDeltaTime;

            float t = durasiJatuh > 0f
                ? Mathf.Clamp01(waktuAnimasi / durasiJatuh)
                : 1f;

            float smooth = 1f - Mathf.Pow(1f - t, 3f);

            panel.anchoredPosition = Vector2.Lerp(
                posisiAwal,
                posisiAsli,
                smooth
            );

            yield return null;
        }

        panel.anchoredPosition = posisiAsli;
        coroutinePanelHasil = null;
    }

    void AturKecepatanAnimasi(float kecepatan)
    {
        if (animatorKorban != null)
            animatorKorban.speed = kecepatan;

        if (animatorPenyelamat != null)
            animatorPenyelamat.speed = kecepatan;
    }
}
