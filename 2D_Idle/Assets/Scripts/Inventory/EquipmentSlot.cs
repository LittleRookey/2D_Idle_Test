using Litkey.InventorySystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.InventorySystem
{
    [CreateAssetMenu(menuName ="Litkey/EquipmentSlot")]
    public class EquipmentSlot : SerializedScriptableObject
    {
        
        [SerializeField] private EquipmentItem equippedItem;
        [SerializeField] private eEquipmentParts parts;


        public UnityEvent<EquipmentItem> OnEquip;
        public UnityEvent<EquipmentItem> OnUnEquip;
        public bool IsEquipped => equippedItem != null;

        public EquipmentItem EquippedItem => equippedItem;

        public void Init()
        {
            this.equippedItem = null;

        }

        public void EquipItem(EquipmentItem equipItem)
        {
            if (equipItem.EquipmentData.Parts != this.parts)
            {
                Debug.LogError("장착된 장비의 파츠가 맞지 않습니다: " + equipItem.EquipmentData.Name);
            }


            if (IsEquipped)
            {
                // 장비가 장착돼있으면 스텟 해제하기
                UnEquipItem();
            }

            // // 스텟 장착하기
            // 장비 장착
            this.equippedItem = equipItem;
            Debug.Log("장비창 장착");
            OnEquip?.Invoke(this.equippedItem);
        }

        public void UnEquipItem()
        {
            if (!IsEquipped)
            {
                Debug.LogError("장비가 장착돼있지 않습니다");
                return;
            }

            OnUnEquip?.Invoke(this.equippedItem);

            // 장비 해제
            this.equippedItem = null;
        }
    }

}