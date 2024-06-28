using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;

public class MineInteractor : MonoBehaviour, IInteractactable
{
    [SerializeField] private ResourceItemData resourceItemData;
    [SerializeField] private ParticleSystem particle;
    
    SpriteRenderer spriteRenderer;

    Material resourceMat;
    LootTable
    private string outlineMatParam = "OUTBASE_ON";
    WaitForSeconds waitTime;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        resourceMat = spriteRenderer.material;
        waitTime = new WaitForSeconds(1f);
    }

    private void EnableOutline()
    {
        resourceMat.EnableKeyword(outlineMatParam);
    }

    private void DisableOutline() => resourceMat.DisableKeyword(outlineMatParam);
    public void Interact()
    {
        
        // start mining 
    }

    private IEnumerator StartMining()
    {
        
    }
   
}
