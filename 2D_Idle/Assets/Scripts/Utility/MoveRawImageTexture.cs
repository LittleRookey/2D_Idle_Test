using UnityEngine;
using UnityEngine.UI;

public class MoveRawImageTexture : MonoBehaviour
{
    public Vector2 moveDirection = new Vector2(1, 0); // The direction in which to move the texture
    public float moveSpeed = 0.1f; // Speed of the movement

    private RawImage rawImage;
    private Rect uvRect;

    void Start()
    {
        // Get the RawImage component
        rawImage = GetComponent<RawImage>();
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        // Calculate the new UV rect position
        uvRect.x += moveDirection.x * moveSpeed * Time.deltaTime;
        uvRect.y += moveDirection.y * moveSpeed * Time.deltaTime;

        // Update the RawImage's UV rect
        rawImage.uvRect = uvRect;
    }
}
