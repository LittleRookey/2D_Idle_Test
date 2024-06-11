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
        [SerializeField] private Inventory _inventory;
        [SerializeField] private RectTransform slotsParent;
        [SerializeField] private ItemSlotUI itemSlotUIPrefab;
        [SerializeField] private RectTransform inventoryWindow;
        private Pool<ItemSlotUI> itemSlotPool;

        public Dictionary<int, ItemSlotUI> itemSlots;
        public Dictionary<int, Item> itemIndex; // ���Ժ� �ε����� ������ ����
        private ItemSlotUI currentSelectedSlot = null; // ���� ���õ� ����

        [ShowInInspector]
        public Dictionary<eEquipmentParts, int> equippedItemIndex; // �� ���Ժ� ������ �κ��丮�� ������ ������ �ε����� ����

        [SerializeField] private PlayerStatContainer playerStatContainer; 

        private void Awake()
        {
            equippedItemIndex = new Dictionary<eEquipmentParts, int>();
            foreach (var parts in (eEquipmentParts[])Enum.GetValues(typeof(eEquipmentParts)))
            {
                equippedItemIndex.Add(parts, -1);
            }
        }
        // ������ ����
        private void OnEnable()
        {
            _inventory.OnGainItem.AddListener(AddItemUI);
        }

        private void OnDisable()
        {
            _inventory.OnGainItem.RemoveListener(AddItemUI);
        }

        private void AddItemUI(Item item)
        {
            if (itemSlotPool == null)
            {
                itemSlots = new Dictionary<int, ItemSlotUI>();
                itemIndex = new Dictionary<int, Item>();
                itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
                itemSlotPool.SetContainer(slotsParent);
            }

            // �������� CountableItem�̸� ���ڸ� �ø���
            if (item is CountableItem countableItem)
            {
                int existingIndex = CheckItemExists(countableItem);
                if (existingIndex != -1)
                {
                    // �̹� �����ϴ� �������̸� ������ �ø�
                    if (itemIndex[existingIndex] is CountableItem existingItem)
                    {
                        existingItem.AddAmount(countableItem.Amount);
                        // ���� UI ������Ʈ
                        UpdateSlotUI(existingIndex, existingItem);
                        return;
                    }
                }
            }

            var slot = itemSlotPool.Get();
            int index = GetNextEmptyIndex();

            slot.SetSlot(item, () =>
            {
                // ������ Ŭ�� 2�� ���� ��, �������� ��� Ȥ�� ���� Ȥ�� ����
                OnSlotSecondClick(index, slot);
            });
            //OnSlotClick(index, slot);
            slot.OnFirstClick.AddListener(() => OnSlotClick(index, slot));

            slot.gameObject.SetActive(true);

            itemSlots[index] = slot;

            if (!itemIndex.ContainsKey(index))
            {
                itemIndex.Add(index, item);
            }
        }

        private int CheckItemExists(CountableItem countableItem)
        {
            foreach (var kvp in itemIndex)
            {
                if (kvp.Value is CountableItem existingItem && existingItem.ID == countableItem.ID)
                {
                    return kvp.Key;
                }
            }
            return -1; // �������� ������ -1 ��ȯ
        }

        private void UpdateSlotUI(int index, CountableItem item)
        {
            // ������ ���� UI ������Ʈ ���� �߰�
            if (itemSlots.TryGetValue(index, out var slot))
            {
                slot.SetSlot(item, () =>
                {
                    OnSlotClick(index, slot);
                });
            }
        }

        private void LoadInventory(List<Item> items)
        {
            if (itemSlotPool == null)
            {
                itemSlots = new Dictionary<int, ItemSlotUI>();
                itemIndex = new Dictionary<int, Item>();
                itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
                itemSlotPool.SetContainer(slotsParent);
            }

            ClearSlots();

            for (int i = 0; i < items.Count; i++)
            {
                var slot = itemSlotPool.Get();
                int index = i;
                slot.SetSlot(items[i], () =>
                {
                    // ������ Ŭ�� 2�� ���� ��, �������� ��� Ȥ�� ���� Ȥ�� ����
                    OnSlotClick(index, slot);
                });
                slot.gameObject.SetActive(true);
                itemSlots[index] = slot;
                if (!itemIndex.ContainsKey(index))
                {
                    itemIndex.Add(index, items[i]);
                }
            }
        }

        private int GetNextEmptyIndex()
        {
            int i = 0;
            while (i < 99999999)
            {
                if (itemIndex.ContainsKey(i))
                {
                    i++;
                    continue;
                }
                return i;
            }
            return -1;
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
            if (itemIndex.TryGetValue(slotIndex, out Item item))
            {
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
                            if (itemIndex[equippedIndex] is EquipmentItem equipItem)
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
                else 
                {
                    // ������ ��� ���� �߰� (�ʿ��� ���)
                    Debug.Log($"������ {item.Data.Name} ���");
                }
            }
            currentSelectedSlot = null;
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
