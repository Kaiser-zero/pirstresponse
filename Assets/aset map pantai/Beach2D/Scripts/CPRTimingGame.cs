
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CPRTimingGame : MonoBehaviour
{
    [Header("Panel Gameplay")]
    public GameObject panelGameplay;
    public GameObject panelMenang;
    public GameObject panelKalah;

    [Header("Timing Bar")]
    public RectTransform timingBar;
    public RectTransform indikatorBergerak;
    public RectTransform areaTargetMerah;
    public RectTransform ikonJantung;

    [Header("Teks UI")]
    public TMP_Text teksInstruksi;
    public TMP_Text teksProgress;
    public TMP_Text teksKesalahan;

    [Header("Pengaturan Game")]
    public float kecepatanIndikator = 450f;
    public int targetBerhasil = 10;
    public int batasKesalahan = 3;

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

    void Start()
    {
        // Periksa referensi UI
        if (panelGameplay == null ||
            panelMenang == null ||
            panelKalah == null ||
            timingBar == null ||
            indikatorBergerak == null ||
            areaTargetMerah == null ||
            teksProgress == null ||
            teksKesalahan == null)
        {
            Debug.LogError(
                "CPRTimingGame: Referensi UI belum lengkap!"
            );

            enabled = false;
            return;
        }

        // Sembunyikan panel hasil
        panelMenang.SetActive(false);
        panelKalah.SetActive(false);
        panelGameplay.SetActive(true);

        // Jalankan kedua animasi
        AturKecepatanAnimasi(1f);

        // Persiapan timing bar
        HitungBatasGerak();
        PindahkanTarget();
        PerbaruiUI();
    }

    void Update()
    {
        if (gameSelesai)
            return;

        // Gerakkan indikator putih
        waktu += Time.deltaTime;

        float posisiX = Mathf.PingPong(
            waktu * kecepatanIndikator,
            batasGerak * 2f
        ) - batasGerak;

        Vector2 posisi = indikatorBergerak.anchoredPosition;
        posisi.x = posisiX;

        indikatorBergerak.anchoredPosition = posisi;

        // Ikon jantung mengikuti target merah
        if (ikonJantung != null)
        {
            Vector2 posisiJantung =
                ikonJantung.anchoredPosition;

            posisiJantung.x =
                areaTargetMerah.anchoredPosition.x;

            ikonJantung.anchoredPosition = posisiJantung;
        }

        // Tekan SPACE untuk mengecek timing
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CekTiming();
        }
    }

    void HitungBatasGerak()
    {
        float lebarBar = timingBar.rect.width;

        float lebarIndikator =
            indikatorBergerak.rect.width;

        batasGerak =
            (lebarBar - lebarIndikator) / 2f;

        batasGerak = Mathf.Max(0f, batasGerak);
    }

    void CekTiming()
    {
        if (gameSelesai)
            return;

        float posisiIndikator =
            indikatorBergerak.anchoredPosition.x;

        float posisiTarget =
            areaTargetMerah.anchoredPosition.x;

        float toleransi =
            (areaTargetMerah.rect.width +
             indikatorBergerak.rect.width) / 2f;

        // Periksa ketepatan timing
        if (Mathf.Abs(
            posisiIndikator - posisiTarget
        ) <= toleransi)
        {
            jumlahBerhasil++;

            Debug.Log("Timing berhasil!");
        }
        else
        {
            jumlahKesalahan++;

            Debug.Log("Timing meleset!");

            // Hentikan kedua animasi sementara
            if (!sedangBerhenti)
            {
                StartCoroutine(BerhentiSementara());
            }
        }

        PerbaruiUI();

        // Periksa kemenangan
        if (jumlahBerhasil >= targetBerhasil)
        {
            Menang();
            return;
        }

        // Periksa kekalahan
        if (jumlahKesalahan >= batasKesalahan)
        {
            Kalah();
            return;
        }

        // Pindahkan target merah
        PindahkanTarget();
    }

    IEnumerator BerhentiSementara()
    {
        sedangBerhenti = true;

        AturKecepatanAnimasi(0f);

        yield return new WaitForSeconds(durasiBerhenti);

        // Lanjutkan kedua animasi
        AturKecepatanAnimasi(1f);

        sedangBerhenti = false;
    }

    void PindahkanTarget()
    {
        float setengahLebarTarget =
            areaTargetMerah.rect.width / 2f;

        float posisiAcak = Random.Range(
            -batasGerak + setengahLebarTarget,
            batasGerak - setengahLebarTarget
        );

        Vector2 posisi =
            areaTargetMerah.anchoredPosition;

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

    void Menang()
    {
        gameSelesai = true;

        AturKecepatanAnimasi(1f);

        panelGameplay.SetActive(false);
        panelMenang.SetActive(true);

        Debug.Log("PEMAIN MENANG!");
    }

    void Kalah()
    {
        gameSelesai = true;

        AturKecepatanAnimasi(1f);

        panelGameplay.SetActive(false);
        panelKalah.SetActive(true);

        Debug.Log("PEMAIN KALAH!");
    }

    void AturKecepatanAnimasi(float kecepatan)
    {
        if (animatorKorban != null)
            animatorKorban.speed = kecepatan;

        if (animatorPenyelamat != null)
            animatorPenyelamat.speed = kecepatan;
    }
}
