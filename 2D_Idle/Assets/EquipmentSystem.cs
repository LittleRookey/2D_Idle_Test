using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSystem : MonoBehaviour
{
    public EquipmentTier weapon;
    public EquipmentTier topArmor;

    [ContextMenu("UpgradeWeapon")]
    public void UpgradeWeapon()
    {
        UpgradeWeapon(weapon);
    }

    public void UpgradeWeapon(EquipmentTier eTier)
    {
        if (ResourceManager.Instance.HasGold(eTier.requiredGold))
        {
            ResourceManager.Instance.UseGold(eTier.requiredGold);
            eTier.UpgradeLevel();
            Debug.Log("Upgrade Success");
        } else
        {
            // Not enouugh gold to upgrade
            Debug.Log("Not ENough Gold");
        }
        
    }

    // 골드
    // 현재 장비 레벨
    // Start is called before the first frame update
   
}

