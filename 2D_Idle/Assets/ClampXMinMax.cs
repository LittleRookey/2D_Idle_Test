using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampXMinMax : MonoBehaviour
{
    [SerializeField] private float minX = -13.5f;
    [SerializeField] private float maxX = 10f;
    Vector2 pos;
    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = pos;
    }
}
