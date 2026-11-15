
using UnityEngine;

public class KlikTempatSampah : MonoBehaviour
{
    public int nomorSceneTujuan = 3;

    public void BukaGameplaySampah()
    {
        Debug.Log("Tempat sampah diklik!");

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager tidak ditemukan!");
            return;
        }

        SceneTransitionManager.Instance.PindahScene(nomorSceneTujuan);
    }
}
