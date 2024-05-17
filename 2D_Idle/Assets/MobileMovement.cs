using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MobileMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnPressing;
    public UnityEvent OnReleased;

    private bool isPressed;
    private Coroutine pressingCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pressingCoroutine = StartCoroutine(PressingCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (pressingCoroutine != null)
        {
            StopCoroutine(pressingCoroutine);
            OnReleased.Invoke();
        }
    }

    private System.Collections.IEnumerator PressingCoroutine()
    {
        while (isPressed)
        {
            OnPressing.Invoke();
            yield return null;
        }
    }


}
