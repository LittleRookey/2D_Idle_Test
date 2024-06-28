using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;

public class Interactor : MonoBehaviour, IInteractactable
{
    [SerializeField] private ResourceItemData resourceItemData;
    SpriteRenderer spriteRenderer;
    [SerializeField] private 
    Material resourceMat;

    private string outlineMatParam = "OUTBASE_ON";
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        resourceMat = spriteRenderer.material;
    }

    private void EnableOutline()
    {
        resourceMat.EnableKeyword(outlineMatParam);
    }

    private void DisableOutline() => resourceMat.DisableKeyword(outlineMatParam);

    public void Interact()
    {
        
    }

   
}
