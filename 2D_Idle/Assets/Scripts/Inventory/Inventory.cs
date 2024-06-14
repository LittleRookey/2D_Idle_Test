using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


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
        // 먼저 _inventory에 아이템 넣기
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