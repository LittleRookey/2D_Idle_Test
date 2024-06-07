using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using DG.Tweening;
using Litkey.Utility;
using Litkey.InventorySystem;

public enum eAbilityType
{
    Dash
}


public abstract class Ability : ScriptableObject
{
    [Header("Debug")]
    public bool showLog;

    [Header("Main Ability Settings")]
    public Sprite _icon;
    public ItemRarity rarity;
    //public bool isPlayer; // is player's ability
    public new string name; // ability name
    public float coolDownTime; // cooldown time for ability
    public float delayTime; // time takes to actually use ability
    public float activeTime; // time ability is activated for, cooldown start afterwards
    [TextArea]
    public string description; // description of the ability

    public bool useSkillWhenTargetExists;
    [SerializeField] public string abilityUseSound;
    protected bool isUsingAbility; // when player is holding on a key(charge?), 

    //public List<SkillGrowthData> skillUpgrades;

    //public Ability Clone()
    //{
    //    Ability ab = new Ability();
    //    ab.name = name;
    //    ab.coolDownTime = coolDownTime;
    //    ab.description = description;
    //    return ab;
    //}


    public virtual void OnCast(GameObject caster, GameObject target)
    {
        Debug.Log("OnCastBegin");
    }


    public virtual IEnumerator DelayCast(GameObject caster, GameObject target)
    {
        Debug.Log("D1");
        yield return null;

    }
    //public virtual void OnHitTarget(SpellEffect spellEffect, GameObject caster, GameObject target, Vector3 hitPoint)
    //{
    //    Debug.Log("Hit " + target.name);
    //}

    public virtual void LevelUp(int level)
    {

    }
}