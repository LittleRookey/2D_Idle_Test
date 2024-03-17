using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum eCountry
{
    FirstCountry,
    SecondCountry,
    ThirdCountry,
    FourthCountry,
    FifthCountry,
    SixthCountry,
    SeventhCountry, 
    EigthCountry
}

public enum eRegion
{
    FirstRegion,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eigth,
    Nineth
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
    

    private void Awake()
    {
        monsterDict = new Dictionary<string, Pool<Health>>();
    }

    public void Spawn()
    {
        var monsterToSpawn = MobManager.Instance.GetEnemy(eCountry.FirstCountry);

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

