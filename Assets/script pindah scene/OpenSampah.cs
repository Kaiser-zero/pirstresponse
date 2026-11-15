
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSampah : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    public int nomorScene = 3;
    public float durasiFade = 1f;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip suaraKlikSampah;

    private bool sedangPindah = false;
    private float alphaFade = 0f;
    private Texture2D teksturHitam;

    private void Awake()
    {
        teksturHitam = new Texture2D(1, 1);
        teksturHitam.SetPixel(0, 0, Color.black);
        teksturHitam.Apply();
    }

    private void OnMouseDown()
    {
        BukaSampah();
    }

    public void BukaSampah()
    {
        if (sedangPindah) return;

        // Putar suara ketika objek sampah diklik.
        if (audioSource != null && suaraKlikSampah != null)
        {
            audioSource.PlayOneShot(suaraKlikSampah);
        }

        StartCoroutine(ProsesPindahScene());
    }

    private IEnumerator ProsesPindahScene()
    {
        sedangPindah = true;

        float waktu = 0f;

        while (waktu < durasiFade)
        {
            waktu += Time.unscaledDeltaTime;
            alphaFade = Mathf.Clamp01(waktu / durasiFade);

            yield return null;
        }

        alphaFade = 1f;

        SceneManager.LoadScene(nomorScene);
    }

    private void OnGUI()
    {
        if (!sedangPindah) return;

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
