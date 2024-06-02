using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Litkey.Interface;
using Redcode.Pools;
using Litkey.InventorySystem;

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


    public static UnityEvent<int> OnGainGold = new();
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
        gold += extraGold;
        OnGainGold?.Invoke(extraGold);
        Save();
    }

    public void UseGold(int usedGold)
    {
        gold -= usedGold;
        Save();
    }

    public void DisplayItem(ItemData item, int count, UnityAction OnEnd)
    {
        var dropUI = dropItemDisplayPool.Get();
        dropUI.SetItemUI(item, count, (DropItemDisplayUI dUI) =>
        {
            OnEnd?.Invoke();
            dropItemDisplayPool.Take(dUI);
        });
    }

    public void DisplayGold(int count, UnityAction OnEnd)
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

    public void Load()
    {
        this.gold = gameData.dataSettings.gold;
        OnResourceLoaded?.Invoke();
    }

    public void Save()
    {
        gameData.dataSettings.SetGold(gold);
        gameData.SaveDataLocal();
    }
}
