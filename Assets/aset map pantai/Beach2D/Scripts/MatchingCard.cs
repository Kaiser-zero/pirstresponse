using UnityEngine;
using UnityEngine.UI;

public class MatchingCard : MonoBehaviour
{
    [HideInInspector] public int idPasangan;
    [HideInInspector] public Sprite gambarDepan;

    [HideInInspector] public Image gambar;
    [HideInInspector] public Image gambarDepanUI;
    [HideInInspector] public Button tombol;

    [HideInInspector] public bool sudahCocok;
    [HideInInspector] public bool sedangDibalik;

    [HideInInspector] public Vector3 skalaAwal;
}