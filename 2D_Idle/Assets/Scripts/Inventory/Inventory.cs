using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;


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

        [SerializeField, InlineEditor] private EquipmentSlot weaponSlot;
        [SerializeField, InlineEditor] private EquipmentSlot subWeaponSlot;
        [SerializeField, InlineEditor] private EquipmentSlot helmetSlot;
        [SerializeField, InlineEditor] private EquipmentSlot topArmorSlot;
        [SerializeField, InlineEditor] private EquipmentSlot gloveSlot;
        [SerializeField, InlineEditor] private EquipmentSlot bottomArmorSlot;
        [SerializeField, InlineEditor] private EquipmentSlot shoeArmorSlot;
        [SerializeField, InlineEditor] private EquipmentSlot miningSlot;
        [SerializeField, InlineEditor] private EquipmentSlot fishingSlot;
        [SerializeField, InlineEditor] private EquipmentSlot axingSlot;

        [SerializeField] private Dictionary<eEquipmentParts, EquipmentSlot> equipmentSlots;

        public UnityEvent<int> OnUseItem;
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
                { eEquipmentParts.Mining,  miningSlot},
                { eEquipmentParts.Fishing,  fishingSlot},
                { eEquipmentParts.Axing,  axingSlot},
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

        public bool IsMiningEquipped() => miningSlot.IsEquipped;
        public bool IsFishingEquipped() => fishingSlot.IsEquipped;
        public bool IsAxingEquipped() => axingSlot.IsEquipped;

        public ResourceGetterItem GetEquippedMiningItem() => miningSlot.EquippedItem as ResourceGetterItem;

        public ResourceGetterItem GetEquippedFishingItem() => fishingSlot.EquippedItem as ResourceGetterItem;

        public ResourceGetterItem GetEquippedAxingItem() => axingSlot.EquippedItem as ResourceGetterItem;

        #endregion

        #region Inventory

        public void InitInventory()
        {
            _inventory = new Dictionary<int, Item>();

            _itemsByType = new Dictionary<eItemType, List<Item>>();

            equipmentSlots = new Dictionary<eEquipmentParts, EquipmentSlot>()
            {
                { eEquipmentParts.Weapon, weaponSlot },
                { eEquipmentParts.Subweapon, subWeaponSlot },
                { eEquipmentParts.helmet, helmetSlot},
                { eEquipmentParts.body, topArmorSlot },
                { eEquipmentParts.pants, bottomArmorSlot },
                { eEquipmentParts.shoe, shoeArmorSlot },
                { eEquipmentParts.Glove, gloveSlot },
                { eEquipmentParts.Mining,  miningSlot},
                { eEquipmentParts.Fishing,  fishingSlot},
                { eEquipmentParts.Axing,  axingSlot},
            };

            // Initialize equipment slots 
            foreach (var slot in equipmentSlots.Values)
            {
                slot.Init();
            }
        }

        private void AddOrUpdateItem(Item item)
        {
            if (item is CountableItem countableItem)
            {
                int itemIndex = FindItemInInventory(countableItem.CountableData.intID);
                if (itemIndex != -1)
                {
                    // Here, you should possibly fetch the existing item and add to its count.
                    CountableItem existingItem = _inventory[itemIndex] as CountableItem;
                    if (existingItem != null)
                    {
                        UpdateItemAmount(countableItem.Amount, itemIndex);  // Increment existing amount by new item's amount.
                        Debug.Log($"Incremented count for {countableItem.CountableData.Name}, total count now: {existingItem.Amount}");
                    }
                }
                else
                {
                    AddNewItem(countableItem);
                    Debug.Log($"New countable item added: {countableItem.CountableData.Name} with count: {countableItem.Amount}");
                }
            }
            else if (item is EquipmentItem equipmentItem)
            {
                AddNewItem(equipmentItem);
            }
        }


        private void UpdateItemAmount(int additionalAmount, int itemIndex)
        {
            ((CountableItem)_inventory[itemIndex]).AddAmount(additionalAmount);
        }


        private void AddNewItem(Item item)
        {
            int emptyIndex = GetNextEmptyIndex();
            _inventory.Add(emptyIndex, item);
        }

        [Button("AddItem")]
        // ���� _inventory�� ������ �ֱ�
        public void AddToInventory(Item item)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            AddOrUpdateItem(item);
            
            OnGainItem?.Invoke(item);
        }

        public void AddToInventory(List<Item> items)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            foreach (var item in items)
            {
                AddToInventory(item);
            }
        }

        // Overloaded method to find a specific item by its ID
        public int FindItemInInventory(int itemId)
        {
            var itemPair = _inventory.FirstOrDefault(slot => slot.Value.Data.intID == itemId);
            return itemPair.Value != null ? itemPair.Key : -1;
        }

        public int FindItemInInventory(string ID)
        {
            var itemPair = _inventory.FirstOrDefault(slot => slot.Value.ID == ID);
            return itemPair.Value != null ? itemPair.Key : -1;
        }

        public Item GetItem(int index)
        {
            return _inventory[index];
        }

        public Item RemoveItem(int index)
        {
            var removedItem = _inventory[index];
            _inventory.Remove(index);
            return removedItem;
        }

        public bool UseItem(int index, PlayerStatContainer playerStat)
        {
            // Check if the item exists in the inventory at the given index.
            if (!_inventory.ContainsKey(index))
            {
                Debug.LogError("No item found at the specified index.");
                return false;
            }

            var item = _inventory[index];

            // Check if the item is a countable and usable item.
            if (item is CountableItem countableItem && countableItem is IUsableItem usableItem)
            {
                // Use the item and update the inventory accordingly.
                bool useSuccess = usableItem.Use(playerStat); // Assuming UseItem() method returns true if the item is successfully used.

                if (useSuccess)
                {
                    // UI���� ���� �����۽����� ����
                    OnUseItem?.Invoke(index);
                    // �κ��丮���� ������ ����
                    if (countableItem.Amount <= 0)
                    {
                        _inventory.Remove(index);
                    }
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to use the item.");
                    return false;
                }
            }
            else
            {
                Debug.LogError("The item at the specified index is not usable.");
                return false;
            }
        }

        public bool UseResourceEquipmentItem(eResourceType resourceType)
        {
            eEquipmentParts resourceGetterType = eEquipmentParts.Accessory;
            switch (resourceType)
            {
                case eResourceType.����:
                    resourceGetterType = eEquipmentParts.Mining;
                    break;
                case eResourceType.����:
                    resourceGetterType = eEquipmentParts.Axing;
                    break;
                case eResourceType.�����:
                    resourceGetterType = eEquipmentParts.Fishing;
                    break;
            }
            // find equipped resourceGetterItem from equipment
            if (equipmentSlots[resourceGetterType].IsEquipped)
            {
                // if exist, find its index from inventory
                int index = FindItemInInventory(equipmentSlots[resourceGetterType].EquippedItem.ID);
                // use that resourceGetterItem, if its durability is less than 0, remove it from inventory and update UI
                if (index != -1)
                {
                    var rEquip = equipmentSlots[resourceGetterType].EquippedItem as ResourceGetterItem;
                    rEquip.Use();
                    OnUseItem?.Invoke(index);
                    return true;
                }
                else
                {
                    Debug.LogError("Can't find ResourceGetterItem of resource type: " + resourceGetterType);
                }
            }
            return false;

            
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



        //public List<Item> GetItems()
        //{
        //    return _inventory;
        //}

        #endregion


    }
}