using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInventory : MonoBehaviour
{
    [SerializeField] private List<Skill> skillInventory;

    public void AddToInventory(Skill skill)
    {
        skillInventory.Add(skill);
    }
}
