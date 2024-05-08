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
        
    }

    private void OnEnable()
    {
        gameData.LoadDataLocal();
    }
    private void StartEvent()
    {
        //currentEvent = questManager.GetEvent();
        //mapManager.SetDestination(currentEvent.eventPosition);
    }

    
}
