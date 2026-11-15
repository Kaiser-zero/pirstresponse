
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMapSelection : MonoBehaviour
{
    [Header("Pengaturan Transisi")]
    public string namaSceneTujuan = "Pilih map";
    public float durasiFade = 1f;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip suaraKlik;

    private bool sedangTransisi = false;
    private Texture2D teksturHitam;
    private float alphaFade = 0f;

    private void Awake()
    {
        teksturHitam = new Texture2D(1, 1);
        teksturHitam.SetPixel(0, 0, Color.black);
        teksturHitam.Apply();
    }

    public void BukaMapSelection()
    {
        if (sedangTransisi)
            return;

        // Memainkan suara klik
        if (audioSource != null && suaraKlik != null)
        {
            audioSource.PlayOneShot(suaraKlik);
        }

        StartCoroutine(ProsesTransisi());
    }

    private IEnumerator ProsesTransisi()
    {
        sedangTransisi = true;

        float waktu = 0f;

        while (waktu < durasiFade)
        {
            waktu += Time.unscaledDeltaTime;
            alphaFade = Mathf.Clamp01(waktu / durasiFade);

            yield return null;
        }

        alphaFade = 1f;

        SceneManager.LoadScene(namaSceneTujuan);
    }

    private void OnGUI()
    {
        if (!sedangTransisi)
            return;

        GUI.depth = -10000;
        GUI.color = new Color(1f, 1f, 1f, alphaFade);

        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            teksturHitam
        );

        GUI.color = Color.white;
    }

    private void OnDestroy()
    {
        if (teksturHitam != null)
            Destroy(teksturHitam);
    }
}
