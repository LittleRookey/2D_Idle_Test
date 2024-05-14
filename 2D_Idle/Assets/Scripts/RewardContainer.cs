using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;

public class RewardContainer : MonoBehaviour
{
    [SerializeField] private LootTable reward;

    Health health;
    LevelSystem levelSystem;

    private readonly string goldPath = "Images/CoinGold";
    private Sprite goldImage;
    private string goldPopupText = "��� +";
    private void Awake()
    {
        health = GetComponent<Health>();
        if (goldImage == null) goldImage = Resources.Load<Sprite>(goldPath);
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
        var popup = ResourcePopupCreator.CreatePopup(transform.position + Vector3.right * 2f, transform,goldImage, goldPopupText + gainGold.ToString("N0"));
        //popup.transform.position = transform.position;
        Debug.Log("popup pos: " + popup.transform.position);
        ResourceManager.Instance.GainGold(gainGold);

        if (reward.HasDropItem())
        {
            // TODO �κ��丮�� �ֱ�
            reward.GetDropItems();
        }
    }
}
