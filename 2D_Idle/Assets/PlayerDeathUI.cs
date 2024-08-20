using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Redcode.Pools;
using Litkey.InventorySystem;
using Sirenix.OdinInspector;

public class PlayerDeathUI : MonoBehaviour
{
    [SerializeField] private RectTransform orientation;
    [SerializeField] private DOTweenAnimation charDeathText;
    [SerializeField] private DOTweenAnimation returnToTownText;

    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private ItemSlotUI itemSlotUIPrefab;
    [SerializeField] private RectTransform itemSlotParent;
    Pool<ItemSlotUI> itemSlotPool;
    List<ItemSlotUI> activeSlots;

    private void Awake()
    {
        activeSlots = new List<ItemSlotUI>();
        itemSlotPool = Pool.Create<ItemSlotUI>(itemSlotUIPrefab);
        itemSlotPool.SetContainer(itemSlotParent);
    }
    [Button("ShowPlayerDeath")]
    public void ShowPlayerDeathUI()
    {
        orientation.gameObject.SetActive(true);
        StartCoroutine(StartShowPlayerDeathUI());
    }

    private IEnumerator StartShowPlayerDeathUI()
    {
        resultText.SetText(string.Empty);
        foreach (var tween in charDeathText.GetTweens())
        {
            tween.Restart();
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var tween in returnToTownText.GetTweens())
        {
            tween.Restart();
        }
        yield return new WaitForSeconds(0.1f);
        string _resultText = $"{1}회차 성과\n\n레벨 {1} → 레벨 {4}\n\n골드 {2242} → {3325}\n\n얻은 아이템";
        string startText = string.Empty;

        var stageProgress = MapManager.Instance.stageProgress;
        DOTween.To(() => startText, x => startText = x, _resultText, _resultText.Length / 5f)
            .OnUpdate(() =>
            {
                resultText.SetText(startText);
            })
            .OnComplete(()=> {
                // start showing item slots
                for(int i = 0; i < stageProgress.gainedItems.Count; i++)
                {
                    var itemSlot = itemSlotPool.Get();
                    itemSlot.SetSlot(stageProgress.gainedItems[i].itemData, stageProgress.gainedItems[i].count);
                    activeSlots.Add(itemSlot);
                }
                
            });
    }

    public void CloseWindow()
    {
        orientation.gameObject.SetActive(false);
        for (int i = 0; i < activeSlots.Count; i++)
        {
            activeSlots[i].ClearSlot();
            itemSlotPool.Take(activeSlots[i]);
        }
        activeSlots.Clear();
    }
}
