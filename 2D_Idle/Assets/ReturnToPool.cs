using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float returnTime = 1f;
    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        Invoke(nameof(Return), returnTime);
    }


    private void Return()
    {
        ShapeCreator.ReturnShape(this.spriteRenderer);
    }
}
