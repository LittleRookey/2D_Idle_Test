using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    public List<Skill> passiveSkills = new List<Skill>();
    public List<ActiveSkill> activeSkills = new List<ActiveSkill>();

    private void Start()
    {
        // Initialize passive skills
        foreach (var skill in passiveSkills)
        {
            //skill.Initialize(this);
        }
    }

    private void Update()
    {
        // Apply passive skill effects
        foreach (var skill in passiveSkills)
        {
            //skill.ApplyEffect(GetComponent<Character>());
        }
    }

    public void ApplyPassiveEffects(Health target)
    {
        foreach (var skill in passiveSkills)
        {
            //skill.ApplyEffect(target);
        }
    }

    public void UseActiveSkill(ActiveSkill skill, Health target)
    {
        skill.Use(target);
    }
}
