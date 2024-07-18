using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.InventorySystem;

[CreateAssetMenu(menuName ="Litkey/InventoryDB")]
public class ItemDatabase : SerializedScriptableObject
{
    public Dictionary<int, ItemData> ItemDB;

    [Button("AddToDB",Style =ButtonStyle.FoldoutButton)]
    public void AddItemToDB(ItemData itemData)
    {
        if (ItemDB.ContainsKey(itemData.intID))
        {
            Debug.LogError($"There is already an Item with ID {itemData.intID} in Item Database");
            return;
        }
        ItemDB.Add(itemData.intID, itemData);
    }
    public ItemData GetItemByID(int ID)
    {
        if (!ItemDB.ContainsKey(ID))
        {
            Debug.LogError($"There is no such Item with ID {ID} in Item Database");
            return null;
        }

        return ItemDB[ID];
    }

}
