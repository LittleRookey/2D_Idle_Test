using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/ItemData/ItemRecipe")]
    public class ItemRecipe : ScriptableObject
    {
        public int intID;
        public string recipeName;
        public string recipeExplanation;

        public int maxIngredientCount = 4;
        public List<RewardItem> requiredItems;

        public RewardItem resultItem;
    
        public int requiredGold;

        public bool isLocked;

        public void SetLocked() => isLocked = true;
        public void SetUnlocked() => isLocked = false;
    }
}
