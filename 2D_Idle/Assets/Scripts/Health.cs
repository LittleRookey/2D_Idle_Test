using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    private bool isDead;
    public bool IsDead => isDead;

    private DamageNumberMesh dmg;

    private Vector3 dmgOffset = new Vector3(0f, 0.5f, 0f);

    BoxCollider2D bCollider;
    // return true when enemy death
    public bool TakeDamage(List<float> damages)
    {
        for (int i = 0; i < damages.Count; i++)
        {
            currentHealth -= damages[i];
            StartCoroutine(ShowDmgText(damages));
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isDead = true;
                bCollider.enabled = false;
                return true;
            }
        }
        return false;
    }

    private IEnumerator ShowDmgText(List<float> damages) 
    {
        WaitForSeconds delay = new WaitForSeconds(0.15f);
        for (int i = 0; i < damages.Count; i++)
        {
            dmg.Spawn(transform.position + Vector3.up + dmgOffset * (i + 1), damages[i]);
            yield return delay;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        bCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        dmg = Resources.Load<DamageNumberMesh>("Prefabs/BaseDamage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
