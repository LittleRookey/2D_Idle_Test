using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class StageInfoWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageTitleText;
    [SerializeField] private TextMeshProUGUI stageDescriptionText;
    [SerializeField] private SPUM_Equip monsterModel;
    [SerializeField] private Button enterStageButton;
    [SerializeField] private DOTweenAnimation openingAnim;

    public void SetStageInfo(Stage stageInfo, UnityAction EnterStage)
    {
        Debug.Log("Added Enter STage transition to button");
        enterStageButton.onClick.AddListener(EnterStage);

        var spumAvatar = stageInfo.Monster.GetComponent<SPUM_Equip>();
        monsterModel.UpdateAvatar(spumAvatar.GetAvatarInfo());
        
        monsterModel.SetGrey();
        
        stageTitleText.SetText(stageInfo.stageTitle);
        stageDescriptionText.SetText($"적: {stageInfo.Monster.Name}\n\n임무: {stageInfo.enemyCount}\n\n요구 전투력: {spumAvatar.GetComponent<StatContainer>().GetTotalPower(true)}");
        gameObject.SetActive(true);

        foreach (var tween in openingAnim.GetTweens()) tween.Restart();
    }

    public void CloseInfoWindow()
    {
        Debug.Log("Deleted Enter STage transition to button");
        enterStageButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

}
