using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.Events;
using Redcode.Pools;
using Litkey.Stat;

public class Health : MonoBehaviour, IPoolObject
{
    public string Name;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [SerializeField] private float maxHealth;
    private float currentHealth;

    private bool isDead;
    public bool IsDead => isDead;

    private DamageNumberMesh dmg;

    private Vector3 dmgOffset = new Vector3(0f, 0.5f, 0f);

    BoxCollider2D bCollider;

    public delegate void OnTakeDamage(float current, float max);
    public OnTakeDamage onTakeDamage;

    public UnityAction OnDeath;
    public UnityAction OnReturnFromPool;

    private StatContainer _statContainer;
    // return true when enemy death
    public bool TakeDamage(List<float> damages)
    {
        StartCoroutine(ShowDmgText(damages));
        for (int i = 0; i < damages.Count; i++)
        {
            currentHealth -= damages[i];
            onTakeDamage?.Invoke(currentHealth, maxHealth);
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isDead = true;
                bCollider.enabled = false;
                OnDeath?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool TakeDamage(List<Damage> damages)
    {
        List<float> finalDamages = new List<float>();
        for (int i = 0; i < damages.Count; i++)
        {
            var dmg = _statContainer.Defend(damages[i].damage);
            finalDamages.Add(dmg);
            currentHealth -= dmg;
            onTakeDamage?.Invoke(currentHealth, maxHealth);
        }

        StartCoroutine(ShowDmgText(finalDamages));

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
            bCollider.enabled = false;
            OnDeath?.Invoke();
            return true;
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

    private void Awake()
    {
        isDead = false;
        bCollider = GetComponent<BoxCollider2D>();
        _statContainer = GetComponent<StatContainer>();
        currentHealth = maxHealth;
        dmg = Resources.Load<DamageNumberMesh>("Prefabs/BaseDamage");
    }

    public void OnCreatedInPool()
    {
        throw new System.NotImplementedException();
    }

    public void OnGettingFromPool()
    {
        currentHealth = maxHealth;
        isDead = false;
        bCollider.enabled = true;
        OnReturnFromPool?.Invoke();
        Debug.Log("@@@@@@GETFROMPOOL");
    }
}
