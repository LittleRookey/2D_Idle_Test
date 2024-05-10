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
    protected bool isParried;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    [SerializeField] protected float maxHealth;
    protected float currentHealth;

    protected bool isDead;
    public bool IsDead => isDead;

    protected DamageNumberMesh dmg;
    protected DamageNumberMesh missText;
    protected DamageNumberMesh critDamageText;

    protected Vector3 dmgOffset = new Vector3(0f, 0.5f, 0f);

    BoxCollider2D bCollider;
    Rigidbody2D rb;

    public delegate void OnTakeDamage(float current, float max);
    public OnTakeDamage onTakeDamage;

    public UnityEvent<LevelSystem> OnDeath;
    public UnityAction OnReturnFromPool;

    protected StatContainer _statContainer;

    private void OnValidate()
    {
        Name = gameObject.name;
    }

    protected virtual void Awake()
    {
        isDead = false;
        bCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _statContainer = GetComponent<StatContainer>();
        _statContainer.OnStatSetupComplete.AddListener(UpdateHealth);

        dmg = Resources.Load<DamageNumberMesh>("Prefabs/BaseDamage");
        missText = Resources.Load<DamageNumberMesh>("Prefabs/Miss");
        critDamageText = Resources.Load<DamageNumberMesh>("Prefabs/CriticalDamage");
    }

    protected virtual void OnEnable()
    {
        
        _statContainer.HP.OnValueChanged.AddListener(UpdateMaxHealth);
       
    }

    protected virtual void OnDisable()
    {
        _statContainer.HP.OnValueChanged.RemoveListener(UpdateMaxHealth);
    }

    protected void UpdateMaxHealth(float mH)
    {
        Debug.Log("Updated Max Health from @@@@@");
        float prevMaxHealth = maxHealth;
        maxHealth = mH;
        AddCurrentHealth(maxHealth - prevMaxHealth);
    }

    protected void RefillToMaxHealth()
    {
        isDead = false;
        this.currentHealth = this.maxHealth;
        onTakeDamage?.Invoke(currentHealth, maxHealth);
    }

    protected void UpdateHealth(StatContainer stat)
    {
        float originMax = maxHealth;
        Debug.Log(_statContainer.HP);
        Debug.Log("Health에서 스텟 불러옴: " + _statContainer.HP.FinalValue);
        maxHealth = stat.subStats[eSubStatType.health].FinalValue;
        Debug.Log($"originMaxHealth: {originMax}\ncurrentHealth: {currentHealth}\nnewMax: {maxHealth}\nnewMax - originMax: {maxHealth - originMax}");
        currentHealth += (maxHealth - originMax);

        onTakeDamage?.Invoke(currentHealth, maxHealth);
        Debug.Log("Health에서 스텟 업데이트: " + currentHealth + " / " + maxHealth );
    }

    // return true when enemy death
    public bool TakeDamage(LevelSystem attacker, List<float> damages)
    {
        if (isDead) return true;
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
        if (isDead) return true;
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

    //public bool TakeDamage(StatContainer attacker, List<Damage> damages)
    //{
    //    if (isDead) return true;
    //    var attackerStat = attacker;
    //    // 명중 회피 계산하기
    //    if (!attackerStat.CalculateHit(_statContainer))
    //    {
    //        // 회피 텍스트, 회피 모션취하기
    //        ShowMissText();
    //        return false;
    //    }
    //    // 명중 통과하면 데미지 계산
    //    List<Damage> finalDamages = new List<Damage>();
    //    for (int i = 0; i < damages.Count; i++)
    //    {
    //        var damageInfo = attackerStat.GetDamageAgainst(_statContainer);

    //        //var dmg = _statContainer.Defend(damages[i].damage);
    //        finalDamages.Add(damageInfo);
    //        //currentHealth -= dmg;
    //        currentHealth -= damageInfo.damage;
    //        onTakeDamage?.Invoke(currentHealth, maxHealth);
    //    }

    //    StartCoroutine(ShowDmgText(finalDamages));

    //    if (currentHealth <= 0f)
    //    {
    //        currentHealth = 0f;
    //        isDead = true;
    //        bCollider.isTrigger = true;
    //        rb.constraints = RigidbodyConstraints2D.FreezeAll;


    //        OnDeath?.Invoke(attacker.GetComponent<LevelSystem>());
    //        return true;
    //    }
    //    return false;
    //}


    protected void ShowMissText()
    {
        missText.Spawn(transform.position + Vector3.up + dmgOffset);
    }

    protected IEnumerator ShowDmgText(List<Damage> damages) 
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
        onTakeDamage?.Invoke(currentHealth, maxHealth);
    }

    public void OnParried()
    {
        // 인터페이스구현
    }

    public void ActivateParry() => canParry = true;

    public void DisactivateParry() => canParry = false;
    
}
