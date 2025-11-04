using UnityEngine;

public class SteppedSpriteMove : MonoBehaviour
{
    public float moveDistance = 2f;   // adjustable in Inspector
    public float frameDuration = 0.2f; // time per frame

    private Vector3 startPos;
    private float timer;
    private int frameIndex;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer = 0f;
            frameIndex = (frameIndex + 1) % 3;

            // Hard "step" between frames
            if (frameIndex == 0)
                transform.position = startPos;
            else if (frameIndex == 1)
                transform.position = startPos + Vector3.right * moveDistance;
            else if (frameIndex == 2)
                transform.position += Vector3.right * moveDistance;
            else if (frameIndex == 3)
                transform.position = startPos;
        }
    }
}
