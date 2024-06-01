using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoon : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1.5f;
    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
}
