using Litkey.Interface;
using UnityEngine;


namespace Litkey.InventorySystem
{
    public enum EquipmentRarity
    {
        노말 = 0,
        레어 = 1,
        매직 = 2,
        유니크 = 3,
        레전드 = 4,
        초월 = 5,
        신화 = 6,
    };

    public enum eEquipmentParts
    {
        helmet,
        body,
        pants,
        shoe,
        Weapon,
        Subweapon,
        Accessory
    }
    /*
        [상속 구조]
        ItemData(abstract)
            - CountableItemData(abstract)
                - PortionItemData
            - EquipmentItemData(abstract)
                - WeaponItemData
                - ArmorItemData
    */
    [System.Serializable]
    public abstract class ItemData : ScriptableObject, IRewardable<ItemData>
    {
        public int intID;
        
        public string Name => _name;
        public string Tooltip => _tooltip;
        public Sprite IconSprite => _iconSprite;
        [HideInInspector]
        public string itemType;
        public EquipmentRarity rarity => _rarity;
        public int Weight => _weight;

        /// <summary> 최대 내구도 </summary>
        [SerializeField] private EquipmentRarity _rarity = EquipmentRarity.노말;
        [SerializeField] private int _weight = 0;
        //protected string _id;
        [SerializeField] private string _name;    // 아이템 이름
        [Multiline]
        [SerializeField] private string _tooltip; // 아이템 설명
        [SerializeField] private Sprite _iconSprite; // 아이템 아이콘

        /// <summary> 타입에 맞는 새로운 아이템 생성 </summary>
        public abstract Item CreateItem();

        public bool IsEquipment()
        {
            return (this is ArmorItemData) ||
                (this is WeaponItemData) ||
                (this is AccessoryItemData);
        }

        public virtual ItemData GetReward() { return this; }

    }

 
}