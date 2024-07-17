using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;
using Litkey.Utility;
using DG.Tweening;
using Litkey.Character.Cooldowns;
using UnityEngine.Events;

public class MineInteractor : Interactor
{
    [Header("Mine-specific Settings")]
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float animationRate=0;
    [SerializeField] private LeveledRankLootTable _mineLoot;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ResourceCapacity _capacity;

    private DOTweenAnimation _dotweenAnim;

    public bool IsEmpty => _capacity.remainingChance <= 0;

    protected override void Awake()
    {
        base.Awake();
        _dotweenAnim = GetComponent<DOTweenAnimation>();
    }

    public MineInteractor SetMine(string id, float cooldownTime, float interactionTime, int remainingChanceToGainResource)
    {
        this.ID = id;
        this._cooldownTime = cooldownTime;
        this._interactionTime = interactionTime;
        this._capacity.SetCapacity(remainingChanceToGainResource);
        return this;
    }

    public override void Interact(PlayerController player, UnityAction OnEnd = null)
    {
        if (!CanInteract(player))
        {
            WarningMessageInvoker.Instance.ShowMessage($"Cannot interact with this mine right now.");
            return;
        }

        StartCoroutine(InteractionCoroutine(player, () =>
        {
            MakeDropResource(player);
            OnEnd?.Invoke();
        }));
    }
    public override bool CanInteract(PlayerController player)
    {
        if (base.CanInteract(player))
        {
            if (IsEmpty)
            {
                WarningMessageInvoker.Instance.ShowMessage($"This resource is depleted.");
                return false;
            }
            if (!_inventory.IsMiningEquipped())
            {
                WarningMessageInvoker.Instance.ShowMessage("Mining tool is not equipped.");
                return false;
            }
            return true;
        }
        return false;
    }

    protected override void OnInteractionTick()
    {
        _dotweenAnim.DORestart();
        _particle.Play();        
    }

    private void MakeDropResource(PlayerController player)
    {
        _capacity.DecrementChance();
        var lootedResource = _mineLoot.GetRankedLootTable().GetSingleItem();
        if (lootedResource is CountableItem countableItem)
        {
            DropItemCreator.CreateDrop(transform.position, countableItem.CountableData, countableItem.Amount, player.bagTarget, () =>
            {
                ResourceManager.Instance.PlayBagDotween();
                ResourceManager.Instance.DisplayItem(countableItem.CountableData, countableItem.Amount);
                _inventory.AddToInventory(countableItem);
            });
        }
    }
}

[System.Serializable]
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
