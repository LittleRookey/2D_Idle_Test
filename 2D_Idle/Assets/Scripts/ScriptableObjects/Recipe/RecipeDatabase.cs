using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{
    [CreateAssetMenu(menuName ="Litkey/InventorySystem/RecipeDB")]
    public class RecipeDatabase : SerializedScriptableObject
    {
        public Dictionary<int, ItemRecipe> recipeDB;

        [Button("AddRecipe")]
        public void AddRecipeToDB(ItemRecipe recipe)
        {
            recipeDB.Add(recipe.intID, recipe);
        }

        public ItemRecipe GetRecipe(int id)
        {
            return recipeDB[id];
        }
    }
}
