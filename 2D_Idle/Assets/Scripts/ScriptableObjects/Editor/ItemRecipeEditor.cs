using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using Sirenix.Utilities;
using Litkey.InventorySystem;

[CustomEditor(typeof(ItemRecipe))]
public class ItemRecipeEditor : OdinEditor
{

    public override void OnInspectorGUI()
    {
        var itemRecipe = target as ItemRecipe;

        SirenixEditorGUI.Title("Required Items", "Items needed for the recipe", TextAlignment.Left, true);

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < itemRecipe.requiredItems.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(80));
            var item = itemRecipe.requiredItems[i];

            // Display icon
            Rect rect = GUILayoutUtility.GetRect(60, 60);
            if (item.itemData != null && item.itemData.IconSprite != null)
            {
                GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(item.itemData.IconSprite), ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUI.DrawRect(rect, Color.gray);
            }

            // Display count
            EditorGUILayout.LabelField($"Count: {item.count}", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.EndVertical();

            if (i < itemRecipe.requiredItems.Count - 1)
            {
                GUILayout.Space(5);
                GUILayout.Label("+", GUILayout.Width(20));
                GUILayout.Space(5);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        SirenixEditorGUI.Title("Required Gold", "", TextAlignment.Left, true);
        EditorGUILayout.LabelField($"Gold: {itemRecipe.requiredGold}");

        EditorGUILayout.Space(10);

        SirenixEditorGUI.Title("Result Item", "", TextAlignment.Left, true);
        EditorGUILayout.BeginHorizontal();
        if (itemRecipe.resultItem != null && itemRecipe.resultItem.itemData != null)
        {
            // Display result item icon
            Rect resultRect = GUILayoutUtility.GetRect(60, 60);
            if (itemRecipe.resultItem.itemData.IconSprite != null)
            {
                GUI.DrawTexture(resultRect, AssetPreview.GetAssetPreview(itemRecipe.resultItem.itemData.IconSprite), ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUI.DrawRect(resultRect, Color.gray);
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Name: {itemRecipe.resultItem.itemData.Name}");
            EditorGUILayout.LabelField($"Count: {itemRecipe.resultItem.count}");
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("No result item set");
        }
        EditorGUILayout.EndHorizontal();

        // Draw the rest of the inspector
        DrawDefaultInspector();
    }
}
