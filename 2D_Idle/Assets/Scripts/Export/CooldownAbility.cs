using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey;
using DarkTonic.MasterAudio;
using Litkey.Character.Cooldowns;

[System.Serializable]
public class CooldownAbility : Ability, IHasCooldown
{
    [Header("Cooldown Ability")]
    [SerializeField] private int SkillID;
    public int ID => SkillID;

    // 90 = 90% of attack or so
    public float skillDamagePercent
    {
        get
        {
            return finalSkillDamagePercent;
        }
    }
    [SerializeField] protected float baseSkillDamagePercent;
    protected float finalSkillDamagePercent;

    public float CooldownDuration => coolDownTime;

    [Header("Spells")]
    [SerializeField] protected GameObject spellPrefab;
    [SerializeField] protected GameObject hitEffectPrefab;
    [SerializeField] protected string hitSound;

    //protected StatContainer statContainer;
    [SerializeField] private string sayString; // shouts skill name or saystring when use ability

    public float usedMana = 0f;

    public List<SkillGrowthData> skillUpgrades;

    //public SkillLevel level;

    private void OnEnable()
    {
        finalSkillDamagePercent = baseSkillDamagePercent;
        //level.SkillID = SkillID;
    }
    public override void OnCast(GameObject caster, GameObject target)
    {
        if (MasterAudio.SoundGroupExists(abilityUseSound))
        {
            MasterAudio.PlaySound(abilityUseSound);
        }
    }


    public override void LevelUp(int level)
    {
        float baseDMG = baseSkillDamagePercent;
        //Mathf.Min(skillUpgrades.Length - 1, level);
        // 스킬 데미지 상승
        for(int i = 0; i < level; i++)
        {
            baseDMG += skillUpgrades[i].addedSkillDamagePercent;
        }
        finalSkillDamagePercent = baseDMG;
    }
}
