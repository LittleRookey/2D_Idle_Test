using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
    [SerializeField] private bool parallax, scrolling;
    [SerializeField] private Transform player;

    private Transform cameraTransform;
    [SerializeField] private Transform[] grounds;
    [SerializeField] private float viewZone = 10;
    [SerializeField] private float parallaxSpeed;
    private int leftIndex;
    private int rightIndex;

    public float backgroundSize;

    private float lastCameraX;
    private float lastGroundY;
    [Range(0f, 2f)]
    [SerializeField] private float speedVar = 1f;
    // 플레이어가 우측
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        lastGroundY = grounds[0].position.y;
        leftIndex = 0;
        rightIndex = grounds.Length - 1;
    }

    private void Update()
    {
        if (parallax)
        {
            float deltaX = cameraTransform.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * parallaxSpeed) * speedVar;
        } else
        {
            //transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }
        lastCameraX = cameraTransform.position.x;
        if (scrolling)
        {
            if (cameraTransform.position.x < (grounds[leftIndex].transform.position.x + viewZone))
                ScrollLeft();
            if (cameraTransform.position.x > (grounds[rightIndex].transform.position.x - viewZone))
                ScrollRight();
        }
    }
    void ScrollLeft()
    {
        int lastRight = rightIndex;
        grounds[rightIndex].position = Vector3.right * (grounds[leftIndex].position.x - backgroundSize) + Vector3.up * lastGroundY;
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = grounds.Length - 1;

    }

    void ScrollRight()
    {
        int lastLeft = leftIndex;
        grounds[leftIndex].position = Vector3.right * (grounds[rightIndex].position.x + backgroundSize) + Vector3.up * lastGroundY;
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == grounds.Length)
            leftIndex = 0;
    }
    
}
