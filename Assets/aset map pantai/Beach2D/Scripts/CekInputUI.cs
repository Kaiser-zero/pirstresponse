
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CekInputUI : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("MOUSE KLIK TERDETEKSI");

            if (EventSystem.current == null)
            {
                Debug.LogError("EVENTSYSTEM TIDAK DITEMUKAN!");
                return;
            }

            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Mouse.current.position.ReadValue();

            List<RaycastResult> hasil = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, hasil);

            if (hasil.Count == 0)
            {
                Debug.LogWarning("KLIK TIDAK MENGENAI UI APA PUN!");
            }
            else
            {
                foreach (RaycastResult item in hasil)
                {
                    Debug.Log("UI TERKENA KLIK: " + item.gameObject.name);
                }
            }
        }
    }
}
