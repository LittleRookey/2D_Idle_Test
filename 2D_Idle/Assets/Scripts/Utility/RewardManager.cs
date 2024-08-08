using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Litkey.InventorySystem;
using Redcode.Pools;
using AssetKits.ParticleImage;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [SerializeField] private Canvas rewardWindow;
    [SerializeField] private DOTweenAnimation openingAnimation;
    [SerializeField] private ItemSlotUI itemSlotUIPrefab;
    [SerializeField] private RectTransform itemSlotParent;
    [SerializeField] private TextMeshProUGUI closeText;
    [SerializeField] private ParticleImage confettiParticleImg;
    Pool<ItemSlotUI> itemSlotPool;
    List<ItemSlotUI> activeSlots;
    WaitForSeconds pointFive;
    private void Awake()
    {
        pointFive = new WaitForSeconds(0.5f);
        Instance = this;
        itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
        itemSlotPool.SetContainer(itemSlotParent);
        activeSlots = new List<ItemSlotUI>();
    }

    public void ShowReward(List<Item> items)
    {
        ClearItemSlots();
        rewardWindow.gameObject.SetActive(true);
        rewardWindow.enabled = true;
        closeText.gameObject.SetActive(false);
        openingAnimation.DORestart();
        StartCoroutine(StartRewardDisplay(items));
    }

    private IEnumerator StartRewardDisplay(List<Item> items)
    {
        yield return pointFive;
        for (int i = 0; i < items.Count; i++)
        {
            var itemSlot = itemSlotPool.Get();
            itemSlot.SetSlot(items[i]);
            itemSlot.GetComponent<DOTweenAnimation>().DORestart();
            activeSlots.Add(itemSlot);
            yield return pointFive;
        }
        confettiParticleImg.Play();
        closeText.gameObject.SetActive(true);
    }

    public void CloseRewardWindow()
    {
        rewardWindow.enabled = false;
        ClearItemSlots();
    }
    private void ClearItemSlots()
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            activeSlots[i].ClearSlot();
            itemSlotPool.Take(activeSlots[i]);
        }
        activeSlots.Clear();
    }
}
