using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Litkey/EquipmentRarity")]
public class EquipmentRarityColor : SerializedScriptableObject
{
    [SerializeField] private Dictionary<EquipmentRarity, Color> rankColor;

    public Color GetColor(EquipmentRarity rarity)
    {
        return rankColor[rarity];
    }
}
