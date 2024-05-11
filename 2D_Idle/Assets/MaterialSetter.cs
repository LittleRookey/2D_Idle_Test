using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    [SerializeField] private Material enemyMaterial;
    private string prefabsPath = "Assets/Prefabs/Monsters";
#if UNITY_EDITOR
    [Button("SetMaterial")]
    public void LoadMonsterPrefabs()
    {

        // Get all prefab files in the Prefabs folder and its subfolders
        string[] prefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { prefabsPath });

        // Load the prefab assets
        GameObject[] monsterPrefabs = new GameObject[prefabGuids.Length];
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            monsterPrefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        // Do something with the loaded prefabs
        foreach (GameObject monsterPrefab in monsterPrefabs)
        {
            foreach(SpriteRenderer sr in monsterPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.material = enemyMaterial;
                Debug.Log("Given material");
            }
            Debug.Log(monsterPrefab.name);
        }
    }
#endif

  
}
