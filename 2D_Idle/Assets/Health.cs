using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    private bool isDead;
    public bool IsDead => isDead;

    public void TakeDamage(List<float> damages)
    {
        for (int i = 0; i < damages.Count; i++)
        {
            currentHealth -= damages[i];
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isDead = true;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
