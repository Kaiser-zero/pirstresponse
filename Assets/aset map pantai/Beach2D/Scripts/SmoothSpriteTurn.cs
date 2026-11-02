
using UnityEngine;

public class SmoothSpriteTurn : MonoBehaviour
{
    [Header("Sprite Animation")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] frames;
    [Range(1, 30)] public float frameRate = 15f;

    [Header("Subtle Movement")]
    public Transform visual;
    public float bobAmount = 0.015f;
    public float bobSpeed = 2f;
    public float tiltAmount = 1.5f;

    private int frameIndex;
    private float frameTimer;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        if (visual == null)
            visual = spriteRenderer != null
                ? spriteRenderer.transform
                : transform;

        startPosition = visual.localPosition;
        startRotation = visual.localRotation;
    }

    void Update()
    {
        // Play sprite frames in sequence
        if (spriteRenderer != null &&
            frames != null && frames.Length > 0)
        {
            frameTimer += Time.deltaTime;

            float interval = 1f / Mathf.Max(1f, frameRate);

            while (frameTimer >= interval)
            {
                frameTimer -= interval;
                frameIndex = (frameIndex + 1) % frames.Length;
                spriteRenderer.sprite = frames[frameIndex];
            }
        }

        // Subtle floating motion
        float wave = Mathf.Sin(Time.time * bobSpeed);

        visual.localPosition = startPosition +
            Vector3.up * (wave * bobAmount);

        // Gentle lean, not a full 3D turn
        visual.localRotation = startRotation *
            Quaternion.Euler(0f, 0f, wave * tiltAmount);
    }
}
