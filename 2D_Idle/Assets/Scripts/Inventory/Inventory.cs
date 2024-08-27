using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;
using Litkey.Interface;


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

    [Serializable]
    public class SerializableInventory
    {
        public List<SerializableItem> items;
        public Dictionary<eEquipmentParts, string> equippedItems;

        public SerializableInventory()
        {
            items = new List<SerializableItem>();
            equippedItems = new Dictionary<eEquipmentParts, string>();
        }
    }

    [Serializable]
    public class SerializableItem
    {
        public int slotIndex;
        public int intID;
        public string itemID;
        public int amount;
        public int durability;
        public int currentUpgrade;
        public SerializableItem(int slotIndex, Item item)
        {
            this.slotIndex = slotIndex;
            this.itemID = item.ID;
            this.intID = item.Data.intID;

            if (item is CountableItem countableItem)
            {
                this.amount = countableItem.Amount;
            }

            if (item is EquipmentItem equipmentItem)
            {
                this.durability = equipmentItem.Durability;
                this.currentUpgrade = equipmentItem.CurrentUpgrade;
            }
        }

    }
    [CreateAssetMenu(menuName = "Litkey/Inventory")]
    public class Inventory : SerializedScriptableObject, ISavable, ILoadable
    {
        [SerializeField, TableList]
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

        public Dictionary<eEquipmentParts, EquipmentSlot> EquipmentSlots => equipmentSlots;

        [SerializeField] public GameDatas gameDatas;
        [SerializeField] private ItemDatabase itemDB;

        public UnityEvent<int> OnUseItem;
        public UnityEvent<int> OnItemSold;
        public UnityEvent OnInventoryLoaded;
        [SerializeField] private InventoryUI _inventoryUI;

        private void Awake()
        {
            //gameDatas.OnGameDataLoaded.AddListener(Load);
        }

        private void OnEnable()
        {
            gameDatas.OnGameDataLoaded.AddListener(Load);
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

        private void OnDisable()
        {
            gameDatas.OnGameDataLoaded.RemoveListener(Load);
        }
        public void SetInventoryUI(InventoryUI ui)
        {
            _inventoryUI = ui;
        }


        #region Save + Load
        public void Save()
        {
            SerializableInventory serializableInventory = new SerializableInventory();

            foreach (var kvp in _inventory)
            {
                serializableInventory.items.Add(new SerializableItem(kvp.Key, kvp.Value));
            }

            foreach (var kvp in equipmentSlots)
            {
                if (kvp.Value.IsEquipped)
                {
                    serializableInventory.equippedItems[kvp.Key] = kvp.Value.EquippedItem.ID;
                }
            }

            // Save serializableInventory to your GameData
            gameDatas.dataSettings.inventoryData = serializableInventory;
            gameDatas.SaveDataLocal();
        }

        public void Load()
        {
            Debug.Log("�κ��丮 �ε� ����");
            SerializableInventory serializableInventory = gameDatas.dataSettings.inventoryData;

            if (serializableInventory == null)
            {
                Debug.Log("No saved inventory data found. Initializing new inventory.");
                InitInventory();
                return;
            }

            _inventory.Clear();
            foreach (var itemData in serializableInventory.items)
            {
                Item item = CreateItemFromData(itemData);
                if (item != null)
                {
                    _inventory[itemData.slotIndex] = item;
                }
            }

            foreach (var kvp in serializableInventory.equippedItems)
            {
                if (equipmentSlots.TryGetValue(kvp.Key, out EquipmentSlot slot))
                {
                    EquipmentItem item = _inventory.Values.FirstOrDefault(i => i.ID == kvp.Value) as EquipmentItem;
                    if (item != null)
                    {
                        slot.EquipItem(item);
                    }
                }
            }
            Debug.Log("�κ��丮 �ε� ����");
            OnInventoryLoaded?.Invoke();
        }

        private Item CreateItemFromData(SerializableItem itemData)
        {
            // You'll need to implement a method to create an Item instance from the saved data
            // This might involve looking up the item in a database or item catalog based on the itemId
            // For now, I'll provide a placeholder implementation
            Item item = itemDB.GetItemByID(itemData.intID).CreateItem(itemData.itemID);

            if (item == null) return null;

            if (item is CountableItem countableItem)
            {
                countableItem.SetAmount(itemData.amount);
            }
            else if (item is EquipmentItem equipmentItem)
            {
                equipmentItem.Durability = itemData.durability;
                for (int i = 0; i < itemData.currentUpgrade; i++)
                {
                    equipmentItem.Upgrade();
                }
            }

            return item;
        }

        #endregion

        #region EquipmentSlot
        public bool IsEquipped(EquipmentItem equipmentItem)
        {
            return equipmentSlots[equipmentItem.EquipmentData.Parts].IsSameEquippedItem(equipmentItem);
        }

        public void EquipEquipment(EquipmentItem equipmentItem)
        {
            var index = FindItemInInventory(equipmentItem.ID);
            if (index != -1)
            {
                _inventoryUI.UseOrEquipItem(index);
            }
        }

        /// <summary>
        /// ��� ���Կ� �������� �ֱ�
        /// </summary>
        /// <param name="item2Equip"></param>
        public void EquipItemToSlot(EquipmentItem item2Equip)
        {
            Debug.Log("Equipped Item in inventory: " + item2Equip.EquipmentData.GetStats().Length);
            equipmentSlots[item2Equip.EquipmentData.Parts].EquipItem(item2Equip);
            Save();
        }

        /// <summary>
        /// ��� ���Կ��� �������� ����
        /// </summary>
        /// <param name="parts"></param>
        public void UnEquipItemFromSlot(eEquipmentParts parts)
        {
            equipmentSlots[parts].UnEquipItem();
            Save();
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
            Debug.Log("Inventory Initialized");
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

        private void AddOrUpdateItem(ItemData itemData, int count)
        {
            var item = itemData.CreateItem();

            if (item is CountableItem countableItem)
            {
                int itemIndex = FindItemInInventory(countableItem.CountableData.intID);
                if (itemIndex != -1)
                {
                    // Here, you should possibly fetch the existing item and add to its count.
                    CountableItem existingItem = _inventory[itemIndex] as CountableItem;
                    existingItem.SetAmount(count);
                    if (existingItem != null)
                    {
                        UpdateItemAmount(count, itemIndex);  // Increment existing amount by new item's amount.
                        Debug.Log($"Incremented count for {countableItem.CountableData.Name}, total count now: {count}");
                        OnGainItem?.Invoke(existingItem);
                    }
                }
                else
                {
                    countableItem.SetAmount(count);
                    AddNewItem(countableItem);
                    OnGainItem?.Invoke(countableItem);
                    Debug.Log($"New countable item added: {countableItem.CountableData.Name} with count: {countableItem.Amount}");
                }
            }
            else if (item is EquipmentItem equipmentItem)
            {
                AddNewItem(equipmentItem);
                OnGainItem?.Invoke(equipmentItem);
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

        public void AddToInventory(ItemData itemData, int itemCount)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();
            AddOrUpdateItem(itemData, itemCount);
            Save();
        }

        [Button("AddItem")]
        // ���� _inventory�� ������ �ֱ�
        public void AddToInventory(Item item)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            AddOrUpdateItem(item);
            Save();
            OnGainItem?.Invoke(item);
        }

        public void AddToInventory(List<Item> items)
        {
            if (_inventory == null || _itemsByType == null) InitInventory();

            foreach (var item in items)
            {
                AddToInventory(item);
            }

            Save();
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
        public bool HasItem(int index)
        {
            return _inventory.ContainsKey(index);
        }


        public bool ContainsItem(ItemData itemData, int count)
        {
            if (count <= 0) return true;
            int totalCount = 0;

            foreach (var item in _inventory.Values)
            {
                if (item.Data.intID == itemData.intID && item is CountableItem countableItem)
                {
                    totalCount += countableItem.Amount;
                    if (totalCount >= count)
                    {
                        return true;
                    }
                }
                else if (item.Data.intID == itemData.intID && item is EquipmentItem equipmentItem)
                {
                    totalCount += 1;
                    if (totalCount >= count)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        // for equipmentItem
        public bool ContainsItem(string uniqueID, int count)
        {
            if (count <= 0) return true;
            int foundCount = 0;
            foreach (var item in _inventory.Values)
            {
                if (item.ID == uniqueID)
                {
                    foundCount++;
                    if (foundCount >= count)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Item RemoveItem(int index)
        {
            if (!HasItem(index)) return null;

            var removedItem = _inventory[index];
            _inventory.Remove(index);
            return removedItem;
        }
        public Item RemoveItem(int index, int count)
        {
            var removedItem = _inventory[index];
            if (removedItem is EquipmentItem)
            {
                _inventory.Remove(index);
                return removedItem;
            }
            else if (removedItem is CountableItem countableItem)
            {
                countableItem.RemoveAmount(count);
                if (countableItem.IsEmpty)
                {
                    _inventory.Remove(index);
                }
                return countableItem;
            }
            return null; 
        }

        public void SellItem(int index, int count)
        {
            if (index == -1)
            {
                Debug.LogError("Can't find item at index " + index);
                return;
            }

            var itemToSell = _inventory[index];
            if (itemToSell is EquipmentItem equipItem)
            {
                if (!IsEquipped(equipItem))
                {
                    ResourceManager.Instance.GainGold(equipItem.EquipmentData.SellPrice);
                    RemoveItem(index, 1);
                    OnItemSold?.Invoke(index);
                }
            }
            else if (itemToSell is CountableItem countableItem)
            {
                if (countableItem.Amount >= count)
                {
                    ResourceManager.Instance.GainGold(count * countableItem.CountableData.SellPrice);
                    RemoveItem(index, count);
                }
                else
                {
                    ResourceManager.Instance.GainGold(countableItem.Amount * countableItem.CountableData.SellPrice);
                    RemoveItem(index, countableItem.Amount);
                }
                OnItemSold?.Invoke(index);
            }
            Save();
            
        }

        public bool UseItem(ItemData itemData, int count)
        {
            // Check if the item exists in the inventory at the given index.
            if (!ContainsItem(itemData, count))
            {
                return false;
            }
            int index = FindItemInInventory(itemData.intID);
            var item = _inventory[index];

            // Check if the item is a countable and usable item.
            if (item is CountableItem countableItem)
            {
                countableItem.RemoveAmount(count);
                
                    // UI���� ���� �����۽����� ����
                OnUseItem?.Invoke(index);
                // �κ��丮���� ������ ����
                if (countableItem.Amount <= 0)
                {
                    _inventory.Remove(index);
                }
                Save();
                return true;
            }
            else if (item is EquipmentItem equipmentItem)
            {
                if (IsEquipped(equipmentItem))
                {
                    //UnEquipItem(equipmentItem.EquipmentData.Parts);
                    // ��� ä���������� inventoryUI���� UseOrEquipItem�� �ѹ��� ����ؼ� ���ݹ� ���â���� ����

                    _inventoryUI.UseOrEquipItem(index);
                }
                _inventory.Remove(index);
                OnUseItem?.Invoke(index);
                return true;
            }
            return false;
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
                    Save();
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
                
                int index = FindItemInInventory(equipmentSlots[resourceGetterType].EquippedItem.ID);
                var resourceGetter = equipmentSlots[resourceGetterType].EquippedItem as ResourceGetterItem;
                resourceGetter.Use();
                if (index == -1) Debug.LogError("Can't find ResourceGetterItem of ID " + equipmentSlots[resourceGetterType].EquippedItem.ID);
                
                OnUseItem?.Invoke(index);
                
                
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