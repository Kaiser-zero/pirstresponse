
using UnityEngine;

public class NPCFloating : MonoBehaviour
{
    [Header("Gerakan NPC")]
    public float amplitude = 0.15f;
    public float frequency = 1.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(
            Time.time * frequency
        ) * amplitude;

        transform.position = new Vector3(
            startPosition.x,
            startPosition.y + newY,
            startPosition.z
        );
    }
}
