using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.Events;
using Redcode.Pools;
using Litkey.Stat;
using Sirenix.OdinInspector;

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
    Rigidbody2D rb;

    public delegate void OnTakeDamage(float current, float max);
    public OnTakeDamage onTakeDamage;

    public UnityAction<LevelSystem> OnDeath;
    public UnityAction OnReturnFromPool;

    private StatContainer _statContainer;

    private void OnValidate()
    {
        Name = gameObject.name;
    }

    private void OnEnable()
    {
        _statContainer = GetComponent<StatContainer>();
        _statContainer.HP.OnValueChanged += UpdateMaxHealth;

    }

    private void OnDisable()
    {
        _statContainer.HP.OnValueChanged -= UpdateMaxHealth;
    }

    private void UpdateMaxHealth(float mH)
    {
        maxHealth = mH;
    }
    // return true when enemy death
    public bool TakeDamage(LevelSystem attacker, List<float> damages)
    {
        //StartCoroutine(ShowDmgText(damages));
        for (int i = 0; i < damages.Count; i++)
        {
            currentHealth -= damages[i];
            onTakeDamage?.Invoke(currentHealth, maxHealth);
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isDead = true;
                bCollider.isTrigger = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;


                OnDeath?.Invoke(attacker);

                return true;
            }
        }
        return false;
    }
    // 피격자의 시점에서 데미지를 입는다.
    [Button("TakeDamage")]
    public bool TakeDamage(LevelSystem attacker, List<Damage> damages)
    {
        List<Damage> finalDamages = new List<Damage>();
        var attackerStat = attacker.GetComponent<StatContainer>();
        for (int i = 0; i < damages.Count; i++)
        {
            var damageInfo = attackerStat.GetDamageAgainst(_statContainer);

            //var dmg = _statContainer.Defend(damages[i].damage);
            finalDamages.Add(damageInfo);
            //currentHealth -= dmg;
            currentHealth -= damageInfo.damage;
            onTakeDamage?.Invoke(currentHealth, maxHealth);
        }

        StartCoroutine(ShowDmgText(finalDamages));

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
            bCollider.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            

            OnDeath?.Invoke(attacker);
            return true;
        }
        return false;
    }

    private IEnumerator ShowDmgText(List<Damage> damages) 
    {
        WaitForSeconds delay = new WaitForSeconds(0.15f);
        for (int i = 0; i < damages.Count; i++)
        {
            dmg.Spawn(transform.position + Vector3.up + dmgOffset * (i + 1), damages[i].damage);
            yield return delay;
        }
    }

    private void Awake()
    {
        isDead = false;
        bCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _statContainer = GetComponent<StatContainer>();
        maxHealth = _statContainer.HP.FinalValue;
        currentHealth = maxHealth;
        dmg = Resources.Load<DamageNumberMesh>("Prefabs/BaseDamage");
    }

    public void OnCreatedInPool()
    {
        throw new System.NotImplementedException();
    }

    public void OnGettingFromPool()
    {
        maxHealth = _statContainer.HP.FinalValue;
        currentHealth = maxHealth;
        isDead = false;
        bCollider.isTrigger = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        OnReturnFromPool?.Invoke();
        Debug.Log("@@@@@@GETFROMPOOL");
    }

    public void AddCurrentHealth(float value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
    }
}
