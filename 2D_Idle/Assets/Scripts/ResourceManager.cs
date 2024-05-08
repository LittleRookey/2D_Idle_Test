using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Litkey.Interface;

public class ResourceManager : MonoBehaviour, ILoadable, ISavable
{
    public static ResourceManager Instance;


    public int Gold => gold;
    public int totalRunDistance;

    private int gold;

    [SerializeField] private GameDatas gameData;

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
