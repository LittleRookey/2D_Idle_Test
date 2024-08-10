using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.InventorySystem;

[InlineEditor]
[CreateAssetMenu(menuName = "Litkey/Reward/RewardGroup")]
public class RewardGroup : SerializedScriptableObject
{
    [VerticalGroup("Top1")]
    public int gold;
    [VerticalGroup("Top1")]
    public int playerExp;
    [VerticalGroup("Top1")]
    [TableList]
    public List<RewardItem> items;
}
[System.Serializable]
public class RewardItem
{
    public ItemData itemData;
    public int count;
}