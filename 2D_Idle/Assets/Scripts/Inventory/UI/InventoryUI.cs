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
    [��� - ������ ����]
    - ���� ���� �� �������� ������ ���� �̸�����(����, ũ�� �̸����� ����)
    [��� - ���� �������̽�]
    - ���Կ� ���콺 �ø���
      - ��� ���� ���� : ���̶���Ʈ �̹��� ǥ��
      - ������ ���� ���� : ������ ���� ���� ǥ��
    - �巡�� �� ���
      - ������ ���� ���� -> ������ ���� ���� : �� ������ ��ġ ��ȯ
      - ������ ���� ���� -> ������ ������ ���� : ������ ��ġ ����
        - Shift �Ǵ� Ctrl ���� ������ ��� : �� �� �ִ� ������ ���� ������
      - ������ ���� ���� -> UI �ٱ� : ������ ������
    - ���� ��Ŭ��
      - ��� ������ �������� ��� : ������ ���
    - ��� ��ư(���� ���)
      - Trim : �տ������� �� ĭ ���� ������ ä���
      - Sort : ������ ����ġ��� ������ ����
    - ���� ��ư(���� ���)
      - [A] : ��� ������ ���͸�
      - [E] : ��� ������ ���͸�
      - [P] : �Һ� ������ ���͸�
      * ���͸����� ���ܵ� ������ ���Ե��� ���� �Ұ�
    [��� - ��Ÿ]
    - InvertMouse(bool) : ���콺 ��Ŭ��/��Ŭ�� ���� ���� ����
*/

namespace Litkey.InventorySystem
{
    // ���߿� �����Ҷ� ����
    public class InventoryUI : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private Inventory _inventory;
        [SerializeField] private RectTransform slotsParent;
        [SerializeField] private ItemSlotUI itemSlotUIPrefab;
        [SerializeField] private RectTransform inventoryWindow;
        private Pool<ItemSlotUI> itemSlotPool;

        public Dictionary<int, ItemSlotUI> itemSlots;
        //public Dictionary<int, Item> itemIndex; // ���Ժ� �ε����� ������ ����
        private ItemSlotUI currentSelectedSlot = null; // ���� ���õ� ����

        [ShowInInspector]
        public Dictionary<eEquipmentParts, int> equippedItemIndex; // �� ���Ժ� ������ �κ��丮�� ������ ������ �ε����� ����

        [SerializeField] private PlayerStatContainer playerStatContainer; 

        private void Awake()
        {
            _inventory.InitInventory();
            
            Init();

            _inventory.OnInventoryLoaded.AddListener(LoadInventoryUI);
            _inventory.OnInventoryLoaded.AddListener(UpdateEquippedItemUI);
            
        }
        // ������ ����
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
            // COuntableItem�̰� ���������� ������ã�Ƽ� ���Ծ�����Ʈ
            if (item is CountableItem && itemSlots.TryGetValue(itemIndex, out slotUI))
            {
                var cItem = _inventory.GetItem(itemIndex) as CountableItem; 
                // Update existing slot UI for countable items.
                slotUI.UpdateCount(cItem.Amount);
            }
            else if (item is EquipmentItem equipItem)
            {
                itemIndex = _inventory.FindItemInInventory(equipItem.ID);
                // ������̸�
                slotUI = itemSlotPool.Get();
                slotUI.SetSlot(item, () => OnSlotSecondClick(itemIndex, slotUI));
                slotUI.OnFirstClick.AddListener(() => OnSlotClick(itemIndex, slotUI));
                slotUI.gameObject.SetActive(true);
                itemSlots[itemIndex] = slotUI;
            } else
            {
                // ����� �ƴϰ� ���� ������
                slotUI = itemSlotPool.Get();
                slotUI.SetSlot(item, () => OnSlotSecondClick(itemIndex, slotUI));
                slotUI.OnFirstClick.AddListener(() => OnSlotClick(itemIndex, slotUI));
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
                slot.SetSlot(kValue.Value, () =>
                {
                    // ������ Ŭ�� 2�� ���� ��, �������� ��� Ȥ�� ���� Ȥ�� ����
                    OnSlotSecondClick(index, slot);
                });
                slot.OnFirstClick.AddListener(() => OnSlotClick(index, slot));
                itemSlots[index] = slot;
            }
        }


        // ������ ó�� ��������
        private void OnSlotClick(int slotIndex, ItemSlotUI slot)
        {
            if (currentSelectedSlot != null && currentSelectedSlot != slot)
            {
                currentSelectedSlot.ResetClickState(); // ���� ������ ���� �ʱ�ȭ
            }

            currentSelectedSlot = slot;
        }

        // ������ ��� ����������
        // ������ 2�� �������� �������� �ε����� �������� ������������ ��������, ��������
        // // ���ο� ������ �������� 
        // ���� �����մ� ������ �������� ��������
        // ���ο� ������ ��������, ���� ����
        private void OnSlotSecondClick(int slotIndex, ItemSlotUI slot)
        {
            var item = _inventory.GetItem(slotIndex);
           
            if (item is EquipmentItem equipmentItem)
            {
                // ������ ���� ���� �߰�
                int equippedIndex = equippedItemIndex[equipmentItem.EquipmentData.Parts];
                if (equippedIndex != -1)
                {
                    // �̹� ������ ���� �ְ� �̰� ���� �ε����� 
                    if (equippedIndex == slotIndex)
                    {
                        // ������ 2�� �������� �������� �ε����� �������� ������������ ��������, ��������
                        // ���� ����
                        itemSlots[equippedIndex].SetUnEquipped();
                        // �������� ����
                        _inventory.UnEquipItem(equipmentItem.EquipmentData.Parts); 
                        // ��������
                        playerStatContainer.UnEquipEquipment(equipmentItem);
                        equippedItemIndex[equipmentItem.EquipmentData.Parts] = -1;

                    }
                    else
                    {
                        // �ٸ� �ε����� �� ��� ����, �׸��� ����
                        if (_inventory.GetItem(equippedIndex) is EquipmentItem equipItem)
                        {
                            // ��������
                            playerStatContainer.UnEquipEquipment(equipItem);
                        } else
                        {
                            Debug.LogError($"Item index at {equippedIndex} is not equipment item");
                        }
                        itemSlots[equippedIndex].SetUnEquipped(); // ����� ����
                        _inventory.UnEquipItem(equipmentItem.EquipmentData.Parts);
                        // �� ��� ����, ���� ����


                        _inventory.EquipItem(equipmentItem); // ���� ���� ����
                        playerStatContainer.EquipEquipment(equipmentItem); // ���� ����

                        equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // �ε�������
                        itemSlots[slotIndex].SetEquipped(); // ��������
                    }
                        
                } else
                {
                    // �ش� ���Կ� �ƹ� ��� ������
                    _inventory.EquipItem(equipmentItem); // ���� ���� ����
                    playerStatContainer.EquipEquipment(equipmentItem); // ���� ����

                    equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // �ε�������
                    itemSlots[slotIndex].SetEquipped(); // ��������
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
            _inventory.EquipItem(equipmentItem); // ���� ���� ����
            playerStatContainer.EquipEquipment(equipmentItem); // ���� ����

            equippedItemIndex[equipmentItem.EquipmentData.Parts] = slotIndex; // �ε�������
            itemSlots[slotIndex].SetEquipped(); // ��������
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
            var item = _inventory.GetItem(index);
            if (item is CountableItem countableItem)
            {
                if (countableItem.Amount <= 0)
                {
                    RemoveItemAndSlot(index);
                }
            }
            else if (item is ResourceGetterItem resourceGetterItem)
            {
                if (!resourceGetterItem.HasDurability())
                {
                    RemoveItemAndSlot(index);

                    _inventory.UnEquipItem(resourceGetterItem.EquipmentData.Parts);
                }
            }
            else if (item is EquipmentItem equipmentItem)
            {
                // ���� ��� �ȷ����� ����

            }
        }

        private void RemoveItemAndSlot(int slotIndex)
        {
            _inventory.RemoveItem(slotIndex);  // Assuming this method removes the item from inventory.
            itemSlots[slotIndex].ClearSlot();  // Clear the UI slot.
            itemSlotPool.Take(itemSlots[slotIndex]);
            itemSlots.Remove(slotIndex);
        }

        private void CheckAllEmptySlotUpdate()
        {
            // ���� �κ��丮�� index�� durability�� count�� 0�̸� ������ �����
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
