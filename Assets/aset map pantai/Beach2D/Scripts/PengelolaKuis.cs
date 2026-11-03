
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private SampahItem sampahTerpilih;
    private int skor = 0;
    private bool jawabanSudahBenar = false;

    void Start()
    {
        // Periksa komponen penting
        if (panelKuis == null ||
            teksPertanyaan == null ||
            teksFeedback == null ||
            teksSkor == null ||
            tombolOrganik == null ||
            tombolAnorganik == null)
        {
            Debug.LogError(
                "PengelolaKuis: Ada referensi UI yang belum diisi di Inspector!"
            );
            enabled = false;
            return;
        }

        // Sembunyikan panel dan gambar saat game dimulai
        panelKuis.SetActive(false);

        if (gambarSampah != null)
            gambarSampah.gameObject.SetActive(false);

        // Pasang listener tombol
        tombolOrganik.onClick.AddListener(
            () => Jawab(true)
        );

        tombolAnorganik.onClick.AddListener(
            () => Jawab(false)
        );

        PerbaruiSkor();
    }


    public void PilihSampah(SampahItem sampah)
    {
        // Jangan izinkan memilih sampah lain
        // selama panel kuis masih terbuka
        if (panelKuis != null && panelKuis.activeSelf)
            return;

        // Cegah pemilihan objek yang tidak valid
        if (sampah == null || sampah.sudahDipilah)
            return;

        if (jawabanSudahBenar)
            return;

        sampahTerpilih = sampah;

        // Tampilkan gambar sampah
        SpriteRenderer sr = sampah.GetComponent<SpriteRenderer>();

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

        teksPertanyaan.text = "Sampah ini termasuk jenis apa?";
        teksFeedback.text = "";

        // Buka panel kuis
        panelKuis.SetActive(true);

        tombolOrganik.interactable = true;
        tombolAnorganik.interactable = true;
    }


    void Jawab(bool pilihanOrganik)
    {
        // Pastikan ada sampah yang sedang dipilih
        if (sampahTerpilih == null || jawabanSudahBenar)
            return;

        // Periksa jawaban
        if (pilihanOrganik == sampahTerpilih.organik)
        {
            jawabanSudahBenar = true;

            // Tambahkan skor dan selesaikan pemilahan
            skor += 10;
            sampahTerpilih.SelesaikanPemilahan();

            teksFeedback.text = "Benar! +10 poin";

            // Cegah klik berulang
            tombolOrganik.interactable = false;
            tombolAnorganik.interactable = false;

            // Perbarui skor
            PerbaruiSkor();

            // Tutup panel setelah 1 detik
            CancelInvoke(nameof(TutupPanel));
            Invoke(nameof(TutupPanel), 1f);
        }
        else
        {
            teksFeedback.text = "Belum tepat, coba lagi!";
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

        // Bersihkan referensi sampah terpilih
        sampahTerpilih = null;

        // Izinkan pemain memilih sampah berikutnya
        jawabanSudahBenar = false;

        // Bersihkan feedback untuk kuis berikutnya
        if (teksFeedback != null)
            teksFeedback.text = "";
    }

    void OnDestroy()
    {
        // Bersihkan Invoke yang masih tertunda
        CancelInvoke(nameof(TutupPanel));
    }
}
