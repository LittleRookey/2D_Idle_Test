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
    [SerializeField] private RectTransform resultItemWindow;
    Pool<ItemSlotUI> itemSlotPool;
    List<ItemSlotUI> activeSlots;
    [SerializeField] private LevelSystem playerLevel;
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
        resultItemWindow.gameObject.SetActive(false);
        resultText.SetText(string.Empty);
        foreach (var tween in charDeathText.GetTweens())
        {
            tween.Restart();
        }
        yield return new WaitForSeconds(0.3f);
        foreach (var tween in returnToTownText.GetTweens())
        {
            tween.Restart();
        }
        yield return new WaitForSeconds(0.3f);
        var stageProgress = MapManager.Instance.stageProgress;
        string _resultText = $"{1}회차 성과\n\n레벨 {stageProgress.previousLevel} → 레벨 {playerLevel.GetLevel()}\n\n골드 {stageProgress.previousGold} → {ResourceManager.Instance.Gold}\n\n얻은 아이템";
        string startText = string.Empty;

        DOTween.To(() => startText, x => startText = x, _resultText, _resultText.Length / 8f)
            .OnPlay(() => resultItemWindow.gameObject.SetActive(true))
            .OnUpdate(() =>
            {
                resultText.SetText(startText);
            })
            .OnComplete(()=> {
                
                // start showing item slots
                for (int i = 0; i < stageProgress.gainedItems.Count; i++)
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
