using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayOnEnable : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation anim;
    [SerializeField] private string parameter;
    private void OnEnable()
    {
        anim.DORestartAllById(parameter);
    }
   
}
