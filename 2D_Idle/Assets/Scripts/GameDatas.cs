using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Linq;
using Litkey.Skill;
using Litkey.InventorySystem;

[System.Serializable]
public class GameData
{
    public int gold = 100;
    public PlayerData playerData;

    public SerializableInventory inventoryData;

    public GameData()
    {
        gold = 100;
        playerData = new PlayerData();
        inventoryData = new SerializableInventory();
    }
    public void SetGold(int gold) => this.gold = gold;

   
}

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public float currentExp = 0;

    // 능력치 저장
    public int leftAbilityPoint = 1;
    public int StrengthLevel = 0;
    public int VitLevel = 0;
    public int AVILevel = 0;
    public int SensationLevel = 0;
    public int IntLevel = 0;

    public EquipmentUpgradeStatus weapon;
    //public EquipmentUpgradeStatus subWeapon;
    public EquipmentUpgradeStatus topArmor;


    public Dictionary<string, SkillData> skillDatas;

    public Dictionary<int, string> equippedActiveSkills;

    public int unlockedActiveSkillSlots;
    public PlayerData()
    {
        level = 1;
        currentExp = 0;
        leftAbilityPoint = 1;
        StrengthLevel = 0;
        VitLevel = 0;
        AVILevel = 0;
        SensationLevel = 0;
        IntLevel = 0;

        skillDatas = new Dictionary<string, SkillData>();
        equippedActiveSkills = new Dictionary<int, string>();
        unlockedActiveSkillSlots = 1;

        for (int i = 0; i < unlockedActiveSkillSlots; i++)
        {
            equippedActiveSkills.Add(i, string.Empty);
        }


        weapon = new EquipmentUpgradeStatus();
        topArmor = new EquipmentUpgradeStatus();
    }
    /// <summary>
    /// 레벨과 현재 경험치를 저장
    /// </summary>
    /// <param name="unitLevel"></param>
    public void SetLevel(UnitLevel unitLevel)
    {
        this.level = unitLevel.level;
        this.currentExp = unitLevel.CurrentExp;
        //Debug.Log($"Set level with level {this.level} with exp {this.currentExp}");

    }

    /// <summary>
    /// 현재 남은 스텟포인트와 각 스텟레벨들을 저장
    /// </summary>
    /// <param name="statContainer"></param>
    public void SetStat(StatContainer statContainer)
    {
        this.leftAbilityPoint = statContainer.AbilityPoint;
        this.StrengthLevel = statContainer.Strength.LevelAddedStats;
        this.VitLevel = statContainer.Vit.LevelAddedStats;
        this.AVILevel = statContainer.Avi.LevelAddedStats;
        this.SensationLevel = statContainer.Sensation.LevelAddedStats;
        this.IntLevel = statContainer.Int.LevelAddedStats;
    }

    public void SaveEquippedSkills(int index, ActiveSkill active)
    {
        if (active == null)
        {
            equippedActiveSkills[index] = string.Empty;
            return;
        }
        equippedActiveSkills.Add(index, active.skillName);
    }

    public void SaveSkills(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            SaveSkill(skills[i]);
        }
    }

    // 현재 장비해있는 스킬을 저장, 없으면 저장안함
    public void SaveEquippedActiveSkills(SkillInventory skillInventory)
    {
        // 현재 스킬창의 장착스킬들
        var equipped = skillInventory.equippedActiveSkills;
        Debug.Log("equipped skill? "+equipped == null);
        Debug.Log("equipped skill length? "+equipped.Length);

        for (int i = 0; i < equipped.Length; i++)
        {
            if (!equippedActiveSkills.ContainsKey(i)) 
                equippedActiveSkills.Add(i, string.Empty);

            // 스킬장착창이 비어있지 않으면 스킬이름을 저장
            if (!equipped[i].skillName.Equals(string.Empty)) 
                equippedActiveSkills[i] 
                    = equipped[i].skillName;
        }
        this.unlockedActiveSkillSlots = Mathf.Max(1, equipped.Length);
    }

    public void SaveSkill(Skill skill)
    {
        if (!skillDatas.ContainsKey(skill.skillName))
        {
            skillDatas.Add(skill.skillName, new SkillData(skill));
        }
        skillDatas[skill.skillName].SetSkillData(skill);
    }

    public List<SkillData> GetSkillDatas()
    {
        return skillDatas.Values.ToList();
    }
}

[System.Serializable]
public class SkillData
{
    public string skillName;
    public int skillLevel;
    public float currentExp;

    public SkillData(Skill skill)
    {
        this.skillName = skill.skillName;
        this.skillLevel = skill.skillLevel;
        this.currentExp = skill.Level.CurrentExp;
    }

    public void SetSkillData(Skill skill)
    {
        this.skillName = skill.skillName;
        this.skillLevel = skill.skillLevel;
        this.currentExp = skill.Level.CurrentExp;
    }
}

[System.Serializable]
public class EquipmentUpgradeStatus
{
    public string equipmentName;
    public int weaponLevel;
    public int weaponRank;
    public int totalUpgradeLevel;
    public EquipmentUpgradeStatus()
    {
        equipmentName = string.Empty;
        weaponLevel = 0;
        weaponRank = 0;
        totalUpgradeLevel = 0;
    }
    public void SetStatus(EquipmentTier eTier)
    {
        this.equipmentName = eTier.equipmentName;
        this.weaponLevel = eTier.currentLevel;
        this.weaponRank = eTier.currentTier;
        this.totalUpgradeLevel = eTier.totalUpgradeLevel;
    }
}

[CreateAssetMenu(fileName = "GameData", menuName = "Litkey/GameData")]
public class GameDatas : ScriptableObject
{
    public GameData dataSettings;

    private string fileName = "gdata.dat";
    private string keyName = "data";
    public UnityEvent OnGameDataLoaded = new();

    #region Save
    [Button("LocalSave")]
    public void SaveDataLocal()
    {
        var cache = new ES3Settings(ES3.Location.File);
        ES3.Save(keyName, dataSettings);
    }

    [Button("LocalLoad")]
    public void LoadDataLocal()
    {
        if (ES3.FileExists(fileName))
        {
            ES3.LoadInto(keyName, dataSettings);
        }
        else
        {
            // Initialize
            InitializeGameData();
            SaveDataLocal();
        }
        Debug.Log("게임 데이터 로드중");
        OnGameDataLoaded?.Invoke();
    }
        
    private void InitializeGameData()
    {
        dataSettings = new GameData();

    }
    public void SaveDataGPGS()
    {

    }

    private void OpenSaveGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                                            DataSource.ReadCacheOrNetwork,
                                                            ConflictResolutionStrategy.UseLastKnownGood,
                                                            OnSavedGameOpened);
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("저장 성공");

            var update = new SavedGameMetadataUpdate.Builder().Build();

            ////JSON
            //var json = JsonUtility.ToJson(dataSettings);
            //byte[] bytes = Encoding.UTF8.GetBytes(json);
            //Debug.Log("저장 데이터: " + bytes);

            // ES3
            var cache = new ES3Settings(ES3.Location.File);
            ES3.Save(keyName, dataSettings);
            byte[] bytes = ES3.LoadRawBytes(cache);


            savedGameClient.CommitUpdate(game, update, bytes, OnSavedGameWritten);
        }
        else
        {
            Debug.Log("저장 실패");
        }
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata data) 
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("저장 성공");
        } 
        else
        {
            Debug.Log("저장 실패");
        }
    }
    #endregion

    #region 불러오기

    public void LoadData()
    {
        OpenLoadGame();
    }

    private void OpenLoadGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                                            DataSource.ReadCacheOrNetwork,
                                                            ConflictResolutionStrategy.UseLastKnownGood,
                                                            LoadGameData);
    }

    private void LoadGameData(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("로드 성공");

            savedGameClient.ReadBinaryData(data, OnSavedGameDataRead);
        }
        else
        {
            Debug.Log("로드 실패");
        }
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        string data = System.Text.Encoding.UTF8.GetString(loadedData);

        if (data == "")
        {
            Debug.Log("데이터 없음. 초기 데이터 저장");
            SaveDataGPGS();
        }
        else
        {
            Debug.Log("로드 데이터 : " + data);

            //JSON
            //dataSettings = JsonUtility.FromJson<GameData>(data);

            // ES3
            var cache = new ES3Settings(ES3.Location.File);
            ES3.SaveRaw(loadedData, cache);
            ES3.LoadInto(keyName, dataSettings, cache);

            OnGameDataLoaded?.Invoke();
        }
    }

    #endregion

    public void DeleteData()
    {
        DeleteGameData();
    }

    private void DeleteGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                                            DataSource.ReadCacheOrNetwork,
                                                            ConflictResolutionStrategy.UseLastKnownGood,
                                                            DeleteSaveGame);
    }

    private void DeleteSaveGame(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            savedGameClient.Delete(data);


            // ES3
            ES3.DeleteFile();
            Debug.Log("삭제 성공");
        }
        else
        {
            Debug.Log("삭제 실패");
        }
    }

    public void DetectCheat()
    {
        Application.Quit();
    }
}
