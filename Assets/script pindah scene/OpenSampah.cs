
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSampah : MonoBehaviour
{
    public string Gameplaysampah = "Gameplay sampah";

    public void BukaSampah()
    {
        Debug.Log("Mencoba membuka Gameplay sampah...");
        SceneManager.LoadScene(3);
    }

    private void OnMouseDown()
    {
        Debug.Log("Tempat sampah diklik!");

        BukaSampah();
    }
}
