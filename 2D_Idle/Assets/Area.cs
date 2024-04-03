using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    public string areaName;

    public UnityAction<string> OnPlayerEnterArea;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMarker"))
        {
            Debug.Log("OnTriggerEnter Area");
            OnPlayerEnterArea?.Invoke(areaName);
        }
    }
    private void OnValidate()
    {
        areaName = gameObject.name;
    }

}
