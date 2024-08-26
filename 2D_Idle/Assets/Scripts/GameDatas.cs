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
using Litkey.Quest;


#region Datas to Save & Load
[System.Serializable]
public class GameData
{
    public int gold = 100;
    public PlayerData playerData;

    public SerializableInventory inventoryData;
    public SerializableRecipes recipeData;
    public QuestData questData;

    public GameData()
    {
        gold = 100;
        playerData = new PlayerData();
        inventoryData = new SerializableInventory();
        questData = new QuestData();
        recipeData = new SerializableRecipes();
    }
    public void SetGold(int gold) => this.gold = gold;

   
}

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public float currentExp = 0;

    // �ɷ�ġ ����
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
    /// ������ ���� ����ġ�� ����
    /// </summary>
    /// <param name="unitLevel"></param>
    public void SetLevel(UnitLevel unitLevel)
    {
        this.level = unitLevel.level;
        this.currentExp = unitLevel.CurrentExp;
        //Debug.Log($"Set level with level {this.level} with exp {this.currentExp}");

    }

    /// <summary>
    /// ���� ���� ��������Ʈ�� �� ���ݷ������� ����
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

    public void SaveSkills(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            SaveSkill(skills[i]);
        }
    }

    // ���� ������ִ� ��ų�� ����, ������ �������
    public void SaveEquippedActiveSkills(SkillInventory skillInventory)
    {
        // ���� ��ųâ�� ������ų��
        var equipped = skillInventory.equippedActiveSkills;

        int unlockedSlotCount = 0;
        foreach (var skillSlot in equipped)
        {
            if (skillSlot.IsLocked) continue;
            
            unlockedSlotCount++;
            
            if (skillSlot.IsEquipped)
            {
                equippedActiveSkills[skillSlot.Index] = skillSlot.EquippedSkill.skillName;
            } else
            {
                equippedActiveSkills[skillSlot.Index] = string.Empty;
            }
        }


        this.unlockedActiveSkillSlots = Mathf.Max(1, unlockedSlotCount);
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

[System.Serializable]
public class SerializableRecipes
{
    public Dictionary<int, RecipeStatus> gainedRecipes;

    public SerializableRecipes()
    {
        gainedRecipes = new Dictionary<int, RecipeStatus>();
    }

    public void SaveRecipes(List<RecipeUISlot> recipeSlots)
    {
        for (int i = 0; i < recipeSlots.Count; i++)
        {
            if (HasGainedRecipe(recipeSlots[i].Recipe)) 
            {
                GetRecipeStatus(recipeSlots[i].Recipe)
                    .SetStatus(recipeSlots[i].Recipe);
            } else
            {
                AddRecipe(recipeSlots[i].Recipe);
            }
        }
    }
    public RecipeStatus GetRecipeStatus(ItemRecipe recipe)
    {
        return gainedRecipes[recipe.intID];
    }
    public RecipeStatus GetRecipeStatus(int intID)
    {
        return gainedRecipes[intID];
    }
    public void AddRecipe(ItemRecipe recipe)
    {
        if (gainedRecipes == null) gainedRecipes = new Dictionary<int, RecipeStatus>();
        if (gainedRecipes.ContainsKey(recipe.intID)) return;
        gainedRecipes.Add(recipe.intID, new RecipeStatus(recipe));
    }

    public bool HasGainedRecipe(ItemRecipe recipe)
    {
        if (gainedRecipes == null) gainedRecipes = new Dictionary<int, RecipeStatus>();
        return gainedRecipes.ContainsKey(recipe.intID);
    }
    public bool HasGainedRecipe(int intID)
    {
        if (gainedRecipes == null) gainedRecipes = new Dictionary<int, RecipeStatus>();
        return gainedRecipes.ContainsKey(intID);
    }
}

[System.Serializable]
public class RecipeStatus
{
    public int RecipeID;
    public int madeCount; // ����̳� ���������
    public bool IsUnlocked;
    public RecipeStatus(ItemRecipe recipe)
    {
        this.RecipeID = recipe.intID;
        this.IsUnlocked = !recipe.isLocked;
        this.madeCount = 0;
    }

    public void SetStatus(ItemRecipe recipe)
    {
        if (this.RecipeID != recipe.intID)
        {
            Debug.LogError($"Recipe {recipe.intID} doesn't have same id as {RecipeID}");
            return;
        }
        IsUnlocked = !recipe.isLocked;
    }
    public void AddMadeCount()
    {
        madeCount++;
    }
}

#endregion

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
        Debug.Log("���� ������ �ε���");
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
            Debug.Log("���� ����");

            var update = new SavedGameMetadataUpdate.Builder().Build();

            ////JSON
            //var json = JsonUtility.ToJson(dataSettings);
            //byte[] bytes = Encoding.UTF8.GetBytes(json);
            //Debug.Log("���� ������: " + bytes);

            // ES3
            var cache = new ES3Settings(ES3.Location.File);
            ES3.Save(keyName, dataSettings);
            byte[] bytes = ES3.LoadRawBytes(cache);


            savedGameClient.CommitUpdate(game, update, bytes, OnSavedGameWritten);
        }
        else
        {
            Debug.Log("���� ����");
        }
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata data) 
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("���� ����");
        } 
        else
        {
            Debug.Log("���� ����");
        }
    }
    #endregion

    #region �ҷ�����

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
            Debug.Log("�ε� ����");

            savedGameClient.ReadBinaryData(data, OnSavedGameDataRead);
        }
        else
        {
            Debug.Log("�ε� ����");
        }
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        string data = System.Text.Encoding.UTF8.GetString(loadedData);

        if (data == "")
        {
            Debug.Log("������ ����. �ʱ� ������ ����");
            SaveDataGPGS();
        }
        else
        {
            Debug.Log("�ε� ������ : " + data);

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
            Debug.Log("���� ����");
        }
        else
        {
            Debug.Log("���� ����");
        }
    }

    public void DetectCheat()
    {
        Application.Quit();
    }
}
