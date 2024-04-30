using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;
using UnityEditor;
using Sirenix.OdinInspector;

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

    [Button(ButtonSizes.Large)]
    private void OpenMonsterFolder()
    {
        string folderPath = "Assets/Prefabs/Monsters/Template";
        Object folderObject = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
        if (folderObject != null)
        {
            EditorGUIUtility.PingObject(folderObject);
        }
        else
        {
            Debug.LogWarning("The 'Prefabs/Monster' folder does not exist in your project.");
        }
    }

    [Button(ButtonSizes.Large)]
    private void OpenMonsterTable()
    {
        string folderPath = "Assets/Resources/ScriptableObject/MonsterTable/Targ";
        Object folderObject = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
        if (folderObject != null)
        {
            EditorGUIUtility.PingObject(folderObject);
        }
        else
        {
            Debug.LogWarning("The 'Assets/Resources/ScriptableObject/MonsterTable' folder does not exist in your project.");
        }
    }
}

[System.Serializable]
public class MonsterWeight
{
    public Health monster;
    public double weight;
}