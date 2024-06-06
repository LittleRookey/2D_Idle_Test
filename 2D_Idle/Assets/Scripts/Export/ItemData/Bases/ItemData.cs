using Litkey.Interface;
using UnityEngine;
using Sirenix.OdinInspector;


namespace Litkey.InventorySystem
{
    public enum EquipmentRarity
    {
        일반 = 0,
        고급 = 1,
        희귀 = 2,
        영웅 = 3,
        전설 = 4,
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
        Accessory,
        Glove,
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
        
        public string Name => _name;
        public string Tooltip => _tooltip;
        public Sprite IconSprite => _iconSprite;
        [HideInInspector]
        public string itemType;
        public EquipmentRarity rarity => _rarity;
        public int Weight => _weight;

        [HorizontalGroup("Item Data", 100)]
        [PreviewField(100f)]
        [HideLabel]
        [SerializeField] private Sprite _iconSprite; // 아이템 아이콘
        
        [VerticalGroup("Item Data/Info")]
        [SerializeField] private string _name;    // 아이템 이름

        [VerticalGroup("Item Data/Info")] public int intID;

        [VerticalGroup("Item Data/Info")]
        /// <summary> 최대 내구도 </summary>
        [SerializeField] private EquipmentRarity _rarity = EquipmentRarity.일반;
        [VerticalGroup("Item Data/Info")]
        [SerializeField] private int _weight = 0;
        //protected string _id;
        [VerticalGroup("Item Data/Info")]
        [TextArea]
        [SerializeField] private string _tooltip; // 아이템 설명
        

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