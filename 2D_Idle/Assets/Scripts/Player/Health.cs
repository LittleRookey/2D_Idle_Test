using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.Events;
using Redcode.Pools;
using Litkey.Stat;
using Sirenix.OdinInspector;
using Litkey.Interface;
using DarkTonic.MasterAudio;
using Litkey.Utility;
using Litkey.Quest;

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

    protected Vector3 dmgOffset = new Vector3(0f, 0.6f, 0f);

    protected BoxCollider bCollider;
    protected Rigidbody2D rb;

    public delegate void OnTakeDamage(float current, float max);
    public OnTakeDamage onTakeDamage;

    public UnityEvent<LevelSystem> OnHit = new();

    public UnityEvent<LevelSystem> OnDeath = new();
    public UnityAction OnReturnFromPool;

    protected StatContainer _statContainer;

    protected float hpRegenerationRate;
    protected float regenerationInterval = 1f; // Regenerate every second
    protected Coroutine regenerationCoroutine;

    protected void OnValidate()
    {
        Name = gameObject.name;
    }

    protected virtual void Awake()
    {
        isDead = false;
        dmgOffset = new Vector3(0f, 0.55f, 0f);
        bCollider = GetComponent<BoxCollider>();
        //rb = GetComponent<Rigidbody2D>();
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _statContainer = GetComponent<StatContainer>();
        _statContainer.OnStatSetupComplete.AddListener(UpdateHealth);

    }

    protected virtual void OnEnable()
    {
        // load health
        _statContainer.HP.OnValueChanged.AddListener(UpdateMaxHealth);
        _statContainer.HP_Regeneration.OnValueChanged.AddListener(UpdateRegenerationRate);
        if (regenerationCoroutine == null && hpRegenerationRate > 0f)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }


    }

    protected virtual void OnDisable()
    {
        _statContainer.HP.OnValueChanged.RemoveListener(UpdateMaxHealth);
        _statContainer.HP_Regeneration.OnValueChanged.RemoveListener(UpdateRegenerationRate);
        if (regenerationCoroutine != null && hpRegenerationRate > 0f)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
        }
    }

    private void Start()
    {
        LoadHealth();
    }

    protected IEnumerator RegenerateHealth()
    {
        WaitForSeconds wait = new WaitForSeconds(regenerationInterval);

        while (true)
        {
            yield return wait;

            if (!isDead && currentHealth < maxHealth)
            {
                float regenerationAmount = hpRegenerationRate * regenerationInterval;
                AddCurrentHealth(regenerationAmount);
            }
        }
    }

    protected void UpdateMaxHealth(float mH)
    {

        float prevMaxHealth = maxHealth;
        maxHealth = mH;
        AddCurrentHealth(maxHealth - prevMaxHealth);
    }

    protected void UpdateRegenerationRate(float regen)
    {
        hpRegenerationRate = _statContainer.HP_Regeneration.FinalValue;
    }

    protected void RefillToMaxHealth()
    {
        isDead = false;
        this.currentHealth = this.maxHealth;
        UpdateHealth();
    }

    private void LoadHealth()
    {
        hpRegenerationRate = _statContainer.HP_Regeneration.FinalValue;
        maxHealth = _statContainer.HP.FinalValue;
        currentHealth = maxHealth;
        UpdateHealth();
    }
    protected void UpdateHealth(StatContainer stat)
    {
        float originMax = maxHealth;
        maxHealth = _statContainer.HP.FinalValue;
        hpRegenerationRate = _statContainer.HP_Regeneration.FinalValue;
        Debug.Log("Health에서 스텟 불러옴: " + _statContainer.HP.FinalValue);
        currentHealth += (maxHealth - originMax);

        onTakeDamage?.Invoke(currentHealth, maxHealth);
        //Debug.Log("Health에서 스텟 업데이트: " + currentHealth + " / " + maxHealth );
    }

    protected void UpdateHealth()
    {
        onTakeDamage?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// 플레이어에서 부르는 TakeDamage
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damages"></param>
    /// <returns></returns>
    [Button("TakeDamage")]
    public virtual bool TakeDamage(LevelSystem attacker, List<Damage> damages, bool showDmgText = true)
    {
        var attackerStat = attacker.GetComponent<StatContainer>();
        bool[] Hit = new bool[damages.Count];
        // 명중 회피 계산하기
        for (int i = 0; i < damages.Count; i++)
        {
            Hit[i] = attackerStat.CalculateHit(_statContainer);
        }
        //if (!attackerStat.CalculateHit(_statContainer))
        //{
        //    // 회피 텍스트, 회피 모션취하기
        //    ShowMissText();
        //    return false;
        //}
        if (showDmgText)
            StartCoroutine(DamagePopup.ShowDmgText(transform.position, damages, Hit));

        // 명중 통과하면 데미지 계산
        for (int i = 0; i < damages.Count; i++)
        {
            if (!Hit[i])
            {
                // 빗나갔을떄 미스 넘어가기
                continue;
            }
            currentHealth -= damages[i].damage;
            onTakeDamage?.Invoke(currentHealth, maxHealth);
        }
        OnHit?.Invoke(attacker);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
            //bCollider.isTrigger = true;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;

            //Debug.Log("attacker is null? " + attacker);
            QuestEvents.ReportAction(QuestType.KillEnemies, Name, 1);
            OnDeath?.Invoke(attacker);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 몬스터에서 부르는TakeDamage
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damages"></param>
    /// <returns></returns>
    public virtual bool TakeDamage(StatContainer attacker, List<Damage> damages, bool showDmgText = true, bool sfxOn = false)
    {
        if (isDead) return true;
        var attackerStat = attacker;
        bool[] Hit = new bool[damages.Count];
        // 명중 회피 계산하기
        for (int i = 0; i < damages.Count; i++)
        {
            Hit[i] = attackerStat.CalculateHit(_statContainer);
        }
        //if (!attackerStat.CalculateHit(_statContainer))
        //{
        //    // 회피 텍스트, 회피 모션취하기
        //    ShowMissText();
        //    return false;
        //}
        if (showDmgText)
        {
            StartCoroutine(DamagePopup.ShowDmgText(transform.position, damages, Hit));
            //StartCoroutine(ShowDmgText(damages, Hit));
        }

        if (sfxOn)
            StartCoroutine(PlaySounds(damages.Count, "칼맞는소리"));

        // 명중 통과하면 데미지 계산
        for (int i = 0; i < damages.Count; i++)
        {
            if (!Hit[i])
            {
                // 빗나갔을떄 미스 넘어가기
                continue;
            }
            currentHealth -= damages[i].damage;
            onTakeDamage?.Invoke(currentHealth, maxHealth);

        }

        OnHit?.Invoke(attacker.GetComponent<LevelSystem>());

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


    protected void ShowMissText()
    {
        missText.Spawn(transform.position + Vector3.up + dmgOffset);
    }

    protected virtual IEnumerator PlaySounds(int soundCount, string soundString)
    {
        WaitForSeconds delay = new WaitForSeconds(0.15f);
        for (int i = 0; i < soundCount; i++)
        {
            MasterAudio.PlaySound(soundString);
            yield return delay;
        }
    }

    //protected virtual IEnumerator ShowDmgText(List<Damage> damages, bool[] misses) 
    //{
    //    WaitForSeconds delay = new WaitForSeconds(0.1f);
    //    if (misses != null)
    //    {
    //        //Debug.Log("Show damage text count: " + damages.Count);
    //        for (int i = 0; i < damages.Count; i++)
    //        {
    //            if (!misses[i])
    //            {
    //                ShowMissText();
    //                yield return delay;
    //                continue;
    //            }

    //            if (damages[i].isCrit)
    //            {
    //                var spawnPos = transform.position + Vector3.up * 0.7f + dmgOffset * (i + 1);
    //                //Debug.Log("Show damage text position: " + spawnPos);
    //                critDamageText.Spawn(transform.position + Vector3.up * 0.7f + dmgOffset * (i + 1), damages[i].damage);
    //            }
    //            else
    //            {
    //                var spawnPos = transform.position + Vector3.up * 0.7f + dmgOffset * (i + 1);
    //                Debug.Log("Show damage text position: " + spawnPos);
    //                dmg.Spawn(spawnPos, damages[i].damage);
    //            }
    //            yield return delay;
    //        }
    //    }
    //}



    public void OnCreatedInPool()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGettingFromPool()
    {
        maxHealth = _statContainer.HP.FinalValue;
        currentHealth = maxHealth;
        isDead = false;
        bCollider.isTrigger = false;
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (regenerationCoroutine == null && hpRegenerationRate > 0f)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }
        OnReturnFromPool?.Invoke();

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
