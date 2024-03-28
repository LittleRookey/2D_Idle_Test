using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipmentSystem : MonoBehaviour
{



    public EquipmentTier weapon;
    public EquipmentTier topArmor;

    //[SerializeField] private 
    public UnityAction OnUpgradeSuccess;

    public bool UpgradeWeapon(EquipmentTier eTier)
    {
        if (ResourceManager.Instance.HasGold(eTier.requiredGold))
        {
            ResourceManager.Instance.UseGold(eTier.requiredGold);
            eTier.UpgradeLevel();
            OnUpgradeSuccess?.Invoke();
            Debug.Log("Upgrade Success");
            return true;
        } else
        {
            // Not enouugh gold to upgrade
            Debug.Log("Not ENough Gold");
            return false;
        }
        
    }

    // 골드
    // 현재 장비 레벨
    // Start is called before the first frame update
   
}

