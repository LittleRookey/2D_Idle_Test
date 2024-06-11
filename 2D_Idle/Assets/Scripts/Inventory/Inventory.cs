using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


/*
    [Item�� ��ӱ���]
    - Item
        - CountableItem
            - PortionItem : IUsableItem.Use() -> ��� �� ���� 1 �Ҹ�
        - EquipmentItem
            - WeaponItem
            - ArmorItem
    [ItemData�� ��ӱ���]
      (ItemData�� �ش� �������� �������� ���� ������ �ʵ� ����)
      (��ü���� �޶����� �ϴ� ���� ������, ��ȭ�� ���� Item Ŭ�������� ����)
    - ItemData
        - CountableItemData
            - PortionItemData : ȿ����(Value : ȸ����, ���ݷ� � ���)
        - EquipmentItemData : �ִ� ������
            - WeaponItemData : �⺻ ���ݷ�
            - ArmorItemData : �⺻ ����
*/

/*
    [API]
    - bool HasItem(int) : �ش� �ε����� ���Կ� �������� �����ϴ��� ����
    - bool IsCountableItem(int) : �ش� �ε����� �������� �� �� �ִ� ���������� ����
    - int GetCurrentAmount(int) : �ش� �ε����� ������ ����
        - -1 : �߸��� �ε���
        -  0 : �� ����
        -  1 : �� �� ���� �������̰ų� ���� 1
    - ItemData GetItemData(int) : �ش� �ε����� ������ ����
    - string GetItemName(int) : �ش� �ε����� ������ �̸�
    - int Add(ItemData, int) : �ش� Ÿ���� �������� ������ ������ŭ �κ��丮�� �߰�
        - �ڸ� �������� ������ ������ŭ ����(0�̸� ��� �߰� �����ߴٴ� �ǹ�)
    - void Remove(int) : �ش� �ε����� ���Կ� �ִ� ������ ����
    - void Swap(int, int) : �� �ε����� ������ ��ġ ���� �ٲٱ�
    - void SeparateAmount(int a, int b, int amount)
        - a �ε����� �������� �� �� �ִ� �������� ���, amount��ŭ �и��Ͽ� b �ε����� ����
    - void Use(int) : �ش� �ε����� ������ ���
    - void UpdateSlot(int) : �ش� �ε����� ���� ���� �� UI ����
    - void UpdateAllSlot() : ��� ���� ���� �� UI ����
    - void UpdateAccessibleStatesAll() : ��� ���� UI�� ���� ���� ���� ����
    - void TrimAll() : �տ������� ������ ���� ä���
    - void SortAll() : �տ������� ������ ���� ä��鼭 ����
*/

namespace Litkey.InventorySystem
{
    public enum eItemType
    {
        Equipment,
        UsableItem, // ��� ������ ������
        ETC, // ���� ����, CountableItem (����� �Ұ���)
    }

    [CreateAssetMenu(menuName ="Litkey/Inventory")]
    public class Inventory : SerializedScriptableObject
    {
        [SerializeField, TableList]
        private List<Item> _inventory; // ��� �������� �����ִ� ����Ʈ

        [ShowInInspector, DictionaryDrawerSettings(KeyLabel = "Item Type", ValueLabel = "Items")]
        private Dictionary<eItemType, List<Item>> _itemsByType; // �� �����ۺ� ������ �����Ѵ�

        public UnityEvent<Item> OnGainItem;

        [SerializeField] private EquipmentSlot weaponSlot;
        [SerializeField] private EquipmentSlot subWeaponSlot;
        [SerializeField] private EquipmentSlot helmetSlot;
        [SerializeField] private EquipmentSlot topArmorSlot;
        [SerializeField] private EquipmentSlot gloveSlot;
        [SerializeField] private EquipmentSlot bottomArmorSlot;
        [SerializeField] private EquipmentSlot shoeArmorSlot;

        [SerializeField] private Dictionary<eEquipmentParts, EquipmentSlot> equipmentSlots;
        
        private void OnEnable()
        {
            equipmentSlots = new Dictionary<eEquipmentParts, EquipmentSlot>()
            {
                { eEquipmentParts.Weapon, weaponSlot },
                { eEquipmentParts.Subweapon, subWeaponSlot },
                { eEquipmentParts.helmet, helmetSlot},
                { eEquipmentParts.body, topArmorSlot },
                { eEquipmentParts.pants, bottomArmorSlot },
                { eEquipmentParts.shoe, shoeArmorSlot },
                { eEquipmentParts.Glove, gloveSlot },
            };
            
        }

        #region EquipmentSlot

        public void EquipItem(EquipmentItem item2Equip)
        {
            Debug.Log("Equipped Item in inventory: " + item2Equip.EquipmentData.GetStats().Length);
            equipmentSlots[item2Equip.EquipmentData.Parts].EquipItem(item2Equip);
        }


        public void UnEquipItem(eEquipmentParts parts)
        {
            equipmentSlots[parts].UnEquipItem();
            
        }

        #endregion

        #region Inventory

        public void InitInventory()
        {
            _inventory = new List<Item>();

            _itemsByType = new Dictionary<eItemType, List<Item>>();
        }

        [Button("AddItem")]
        // ���� _inventory�� ������ �ֱ�
        public void AddToInventory(Item item)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            // 
            if (item is EquipmentItem equipItem)
            {
                _inventory.Add(item);

                if (!_itemsByType.ContainsKey(eItemType.Equipment))
                {
                    _itemsByType[eItemType.Equipment] = new List<Item>();
                }
                _itemsByType[eItemType.Equipment].Add(equipItem);
            }
            else if (item is CountableItem countableItem)
            {
                // Check if the countable item already exists in the inventory
                var existingItem = _inventory.Find(i => i.ID.Equals(countableItem.ID)) as CountableItem;
                if (existingItem != null)
                {
                    // If it exists, update the amount
                    existingItem.AddAmount(countableItem.Amount);
                }
                else
                {
                    // If it does not exist, add the item to the inventory
                    _inventory.Add(countableItem);
                }

                // Also add/update in the _itemsByType dictionary
                if (!_itemsByType.ContainsKey(eItemType.ETC))
                {
                    _itemsByType[eItemType.ETC] = new List<Item>();
                }
                var etcItems = _itemsByType[eItemType.ETC];

                var itemFound = etcItems.Find(i => i.ID.Equals(countableItem.ID)) as CountableItem;

                if (itemFound != null)
                {
                    itemFound.AddAmount(countableItem.Amount);
                }
                else
                {
                    etcItems.Add(countableItem);
                }
            }
            OnGainItem?.Invoke(item);
        }

        public void AddToInventory(List<Item> items)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            foreach (var item in items)
            {
                if (item is EquipmentItem equipItem)
                {
                    _inventory.Add(item);

                    if (!_itemsByType.ContainsKey(eItemType.Equipment))
                    {
                        _itemsByType[eItemType.Equipment] = new List<Item>();
                    }
                    _itemsByType[eItemType.Equipment].Add(equipItem);
                }
                else if (item is CountableItem countableItem)
                {
                    // Check if the countable item already exists in the inventory
                    var existingItem = _inventory.Find(i => i.ID.Equals(countableItem.ID)) as CountableItem;
                    if (existingItem != null)
                    {
                        // If it exists, update the amount
                        existingItem.AddAmount(countableItem.Amount);
                    }
                    else
                    {
                        // If it does not exist, add the item to the inventory
                        _inventory.Add(countableItem);
                    }

                    // Also add/update in the _itemsByType dictionary
                    if (!_itemsByType.ContainsKey(eItemType.ETC))
                    {
                        _itemsByType[eItemType.ETC] = new List<Item>();
                    }
                    var etcItems = _itemsByType[eItemType.ETC];

                    var itemFound = etcItems.Find(i => i.ID.Equals(countableItem.ID)) as CountableItem;

                    if (itemFound != null)
                    {
                        itemFound.AddAmount(countableItem.Amount);
                    }
                    else
                    {
                        etcItems.Add(countableItem);
                    }
                }
                OnGainItem?.Invoke(item);
            }

        }

        public List<Item> GetItems()
        {
            return _inventory;
        }

        #endregion

    }
}