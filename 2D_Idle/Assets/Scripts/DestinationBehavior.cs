using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestinationBehavior : MonoBehaviour
{

    public UnityEvent OnDestinationArrival;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMarker"))
        {
            //mapManager.StopMovement();
            OnDestinationArrival?.Invoke();
        }
    }
  
}
