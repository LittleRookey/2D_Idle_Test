using DG.Tweening;
using Litkey.InventorySystem;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeInteractor : Interactor
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private LeveledRankLootTable loot;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ResourceCapacity _capacity;

    private DOTweenAnimation _dotweenAnim;

    public bool IsEmpty => _capacity.remainingChance <= 0;

    protected override void Awake()
    {
        base.Awake();
        _dotweenAnim = GetComponent<DOTweenAnimation>();
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
            if (!_inventory.IsAxingEquipped())
            {
                WarningMessageInvoker.Instance.ShowMessage("도끼를 착용하지 않았습니다");
                return false;
            }
            return true;
        }
        return false;
    }

    protected override void OnInteractionTick(PlayerController player)
    {
        _dotweenAnim.DORestart();
        _particle.Play();
        //player.PlayMineInteract();
    }

    private void MakeDropResource(PlayerController player)
    {
        _inventory.UseResourceEquipmentItem(eResourceType.나무);
        _capacity.DecrementChance();
        var lootedResource = loot.GetRankedLootTable().GetSingleItem();
        if (lootedResource is CountableItem countableItem)
        {
            DropItemCreator.CreateDrop(transform.position, countableItem.CountableData, countableItem.Amount, player.bagTarget, () =>
            {
                ResourceManager.Instance.PlayBagDotween();
                ResourceManager.Instance.DisplayItem(countableItem.CountableData, countableItem.Amount);
                _inventory.AddToInventory(countableItem);
                if (IsEmpty)
                {
                    gameObject.SetActive(false);
                }
            });
        }
    }
}
