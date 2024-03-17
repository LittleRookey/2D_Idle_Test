using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;

[CreateAssetMenu(fileName = "MonsterTable", menuName = "Litkey/MonsterTable")]
public class MonsterTable : ScriptableObject
{
    [SerializeField] private WeightedRandomPicker<Health> monsterTable;
    [SerializeField] private MonsterWeight[] monsterWeights;

    private void OnEnable()
    {
        monsterTable = null;
        monsterTable = new WeightedRandomPicker<Health>();
        for (int i = 0; i < monsterWeights.Length; i++)
        {
            monsterTable.Add(monsterWeights[i].monster, monsterWeights[i].weight);

        }
    }

    public Health GetRandomMonster()
    {
        return monsterTable.GetRandomPick();
    }
}

[System.Serializable]
public class MonsterWeight
{
    public Health monster;
    public double weight;
}