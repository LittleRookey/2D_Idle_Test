using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;

public class PlayerStatContainer : StatContainer, ILoadable, ISavable
{


    [SerializeField] private GameDatas gameDatas;

    // 로드 된 뒤에 능력치 로드
    PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        if (TryGetComponent<LevelSystem>(out LevelSystem lvlSystem))
        {
            lvlSystem.unitLevel.OnLevelUp += (float a, float b) =>
            {
                Debug.Log("Leveled up");
                // TODO stat per level 로드하기
                IncreaseAbilityPoint(1);
            };
        }

        gameDatas.OnGameDataLoaded.AddListener(Load);
        this.OnApplyStat.AddListener(Save);
    }

    private void OnEnable()
    {
        Load();
    }


    private void IncreaseAbilityPoint(int val)
    {
        this.AbilityPoint += val;
        Save();
    }

    public void Load()
    {
        ClearMainStats();
        ClearStatGivenPoints();

        playerData = gameDatas.dataSettings.playerData;

        this.Strength.IncreaseStat(playerData.StrengthLevel);
        this.Avi.IncreaseStat(playerData.AVILevel);
        this.Vit.IncreaseStat(playerData.VitLevel);
        this.Sensation.IncreaseStat(playerData.SensationLevel);
        this.Int.IncreaseStat(playerData.IntLevel);
        this.AbilityPoint = playerData.leftAbilityPoint;

        OnStatSetupComplete?.Invoke(this);
        Debug.Log("PlayerStat loaded in PlayerStatContainer");
    }

    public void Save()
    {
        playerData.SetStat(this);

        gameDatas.SaveDataLocal();
    }

    public void EquipEquipment(EquipmentItem equipItem)
    {
        var baseStats = equipItem.EquipmentData.GetStats();
        
        foreach(var stat in baseStats)
        {
            subStats[stat.statType].EquipValue(equipItem.ID, stat);
        }
    }

    public void UnEquipEquipment(EquipmentItem equipItem)
    {
        var baseStats = equipItem.EquipmentData.GetStats();

        foreach (var stat in baseStats)
        {
            subStats[stat.statType].UnEquipValue(equipItem.ID, stat);
        }
    }
}
