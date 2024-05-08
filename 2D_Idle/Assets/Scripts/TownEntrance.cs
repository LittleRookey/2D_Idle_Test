using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TransitionsPlus;

public class TownEntrance : MonoBehaviour
{
    public TransitionAnimator anim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // make Transition to Castle
            Debug.Log("Played Transition@@@@@@@@@@@");
            anim.Play();
        }
    }

}
