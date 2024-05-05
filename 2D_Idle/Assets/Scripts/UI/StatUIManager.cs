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

    private void OnEnable()
    {
        if (playerStat == null)
        {
            Debug.LogError("playerStat is null");
            return;
        }
        InitUpdateStats();
        playerStat.OnTryIncreaseStat.AddListener(TryUpdateStat);
        playerStat.OnIncreaseStat.AddListener(UpdateStat);
        playerStat.OnCancelStat.AddListener(UpdateStats);
        playerStat.OnApplyStat.AddListener(UpdateStats);

        // 각 StatBarUI의 UI를 이벤트에 넣어준다. 
        Debug.Log(playerStat.mainStats);
        Debug.Log(playerStat.mainStats.Keys.Count);
        foreach (var mainStatType in playerStat.mainStats.Keys)
        {
            //statBarUIDict[mainStatType].OnPlusClicked.AddListener()
            playerStat.OnTryIncreaseStat.AddListener(statBarUIDict[mainStatType].SetStatBarUIs);

            //emptyOne.SetStatBarUI(playerStat.mainStats[mainStatType]);
            //emptyOne.gameObject.SetActive(true);


        }
    }

    private void OnDisable()
    {
        playerStat.OnCancelStat.RemoveListener(UpdateStats);
        playerStat.OnApplyStat.RemoveListener(UpdateStats);
        playerStat.OnTryIncreaseStat.RemoveListener(TryUpdateStat);
        playerStat.OnIncreaseStat.RemoveListener(UpdateStat);
    }

    private void Start()
    {
        
    }

    private void InitStatDisplayUI()
    {
         foreach(var subStatType in playerStat.subStats.Keys)
        {
            var statDisplayUI = Instantiate(statDisplayPrefab, statDisplayParent);
            //statDisplayUI.SetStatDisplay(playerStat, subStatType, statColors.GetColor(subStatType), ,playerStat.subStats[subStatType].UIMaxValue);
        }
    }
    public void OpenStatWindow()
    {
        statWindow.gameObject.SetActive(true);

        UpdateStats();
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
            Debug.Log("Empty Added");
            emptyOne.SetStatBarUI(playerStat.mainStats[mainStatType]);
            emptyOne.plusButton.onClick.AddListener(()=>playerStat.TryAddMainStat(mainStatType));

            emptyOne.gameObject.SetActive(true);


        }
    }
    //public void Try
    public void UpdateStats()
    {
        apText.SetText($"{TMProUtility.GetColorText("AP: ", Color.green)}{playerStat.AbilityPoint}");
        Debug.Log(playerStat.mainStats.Keys.Count);
        foreach (var mainStatType in playerStat.mainStats.Keys)
        {
            UpdateStat(mainStatType);

        }
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
