using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameData
{
    public ObscuredInt gold;
    public PlayerData playerData;
}

[System.Serializable]
public class PlayerData
{
    public ObscuredInt level;
    public ObscuredFloat currentExp;

    // �ɷ�ġ ����
    public ObscuredInt leftAbilityPoint;
    public ObscuredInt StrengthLevel;
    public ObscuredInt VitLevel;
    public ObscuredInt AVILevel;
    public ObscuredInt SensationLevel;
    public ObscuredInt IntLevel;


}

public class GameDatas : MonoBehaviour
{
    public GameData dataSettings = new GameData();

    private string fileName = "gdata.dat";

    private string keyName = "data";

    #region Save
    public void SaveData()
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
            SaveData();
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
