using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;

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

    private void IncreaseAbilityPoint(int val)
    {
        this.AbilityPoint += val;
        Save();
    }

    public void Load()
    {
        ClearMainStats();
        ClearStatGivenPoints();

        //SaveManager.Instance.    
        playerData = gameDatas.dataSettings.playerData;
        //Debug.Log(gameDatas.dataSettings);
        //Debug.Log(gameDatas.dataSettings.playerData);
        this.Strength.IncreaseStat(playerData.StrengthLevel);
        this.Avi.IncreaseStat(playerData.AVILevel);
        this.Vit.IncreaseStat(playerData.VitLevel);
        this.Sensation.IncreaseStat(playerData.SensationLevel);
        this.Int.IncreaseStat(playerData.IntLevel);
        this.AbilityPoint = playerData.leftAbilityPoint;
    }

    public void Save()
    {
        playerData.SetStat(this);

        gameDatas.SaveDataLocal();
    }
}
