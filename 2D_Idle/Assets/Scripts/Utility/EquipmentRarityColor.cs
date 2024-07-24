using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[InlineEditor]
[CreateAssetMenu(menuName = "Litkey/EquipmentRarity")]
public class EquipmentRarityColor : SerializedScriptableObject
{
    [SerializeField] private Dictionary<ItemRarity, Color> rankColor;

    public Color GetColor(ItemRarity rarity)
    {
        return rankColor[rarity];
    }
}
