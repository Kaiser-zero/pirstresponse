using UnityEngine;

// Moves a sprite horizontally and wraps it around for seamless looping.
// For seamless scrolling, place two copies of the same sprite side by side.
// Set loopWidth to the sprite's world-space width.
public class HorizontalLoop : MonoBehaviour
{
    [Min(0f)] public float speed = 0.25f;
    [Min(0.01f)] public float loopWidth = 20f;
    public bool moveLeft = true;

    void Update()
    {
        float direction = moveLeft ? -1f : 1f;
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        float halfWidth = loopWidth * 0.5f;
        if (moveLeft && transform.position.x < -loopWidth)
            transform.position += Vector3.right * (loopWidth * 2f);
        else if (!moveLeft && transform.position.x > loopWidth)
            transform.position += Vector3.left * (loopWidth * 2f);
    }
}
