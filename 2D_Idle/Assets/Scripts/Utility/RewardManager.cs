using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Litkey.InventorySystem;
using Redcode.Pools;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [SerializeField] private Canvas rewardWindow;
    [SerializeField] private DOTweenAnimation openingAnimation;
    [SerializeField] private ItemSlotUI itemSlotUIPrefab;
    [SerializeField] private RectTransform itemSlotParent;
    [SerializeField] private DOTweenAnimation closeText;
    [SerializeField] private ParticleImage confettiParticleImg;
    [SerializeField] private HorizontalLayoutGroup horGroup;

    [Header("Gold Exp")]
    [SerializeField] private RectTransform goldSlot;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private RectTransform expSlot;
    [SerializeField] private TextMeshProUGUI expText;

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
        CloseRewardWindow();
    }
    [Button("ShowItems")]
    public void ShowReward(List<Item> items, int gold, int experience)
    {
        ClearItemSlots();
        rewardWindow.gameObject.SetActive(true);
        rewardWindow.enabled = true;
        closeText.gameObject.SetActive(false);
        openingAnimation.DORestart();
        StartCoroutine(StartRewardDisplay(items, gold, experience));
    }

    public void ShowReward(List<RewardItem> rewardItems, int gold, int experience)
    {
        ClearItemSlots();
        rewardWindow.gameObject.SetActive(true);
        rewardWindow.enabled = true;
        closeText.gameObject.SetActive(false);
        openingAnimation.DORestart();
      
        StartCoroutine(StartRewardDisplay(rewardItems, gold, experience));
    }

    private IEnumerator StartRewardDisplay(List<Item> items, int gold, int experience)
    {
        yield return pointFive;
        horGroup.enabled = true;
        if (gold > 0)
        {
            goldText.SetText(gold.ToString());
            goldSlot.gameObject.SetActive(true);

            goldSlot.GetComponent<DOTweenAnimation>().DORestart();
            yield return pointFive;
        }
        if (experience > 0f)
        {

            expText.SetText(experience.ToString("F0"));
            expSlot.gameObject.SetActive(true);

            expSlot.GetComponent<DOTweenAnimation>().DORestart();
            yield return pointFive;
        }
        yield return pointFive;
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                horGroup.enabled = false;
                var itemSlot = itemSlotPool.Get();
                itemSlot.SetSlot(items[i]);
                horGroup.enabled = true;
                horGroup.enabled = false;
                itemSlot.GetComponent<DOTweenAnimation>().DORestart();
                activeSlots.Add(itemSlot);
                yield return pointFive;
            }

        }
        confettiParticleImg.Play();
        closeText.gameObject.SetActive(true);
        closeText.DORestart();

    }

    private IEnumerator StartRewardDisplay(List<RewardItem> items, int gold, int experience)
    {
        yield return pointFive;
        horGroup.enabled = true;
        if (gold > 0)
        {
            goldText.SetText(gold.ToString());
            goldSlot.gameObject.SetActive(true);

            goldSlot.GetComponent<DOTweenAnimation>().DORestart();
            yield return pointFive;
        }
        if (experience > 0f)
        {

            expText.SetText(experience.ToString("F0"));
            expSlot.gameObject.SetActive(true);

            expSlot.GetComponent<DOTweenAnimation>().DORestart();
            yield return pointFive;
        }
        yield return pointFive;
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                horGroup.enabled = false;
                var itemSlot = itemSlotPool.Get();
                itemSlot.SetSlot(items[i].itemData, items[i].count);
                horGroup.enabled = true;
                horGroup.enabled = false;
                itemSlot.GetComponent<DOTweenAnimation>().DORestart();
                activeSlots.Add (itemSlot);
                yield return pointFive;
            }

        }
        confettiParticleImg.Play();
        closeText.gameObject.SetActive(true);
        closeText.DORestart();

    }

    public void CloseRewardWindow()
    {
        rewardWindow.enabled = false;
        ClearItemSlots();
        goldSlot.gameObject.SetActive(false);
        expSlot.gameObject.SetActive(false);
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
