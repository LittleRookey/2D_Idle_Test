using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;

public class StatUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI apText;
    [SerializeField] private List<StatBarUI> statBarUIs;

    [SerializeField] private Transform statWindow;

    [SerializeField] private StatContainer playerStat;

    public Dictionary<eMainStatType, StatBarUI> statBarUIDict;

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

        // �� StatBarUI�� UI�� �̺�Ʈ�� �־��ش�. 
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
        playerStat.OnTryIncreaseStat.RemoveListener(TryUpdateStat);
        playerStat.OnIncreaseStat.RemoveListener(UpdateStat);
    }

    private void Start()
    {
        
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
        statBarUIDict[mainStat].SetStatBarUIs(mainStat, val);
    }

    private void UpdateStat(eMainStatType mainStat)
    {
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

            // �ʱ⼳��
            emptyOne.InitMainStat(playerStat, mainStatType);
            Debug.Log("Empty Added");
            emptyOne.SetStatBarUI(playerStat.mainStats[mainStatType]);
            emptyOne.plusButton.onClick.AddListener(()=>playerStat.TryAddMainStat(mainStatType));

            emptyOne.gameObject.SetActive(true);


        }
    }

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
