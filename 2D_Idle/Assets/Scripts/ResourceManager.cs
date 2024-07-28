using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Litkey.Interface;
using Redcode.Pools;
using Litkey.InventorySystem;
using DG.Tweening;

public class ResourceManager : MonoBehaviour, ILoadable, ISavable
{
    public static ResourceManager Instance;


    public int Gold => gold;
    public int totalRunDistance;

    private int gold;

    [SerializeField] private GameDatas gameData;
    [Header("Drop Items")]
    [SerializeField] private RectTransform dropItemDisplayParent;
    [SerializeField] private DropItemDisplayUI dropItemDisplayUIPrefab;

    Pool<DropItemDisplayUI> dropItemDisplayPool;
    [SerializeField] private DOTweenAnimation goldDotween;
    [SerializeField] private DOTweenAnimation bagDotween;

    public static UnityEvent<int> OnGainGold = new();
    public static UnityEvent<int> OnUseGold = new();
    public UnityEvent OnResourceLoaded;
    private void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

        dropItemDisplayPool = Pool.Create<DropItemDisplayUI>(dropItemDisplayUIPrefab);
        dropItemDisplayPool.SetContainer(dropItemDisplayParent);
        gameData.OnGameDataLoaded.AddListener(Load);
        
        //gold = 1000;
    }

    public void GainGold(int extraGold)
    {
        OnGainGold?.Invoke(extraGold);
        gold += extraGold;
        Save();
    }

    public void UseGold(int usedGold)
    {
        OnUseGold?.Invoke(-1 * usedGold);
        gold -= usedGold;
        Save();
    }

    public void DisplayItem(ItemData item, int count, UnityAction OnEnd=null)
    {
        var dropUI = dropItemDisplayPool.Get();
        dropUI.gameObject.SetActive(true);
        dropUI.SetItemUI(item, count, (DropItemDisplayUI dUI) =>
        {
            OnEnd?.Invoke();
            dropItemDisplayPool.Take(dUI);
        });
    }

    public void DisplayGold(int count, UnityAction OnEnd=null)
    {
        var dropUI = dropItemDisplayPool.Get();
        dropUI.gameObject.SetActive(true);
        dropUI.SetGoldUI(count, (DropItemDisplayUI dUI) =>
        {
            OnEnd?.Invoke();
            dropItemDisplayPool.Take(dUI);
        });
    }
    public bool HasGold(int reqGold)
    {
        return gold >= reqGold;
    }

    public void PlayBagDotween()
    {
        foreach (var tween in bagDotween.GetTweens())
        {
            tween.Restart();
        }
    }
    
    public void PlayCoinDotween()
    {
        foreach (var tween in goldDotween.GetTweens())
        {
            tween.Restart();
        }
    }

    public void Load()
    {
        this.gold = gameData.dataSettings.gold;
        OnResourceLoaded?.Invoke();
        Debug.Log("자원 현황 로드 성공");
    }

    public void Save()
    {
        gameData.dataSettings.SetGold(gold);
        gameData.SaveDataLocal();
    }
}
