
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackMainMenu : MonoBehaviour
{
    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip suaraKlik;

    [Header("Pengaturan Fade")]
    public float durasiFade = 1f;

    private bool sedangTransisi = false;
    private Texture2D teksturHitam;
    private float alphaFade = 0f;

    private void Awake()
    {
        teksturHitam = new Texture2D(1, 1);
        teksturHitam.SetPixel(0, 0, Color.black);
        teksturHitam.Apply();
    }


    public void BukaMainMenu()
    {
        if (sedangTransisi)
            return;

        sedangTransisi = true;

        if (audioSource != null && suaraKlik != null)
        {
            audioSource.PlayOneShot(suaraKlik);
        }

        StartCoroutine(ProsesTransisi());
    }

    private IEnumerator ProsesTransisi()
    {
        float waktu = 0f;

        while (waktu < durasiFade)
        {
            waktu += Time.unscaledDeltaTime;
            alphaFade = Mathf.Clamp01(waktu / durasiFade);

            yield return null;
        }

        alphaFade = 1f;

        // Tunggu suara klik selesai jika durasinya lebih panjang dari fade
        if (audioSource != null && suaraKlik != null)
        {
            float sisaWaktu = suaraKlik.length - durasiFade;

            if (sisaWaktu > 0f)
                yield return new WaitForSecondsRealtime(sisaWaktu);
        }

        StopAllCoroutines();
        SceneManager.LoadScene("Main Menu");
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
