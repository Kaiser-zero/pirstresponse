
using UnityEngine;

public class SampahItem : MonoBehaviour
{
    [Header("Kategori Sampah")]
    public bool organik;

    [HideInInspector]
    public bool sudahDipilah = false;

    private PengelolaKuis pengelola;

    void Start()
    {
        pengelola = FindAnyObjectByType<PengelolaKuis>();
    }

    private void OnMouseDown()
    {
        if (sudahDipilah)
            return;

        if (pengelola != null)
        {
            pengelola.PilihSampah(this);
        }
        else
        {
            Debug.LogError("PengelolaKuis belum ditemukan!");
        }
    }

    public void SelesaikanPemilahan()
    {
        sudahDipilah = true;
        gameObject.SetActive(false);
    }
}
