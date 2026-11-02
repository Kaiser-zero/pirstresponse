using UnityEngine;

// Assign sliced sprites in the desired playback order in the Inspector.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSheetAnimator : MonoBehaviour
{
    public Sprite[] frames;
    [Min(1f)] public float framesPerSecond = 8f;
    public bool loop = true;

    SpriteRenderer spriteRenderer;
    int frameIndex;
    float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames != null && frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, framesPerSecond);
        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;

            if (frameIndex >= frames.Length)
            {
                if (loop) frameIndex = 0;
                else { frameIndex = frames.Length - 1; enabled = false; }
            }

            spriteRenderer.sprite = frames[frameIndex];
        }
    }
}
