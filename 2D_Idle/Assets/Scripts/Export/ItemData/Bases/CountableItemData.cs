using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{
    /// <summary> �� �� �ִ� ������ ������ </summary>
    [InlineEditor]
    public abstract class CountableItemData : ItemData
    {
        public int MaxAmount => _maxAmount;
        [SerializeField] private int _maxAmount = 999;

    }
}