using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayOnEnable : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation anim;
    [SerializeField] private string parameter;
    
    public bool playOnEnable = true;


    private void OnEnable()
    {
        if (playOnEnable)
        {
            anim.DORestartAllById(parameter);

        }
    }
   
}
