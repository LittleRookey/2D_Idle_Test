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
        //private List<Item> _inventory; // ��� �������� �����ִ� ����Ʈ

        public Dictionary<int, Item> _inventory; // ���Ժ� �ε����� ������ ����

        [ShowInInspector, DictionaryDrawerSettings(KeyLabel = "Item Type", ValueLabel = "Items")]
        private Dictionary<eItemType, List<Item>> _itemsByType; // �� �����ۺ� ������ �����Ѵ�, �������� ������ intID�� �������� ã���� �ִ¿����� �Ѵ�

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
            _inventory = new Dictionary<int, Item>();

            _itemsByType = new Dictionary<eItemType, List<Item>>();
        }

        [Button("AddItem")]
        // ���� _inventory�� ������ �ֱ�
        public void AddToInventory(Item item)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            int emptyIndex = GetNextEmptyIndex();
            // 
            if (item is EquipmentItem equipItem)
            {

                _inventory.Add(emptyIndex, item);

                if (!_itemsByType.ContainsKey(eItemType.Equipment))
                {
                    _itemsByType[eItemType.Equipment] = new List<Item>();
                }

                _itemsByType[eItemType.Equipment].Add(equipItem);

            }
            else if (item is CountableItem countableItem)
            {
                // Check if the countable item already exists in the inventory
                if (countableItem is IUsableItem)
                {
                    if (!_itemsByType.ContainsKey(eItemType.UsableItem))
                    {
                        _itemsByType[eItemType.UsableItem] = new List<Item>();
                    }
                    CountableItem foundItem = _itemsByType[eItemType.UsableItem].Find((Item item) => item.Data.intID == countableItem.CountableData.intID) as CountableItem;
                    if (foundItem != null)
                    {
                        foundItem.AddAmount(countableItem.Amount);
                    } 
                    else
                    {
                        _itemsByType[eItemType.UsableItem].Add(countableItem);
                    }
                    
                } 
                else
                {
                    if (!_itemsByType.ContainsKey(eItemType.ETC))
                    {
                        _itemsByType[eItemType.ETC] = new List<Item>();
                    }
                    
                    CountableItem foundItem = _itemsByType[eItemType.UsableItem].Find((Item item) => item.Data.intID == countableItem.CountableData.intID) as CountableItem;
                    
                    if (foundItem != null)
                    {
                        foundItem.AddAmount(countableItem.Amount);
                    }
                    else
                    {
                        _itemsByType[eItemType.ETC].Add(countableItem);
                    }
                }
                int itemIndex = FindItemInInventory(countableItem);

                if (itemIndex != -1)
                {
                    var cItem = _inventory[itemIndex] as CountableItem;
                    cItem.AddAmount(countableItem.Amount);
                } else
                {
                    _inventory.Add(emptyIndex, countableItem);
                }
                
            }
            OnGainItem?.Invoke(item);
        }

        public int FindItemInInventory(Item item2Find)
        {
            foreach(int index in _inventory.Keys)
            {
                var item = _inventory[index];
                if (item2Find is CountableItem countableItem)
                {
                    if (countableItem.CountableData.intID == item.Data.intID)
                    {
                        return index;
                    }
                } 
                else if (item2Find is EquipmentItem equipItem)
                {
                    if (equipItem.ID == item.ID)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        public Item GetItem(int index)
        {
            return _inventory[index];
        }

        public void UseItem(int index)
        {
            var item = _inventory[index];
        }

        private int GetNextEmptyIndex()
        {
            int i = 0;
            while (i < 99999999)
            {
                if (_inventory.ContainsKey(i))
                {
                    i++;
                    continue;
                }
                return i;
            }
            return -1;
        }

        public void AddToInventory(List<Item> items)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            foreach (var item in items)
            {
                int emptyIndex = GetNextEmptyIndex();
                // 
                if (item is EquipmentItem equipItem)
                {

                    _inventory.Add(emptyIndex, item);

                    if (!_itemsByType.ContainsKey(eItemType.Equipment))
                    {
                        _itemsByType[eItemType.Equipment] = new List<Item>();
                    }

                    _itemsByType[eItemType.Equipment].Add(equipItem);

                }
                else if (item is CountableItem countableItem)
                {
                    // Check if the countable item already exists in the inventory
                    if (countableItem is IUsableItem)
                    {
                        if (!_itemsByType.ContainsKey(eItemType.UsableItem))
                        {
                            _itemsByType[eItemType.UsableItem] = new List<Item>();
                        }
                        CountableItem foundItem = _itemsByType[eItemType.UsableItem].Find((Item item) => item.Data.intID == countableItem.CountableData.intID) as CountableItem;
                        if (foundItem != null)
                        {
                            foundItem.AddAmount(countableItem.Amount);
                        }
                        else
                        {
                            _itemsByType[eItemType.UsableItem].Add(countableItem);
                        }

                    }
                    else
                    {
                        if (!_itemsByType.ContainsKey(eItemType.ETC))
                        {
                            _itemsByType[eItemType.ETC] = new List<Item>();
                        }

                        CountableItem foundItem = _itemsByType[eItemType.UsableItem].Find((Item item) => item.Data.intID == countableItem.CountableData.intID) as CountableItem;

                        if (foundItem != null)
                        {
                            foundItem.AddAmount(countableItem.Amount);
                        }
                        else
                        {
                            _itemsByType[eItemType.ETC].Add(countableItem);
                        }
                    }
                    int itemIndex = FindItemInInventory(countableItem);

                    if (itemIndex != -1)
                    {
                        var cItem = _inventory[itemIndex] as CountableItem;
                        cItem.AddAmount(countableItem.Amount);
                    }
                    else
                    {
                        _inventory.Add(emptyIndex, countableItem);
                    }

                }
                OnGainItem?.Invoke(item);
            }

        }

        //public List<Item> GetItems()
        //{
        //    return _inventory;
        //}

        #endregion

    }
}