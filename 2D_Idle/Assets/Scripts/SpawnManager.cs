using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum eCountry
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten


}

public enum eRegion
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Eleven,
    Twelve,
    Thirteen,
    Fourteen,
    Fifteen,
    Sixteen,
    Seventeen,
    Eighteen,
    Nineteen,
    Town,
    Boss,
}

public enum eHuntMode
{
    afk,

}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] private PlayerController player;
    [SerializeField] private float spawnDelay = 5f;

    [SerializeField] private Transform spawnPosition;

    [SerializeField] private eCountry currentCountry;
    [SerializeField] private eRegion currentRegion;

    // 맵에따라 다른 몬스터를 소환
    private Dictionary<string, Pool<Health>> monsterDict;

    // 지역이 어디잇는지에 따라 나오는 몬스터가 다름, 몬스터 지역이다름
    [SerializeField] private Health monsterPrefab;

    private Health spawnedMonster;

    // TODO
    public List<MobInfo> mobs;

    // 맵에 쓸 모든 몬스터 프리팹들을 미리 풀을 만들어서 저장한다. 
    // 플레이어가 해당 지역을 진입했을떄 그 지역의 몬스터들의 풀을 활성화 시킨다. 
    Pool<Health> enemyPool;

    [SerializeField] private MapManager mapManager;

    private float spawnTimer = 5f;
    private static float spawnCounter = 0f;

    public static bool stopTimer;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
        monsterDict = new Dictionary<string, Pool<Health>>();
        StopTimer();
    }

    public void Spawn()
    {
        StopTimer();
        var monsterToSpawn = mapManager.CurrentArea.monsterTable.GetRandomMonster();
        //var monsterToSpawn = MobManager.Instance.GetEnemy(eRegion.One);

        // 가져온 몬스터가 풀에 없으면 풀을 만들어서 딕셔너리에 저장
        if (!monsterDict.TryGetValue(monsterToSpawn.Name, out Pool<Health> pool))
        {
            monsterDict[monsterToSpawn.Name] = Pool.Create(monsterToSpawn, 3).NonLazy();
            Debug.Log($"{monsterToSpawn.Name}의 풀을 생성 성공");
        }
        var mons = monsterDict[monsterToSpawn.Name].Get();
        mons.transform.position = spawnPosition.position;
        //var mons = Instantiate(, spawnPosition.position, Quaternion.identity);
        mons.OnDeath.AddListener(OnMonsterDeath);
        spawnedMonster = mons;
    }

    private void OnMonsterDeath(LevelSystem attacker)
    {
        // Check if spawnedMonster is not null before proceeding
        if (spawnedMonster != null)
        {
            // Check if the key exists in the dictionary
            if (monsterDict.ContainsKey(spawnedMonster.Name))
            {
                spawnedMonster.OnDeath.RemoveListener(OnMonsterDeath);
                //Debug.Log("Monster returned to pool: " + spawnedMonster.Name);
                // monsterDict[spawnedMonster.Name].Take(spawnedMonster);
            }
            else
            {
                Debug.LogError("몬스터" + spawnedMonster.Name + " 키가 없습니다");
            }

            spawnedMonster = null;
        }
        else
        {
            Debug.LogWarning("spawnedMonster is null. Skipping monster return to pool.");
        }

        StartTimer();
    }

    public void TakeToPool(Health targ)
    {
        monsterDict[targ.Name].Take(targ);
    }

    private IEnumerator StartSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        Spawn();
    }

    void Start()
    {
        //Spawn();
        //Resources.LoadAll<Health>
        //monsterDict = new Dictionary<eRegion, Transform> { eRegion.FirstRegion, }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead) return;
        spawnCounter += Time.deltaTime;
        if (spawnCounter >= spawnTimer && !stopTimer)
        {

            Spawn();
            spawnCounter = 0f;
        }
    }

    public static void StopTimer()
    {
        stopTimer = true;

    }

    public static void StartTimer()
    {
        
        spawnCounter = 0f;
        stopTimer = false;
    }
}

public class MobInfo
{
    // 몹한테 필요한것
    // 레벨, 보상 아이디, 레벨
    public eCountry Country;
    public Health Mob;
    public int Level;
    public string RewardID;

}

