using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;



    [SerializeField] private GameDatas gameData;
    private InGameEvent currentEvent;

    private void Awake()
    {
        
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        //gameData.LoadDataLocal();
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene m_scene, UnityEngine.SceneManagement.LoadSceneMode m_sceneMode)
    {
        //if (m_scene.name == "Main")
        //{
        //    MasterAudio.StartPlaylist("BattleScene");
        //} 
        //else if (m_scene.name == "Town")
        //{
        //    MasterAudio.StartPlaylist("Town");
        //} 
        //else if (m_scene.name == "Lobby")
        //{
        //    MasterAudio.StartPlaylist("Lobby");
        //}
        //gameData.LoadDataLocal();
    }

    private void OnEnable()
    {
        gameData.LoadDataLocal();
    }

    private void Start()
    {
        //gameData.LoadDataLocal();
    }
    private void StartEvent()
    {
        //currentEvent = questManager.GetEvent();
        //mapManager.SetDestination(currentEvent.eventPosition);
    }

    
}
