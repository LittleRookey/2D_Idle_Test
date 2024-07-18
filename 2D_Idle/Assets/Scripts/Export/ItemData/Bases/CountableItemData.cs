using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{
    /// <summary> 셀 수 있는 아이템 데이터 </summary>
    [InlineEditor]
    public abstract class CountableItemData : ItemData
    {
        public int MaxAmount => _maxAmount;
        [SerializeField] private int _maxAmount = 999;

    }
}