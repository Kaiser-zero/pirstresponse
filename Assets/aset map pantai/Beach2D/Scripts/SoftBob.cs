using UnityEngine;

// Gentle vertical motion for ocean layers or foam strips.
public class SoftBob : MonoBehaviour
{
    public float amplitude = 0.08f;
    public float frequency = 0.7f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 p = startPosition;
        p.y += Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;
        transform.position = p;
    }
}
