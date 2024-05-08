using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Litkey.InventorySystem;

public class MobManager : MonoBehaviour
{
    public static MobManager Instance;

    // 맵이 어디잇는지 알아야함


    public Dictionary<eRegion, MonsterTable> monsterPool;
    private readonly string monsterTablePath = "ScriptableObject/MonsterTable/";
    public void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        } else if(Instance != this)
        {
            Destroy(gameObject);
        }
        Debug.Log(monsterTablePath + eCountry.One.ToString());
        monsterPool = new Dictionary<eRegion, MonsterTable>()
        {
            { eRegion.One, Resources.Load<MonsterTable>(monsterTablePath+eRegion.One.ToString()) },
            { eRegion.Two, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Two.ToString()) },
            { eRegion.Three, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Three.ToString()) },
            { eRegion.Four, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Four.ToString()) },
            { eRegion.Five, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Five.ToString()) },
            { eRegion.Six, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Six.ToString()) },
            { eRegion.Seven, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Seven.ToString()) },
            { eRegion.Eight, Resources.Load<MonsterTable>(monsterTablePath+eRegion.Eight.ToString()) },

        };


    }

    //public Health GetEnemy(eRegion reg)
    //{
    //    return monsterPool[reg].GetRandomMonster();
    //}

}


