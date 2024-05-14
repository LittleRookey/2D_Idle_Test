using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;

public class ReturnPopupToPool : MonoBehaviour
{
    [SerializeField] private ResourceInfo resourceInfo;
    [SerializeField] private float returnTime = 1.3f;
    private void Awake()
    {
        if (resourceInfo == null) resourceInfo = GetComponent<ResourceInfo>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Return), returnTime);
    }

    private void Return()
    {
        ResourcePopupCreator.ReturnBar(resourceInfo);
    }
}
