using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public Sprite _icon;
    public string skillName;
    public int skillLevel;
    [SerializeField] protected string abilityUseSound;
    [SerializeField] protected StatContainer allyStat;
    public abstract void ApplyEffect(Health target);
}