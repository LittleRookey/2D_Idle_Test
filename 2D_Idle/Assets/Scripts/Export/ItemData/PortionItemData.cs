using UnityEngine;
using Litkey.Stat;

namespace Litkey.InventorySystem
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion")]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        [SerializeField] protected StatModifier[] stats;

        public override Item CreateItem()
        {
            //string _id = UniqueIDGenerator.GenerateUnqiueIDDateTime(Name);
            return new PortionItem(this, intID.ToString());
        }


        public StatModifier[] GetStats()
        {
            return stats;
        }

    }
}