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
        health.OnDeath += GainReward;
    }

    private void OnDisable()
    {
        health.OnDeath -= GainReward;
    }

    public void GainReward(LevelSystem attacker)
    {
        attacker.GainExp(reward.GetExpReward());
        

        ResourceManager.Instance.GainGold(reward.GetGoldReward());

        if (reward.HasDropItem())
        {
            reward.GetDropItems();
        }
    }
}
