using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] public UnitLevel unitLevel;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
