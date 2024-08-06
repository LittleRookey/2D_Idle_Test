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
using TransitionsPlus;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Canvas mapWindow;
    [SerializeField] private RectTransform stagesParent;
    [SerializeField] private StageSlotUI stageSlotUIPrefab;
    [SerializeField] private UIChildConnector stageUIChildConnector;
    [SerializeField] private TransitionAnimator transition;
    [SerializeField] private StageInfoWindow stageInfoWindow;
    [SerializeField] private List<Stage> stageList;

    private Dictionary<int, StageSlotUI> stageSlots = new Dictionary<int, StageSlotUI>();
    private Dictionary<int, Stage> stages = new Dictionary<int, Stage>();
    private Pool<StageSlotUI> stageSlotPool;

    public int ActiveStage = 1;
    private StageSlotUI prevSelected;

    private bool isLoadingStage = false;
    [SerializeField] private ResourceInteractor player;
    private void Awake()
    {
        Instance = this;
        InitializeStages();
        SetupStageUI();
        stageInfoWindow.CloseInfoWindow();
    }

    private void InitializeStages()
    {
        for (int i = 0; i < stageList.Count; i++)
        {
            stages.Add(i, stageList[i]);
        }
    }

    private void SetupStageUI()
    {
        var stageUIs = stagesParent.GetComponentsInChildren<StageSlotUI>();
        foreach (var stageUI in stageUIs)
        {
            ConfigureStageUI(stageUI);
        }

        for (int i = 0; i < ActiveStage; i++)
        {
            ActivateStage(i);
        }
    }

    private void ConfigureStageUI(StageSlotUI stageUI)
    {
        int stageIndex = stageUI.transform.GetSiblingIndex();
        stageUI.gameObject.SetActive(false);
        if (!stages.ContainsKey(stageIndex)) return;
        Debug.Log("Added function to stage UI");
        stageUI.GetComponent<Button>().onClick.AddListener(() => OnStageClicked(stageIndex, stageUI));
        
        if (!stageSlots.ContainsKey(stageIndex)) stageSlots.Add(stageIndex, stageUI);
    }

    private void OnStageClicked(int stageIndex, StageSlotUI stageUI)
    {
        if (isLoadingStage) return; // Exit if already loading a stage


        Debug.Log($"Stage {stageIndex} clicked");
        SelectStage(stageUI);
        stageInfoWindow.SetStageInfo(stages[stageIndex], () =>
        {
            isLoadingStage = true; // Set the flag to prevent further executions
            //transition.Play();
            StartCoroutine(LoadBattleSceneAsync(stages[stageIndex]));
            transition.onTransitionEnd.AddListener(() =>
            {
                //Debug.Log("Starting Transition Scene");
            });
        });
    }

    public void OpenMap()
    {
        mapWindow.gameObject.SetActive(true);
        //SetupStageUI();
        for (int i = 0; i < ActiveStage; i++)
        {
            ActivateStage(i);
        }
    }

    private void SelectStage(StageSlotUI selectedStage)
    {
        prevSelected?.Deselect();
        prevSelected = selectedStage;
        Debug.Log("Selected stage ");
        prevSelected.Select();
    }

    public void ActivateStage(int index)
    {
        if (!stageSlots.TryGetValue(index, out StageSlotUI stageSlot))
        {
            Debug.LogError($"Stage {index} does not exist");
            return;
        }
        stageSlot.gameObject.SetActive(true);
        stageSlot.Unlock();
        stageSlot.Deselect();
        stageUIChildConnector.ConnectLines();
    }

    private void ClearStages()
    {
        prevSelected?.Deselect();
        prevSelected = null;
    }

    public void CloseWindow()
    {
        mapWindow.gameObject.SetActive(false);
        ClearStages();
        stageInfoWindow.CloseInfoWindow();
    }
    private IEnumerator LoadBattleSceneAsync(Stage stage)
    {
        // Step 1: Fade in
        Debug.Log("11111111111");
        yield return StartCoroutine(FadeInScreenCoroutine());
        Debug.Log("222222222222");
        CloseWindow();

        player.ResetInteractor();
        // Step 2: Unload Town scene
        yield return StartCoroutine(UnloadSceneAsync("Town"));
        Debug.Log("3333333333");
        // Step 3: Load Battle scene
        yield return StartCoroutine(LoadSceneAsync("BattleScene", LoadSceneMode.Additive));
        Debug.Log("44444444444");
        // Step 4: Setup the stage
        StageManager stageManager = FindObjectOfType<StageManager>();
        if (stageManager != null)
        {
            Debug.Log("StageManager Setting up stage");
            stageManager.SetupStage(stage);
            Debug.Log("StageManager Setup Complete");
            yield return null;
            Debug.Log("Fading out screen");
            yield return FadeOutScreen().WaitForCompletion();
            StageTitleUI.Instance.SetStageTitleUI(stage.stageTitle);
        }
        else
        {
            Debug.LogError("StageManager not found in the Battle scene!");
        }
        isLoadingStage = false;
    }

    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
    [SerializeField] private float fadeDuration = 1f;
    private Tween FadeInScreen()
    {
        //return transition.Play();
        float startValue = 0f;
        return DOTween.To(() => startValue, x => startValue = x, 1f, fadeDuration)
            .OnUpdate(() => transition.SetProgress(startValue))
            .SetEase(Ease.InOutSine)
            .OnComplete(() => transition.onTransitionEnd?.Invoke());
    }

    private Tween FadeOutScreen()
    {
        float startValue = 1f;
        return DOTween.To(() => startValue, x => startValue = x, 0f, fadeDuration)
            .OnUpdate(() => transition.SetProgress(startValue))
            .SetEase(Ease.InOutSine);
    }

    private IEnumerator FadeInScreenCoroutine()
    {
        var fadeInTween = FadeInScreen();
        yield return fadeInTween.WaitForCompletion();
    }
}