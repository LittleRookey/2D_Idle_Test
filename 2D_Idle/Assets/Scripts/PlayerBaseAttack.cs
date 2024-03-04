using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseAttack : MonoBehaviour
{
    
    private void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<Health>().TakeDamage(new List<float> { 3, 2 });
        }
    }
}
