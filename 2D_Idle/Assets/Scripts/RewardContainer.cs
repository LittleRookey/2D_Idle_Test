using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardContainer : MonoBehaviour
{
    [SerializeField] private LootTable reward;

    Health health;
    LevelSystem levelSystem;

    private void Awake()
    {
        health = GetComponent<Health>();
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
        

        ResourceManager.Instance.GainGold(reward.GetGoldReward());

        if (reward.HasDropItem())
        {
            // TODO 인벤토리에 넣기
            reward.GetDropItems();
        }
    }
}
