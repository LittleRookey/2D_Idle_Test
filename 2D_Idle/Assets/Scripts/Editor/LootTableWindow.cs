using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.Linq;


public class LootTableWindow : OdinMenuEditorWindow
{
    private LootTable[] lootTables;

    [MenuItem("Assets/Loot Tables")]
    private static void OpenWindow()
    {
        var window = GetWindow<LootTableWindow>("Loot Tables");
        window.Show();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        lootTables = AssetDatabase.FindAssets("t:LootTable")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<LootTable>)
            .ToArray();
    }


    protected override void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", EditorStyles.boldLabel);
        GUILayout.Label("ID", EditorStyles.boldLabel);
        GUILayout.Label("Gold Value", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        foreach (var lootTable in lootTables)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(lootTable.name);
            GUILayout.Label(lootTable._lootID);
            GUILayout.Label($"{lootTable.gold.x} ~ {lootTable.gold.y}");
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                AssetDatabase.OpenAsset(lootTable);
            }
            GUILayout.EndHorizontal();
        }
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        throw new System.NotImplementedException();
    }

   
}
