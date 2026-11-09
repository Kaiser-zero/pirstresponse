
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenCPR : MonoBehaviour
{
    public string CPR_Final = "CPR_Final";

    public void BukaCPR_Final()
    {
        Debug.Log("Mencoba membuka CPR Final...");
        SceneManager.LoadScene(4);
    }

    private void OnMouseDown()
    {
        Debug.Log("NPC diklik!");

        BukaCPR_Final();
    }
}
