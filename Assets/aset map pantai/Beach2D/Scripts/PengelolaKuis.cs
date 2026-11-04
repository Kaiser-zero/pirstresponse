
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

    [Header("Bintang")]
    public Image bintang1;
    public Image bintang2;
    public Image bintang3;

    public Sprite bintangAktif;
    public Sprite bintangNonaktif;

    [Header("Scene Map Pantai")]
    public string namaSceneMapPantai = "NamaSceneMap";

    private SampahItem sampahTerpilih;

    private int skor = 0;
    private int totalSampah = 0;
    private int jumlahSampahSelesai = 0;

    private bool jawabanSudahBenar = false;

    void Start()
    {
        // Periksa komponen penting kuis
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

        // Hitung seluruh sampah di scene
        totalSampah = FindObjectsByType<SampahItem>(
            FindObjectsInactive.Include
        ).Length;

        Debug.Log("Total sampah: " + totalSampah);

        // Sembunyikan panel kuis
        panelKuis.SetActive(false);

        if (gambarSampah != null)
            gambarSampah.gameObject.SetActive(false);

        // Sembunyikan menu selesai
        if (panelSelesai != null)
            panelSelesai.SetActive(false);

        // Pasang listener tombol kuis
        tombolOrganik.onClick.AddListener(
            () => Jawab(true)
        );

        tombolAnorganik.onClick.AddListener(
            () => Jawab(false)
        );

        // Pasang listener tombol kembali
        if (tombolKembaliPantai != null)
        {
            tombolKembaliPantai.onClick.AddListener(
                KembaliKePantai
            );
        }

        PerbaruiSkor();
    }

    public void PilihSampah(SampahItem sampah)
    {
        // Jangan izinkan memilih sampah lain
        // selama panel kuis masih terbuka
        if (panelKuis != null && panelKuis.activeSelf)
            return;

        if (sampah == null || sampah.sudahDipilah)
            return;

        if (jawabanSudahBenar)
            return;

        sampahTerpilih = sampah;

        // Tampilkan gambar sampah
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

        // Buka panel kuis
        panelKuis.SetActive(true);

        tombolOrganik.interactable = true;
        tombolAnorganik.interactable = true;
    }

    void Jawab(bool pilihanOrganik)
    {
        if (sampahTerpilih == null || jawabanSudahBenar)
            return;

        // Periksa jawaban
        if (pilihanOrganik == sampahTerpilih.organik)
        {
            jawabanSudahBenar = true;

            // Tambahkan skor
            skor += 10;
            jumlahSampahSelesai++;

            // Selesaikan sampah
            sampahTerpilih.SelesaikanPemilahan();

            teksFeedback.text = "Benar! +10 poin";

            // Cegah klik berulang
            tombolOrganik.interactable = false;
            tombolAnorganik.interactable = false;

            PerbaruiSkor();

            // Tutup panel setelah 1 detik
            CancelInvoke(nameof(TutupPanel));
            Invoke(nameof(TutupPanel), 1f);
        }
        else
        {
            teksFeedback.text =
                "Belum tepat, coba lagi!";
        }
    }

    void PerbaruiSkor()
    {
        if (teksSkor != null)
            teksSkor.text = "Skor: " + skor;
    }

    void TutupPanel()
    {
        if (panelKuis != null)
            panelKuis.SetActive(false);

        if (gambarSampah != null)
            gambarSampah.gameObject.SetActive(false);

        // Bersihkan referensi sampah
        sampahTerpilih = null;

        jawabanSudahBenar = false;

        if (teksFeedback != null)
            teksFeedback.text = "";

        // Periksa apakah semua sampah selesai
        if (totalSampah > 0 &&
            jumlahSampahSelesai >= totalSampah)
        {
            TampilkanMenuSelesai();
        }
    }

    void TampilkanMenuSelesai()
    {
        // Tampilkan panel selesai
        if (panelSelesai != null)
            panelSelesai.SetActive(true);

        // Hitung persentase skor
        float persentase =
            (float)skor / (totalSampah * 10) * 100f;

        int jumlahBintang = 0;

        if (persentase >= 30f)
            jumlahBintang = 1;

        if (persentase >= 60f)
            jumlahBintang = 2;

        if (persentase >= 90f)
            jumlahBintang = 3;

        // Atur tampilan bintang
        AturBintang(bintang1, jumlahBintang >= 1);
        AturBintang(bintang2, jumlahBintang >= 2);
        AturBintang(bintang3, jumlahBintang >= 3);

        // Tampilkan skor akhir
        if (teksSkorAkhir != null)
        {
            teksSkorAkhir.text =
                skor + " / " + (totalSampah * 10);
        }

        // Tampilkan apresiasi
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

    void AturBintang(Image bintang, bool aktif)
    {
        if (bintang == null)
            return;

        bintang.sprite =
            aktif ? bintangAktif : bintangNonaktif;

        bintang.gameObject.SetActive(true);
        bintang.preserveAspect = true;
    }

    void KembaliKePantai()
    {
        SceneManager.LoadScene(namaSceneMapPantai);
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(TutupPanel));
    }
}
