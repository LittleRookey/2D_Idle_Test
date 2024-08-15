using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Litkey/ItemData/ItemRecipe")]
public class ItemRecipe : ScriptableObject
{

    public List<RewardItem> requiredItems;

    public RewardItem resultItem;
    
    public int requiredGold;

}
