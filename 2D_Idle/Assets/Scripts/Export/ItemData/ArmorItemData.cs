using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - �� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor")]
    public class ArmorItemData : EquipmentItemData
    {
        /// <summary> ���� </summary>
        public override Item CreateItem()
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            var item = new ArmorItem(this, _id);
            //ResourceManager.Instance.MakeRandomStats(item);
            return item;
        }

    }
}