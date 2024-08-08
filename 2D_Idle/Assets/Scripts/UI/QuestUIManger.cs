using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Redcode.Pools;
using Litkey.Quest;
using DG.Tweening;

public class QuestUIManger : MonoBehaviour
{
    [SerializeField] private QuestSlotUI questSlotUIPrefab;
    [SerializeField] private RectTransform questSlotParent;
    [SerializeField] private DOTweenAnimation openCloseAnim;
    [SerializeField] private Image arrowImage;

    Pool<QuestSlotUI> questSlotPool;
    Dictionary<string, QuestSlotUI> questSlotList;
    [SerializeField] private QuestDatabase questDB;
    Vector3 one;
    Vector3 minusOne;
    private void Awake()
    {
        one = Vector3.one;
        minusOne = new Vector3(-1, 1, 1);
        isQuestOpen = true;
        arrowImage.transform.localScale = minusOne;

        questSlotList = new Dictionary<string, QuestSlotUI>();
        questSlotPool = Pool.Create<QuestSlotUI>(questSlotUIPrefab);
        questSlotPool.SetContainer(questSlotParent);
        QuestEvents.OnQuestActivated += ShowQuestUI;
    }
    [SerializeField] private bool isQuestOpen = true;
    public void ToggleOpenCloseQuestUI()
    {
        if (isQuestOpen)
        {
            // Close
            openCloseAnim.DOPlayForward();
            isQuestOpen = false;
            arrowImage.transform.localScale = one;
        }
        else
        {
            // if quest is closed, open
            openCloseAnim.DOPlayBackwards();
            isQuestOpen = true;
            arrowImage.transform.localScale = minusOne;
        }
    }
    private void ShowQuestUI(string questID)
    {
        var questSlot = questSlotPool.Get();

        if (questSlotList.ContainsKey(questID))
        {
            Debug.LogError($"Already contains quest slot of quest ID {questID}");
            return;
        }

        questSlotList.Add(questID, questSlot);
        var quest = questDB.GetQuest(questID);
        questSlot.SetQuestSlotUI(quest, () =>
        {
            Debug.Log($"[System]: Quest {questID} Reward Given");
            //RemoveQuestSlot(questID);
        });
    }

    private void RemoveQuestSlot(string questID)
    {
        if (!questSlotList.ContainsKey(questID))
        {
            Debug.LogError("QuestSlots does not contain slot with quest ID " + questID);
            return;
        }
        questSlotList[questID].ClearQuestSlot();
        questSlotList.Remove(questID);
    }
}
