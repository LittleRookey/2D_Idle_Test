using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // ���� ����մ��� �˾ƾ���
    private eCountry currentCountry;
    private eRegion currentRegion;

    // �ʿ����� �ٸ� ���͸� ��ȯ
    private Dictionary<eRegion, Health> monsterDict;

    // ������ ����մ����� ���� ������ ���Ͱ� �ٸ�, ���� �����̴ٸ�
    [SerializeField] private Health monsterPrefab;

    private Health spawnedMonster;

    public void Spawn()
    {
        var mons = Instantiate(monsterPrefab, spawnPosition.position, Quaternion.identity);
        mons.OnDeath += OnMonsterDeath;
        spawnedMonster = mons;
    }

    private void OnMonsterDeath()
    {
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
