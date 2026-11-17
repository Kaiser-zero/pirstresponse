
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchingGameManager : MonoBehaviour
{
    [System.Serializable]
    public class DataKartu
    {
        public Sprite gambarDepan;
    }

    [Header("Pengaturan Kartu")]
    public Transform gridKartu;
    public Sprite gambarBelakang;
    public List<DataKartu> daftarKartu = new List<DataKartu>();

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip soundKlik;
    public AudioClip soundBenar;
    public AudioClip soundSalah;
    public AudioClip soundHasilMenang;
    public AudioClip soundHasilKalah;

    [Header("Animasi")]
    [Min(0.05f)] public float durasiBalik = 0.3f;
    [Min(0f)] public float jedaSalah = 0.7f;

    [Header("Animasi Popup Hasil")]
    public float durasiPopup = 0.35f;
    public float skalaAwalPopup = 0.7f;
    public float skalaOvershoot = 1.1f;

    [Header("UI")]
    public TMP_Text teksWaktu;
    public TMP_Text teksSkor;
    public GameObject panelHasil;
    public TMP_Text teksHasil;

    [Header("Pengaturan Game")]
    [Min(1f)] public float durasiGame = 60f;

    private readonly List<MatchingCard> kartuAktif = new List<MatchingCard>();

    private MatchingCard kartuPertama;
    private MatchingCard kartuKedua;

    private int skor;
    private int pasanganDitemukan;
    private float waktuTersisa;

    private bool sedangMemeriksa;
    private bool gameSelesai;
    private bool kartuSiap;

    void Start()
    {
        waktuTersisa = durasiGame;
        skor = 0;
        pasanganDitemukan = 0;

        if (panelHasil != null)
            panelHasil.SetActive(false);

        kartuSiap = BuatKartu();
        PerbaruiUI();
    }

    void Update()
    {
        if (gameSelesai || !kartuSiap)
            return;

        waktuTersisa -= Time.deltaTime;

        if (waktuTersisa <= 0f)
        {
            waktuTersisa = 0f;
            SelesaikanGame(false);
        }

        PerbaruiUI();
    }

    bool BuatKartu()
    {
        if (gridKartu == null)
        {
            Debug.LogError("Grid Kartu belum diisi!", this);
            return false;
        }

        if (daftarKartu == null || daftarKartu.Count == 0)
        {
            Debug.LogError("Daftar Kartu masih kosong!", this);
            return false;
        }

        if (gambarBelakang == null)
        {
            Debug.LogError("Gambar Belakang belum diisi!", this);
            return false;
        }

        int jumlahKartu = daftarKartu.Count * 2;

        if (gridKartu.childCount != jumlahKartu)
        {
            Debug.LogError(
                "Jumlah kartu harus " + jumlahKartu +
                ", tetapi ditemukan " + gridKartu.childCount + ".",
                this
            );
            return false;
        }

        for (int i = 0; i < daftarKartu.Count; i++)
        {
            if (daftarKartu[i] == null ||
                daftarKartu[i].gambarDepan == null)
            {
                Debug.LogError(
                    "Sprite depan pada elemen ke-" + i + " belum diisi!",
                    this
                );
                return false;
            }
        }

        // Buat dua kartu untuk setiap sprite.
        List<int> urutanAcak = new List<int>();

        for (int i = 0; i < daftarKartu.Count; i++)
        {
            urutanAcak.Add(i);
            urutanAcak.Add(i);
        }

        // Acak urutan kartu.
        for (int i = 0; i < urutanAcak.Count; i++)
        {
            int randomIndex = Random.Range(i, urutanAcak.Count);

            int temp = urutanAcak[i];
            urutanAcak[i] = urutanAcak[randomIndex];
            urutanAcak[randomIndex] = temp;
        }

        // Validasi seluruh kartu terlebih dahulu.
        for (int i = 0; i < gridKartu.childCount; i++)
        {
            Transform objekKartu = gridKartu.GetChild(i);
            Transform childDepan = objekKartu.Find("gambardepan");

            if (objekKartu.GetComponent<Image>() == null ||
                objekKartu.GetComponent<Button>() == null)
            {
                Debug.LogError(
                    objekKartu.name + " harus memiliki Image dan Button.",
                    objekKartu
                );
                return false;
            }

            if (childDepan == null ||
                childDepan.GetComponent<Image>() == null)
            {
                Debug.LogError(
                    objekKartu.name +
                    " harus memiliki Child bernama gambardepan dengan komponen Image.",
                    objekKartu
                );
                return false;
            }
        }

        kartuAktif.Clear();

        // Pasangkan sprite ke kartu.
        for (int i = 0; i < gridKartu.childCount; i++)
        {
            Transform objekKartu = gridKartu.GetChild(i);

            MatchingCard kartu =
                objekKartu.GetComponent<MatchingCard>();

            if (kartu == null)
                kartu = objekKartu.gameObject.AddComponent<MatchingCard>();

            Image imageBelakang = objekKartu.GetComponent<Image>();
            Button tombol = objekKartu.GetComponent<Button>();

            Image imageDepan = objekKartu
                .Find("gambardepan")
                .GetComponent<Image>();

            int id = urutanAcak[i];

            kartu.idPasangan = id;
            kartu.gambarDepan = daftarKartu[id].gambarDepan;

            kartu.gambar = imageBelakang;
            kartu.gambarDepanUI = imageDepan;
            kartu.tombol = tombol;

            kartu.sudahCocok = false;
            kartu.sedangDibalik = false;

            kartu.skalaAwal = objekKartu.localScale;

            imageDepan.sprite = kartu.gambarDepan;
            imageDepan.enabled = false;
            imageDepan.raycastTarget = false;

            imageDepan.type = Image.Type.Simple;
            imageDepan.preserveAspect = false;
            imageDepan.useSpriteMesh = false;

            tombol.interactable = true;
            tombol.onClick.RemoveAllListeners();

            MatchingCard kartuTarget = kartu;

            tombol.onClick.AddListener(() =>
            {
                BukaKartu(kartuTarget);
            });

            kartuAktif.Add(kartu);
        }

        return true;
    }

    void BukaKartu(MatchingCard kartu)
    {
        if (gameSelesai || sedangMemeriksa || kartu == null)
            return;

        if (kartu.sudahCocok ||
            kartu.sedangDibalik ||
            kartu == kartuPertama)
            return;

        if (kartu.tombol == null ||
            !kartu.tombol.interactable)
            return;

        PutarSound(soundKlik);

        StartCoroutine(AnimasiBalikKartu(kartu));
    }

    IEnumerator AnimasiBalikKartu(MatchingCard kartu)
    {
        yield return StartCoroutine(
            AnimasiFlipKartu(kartu, true)
        );

        if (gameSelesai)
            yield break;

        if (kartuPertama == null)
        {
            kartuPertama = kartu;
        }
        else
        {
            kartuKedua = kartu;
            StartCoroutine(PeriksaPasangan());
        }
    }

    IEnumerator AnimasiBalikKartuTertutup(MatchingCard kartu)
    {
        yield return StartCoroutine(
            AnimasiFlipKartu(kartu, false)
        );
    }

    // Animasi flip horizontal tanpa efek pop-up.
    IEnumerator AnimasiFlipKartu(
        MatchingCard kartu,
        bool tampilkanDepan
    )
    {
        if (kartu == null)
            yield break;

        kartu.sedangDibalik = true;

        RectTransform rect =
            kartu.transform as RectTransform;

        if (rect == null)
        {
            AturTampilanKartu(kartu, tampilkanDepan);
            kartu.sedangDibalik = false;
            yield break;
        }

        Vector3 skalaAsli = kartu.skalaAwal;

        float setengahDurasi =
            Mathf.Max(0.025f, durasiBalik / 2f);

        float waktu = 0f;

        // Kartu menyempit secara horizontal.
        while (waktu < setengahDurasi)
        {
            waktu += Time.deltaTime;

            float t =
                Mathf.Clamp01(waktu / setengahDurasi);

            float faktorX =
                Mathf.Lerp(1f, 0f, t);

            rect.localScale = new Vector3(
                skalaAsli.x * faktorX,
                skalaAsli.y,
                skalaAsli.z
            );

            yield return null;
        }

        // Ganti gambar saat kartu menyempit.
        AturTampilanKartu(kartu, tampilkanDepan);

        waktu = 0f;

        // Kartu melebar kembali.
        while (waktu < setengahDurasi)
        {
            waktu += Time.deltaTime;

            float t =
                Mathf.Clamp01(waktu / setengahDurasi);

            float faktorX =
                Mathf.Lerp(0f, 1f, t);

            rect.localScale = new Vector3(
                skalaAsli.x * faktorX,
                skalaAsli.y,
                skalaAsli.z
            );

            yield return null;
        }

        rect.localScale = skalaAsli;

        kartu.sedangDibalik = false;
    }

    void AturTampilanKartu(
        MatchingCard kartu,
        bool tampilkanDepan
    )
    {
        if (kartu.gambar != null)
        {
            kartu.gambar.sprite = gambarBelakang;
            kartu.gambar.enabled = true;
        }

        if (kartu.gambarDepanUI != null)
            kartu.gambarDepanUI.enabled = tampilkanDepan;
    }

    IEnumerator PeriksaPasangan()
    {
        sedangMemeriksa = true;

        yield return new WaitForSeconds(jedaSalah);

        if (gameSelesai)
        {
            sedangMemeriksa = false;
            yield break;
        }

        if (kartuPertama == null || kartuKedua == null)
        {
            sedangMemeriksa = false;
            yield break;
        }

        if (kartuPertama.idPasangan ==
            kartuKedua.idPasangan)
        {
            PutarSound(soundBenar);

            kartuPertama.sudahCocok = true;
            kartuKedua.sudahCocok = true;

            kartuPertama.tombol.interactable = false;
            kartuKedua.tombol.interactable = false;

            skor += 10;
            pasanganDitemukan++;

            if (pasanganDitemukan >= daftarKartu.Count)
                SelesaikanGame(true);
        }
        else
        {
            PutarSound(soundSalah);

            yield return StartCoroutine(
                AnimasiBalikKartuTertutup(kartuPertama)
            );

            yield return StartCoroutine(
                AnimasiBalikKartuTertutup(kartuKedua)
            );
        }

        kartuPertama = null;
        kartuKedua = null;

        sedangMemeriksa = false;

        PerbaruiUI();
    }

    void PutarSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void PerbaruiUI()
    {
        if (teksWaktu != null)
            teksWaktu.text =
                Mathf.CeilToInt(waktuTersisa).ToString();

        if (teksSkor != null)
            teksSkor.text = skor.ToString();
    }

    IEnumerator AnimasiPopupHasil()
    {
        if (panelHasil == null)
            yield break;

        Transform panel = panelHasil.transform;

        Vector3 skalaNormal = Vector3.one;
        Vector3 skalaAwal = Vector3.one * skalaAwalPopup;
        Vector3 skalaBesar = Vector3.one * skalaOvershoot;

        panel.localScale = skalaAwal;

        float durasi = Mathf.Max(0.05f, durasiPopup);
        float waktu = 0f;

        // Tahap 1: Panel membesar.
        while (waktu < durasi * 0.7f)
        {
            waktu += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(waktu / (durasi * 0.7f));
            t = Mathf.SmoothStep(0f, 1f, t);

            panel.localScale = Vector3.Lerp(
                skalaAwal,
                skalaBesar,
                t
            );

            yield return null;
        }

        waktu = 0f;

        // Tahap 2: Panel kembali ke ukuran normal.
        while (waktu < durasi * 0.3f)
        {
            waktu += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(waktu / (durasi * 0.3f));
            t = Mathf.SmoothStep(0f, 1f, t);

            panel.localScale = Vector3.Lerp(
                skalaBesar,
                skalaNormal,
                t
            );

            yield return null;
        }

        panel.localScale = skalaNormal;
    }

    void SelesaikanGame(bool menang)
    {
        if (gameSelesai)
            return;

        gameSelesai = true;

        if (teksHasil != null)
        {
            teksHasil.text = menang
                ? "KAMU MENANG!"
                : "WAKTU HABIS!";
        }

        // Mainkan sound hasil.
        if (menang)
            PutarSound(soundHasilMenang);
        else
            PutarSound(soundHasilKalah);

        // Tampilkan panel hasil.
        if (panelHasil != null)
        {
            panelHasil.SetActive(true);

            StartCoroutine(AnimasiPopupHasil());
        }
    }
}
