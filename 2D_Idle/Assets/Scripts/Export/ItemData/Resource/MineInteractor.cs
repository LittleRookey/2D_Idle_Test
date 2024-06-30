using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;
using Litkey.Utility;
using DG.Tweening;
using Litkey.Character.Cooldowns;

public class MineInteractor : MonoBehaviour, IInteractactable, ISelectable, IDeselectable, IHasCooldown
{
    [Header("References")]
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private LeveledRankLootTable _mineLoot;
    [SerializeField] private Inventory _inventory;
    [Header("Resource Info")]
    [SerializeField] private ResourceCapacity _capacity;
    public bool IsSelected { private set; get; }
    public bool IsEmpty 
    { 
        get
        {
            return _capacity.remainingChance <= 0;
        }    
    }

    public string ID => _id;

    public float CooldownDuration => _cooldownTime;


    private string _id;
    private float _cooldownTime;

    private string outlineMatParam = "OUTBASE_ON";
    private string shakeInput = "shake";

    DOTweenAnimation _dotweenAnim;
    SpriteRenderer _spriteRenderer;
    Material _resourceMat;
    WaitForSeconds _waitTime;

    public bool disableOutlineOnStart;
    CooldownSystem _cooldown;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _dotweenAnim = GetComponent<DOTweenAnimation>();
        _resourceMat = _spriteRenderer.material;
        _cooldown = GetComponent<CooldownSystem>();
        _waitTime = new WaitForSeconds(1f);
        if (disableOutlineOnStart) DisableOutline();
    }

    private void EnableOutline()
    {
        _resourceMat.EnableKeyword(outlineMatParam);
    }

    private void DisableOutline() => _resourceMat.DisableKeyword(outlineMatParam);

    public MineInteractor SetMine(string id, float cooldownTime, int remainingChanceToGainResource)
    {
        this._id = id;
        this._cooldownTime = cooldownTime;
        this._capacity.SetCapacity(remainingChanceToGainResource);
        return this;
    }

    public MineInteractor SetToCooldown(float remainingTime)
    {
        _cooldown.PutOnColdown(_id, remainingTime);
        return this;
    }
    public void Interact(int interactTime)
    {

        // start mining 
        Debug.Log("Mine Interacted!");
        StartCoroutine(StartMining(interactTime));
    }

    private IEnumerator StartMining(int totalTime)
    {
        var barProgress = BarCreator.CreateFillBar(transform.position + Vector3.up * 0.5f);
        barProgress.SetBar(false)
            .SetInnerColor(Color.green)
            .SetOuterColor(Color.black);

        barProgress.StartFillBar(totalTime, MakeDropResource);

        _particle.gameObject.SetActive(true);

        for (int i = 0; i < totalTime; i++) 
        {
            yield return _waitTime;
            _dotweenAnim.DORestart();
            _particle.Play();
        }
    }

    private void MakeDropResource()
    {
        var lootedResource = _mineLoot.GetRankedLootTable().GetSingleItem();
        if (lootedResource is CountableItem countableItem)
        {
            DropItemCreator.CreateDrop(transform.position, countableItem.CountableData, countableItem.Amount, () => 
            {
                ResourceManager.Instance.DisplayItem(countableItem.CountableData, countableItem.Amount);
                _inventory.AddToInventory(countableItem);
            });
             
        }
        
    }

    
    public void Deselect()
    {
        DisableOutline();
        Debug.Log("Mine Deselected");
        IsSelected = false;
    }

    public void Select()
    {
        EnableOutline();
        IsSelected = true;
        Debug.Log("Mine selected");
    }
}

public class ResourceCapacity
{
    public int ID; // ÀÌ°÷ÀÇ ID
    public int remainingChance; // ³²Àº Ã¤±¤, ¹ú¸ñ, ³¬½Ã È½¼ö

    public void SetCapacity(int chance)
    {
        remainingChance = chance;
    } 

    public bool DecrementChance()
    {
        if (remainingChance <= 0)
        {
            Debug.LogError("There is no more remaining chance to gain resource");
            return false;
        }
        remainingChance -= 1;
        return true;
    }
}
