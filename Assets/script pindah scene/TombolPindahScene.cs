
using UnityEngine;

public class TombolPindahScene : MonoBehaviour
{
    public int nomorSceneTujuan;

    public void PindahScene()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PindahScene(
                nomorSceneTujuan
            );
        }
        else
        {
            Debug.LogError(
                "SceneTransitionManager tidak ditemukan!"
            );
        }
    }
}
