using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamage : MonoBehaviour
{
    [SerializeField] public int targetNum;
    public float skillDamage;
    public PlayerStatContainer playerStat;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision.
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
