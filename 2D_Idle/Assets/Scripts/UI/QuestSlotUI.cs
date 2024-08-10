using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Litkey.Quest;
using UnityEngine.Events;

public class QuestSlotUI : MonoBehaviour
{
    [SerializeField] private Button questSlotButton;
    [SerializeField] private TextMeshProUGUI questTypeText;
    [SerializeField] private TextMeshProUGUI questProgressionText;

    [Header("Completion")]
    [SerializeField] private RectTransform completedImage;
    [SerializeField] private DOTweenAnimation completedAnim;

    private const string mainQuest = "Quest";

    private QuestProgress currentQuestProgress;
    Quest currentQuest;
    public void SetQuestSlotUI(Quest quest, UnityAction giveReward)
    {
        ClearQuestSlot();
        //quest.
        currentQuest = quest;
        var objective = quest.objectives[0];
        if (objective == null)
        {
            Debug.LogError($"Quest {quest.questName} objective is null");
            return;
        }

        questTypeText.SetText(mainQuest);
        currentQuestProgress = QuestManager.Instance.GetQuestProgress(quest.questID);
   
        questProgressionText.SetText($"{objective.targetId} ({currentQuestProgress.objectiveProgress[objective.objectiveId]} / {objective.requiredAmount})");
        //버튼에 퀘스트 완료시 보상 주기
        questSlotButton.onClick.AddListener(giveReward);
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
        QuestEvents.OnActionPerformed += UpdateQuestProgression;
        gameObject.SetActive(true);
    }

    private void UpdateQuestProgression(QuestType qType, string targetId, int amount)
    {
        if (currentQuest == null || currentQuestProgress == null) return;

        var objective = currentQuest.objectives[0];
        if (objective.actionType == qType && (objective.targetId == targetId || objective.targetId == "any"))
        {
            // Update the current quest progress
            if (currentQuestProgress.objectiveProgress.TryGetValue(objective.objectiveId, out int currentAmount))
            {
                //currentQuestProgress.objectiveProgress[objective.objectiveId] = currentAmount + amount;
                UpdateProgressionText();
            }
        }
    }

    private void UpdateProgressionText()
    {
        var objective = currentQuest.objectives[0];
        int currentAmount = currentQuestProgress.objectiveProgress[objective.objectiveId];
        questProgressionText.SetText($"{objective.description} ({currentAmount} / {objective.requiredAmount})");
        Debug.Log($"Updated quest progression for {currentQuest.questName}: {currentAmount} / {objective.requiredAmount}");
    }


    private void OnQuestCompleted(string questID)
    {
        if (questID != currentQuest.questID)
        {
            Debug.Log($"퀘스트 {questID}가 완료돼었습니다. 하지만 퀘스트 {currentQuestProgress.questId}는 아직입니다.");
            return;
        }

        completedImage.gameObject.SetActive(true);
        completedAnim.DORestart();
    }

    public void ClearQuestSlot()
    {
        currentQuest = null;
        currentQuestProgress = null;
        questSlotButton.onClick.RemoveAllListeners();
        completedImage.gameObject.SetActive(false);
        completedAnim.DOPause();
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
        QuestEvents.OnActionPerformed -= UpdateQuestProgression;
        gameObject.SetActive(false);
    }
}
