
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PengelolaKuis : MonoBehaviour
{
    [Header("Panel Kuis")]
    public GameObject panelKuis;
    public TMP_Text teksPertanyaan;
    public TMP_Text teksFeedback;
    public TMP_Text teksSkor;
    public Button tombolOrganik;
    public Button tombolAnorganik;

    [Header("Gambar Sampah")]
    public Image gambarSampah;

    [Header("Menu Selesai")]
    public GameObject panelSelesai;
    public TMP_Text teksSkorAkhir;
    public TMP_Text teksApresiasi;
    public Button tombolKembaliPantai;

    [Header("Animasi Pop Up Panel Selesai")]
    public float durasiPopUp = 0.4f;
    public float skalaAwal = 0f;
    public float skalaMaksimal = 1.15f;

    [Header("Bintang")]
    public Image bintang1;
    public Image bintang2;
    public Image bintang3;

    public Sprite bintangAktif;
    public Sprite bintangNonaktif;

    [Header("Scene Map Pantai")]
    public string namaSceneMapPantai = "NamaSceneMap";

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip suaraPilihSampah;
    public AudioClip suaraKlikJawaban;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;
    public AudioClip suaraPopUpSelesai;

    private SampahItem sampahTerpilih;

    private int skor = 0;
    private int totalSampah = 0;
    private int jumlahSampahSelesai = 0;

    private bool jawabanSudahBenar = false;
    private bool pernahSalah = false;

    private Coroutine animasiPopUp;

    void Start()
    {
        if (panelKuis == null ||
            teksPertanyaan == null ||
            teksFeedback == null ||
            teksSkor == null ||
            tombolOrganik == null ||
            tombolAnorganik == null)
        {
            Debug.LogError(
                "PengelolaKuis: Referensi UI kuis belum lengkap!"
            );

            enabled = false;
            return;
        }

        // Hitung seluruh sampah di scene.
        totalSampah = FindObjectsByType<SampahItem>(
            FindObjectsInactive.Include
        ).Length;

        Debug.Log("Total sampah: " + totalSampah);

        panelKuis.SetActive(false);

        if (gambarSampah != null)
            gambarSampah.gameObject.SetActive(false);

        // Sembunyikan menu selesai.
        if (panelSelesai != null)
        {
            panelSelesai.SetActive(false);
            panelSelesai.transform.localScale =
                Vector3.one * skalaAwal;
        }

        // Pasang listener tombol kuis.
        tombolOrganik.onClick.AddListener(
            () => Jawab(true)
        );

        tombolAnorganik.onClick.AddListener(
            () => Jawab(false)
        );

        // Pasang listener tombol kembali.
        if (tombolKembaliPantai != null)
        {
            tombolKembaliPantai.onClick.AddListener(
                KembaliKePantai
            );
        }

        PerbaruiSkor();
    }

    // =========================
    // PUTAR SOUND EFFECT
    // =========================

    void PutarSuara(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // =========================
    // PILIH SAMPAH
    // =========================

    public void PilihSampah(SampahItem sampah)
    {
        if (panelKuis != null && panelKuis.activeSelf)
            return;

        if (sampah == null || sampah.sudahDipilah)
            return;

        if (jawabanSudahBenar)
            return;

        sampahTerpilih = sampah;

        // Reset status kesalahan untuk sampah baru.
        pernahSalah = false;

        PutarSuara(suaraPilihSampah);

        // Tampilkan gambar sampah.
        SpriteRenderer sr =
            sampah.GetComponent<SpriteRenderer>();

        if (gambarSampah != null)
        {
            if (sr != null && sr.sprite != null)
            {
                gambarSampah.sprite = sr.sprite;
                gambarSampah.preserveAspect = true;
                gambarSampah.gameObject.SetActive(true);
            }
            else
            {
                gambarSampah.sprite = null;
                gambarSampah.gameObject.SetActive(false);
            }
        }

        teksPertanyaan.text =
            "Sampah ini termasuk jenis apa?";

        teksFeedback.text = "";

        panelKuis.SetActive(true);

        tombolOrganik.interactable = true;
        tombolAnorganik.interactable = true;
    }

    // =========================
    // JAWAB PERTANYAAN
    // =========================

    void Jawab(bool pilihanOrganik)
    {
        if (sampahTerpilih == null || jawabanSudahBenar)
            return;

        PutarSuara(suaraKlikJawaban);

        if (pilihanOrganik == sampahTerpilih.organik)
        {
            jawabanSudahBenar = true;

            // Poin hanya diberikan jika belum pernah salah.
            if (!pernahSalah)
            {
                skor += 10;
                teksFeedback.text = "Benar! +10 poin";
            }
            else
            {
                teksFeedback.text =
                    "Benar! Tapi tidak mendapat poin.";
            }

            jumlahSampahSelesai++;

            sampahTerpilih.SelesaikanPemilahan();

            PutarSuara(suaraBenar);

            tombolOrganik.interactable = false;
            tombolAnorganik.interactable = false;

            PerbaruiSkor();

            CancelInvoke(nameof(TutupPanel));
            Invoke(nameof(TutupPanel), 1f);
        }
        else
        {
            // Kurangi 10 poin setiap jawaban salah.
            skor = Mathf.Max(0, skor - 10);

            pernahSalah = true;

            PutarSuara(suaraSalah);

            teksFeedback.text = "Belum tepat! -10 poin";

            PerbaruiSkor();
        }
    }

    // =========================
    // PERBARUI SKOR
    // =========================

    void PerbaruiSkor()
    {
        if (teksSkor != null)
            teksSkor.text = "Skor: " + skor;
    }

    // =========================
    // TUTUP PANEL KUIS
    // =========================

    void TutupPanel()
    {
        if (panelKuis != null)
            panelKuis.SetActive(false);

        if (gambarSampah != null)
            gambarSampah.gameObject.SetActive(false);

        sampahTerpilih = null;

        jawabanSudahBenar = false;
        pernahSalah = false;

        if (teksFeedback != null)
            teksFeedback.text = "";

        // Periksa apakah semua sampah sudah selesai.
        if (totalSampah > 0 &&
            jumlahSampahSelesai >= totalSampah)
        {
            TampilkanMenuSelesai();
        }
    }

    // =========================
    // TAMPILKAN MENU SELESAI
    // =========================

    void TampilkanMenuSelesai()
    {
        if (panelSelesai != null)
        {
            panelSelesai.SetActive(true);

            // Putar suara saat panel selesai muncul.
            PutarSuara(suaraPopUpSelesai);

            // Mulai animasi pop-up.
            if (animasiPopUp != null)
                StopCoroutine(animasiPopUp);

            animasiPopUp = StartCoroutine(
                AnimasiPopUpPanel()
            );
        }

        // Hitung persentase skor.
        float persentase =
            (float)skor / (totalSampah * 10) * 100f;

        int jumlahBintang = 0;

        if (persentase >= 30f)
            jumlahBintang = 1;

        if (persentase >= 60f)
            jumlahBintang = 2;

        if (persentase >= 90f)
            jumlahBintang = 3;

        // Atur tampilan bintang.
        AturBintang(bintang1, jumlahBintang >= 1);
        AturBintang(bintang2, jumlahBintang >= 2);
        AturBintang(bintang3, jumlahBintang >= 3);

        // Tampilkan skor akhir.
        if (teksSkorAkhir != null)
        {
            teksSkorAkhir.text =
                skor + " / " + (totalSampah * 10);
        }

        // Tampilkan apresiasi.
        if (teksApresiasi != null)
        {
            if (jumlahBintang == 3)
                teksApresiasi.text =
                    "Luar biasa! Kamu hebat!";

            else if (jumlahBintang == 2)
                teksApresiasi.text =
                    "Bagus! Terus tingkatkan!";

            else if (jumlahBintang == 1)
                teksApresiasi.text =
                    "Bagus! Tetap semangat!";

            else
                teksApresiasi.text =
                    "Ayo belajar lagi!";
        }

        Debug.Log(
            "LEVEL SELESAI! Skor: " + skor +
            " | Bintang: " + jumlahBintang
        );
    }

    // =========================
    // ANIMASI POP UP
    // =========================

    IEnumerator AnimasiPopUpPanel()
    {
        Transform target = panelSelesai.transform;

        Vector3 skalaNormal = Vector3.one;
        Vector3 skalaKecil = Vector3.one * skalaAwal;
        Vector3 skalaBesar = Vector3.one * skalaMaksimal;

        target.localScale = skalaKecil;

        // Tahap pertama: membesar.
        float waktu = 0f;
        float durasi = durasiPopUp * 0.7f;

        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasi > 0f
                ? Mathf.Clamp01(waktu / durasi)
                : 1f;

            float smooth = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(
                skalaKecil,
                skalaBesar,
                smooth
            );

            yield return null;
        }

        // Tahap kedua: kembali ke ukuran normal.
        waktu = 0f;
        durasi = durasiPopUp * 0.3f;

        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;

            float t = durasi > 0f
                ? Mathf.Clamp01(waktu / durasi)
                : 1f;

            float smooth = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(
                skalaBesar,
                skalaNormal,
                smooth
            );

            yield return null;
        }

        target.localScale = skalaNormal;
        animasiPopUp = null;
    }

    // =========================
    // ATUR BINTANG
    // =========================

    void AturBintang(Image bintang, bool aktif)
    {
        if (bintang == null)
            return;

        bintang.sprite =
            aktif ? bintangAktif : bintangNonaktif;

        bintang.gameObject.SetActive(true);
        bintang.preserveAspect = true;
    }

    // =========================
    // KEMBALI KE MAP PANTAI
    // =========================

    void KembaliKePantai()
    {
        SceneManager.LoadScene(namaSceneMapPantai);
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(TutupPanel));

        if (animasiPopUp != null)
            StopCoroutine(animasiPopUp);
    }
}
