using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Redcode.Pools;
using UnityEngine.Events;

/*
    [기능 - 에디터 전용]
    - 게임 시작 시 동적으로 생성될 슬롯 미리보기(개수, 크기 미리보기 가능)
    [기능 - 유저 인터페이스]
    - 슬롯에 마우스 올리기
      - 사용 가능 슬롯 : 하이라이트 이미지 표시
      - 아이템 존재 슬롯 : 아이템 정보 툴팁 표시
    - 드래그 앤 드롭
      - 아이템 존재 슬롯 -> 아이템 존재 슬롯 : 두 아이템 위치 교환
      - 아이템 존재 슬롯 -> 아이템 미존재 슬롯 : 아이템 위치 변경
        - Shift 또는 Ctrl 누른 상태일 경우 : 셀 수 있는 아이템 수량 나누기
      - 아이템 존재 슬롯 -> UI 바깥 : 아이템 버리기
    - 슬롯 우클릭
      - 사용 가능한 아이템일 경우 : 아이템 사용
    - 기능 버튼(좌측 상단)
      - Trim : 앞에서부터 빈 칸 없이 아이템 채우기
      - Sort : 정해진 가중치대로 아이템 정렬
    - 필터 버튼(우측 상단)
      - [A] : 모든 아이템 필터링
      - [E] : 장비 아이템 필터링
      - [P] : 소비 아이템 필터링
      * 필터링에서 제외된 아이템 슬롯들은 조작 불가
    [기능 - 기타]
    - InvertMouse(bool) : 마우스 좌클릭/우클릭 반전 여부 설정
*/

namespace Litkey.InventorySystem
{
    // 나중에 필터할때 쓰임
    public class InventoryUI : MonoBehaviour
    {

        [SerializeField] private Inventory _inventory;
        [SerializeField] private RectTransform slotsParent;
        [SerializeField] private ItemSlotUI itemSlotUIPrefab;
        [SerializeField] private RectTransform inventoryWindow;
        private Pool<ItemSlotUI> itemSlotPool;

        public Dictionary<int, ItemSlotUI> itemSlots;
        //public Dictionary<int, Item> itemIndex; // 슬롯별 인덱스의 아이템 저장
        private ItemSlotUI currentSelectedSlot = null; // 현재 선택된 슬롯

        [ShowInInspector]
        public Dictionary<eEquipmentParts, int> equippedItemIndex; // 각 슬롯별 아이템 인벤토리의 장착한 아이템 인덱스를 저장

        [SerializeField] private PlayerStatContainer playerStatContainer;

        [Header("아이템 강화창")]
        [SerializeField] private ItemUpgradeUI itemUpgradeUI;

        private void Awake()
        {
            _inventory.InitInventory();
            _inventory.SetInventoryUI(this);
            Init();

            _inventory.OnInventoryLoaded.AddListener(LoadInventoryUI);
            _inventory.OnInventoryLoaded.AddListener(UpdateEquippedItemUI);
            _inventory.OnItemSold.AddListener(UpdateSlot);
            CloseItemUpgradeWindow();
        }
        // 아이템 슬롯
        private void OnEnable()
        {
            _inventory.OnGainItem.AddListener(AddItemUI);
            _inventory.OnUseItem.AddListener(CheckSlotRemovable);
        }

        private void OnDisable()
        {
            _inventory.OnGainItem.RemoveListener(AddItemUI);
            _inventory.OnUseItem.RemoveListener(CheckSlotRemovable);
        }

        private void Init()
        {
            itemSlots = new Dictionary<int, ItemSlotUI>();
            itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
            itemSlotPool.SetContainer(slotsParent);
            equippedItemIndex = new Dictionary<eEquipmentParts, int>();
            
            foreach (var parts in (eEquipmentParts[])Enum.GetValues(typeof(eEquipmentParts)))
            {
                equippedItemIndex.Add(parts, -1);
            }
        }

        #region 아이템 강화

        public void OpenItemUpgradeWindow(EquipmentItem equipmentItem)
        {
            itemUpgradeUI.ShowUpgradeWindow(equipmentItem, () => {
                int index = _inventory.FindItemInInventory(equipmentItem.ID);
                if (index != -1)
                {
                    UpdateSlot(index);
                }});
            itemUpgradeUI.gameObject.SetActive(true);
        }

        public void CloseItemUpgradeWindow()
        {
            itemUpgradeUI.gameObject.SetActive(false);
        }

        #endregion
        private void AddItemUI(Item item)
        {
            if (itemSlotPool == null)
            {
                itemSlots = new Dictionary<int, ItemSlotUI>();
                itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
                itemSlotPool.SetContainer(slotsParent);
            }
            // Find the index of the item in the inventory. This is needed to map the UI slot correctly.
            int itemIndex = _inventory.FindItemInInventory(item.Data.intID);
            if (itemIndex == -1)
            {
                Debug.LogError("Failed to find item in inventory: " + item.Data.Name);
                return; // Early return if the item isn't actually in the inventory.
            }
           
            ItemSlotUI slotUI;
            // COuntableItem이고 슬롯잇으면 아이템찾아서 슬롯업데이트
            if (item is CountableItem && itemSlots.TryGetValue(itemIndex, out slotUI))
            {
                var cItem = _inventory.GetItem(itemIndex) as CountableItem; 
                // Update existing slot UI for countable items.
                slotUI.UpdateCount(cItem.Amount);
            }
            else if (item is EquipmentItem equipItem)
            {
                itemIndex = _inventory.FindItemInInventory(equipItem.ID);
                // 장비템이면
                slotUI = itemSlotPool.Get();
                slotUI.SetSlot(item);

                slotUI.gameObject.SetActive(true);
                itemSlots[itemIndex] = slotUI;
            } else
            {
                // 장비템 아니고 슬롯 없으면
                slotUI = itemSlotPool.Get();
                slotUI.SetSlot(item);

                slotUI.gameObject.SetActive(true);
                itemSlots[itemIndex] = slotUI;
            }
        }

        private void LoadInventoryUI()
        {
            if (itemSlotPool == null)
            {
                itemSlots = new Dictionary<int, ItemSlotUI>();
                itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
                itemSlotPool.SetContainer(slotsParent);
            }

            ClearSlots();
            foreach (var kValue in _inventory._inventory)
            {
                int index = kValue.Key;

                var slot = itemSlotPool.Get();
                slot.SetSlot(kValue.Value);
                itemSlots[index] = slot;
            }
        }



        // 장착한 장비를 장착햇을떄
        // 슬롯이 2번 눌렷을때 누른곳의 인덱스의 아이템이 장착돼있으면 스텟해제, 장착해제
        // // 새로운 아이템 눌럿을떄 
        // 전에 끼고잇는 아이템 스텟해제 슬롯해제
        // 새로운 아이템 스텟장착, 슬롯 장착
        public void UseOrEquipItem(int slotIndex)
        {
            var item = _inventory.GetItem(slotIndex);
           
            if (item is EquipmentItem equipmentItem)
            {
                // 아이템 장착 로직 추가
                int equippedIndex = equippedItemIndex[equipmentItem.EquipmentData.Parts];
                if (equippedIndex != -1)
                {
                    // 이미 장착한 템이 있고 이게 같은 인덱스면 
                    if (equippedIndex == slotIndex)
                    {
                        // 슬롯이 2번 눌렷을때 누른곳의 인덱스의 아이템이 장착돼있으면 스텟해제, 장착해제
                        // 슬롯 해제
                        itemSlots[equippedIndex].SetUnEquipped();
                        // 장착슬롯 해제
                        _inventory.UnEquipItemFromSlot(equipmentItem.EquipmentData.Parts); 
                        // 스텟해제
                        playerStatContainer.UnEquipEquipment(equipmentItem);
                        equippedItemIndex[equipmentItem.EquipmentData.Parts] = -1;

                    }
                    else
                    {
                        // 다른 인덱스면 전 장비를 해제, 그리고 장착
                        if (_inventory.GetItem(equippedIndex) is EquipmentItem equipItem)
                        {
                            // 스텟해제
                            playerStatContainer.UnEquipEquipment(equipItem);
                        } else
                        {
                            Debug.LogError($"Item index at {equippedIndex} is not equipment item");
                        }
                        itemSlots[equippedIndex].SetUnEquipped(); // 전장비 해제
                        _inventory.UnEquipItemFromSlot(equipmentItem.EquipmentData.Parts);
                        // 새 장비를 장착, 슬롯 장착


                        _inventory.EquipItemToSlot(equipmentItem); // 장착 슬롯 장착
                        playerStatContainer.EquipEquipment(equipmentItem); // 스텟 장착

                        equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // 인덱스저장
                        itemSlots[slotIndex].SetEquipped(); // 슬롯장착
                    }
                        
                } else
                {
                    // 해당 슬롯에 아무 장비도 없을떄
                    _inventory.EquipItemToSlot(equipmentItem); // 장착 슬롯 장착
                    playerStatContainer.EquipEquipment(equipmentItem); // 스텟 장착

                    equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // 인덱스저장
                    itemSlots[slotIndex].SetEquipped(); // 슬롯장착
                }
            }
            else if (item is CountableItem countableItem)
            {
                Debug.Log($"Using item: {item.Data.Name}");
                bool wasUsed = _inventory.UseItem(slotIndex, playerStatContainer);  // Assuming this method decreases the count and returns true if successful.
                if (wasUsed)
                {
                    if (countableItem.Amount <= 0)
                    {
                        RemoveItemAndSlot(slotIndex);
                    }
                    else
                    {
                        itemSlots[slotIndex].UpdateCount(countableItem.Amount);  // Update the count in the UI.
                    }
                }
            }
            currentSelectedSlot = null;
        }

        private void UpdateEquippedItemUI()
        {
            foreach(var slot in _inventory.EquipmentSlots.Values)
            {
                if (slot.IsEquipped)
                {
                    int index = _inventory.FindItemInInventory(slot.EquippedItem.ID);
                    if (index != -1)
                    {
                        SetItemEquipped(slot.EquippedItem, index);

                    } else
                    {
                        Debug.LogError($"Could not Item with ID {slot.EquippedItem.ID} in inventory");
                    }
                }
            }
        }
        private void SetItemEquipped(EquipmentItem equipmentItem, int slotIndex)
        {
            _inventory.EquipItemToSlot(equipmentItem); // 장착 슬롯 장착
            playerStatContainer.EquipEquipment(equipmentItem); // 스텟 장착

            equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // 인덱스저장
            itemSlots[slotIndex].SetEquipped(); // 슬롯장착
        }


        public void OpenInventory()
        {
            inventoryWindow.gameObject.SetActive(true);
        }

        public void CloseInventory()
        {
            inventoryWindow.gameObject.SetActive(false);
            if (currentSelectedSlot != null)
                currentSelectedSlot.ResetClickState();
        }

        private void CheckSlotRemovable(int index)
        {
            if (!_inventory.HasItem(index))
            {
                RemoveItemAndSlot(index);
                return;
            }

            var item = _inventory.GetItem(index);
            if (item == null) RemoveSlot(index);

            if (item is CountableItem countableItem)
            {
                if (countableItem.Amount <= 0)
                {
                    RemoveItemAndSlot(index);
                }
                else
                {
                    UpdateSlot(index);
                }
            }
            else if (item is ResourceGetterItem resourceGetterItem)
            {
                if (!resourceGetterItem.HasDurability())
                {
                    RemoveItemAndSlot(index);

                    _inventory.UnEquipItemFromSlot(resourceGetterItem.EquipmentData.Parts);
                }
            }
            else if (item is EquipmentItem equipmentItem)
            {
                    // 만약 장비가 팔렷으면 삭제
                RemoveItemAndSlot(index);
            }
        }

        private void RemoveItemAndSlot(int slotIndex)
        {
            _inventory.RemoveItem(slotIndex);  // Assuming this method removes the item from inventory.
            RemoveSlot(slotIndex);
        }

        private void RemoveSlot(int slotIndex)
        {
            itemSlots[slotIndex].ClearSlot();  // Clear the UI slot.
            itemSlotPool.Take(itemSlots[slotIndex]);
            itemSlots.Remove(slotIndex);
        }
        public void SellItem(int index, int count)
        {
            _inventory.SellItem(index, count);
            UpdateSlot(index);
        }
        public void UpdateSlot(int index)
        {
            if (!_inventory.HasItem(index))
            {
                if (itemSlots.ContainsKey(index))
                {
                    RemoveSlot(index);
                }
                return;
            }
            var item = _inventory.GetItem(index);

            if (item is CountableItem countableItem)
            {
                itemSlots[index].UpdateCount(countableItem.Amount);
            }
            else if (item is EquipmentItem equipmentItem)
            {
                itemSlots[index].UpdateUpgrade(equipmentItem.CurrentUpgrade);
            }
        }

        private void CheckAllEmptySlotUpdate()
        {
            // 만약 인벤토리의 index에 durability나 count가 0이면 슬롯을 지운다
            foreach (int ind in _inventory._inventory.Keys)
            {
                if (_inventory._inventory[ind] is CountableItem countableItem)
                {
                    if (countableItem.Amount <= 0)
                    {
                        itemSlots[ind].ClearSlot();
                        itemSlotPool.Take(itemSlots[ind]);
                    }
                } else if (_inventory._inventory[ind] is ResourceGetterItem resourceGetterItem)
                {
                    if (!resourceGetterItem.HasDurability())
                    {
                        itemSlots[ind].ClearSlot();
                        itemSlotPool.Take(itemSlots[ind]);
                    }
                }
            }
        }

        private void ClearSlots()
        {
            foreach (var slot in itemSlots.Values)
            {
                slot.ClearSlot();
                itemSlotPool.Take(slot);
            }
            itemSlots.Clear();
        }
    }
}
