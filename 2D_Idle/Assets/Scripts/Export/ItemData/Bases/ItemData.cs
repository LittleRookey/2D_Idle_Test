using Litkey.Interface;
using UnityEngine;
using Sirenix.OdinInspector;


namespace Litkey.InventorySystem
{
    public enum ItemRarity
    {
        �Ϲ� = 0,
        ��� = 1,
        ��� = 2,
        ���� = 3,
        ���� = 4,
        �ʿ� = 5,
        ��ȭ = 6,
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
        Mining,
        Fishing,
        Axing
    }
    /*
        [��� ����]
        ItemData(abstract)
            - CountableItemData(abstract)
                - PortionItemData
            - EquipmentItemData(abstract)
                - WeaponItemData
                - ArmorItemData
    */
    [System.Serializable]
    public abstract class ItemData : ScriptableObject
    {
        
        public string Name => _name;
        public string Tooltip => _tooltip;
        public Sprite IconSprite => _iconSprite;
        public ItemRarity rarity => _rarity;
        public int Weight => _weight;

        [HorizontalGroup("Item Data", 100)]
        [PreviewField(100f)]
        [HideLabel]
        [SerializeField] private Sprite _iconSprite; // ������ ������
        
        [VerticalGroup("Item Data/Info")]
        [SerializeField] private string _name;    // ������ �̸�

        [VerticalGroup("Item Data/Info")] public int intID;

        [VerticalGroup("Item Data/Info")]
        /// <summary> �ִ� ������ </summary>
        [SerializeField] private ItemRarity _rarity = ItemRarity.�Ϲ�;

        [VerticalGroup("Item Data/Info")]
        [SerializeField] private eItemType _itemType;
        public eItemType ItemType => _itemType;
          
        [VerticalGroup("Item Data/Info")]
        [SerializeField] private int _weight = 0;

        [VerticalGroup("Item Data/Info")]
        [SerializeField] private int _sellPrice = 0;
        public int SellPrice => _sellPrice;
        //protected string _id;
        [VerticalGroup("Item Data/Info")]
        [TextArea]
        [SerializeField] private string _tooltip; // ������ ����
        


        /// <summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
        public abstract Item CreateItem(string newID=default);

        public bool IsEquipment()
        {
            return (this is ArmorItemData) ||
                (this is WeaponItemData) ||
                (this is AccessoryItemData);
        }

        //public virtual ItemData GetReward() { return this; }

    }

 
}