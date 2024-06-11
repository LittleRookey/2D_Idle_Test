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
        [SerializeField] private PlayerStatContainer statContainer; // StatContainer ���۷��� �߰�


        public UnityEvent<EquipmentItem> OnEquip;
        public UnityEvent<EquipmentItem> OnUnEquip;
        public bool IsEquipped => equippedItem != null;




        public void EquipItem(EquipmentItem equipItem)
        {
            if (equipItem.EquipmentData.Parts != this.parts)
            {
                Debug.LogError("������ ����� ������ ���� �ʽ��ϴ�: " + equipItem.EquipmentData.Name);
            }


            if (IsEquipped)
            {
                // ��� ������������ ���� �����ϱ�
                UnEquipItem();
            }

            // // ���� �����ϱ�
            // ��� ����
            this.equippedItem = equipItem;

            OnEquip?.Invoke(this.equippedItem);

            // ���� �����ϱ�
            //statContainer.EquipEquipment(this.equippedItem.ID, this.equippedItem.EquipmentData.GetStats());
        }

        public void UnEquipItem()
        {
            if (!IsEquipped)
            {
                Debug.LogError("��� ���������� �ʽ��ϴ�");
                return;
            }

            // ���� ����
            //statContainer.UnEquipEquipment(this.equippedItem);

            OnUnEquip?.Invoke(this.equippedItem);

            // ��� ����
            this.equippedItem = null;
        }
    }

}