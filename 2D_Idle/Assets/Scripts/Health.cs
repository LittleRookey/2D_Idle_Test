using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.Events;
using Redcode.Pools;
using Litkey.Stat;
using Sirenix.OdinInspector;
using Litkey.Interface;

public class Health : MonoBehaviour, IPoolObject, IParryable
{
    public string Name;

    public bool canParry;
    public bool isInterrupted;
    private bool isParried;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [SerializeField] private float maxHealth;
    private float currentHealth;

    private bool isDead;
    public bool IsDead => isDead;

    private DamageNumberMesh dmg;
    private DamageNumberMesh missText;
    private DamageNumberMesh critDamageText;

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
        var attackerStat = attacker.GetComponent<StatContainer>();
        // 명중 회피 계산하기
        if (!attackerStat.CalculateHit(_statContainer))
        {
            // 회피 텍스트, 회피 모션취하기
            ShowMissText();
            return false;
        }
        // 명중 통과하면 데미지 계산
        List<Damage> finalDamages = new List<Damage>();
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

    public bool TakeDamage(StatContainer attacker, List<Damage> damages)
    {
        var attackerStat = attacker;
        // 명중 회피 계산하기
        if (!attackerStat.CalculateHit(_statContainer))
        {
            // 회피 텍스트, 회피 모션취하기
            ShowMissText();
            return false;
        }
        // 명중 통과하면 데미지 계산
        List<Damage> finalDamages = new List<Damage>();
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


            OnDeath?.Invoke(attacker.GetComponent<LevelSystem>());
            return true;
        }
        return false;
    }


    private void ShowMissText()
    {
        missText.Spawn(transform.position + Vector3.up + dmgOffset);
    }

    private IEnumerator ShowDmgText(List<Damage> damages) 
    {
        WaitForSeconds delay = new WaitForSeconds(0.15f);
        for (int i = 0; i < damages.Count; i++)
        {
            if (damages[i].isCrit)
            {
                critDamageText.Spawn(transform.position + Vector3.up + dmgOffset * (i + 1), damages[i].damage);
            }
            else
            {
                dmg.Spawn(transform.position + Vector3.up + dmgOffset * (i + 1), damages[i].damage);
            }
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
        missText = Resources.Load<DamageNumberMesh>("Prefabs/Miss");
        critDamageText = Resources.Load<DamageNumberMesh>("Prefabs/CriticalDamage");
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

    public void OnParried()
    {
        // 인터페이스구현
    }

    public void ActivateParry() => canParry = true;

    public void DisactivateParry() => canParry = false;
    
}
