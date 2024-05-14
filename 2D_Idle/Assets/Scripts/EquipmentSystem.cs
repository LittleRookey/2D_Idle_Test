using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Litkey.Interface;


public class EquipmentSystem : MonoBehaviour, ILoadable, ISavable
{
    public static EquipmentSystem Instance;


    public EquipmentTier weapon;
    public EquipmentTier topArmor;

    [SerializeField]
    private GameDatas gameData;
    public UnityAction OnUpgradeSuccess;

    private PlayerData playerData;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        gameData.OnGameDataLoaded.AddListener(Load);
    }

    private void OnDisable()
    {
        gameData.OnGameDataLoaded.RemoveListener(Load);
    }
    public bool UpgradeWeapon(EquipmentTier eTier)
    {
        if (ResourceManager.Instance.HasGold(eTier.requiredGold))
        {
            ResourceManager.Instance.UseGold(eTier.requiredGold);
            eTier.UpgradeLevel();
            OnUpgradeSuccess?.Invoke();
            Debug.Log("Upgrade Success");
            Save();
            return true;
        } else
        {
            // Not enouugh gold to upgrade
            Debug.Log("Not ENough Gold");
            return false;
        }
        
    }

    public void Save()
    {
        Debug.Log(playerData);
        Debug.Log(playerData.weapon);
        playerData.weapon.SetStatus(this.weapon);
        playerData.topArmor.SetStatus(this.topArmor);
        gameData.SaveDataLocal();
    }

    public void Load()
    {
        playerData = gameData.dataSettings.playerData;
        this.weapon.Initialize(playerData.weapon);
        this.topArmor.Initialize(playerData.topArmor);
        Debug.Log("WEquipments Initialized");
    }

    // 골드
    // 현재 장비 레벨
    // Start is called before the first frame update


}

