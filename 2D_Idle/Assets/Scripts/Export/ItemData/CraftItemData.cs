using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Craft_", menuName = "Inventory System/Item Data/CraftItem")]
    public class CraftItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        public float Value => _value;
        [SerializeField] private float _value;

        public override Item CreateItem(string newID = default)
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            if (!string.IsNullOrEmpty(newID))
            {
                _id = newID;
            }
            return new CraftItem(this, _id);
        }
    }
}
