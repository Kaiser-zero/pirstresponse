
using UnityEngine;

public class LifeguardIdleMotion : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.02f;
    [SerializeField] private float frequency = 1.2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(
            Time.time * frequency * Mathf.PI * 2f
        ) * amplitude;

        transform.localPosition =
            startPosition + Vector3.up * yOffset;
    }
}
