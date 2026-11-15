
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PulihkanMapUI : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Tunggu proses pergantian scene selesai.
        yield return null;
        yield return new WaitForEndOfFrame();

        // Pastikan game tidak dalam kondisi pause.
        Time.timeScale = 1f;

        // Pulihkan CanvasGroup.
        CanvasGroup[] groups =
            FindObjectsByType<CanvasGroup>(
                FindObjectsInactive.Include
            );

        foreach (CanvasGroup group in groups)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        // Pulihkan tombol UI.
        Button[] buttons =
            FindObjectsByType<Button>(
                FindObjectsInactive.Include
            );

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }

        // Reset panel transisi.
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.ResetTransisi();
        }

        Debug.Log("UI Pilih Map berhasil dipulihkan.");
    }
}
