using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] public UnitLevel unitLevel;

    Health health;

    public float GetCurrentExp()
    {
        return unitLevel.CurrentExp;
    }

    public float GetMaxExp()
    {
        return unitLevel.MaxExp;
    }

    public int GetLevel()
    {
        //unitLevel.Init()
        return unitLevel.level;
    }

    public void GainExp(int val)
    {
        unitLevel.GainExp(val);
    }
    public float GetCurrentExpRate()
    {
        return unitLevel.CurrentExp / unitLevel.MaxExp;
    }

    public void SaveLevel()
    {

    }

    public void LoadLevel()
    {

    }


}
