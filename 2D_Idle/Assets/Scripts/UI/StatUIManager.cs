using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;

public class StatUIManager : MonoBehaviour
{
    [Header("StatBar")]
    [SerializeField] private TextMeshProUGUI apText;
    [SerializeField] private List<StatBarUI> statBarUIs;
    [SerializeField] private Transform statWindow;

    [SerializeField] private StatContainer playerStat;

    public Dictionary<eMainStatType, StatBarUI> statBarUIDict;


    [Header("StatDisplay")]
    [SerializeField] private StatDisplayUI statDisplayPrefab;
    [SerializeField] private RectTransform statDisplayParent;
    [SerializeField] private StatColor statColors;

    private Dictionary<eSubStatType, StatDisplayUI> statDisplayUIDict; 
    private void OnEnable()
    {
        if (playerStat == null)
        {
            Debug.LogError("playerStat is null");
            return;
        }
        InitUpdateStats();
        playerStat.OnTryIncreaseStat.AddListener(TryUpdateStat);
        playerStat.OnTryIncreaseStat.AddListener(TryUpdateSubStatDisplay);
        playerStat.OnIncreaseStat.AddListener(UpdateStat);
        
        playerStat.OnCancelStat.AddListener(UpdateStats);
        playerStat.OnCancelStat.AddListener(UpdateStatDisplays);
        
        playerStat.OnApplyStat.AddListener(UpdateStats);
        playerStat.OnApplyStat.AddListener(UpdatePreviewStatDisplays);

        // 각 StatBarUI의 UI를 이벤트에 넣어준다. 

        foreach (var mainStatType in playerStat.mainStats.Keys)
        {
            playerStat.OnTryIncreaseStat.AddListener(statBarUIDict[mainStatType].SetStatBarUIs);
        }
    }

    private void OnDisable()
    {
        playerStat.OnCancelStat.RemoveListener(UpdateStats);
        playerStat.OnCancelStat.RemoveListener(UpdateStatDisplays);
        playerStat.OnApplyStat.RemoveListener(UpdateStats);
        playerStat.OnApplyStat.RemoveListener(UpdatePreviewStatDisplays);
        playerStat.OnTryIncreaseStat.RemoveListener(TryUpdateStat);
        playerStat.OnTryIncreaseStat.RemoveListener(TryUpdateSubStatDisplay);
        playerStat.OnIncreaseStat.RemoveListener(UpdateStat);
    }

    private void Start()
    {
        InitStatDisplayUI();
        statWindow.gameObject.SetActive(false);
    }

    private void InitStatDisplayUI()
    {
        statDisplayUIDict = new Dictionary<eSubStatType, StatDisplayUI>();

         foreach (var subStatType in playerStat.subStats.Keys)
        {
            var statDisplayUI = Instantiate(statDisplayPrefab, statDisplayParent);
            //Debug.Log($"{subStatType}: {statColors.GetStatIcon(subStatType)}");

            statDisplayUIDict.Add(subStatType, statDisplayUI);
            //Debug.Log(playerStat.subStats);
            //Debug.Log(playerStat.subStats.Count);
            //Debug.Log(playerStat.subStats[subStatType]);
            statDisplayUI.SetStatDisplay(playerStat, subStatType, statColors.GetStatIcon(subStatType), statColors.GetColor(subStatType),  playerStat.subStats[subStatType].UIMaxValue);

        }
    }

    public void OpenStatWindow()
    {
        statWindow.gameObject.SetActive(true);
        //SetInfoModes(false);
        UpdateStats();
        UpdateStatDisplays();
    }

    public void CloseStatWindow()
    {
        // 올린 능력치 초기화
        playerStat.CancelStatChange();
        statWindow.gameObject.SetActive(false);
    }

    private StatBarUI GetEmptyStatBarUI()
    {
        for (int i = 0; i < statBarUIs.Count; i++)
        {
            if (statBarUIs[i].IsEmpty()) return statBarUIs[i];
        }
        return null;
    }

    private void TryUpdateStat(eMainStatType mainStat, int val)
    {
        apText.SetText($"{TMProUtility.GetColorText("AP: ", Color.green)}{playerStat.AbilityPoint - playerStat.addedStat}");
        statBarUIDict[mainStat].SetStatBarUIs(mainStat, val);
    }

    private void TryUpdateSubStatDisplay(eMainStatType mainStat, int val)
    {
        var childStats = playerStat.mainStats[mainStat].ChildSubstats;
        foreach(var subStat in childStats)
        {
            //Debug.Log($"{subStat.statType}: {playerStat.GetTotalPreviewOf(subStat.statType)}");
            statDisplayUIDict[subStat.statType].PreviewStat(playerStat.GetTotalPreviewOf(subStat.statType));
        }
    }

    private void UpdateStat(eMainStatType mainStat)
    {
        apText.SetText($"{TMProUtility.GetColorText("AP: ", Color.green)}{playerStat.AbilityPoint}");
        var statBarUI = statBarUIDict[mainStat];
        statBarUI.SetStatBarUI(playerStat.mainStats[mainStat]);
    }

    private void InitUpdateStats()
    {
        if (playerStat == null)
        {
            Debug.LogWarning("playerStat is null");
            return;
        }

        apText.SetText($"{TMProUtility.GetColorText("AP: ", Color.green)}{playerStat.addedStat}");
        
        if (playerStat.mainStats == null)
        {
            Debug.LogWarning("playerStat.mainStats is null");
            return;
        }
        
        foreach (var mainStatType in playerStat.mainStats.Keys)
        {
            var emptyOne = GetEmptyStatBarUI();
            statBarUIDict.Add(mainStatType, emptyOne);

            // 초기설정
            emptyOne.InitMainStat(playerStat, mainStatType);
            emptyOne.SetStatBarUI(playerStat.mainStats[mainStatType]);
            emptyOne.plusButton.onClick.AddListener(()=>playerStat.TryAddMainStat(mainStatType));
            emptyOne.minusButton.onClick.AddListener(() => playerStat.TryAddMainStat(mainStatType, -1));

            emptyOne.gameObject.SetActive(true);


        }
    }
    //public void Try
    public void UpdateStats()
    {
        Debug.Log("abilityPoint: "+playerStat.AbilityPoint);
        apText.SetText($"{TMProUtility.GetColorText("AP: ", Color.green)}{playerStat.AbilityPoint}");
        //Debug.Log(playerStat.mainStats.Keys.Count);
        foreach (var mainStatType in playerStat.mainStats.Keys)
        {
            UpdateStat(mainStatType);

        }
    }

    //private void SetInfoModes(bool infoModeOn)
    //{
    //    foreach (var mainStatType in playerStat.mainStats.Keys)
    //    {
    //        statBarUIDict[mainStatType].InfoMode(infoModeOn);

    //    }
    //}


    private void UpdateStatDisplays()
    {
        foreach (var statType in playerStat.subStats.Keys)
        {
            UpdateStatDisplay(statType);
        }
    }

    // 창 오픈하거나 스텟 취소할떄 쓰임
    private void UpdateStatDisplay(eSubStatType statType)
    {
        this.statDisplayUIDict[statType].UpdateStat();
    }

    // applystat 할때 쓰임
    private void UpdatePreviewStatDisplay(eSubStatType statType)
    {
        statDisplayUIDict[statType].UpdatePreviewedStat();
    }

    private void UpdatePreviewStatDisplays()
    {
        foreach (var statType in playerStat.subStats.Keys)
        {
            statDisplayUIDict[statType].UpdatePreviewedStat();
        }
    }

    private void PreviewStatDisplay(eSubStatType statType, float extraValue)
    {
        statDisplayUIDict[statType].PreviewStat(extraValue);
    }

    private void Awake()
    {
        statBarUIDict = new Dictionary<eMainStatType, StatBarUI>();
        for (int i = 0; i < statBarUIs.Count; i++)
        {
            statBarUIs[i].gameObject.SetActive(false);
        }
        

    }
}
