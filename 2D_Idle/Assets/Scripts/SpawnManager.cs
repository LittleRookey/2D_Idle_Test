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

    // �ʿ����� �ٸ� ���͸� ��ȯ
    private Dictionary<string, Pool<Health>> monsterDict;

    // ������ ����մ����� ���� ������ ���Ͱ� �ٸ�, ���� �����̴ٸ�
    [SerializeField] private Health monsterPrefab;

    private Health spawnedMonster;

    // TODO
    public List<MobInfo> mobs;

    // �ʿ� �� ��� ���� �����յ��� �̸� Ǯ�� ���� �����Ѵ�. 
    // �÷��̾ �ش� ������ ���������� �� ������ ���͵��� Ǯ�� Ȱ��ȭ ��Ų��. 
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

        // ������ ���Ͱ� Ǯ�� ������ Ǯ�� ���� ��ųʸ��� ����
        if (!monsterDict.TryGetValue(monsterToSpawn.Name, out Pool<Health> pool))
        {
            monsterDict[monsterToSpawn.Name] = Pool.Create(monsterToSpawn, 3).NonLazy();
            Debug.Log($"{monsterToSpawn.Name}�� Ǯ�� ���� ����");
        }
        var mons = monsterDict[monsterToSpawn.Name].Get();
        mons.transform.position = spawnPosition.position;
        //var mons = Instantiate(, spawnPosition.position, Quaternion.identity);
        mons.OnDeath += OnMonsterDeath;
        spawnedMonster = mons;
    }

    private void OnMonsterDeath()
    {
        // ����Ǯ�� �ǵ��� ������
        if (!monsterDict.ContainsKey(spawnedMonster.Name)) Debug.LogError("����" + spawnedMonster.Name+" Ű�� �����ϴ�");
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
    // ������ �ʿ��Ѱ�
    // ����, ���� ���̵�, ����
    public eCountry Country;
    public Health Mob;
    public int Level;
    public string RewardID;

}

