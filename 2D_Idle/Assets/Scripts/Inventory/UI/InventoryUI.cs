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
    // ���߿� �����ҋ� ����
    
    public class InventoryUI : MonoBehaviour
    {
        
        [SerializeField] private Inventory _inventory;
        [SerializeField] private RectTransform slotsParent;
        [SerializeField] private ItemSlotUI itemSlotUIPrefab;

        Pool<ItemSlotUI> itemSlotPool;

        private void OnEnable()
        {
            _inventory.OnGainItem.AddListener(LoadInventory);
        }

        private void OnDisable()
        {
            _inventory.OnGainItem.RemoveListener(LoadInventory);
        }

        private List<ItemSlotUI> activeSlots;
        private void LoadInventory(List<Item> items)
        {
            if (itemSlotPool == null)
            {
                activeSlots = new List<ItemSlotUI>();
                itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
                itemSlotPool.SetContainer(slotsParent);
            }

            ClearSlots();

            foreach(var item in items)
            {
                var slot = itemSlotPool.Get();
                slot.SetSlot(item);
                slot.gameObject.SetActive(true);
                activeSlots.Add(slot);
                
            }
        }

        private void ClearSlots()
        {

            foreach(var slot in activeSlots)
            {
                slot.ClearSlot();
                itemSlotPool.Take(slot);
            }
            activeSlots.Clear();

        }
    }
}