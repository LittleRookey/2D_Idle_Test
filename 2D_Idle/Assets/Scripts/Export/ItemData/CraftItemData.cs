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

        public override Item CreateItem()
        {
            //string _id = UniqueIDGenerator.GenerateUnqiueIDDateTime(Name);
            return new CraftItem(this, intID.ToString());
        }
    }
}
