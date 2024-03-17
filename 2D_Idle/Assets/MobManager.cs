using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Litkey.InventorySystem;

public class MobManager : MonoBehaviour
{
    public static MobManager Instance;

    // 맵이 어디잇는지 알아야함


    public Dictionary<eCountry, MonsterTable> monsterPool;
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
        Debug.Log(monsterTablePath + eCountry.FirstCountry.ToString());
        monsterPool = new Dictionary<eCountry, MonsterTable>()
        {
            { eCountry.FirstCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.FirstCountry.ToString()) },
            { eCountry.SecondCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.SecondCountry.ToString()) },
            { eCountry.ThirdCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.ThirdCountry.ToString()) },
            { eCountry.FourthCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.FourthCountry.ToString()) },
            { eCountry.FifthCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.FifthCountry.ToString()) },
            { eCountry.SixthCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.SixthCountry.ToString()) },
            { eCountry.SeventhCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.SeventhCountry.ToString()) },
            { eCountry.EigthCountry, Resources.Load<MonsterTable>(monsterTablePath+eCountry.EigthCountry.ToString()) },

        };


    }

    public Health GetEnemy(eCountry country)
    {
        return monsterPool[country].GetRandomMonster();
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


