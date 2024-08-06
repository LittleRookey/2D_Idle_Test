using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPUM_Equip : MonoBehaviour
{
    [SerializeField] private SPUM_SpriteList spriteList;

    [Header("Weapons")]
    SpriteRenderer LWeapon;
    SpriteRenderer LShield;
    SpriteRenderer RWeapon;
    SpriteRenderer RShield;

    SpriteRenderer[] allSprites;
    private MaterialPropertyBlock mpb;

    private static readonly int GreyScaleProperty = Shader.PropertyToID("_GreyscaleLuminosity");

    private void Awake()
    {
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        
        if (spriteList == null) spriteList = GetComponent<SPUM_SpriteList>();


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

    public void SetBlack()
    {
        if (allSprites == null) allSprites = GetComponentsInChildren<SpriteRenderer>();
        ApplyToAllSprites(renderer =>
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat(GreyScaleProperty, -1);
            renderer.SetPropertyBlock(mpb);
        });
    }

    public void SetGrey()
    {
        if (allSprites == null) allSprites = GetComponentsInChildren<SpriteRenderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();
        ApplyToAllSprites(renderer =>
        {
            if (renderer == null) Debug.LogError("SpriteRenderer is null");
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat(GreyScaleProperty, 0);
            renderer.SetPropertyBlock(mpb);
        });
    }
    private void ApplyToAllSprites(System.Action<SpriteRenderer> action)
    {
        if (allSprites == null) allSprites = GetComponentsInChildren<SpriteRenderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();
        foreach (var sprite in allSprites)
        {
            action(sprite);
        }
    }

    public SPUM_SpriteList GetAvatarInfo()
    {
        return spriteList;
    } 

    public void UpdateAvatar(SPUM_SpriteList spriteList)
    {
        this.spriteList.LoadSprite(spriteList);
    }
}
