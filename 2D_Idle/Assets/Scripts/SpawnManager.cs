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
    Town,
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
    Nineteen
}

public enum eHuntMode
{
    afk,

}

public class SpawnManager : MonoBehaviour
{
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

    private void Awake()
    {
        monsterDict = new Dictionary<string, Pool<Health>>();
    }

    public void Spawn()
    {
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
        mons.OnDeath += OnMonsterDeath;
        spawnedMonster = mons;
    }

    private void OnMonsterDeath()
    {
        // 몬스터풀로 되돌려 보내기
        if (!monsterDict.ContainsKey(spawnedMonster.Name)) Debug.LogError("몬스터" + spawnedMonster.Name+" 키가 없습니다");
        spawnedMonster.OnDeath -= OnMonsterDeath;
        monsterDict[spawnedMonster.Name].Take(spawnedMonster);
        spawnedMonster = null;
        StartCoroutine(StartSpawn());
    }

    private IEnumerator StartSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        Spawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        //Resources.LoadAll<Health>
        //monsterDict = new Dictionary<eRegion, Transform> { eRegion.FirstRegion, }
    }

    // Update is called once per frame
    void Update()
    {
        
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

