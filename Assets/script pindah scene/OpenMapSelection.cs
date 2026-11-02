using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMapSelection : MonoBehaviour
{
    public void BukaMapSelection()
    {
        SceneManager.LoadScene("Pilih map");
    }
}