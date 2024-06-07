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
    // 나중에 필터할떄 쓰임
    
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