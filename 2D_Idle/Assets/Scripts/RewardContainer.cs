using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;
using Litkey.InventorySystem;

public class RewardContainer : MonoBehaviour
{
    [SerializeField] private LootTable reward;

    Health health;
    LevelSystem levelSystem;

    private readonly string goldPath = "Images/CoinGold";
    private Inventory _inventory;
    private Sprite goldImage;
    private string goldPopupText = "골드 +";
    private void Awake()
    {
        health = GetComponent<Health>();
        if (goldImage == null) goldImage = Resources.Load<Sprite>(goldPath);
        _inventory = Resources.Load<Inventory>("ScriptableObject/Inventory");
    }
    public LootTable GetReward()
    {
        return reward;
    }

    private void OnEnable()
    {
        health.OnDeath.AddListener(GainReward);
    }

    private void OnDisable()
    {
        health.OnDeath.RemoveListener(GainReward);
    }

    public void GainReward(LevelSystem attacker)
    {
        if (attacker == null) return;
        attacker.GainExp(reward.GetExpReward());

        var gainGold = reward.GetGoldReward();
        //Debug.Log("Reward Dropper name + pos: " + gameObject.name + " // " + transform.position);
        DropItemCreator.CreateGoldDrop(transform.position, gainGold);
        ResourceManager.Instance.DisplayGold(gainGold, () => ResourceManager.Instance.GainGold(gainGold));

        if (reward.HasDropItem())
        {
            // TODO 인벤토리에 넣기
            var drops = reward.GetDropItems();
            if (drops != null && drops.Count > 0)
            {
                _inventory.AddToInventory(drops);
                for (int i = 0; i < drops.Count; i++)
                {
                    var droppedItem = drops[i];
                    if (droppedItem is CountableItem countableItem)
                    {
                        DropItemCreator.CreateDrop(transform.position, countableItem.CountableData, countableItem.Amount);
                        ResourceManager.Instance.DisplayItem(countableItem.CountableData, countableItem.Amount);
                    } 
                    else if (droppedItem is EquipmentItem equipItem)
                    {
                        DropItemCreator.CreateDrop(transform.position, equipItem.EquipmentData, 1);
                        ResourceManager.Instance.DisplayItem(equipItem.EquipmentData, 1);
                    }
                }
            }
        }
    }
}
