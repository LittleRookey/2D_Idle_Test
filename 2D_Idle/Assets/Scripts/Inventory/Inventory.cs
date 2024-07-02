using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;


/*
    [Item의 상속구조]
    - Item
        - CountableItem
            - PortionItem : IUsableItem.Use() -> 사용 및 수량 1 소모
        - EquipmentItem
            - WeaponItem
            - ArmorItem
    [ItemData의 상속구조]
      (ItemData는 해당 아이템이 공통으로 가질 데이터 필드 모음)
      (개체마다 달라져야 하는 현재 내구도, 강화도 등은 Item 클래스에서 관리)
    - ItemData
        - CountableItemData
            - PortionItemData : 효과량(Value : 회복량, 공격력 등에 사용)
        - EquipmentItemData : 최대 내구도
            - WeaponItemData : 기본 공격력
            - ArmorItemData : 기본 방어력
*/

/*
    [API]
    - bool HasItem(int) : 해당 인덱스의 슬롯에 아이템이 존재하는지 여부
    - bool IsCountableItem(int) : 해당 인덱스의 아이템이 셀 수 있는 아이템인지 여부
    - int GetCurrentAmount(int) : 해당 인덱스의 아이템 수량
        - -1 : 잘못된 인덱스
        -  0 : 빈 슬롯
        -  1 : 셀 수 없는 아이템이거나 수량 1
    - ItemData GetItemData(int) : 해당 인덱스의 아이템 정보
    - string GetItemName(int) : 해당 인덱스의 아이템 이름
    - int Add(ItemData, int) : 해당 타입의 아이템을 지정한 개수만큼 인벤토리에 추가
        - 자리 부족으로 못넣은 개수만큼 리턴(0이면 모두 추가 성공했다는 의미)
    - void Remove(int) : 해당 인덱스의 슬롯에 있는 아이템 제거
    - void Swap(int, int) : 두 인덱스의 아이템 위치 서로 바꾸기
    - void SeparateAmount(int a, int b, int amount)
        - a 인덱스의 아이템이 셀 수 있는 아이템일 경우, amount만큼 분리하여 b 인덱스로 복제
    - void Use(int) : 해당 인덱스의 아이템 사용
    - void UpdateSlot(int) : 해당 인덱스의 슬롯 상태 및 UI 갱신
    - void UpdateAllSlot() : 모든 슬롯 상태 및 UI 갱신
    - void UpdateAccessibleStatesAll() : 모든 슬롯 UI에 접근 가능 여부 갱신
    - void TrimAll() : 앞에서부터 아이템 슬롯 채우기
    - void SortAll() : 앞에서부터 아이템 슬롯 채우면서 정렬
*/

namespace Litkey.InventorySystem
{
    public enum eItemType
    {
        Equipment,
        UsableItem, // 사용 가능한 아이템
        ETC, // 각종 광물, CountableItem (사용은 불가능)
    }

    [CreateAssetMenu(menuName ="Litkey/Inventory")]
    public class Inventory : SerializedScriptableObject
    {
        [SerializeField, TableList]
        //private List<Item> _inventory; // 모든 아이템을 갖고있는 리스트

        public Dictionary<int, Item> _inventory; // 슬롯별 인덱스의 아이템 저장

        [ShowInInspector, DictionaryDrawerSettings(KeyLabel = "Item Type", ValueLabel = "Items")]
        private Dictionary<eItemType, List<Item>> _itemsByType; // 각 아이템별 정보를 저장한다, 저장한후 종류별 intID로 아이템을 찾을수 있는역할을 한다

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
        // 먼저 _inventory에 아이템 넣기
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
                    // Decrease the item count.
                    //countableItem.Amount -= 1;

                    // If the item count goes to zero, remove it from the inventory.
                    if (countableItem.Amount <= 0)
                    {
                        _inventory.Remove(index);
                    }
                    OnUseItem?.Invoke(index);
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
                case eResourceType.광석:
                    resourceGetterType = eEquipmentParts.Mining;
                    break;
                case eResourceType.나무:
                    resourceGetterType = eEquipmentParts.Axing;
                    break;
                case eResourceType.물고기:
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