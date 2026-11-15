
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void KeluarGame()
    {
        Debug.Log("Game ditutup.");

        Application.Quit();
    }
}
