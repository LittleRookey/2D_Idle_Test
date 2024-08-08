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
        //��ư�� ����Ʈ �Ϸ�� ���� �ֱ�
        questSlotButton.onClick.AddListener(giveReward);
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
        QuestEvents.OnActionPerformed += UpdateQuestProgression;
        gameObject.SetActive(true);
    }

    private void UpdateQuestProgression(QuestType qType, string questID, int amount)
    {
        if (questID != currentQuest.questID)
        {
            Debug.Log($"Quest {currentQuest.questID} is not update beause it's not same quest {questID}");
            return;
        }
        var objective = currentQuest.objectives[0];
        Debug.Log("Updated quest progression" + $"{objective.description} ({currentQuestProgress.objectiveProgress[objective.objectiveId]+amount} / {objective.requiredAmount})");
        questProgressionText.SetText($"{objective.description} ({currentQuestProgress.objectiveProgress[objective.objectiveId] + amount} / {objective.requiredAmount})");
    }

    private void OnQuestCompleted(string questID)
    {
        if (questID != currentQuest.questID)
        {
            Debug.Log($"����Ʈ {questID}�� �Ϸ�ž����ϴ�. ������ ����Ʈ {currentQuestProgress.questId}�� �����Դϴ�.");
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
