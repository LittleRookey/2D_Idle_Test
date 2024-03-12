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

    // 맵이 어디잇는지 알아야함
    private eCountry currentCountry;
    private eRegion currentRegion;

    // 맵에따라 다른 몬스터를 소환
    private Dictionary<eRegion, Health> monsterDict;

    // 지역이 어디잇는지에 따라 나오는 몬스터가 다름, 몬스터 지역이다름
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
