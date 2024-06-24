using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPUM_Equip : MonoBehaviour
{
    SPUM_SpriteList spriteList;

    [Header("Weapons")]
    SpriteRenderer LWeapon;
    SpriteRenderer LShield;
    SpriteRenderer RWeapon;
    SpriteRenderer RShield;

    private void Awake()
    {
        spriteList = GetComponent<SPUM_SpriteList>();

        if (spriteList == null)
        {
            spriteList = GetComponentInChildren<SPUM_SpriteList>();
        }

        if (spriteList != null)
        {
            RWeapon = spriteList._weaponList.Count > 0 ? spriteList._weaponList[0] : null;
            RShield = spriteList._weaponList.Count > 1 ? spriteList._weaponList[1] : null;
            LWeapon = spriteList._weaponList.Count > 2 ? spriteList._weaponList[2] : null;
            LShield = spriteList._weaponList.Count > 3 ? spriteList._weaponList[3] : null;
        }
    }
    public void SetRightHand(Sprite sprite = null, bool isShield = false)
    {
        if (isShield && RShield == null)
            return;

        if (!isShield && RWeapon == null)
            return;

        RShield.sprite = isShield ? sprite : null;
        RWeapon.sprite = !isShield ? sprite : null;
    }

    public void SetLeftHand(Sprite sprite = null, bool isShield = false)
    {
        if (isShield && LShield == null)
            return;

        if (!isShield && LWeapon == null)
            return;

        LShield.sprite = isShield ? sprite : null;
        LWeapon.sprite = !isShield ? sprite : null;
    }
}
