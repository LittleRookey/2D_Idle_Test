using UnityEngine;
using Litkey.Stat;

namespace Litkey.InventorySystem
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion")]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        [SerializeField] public float HealAmount;

        public override Item CreateItem(string newID=default)
        {
            //string _id = UniqueIDGenerator.GenerateUnqiueIDDateTime(Name);
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            if (!string.IsNullOrEmpty(newID))
            {
                _id = newID;
            }

            return new PortionItem(this, _id);
        }


        

    }
}