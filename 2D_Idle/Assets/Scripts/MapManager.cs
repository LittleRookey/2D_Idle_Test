using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using Litkey.Utility;
using Sirenix.OdinInspector;
using Redcode.Pools;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] private Canvas mapWindow;
    [SerializeField] private RectTransform stagesParent;
    private Dictionary<int, StageSlotUI> stageSlots;
    [SerializeField] private StageSlotUI stageSlotUIPrefab;
    [SerializeField] private UIChildConnector stageUIChildConnector;
    private Pool<StageSlotUI> stageSlotPool;

    public int activeStage = 1;
    private StageSlotUI prevSelected;
    private void Awake()
    {
        Instance = this;
        stageSlots = new Dictionary<int, StageSlotUI>();
        //stageSlotPool = Pool.Create<StageSlotUI>(stageSlotUIPrefab);
        //stageSlotPool.SetContainer(stagesParent);

        int index = 0;
        foreach(var stageUI in stagesParent.GetComponentsInChildren<StageSlotUI>())
        {
            stageUI.gameObject.SetActive(false);
            stageUI.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectStage(stageUI);
            });
            stageSlots.Add(index, stageUI);
            index++;
        }

        for (int i = 0; i < activeStage; i++)
        {
            ActivateStage(i);
        }
    }
    public void OpenMap()
    {
        mapWindow.enabled = true;
        for (int i = 0; i < activeStage; i++)
        {
            ActivateStage(i);
        }
    }

    private void SelectStage(StageSlotUI selectedStage)
    {
        if (prevSelected == null) prevSelected = selectedStage;
        else
        {
            prevSelected.Deselect();
            prevSelected = selectedStage;
        }
        prevSelected.Select();

    }

    public void ActivateStage(int index)
    {
        if (!stageSlots.ContainsKey(index))
        {
            Debug.LogError($"Stage {index} does not exist");
            return;
        }
        stageSlots[index].gameObject.SetActive(true);
        stageSlots[index].Unlock();
        stageSlots[index].Deselect();
        stageUIChildConnector.ConnectLines();
    }

    private void ClearStages()
    {
        if (prevSelected == null) return;

        prevSelected.Deselect();

        prevSelected = null;
    }

    public void CloseWindow()
    {
        mapWindow.enabled = false;
        ClearStages();
    }
}
