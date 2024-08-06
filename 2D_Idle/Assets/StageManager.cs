using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using Redcode.Pools;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [InlineEditor]
    [SerializeField] private Stage _currentStage;

    private int currentStageIndex = 0;

    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private Transform[] resourcesPositions;

    private Dictionary<eResourceType, Pool<Interactor>> resources;

    private void Awake()
    {
        Instance = this;
    }

    public int resourceNumber;

    // ¾À ³Ñ¾î°¡¼­ ¹èÆ²¾À¿¡¼­ ¸Ê ¼¼ÆÃ 
    public void SetupStage(Stage stageInfo)
    {
        this._currentStage = stageInfo;

        bool hasBoss = stageInfo.Boss != null;
        int resourceNumber = Random.Range(1, resourcesPositions.Length);
        //stageInfo.appearingResourceType;
        
        
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.SetSpawnPoint(stageInfo.Monster);
        }
        // TODO load resource informations
        foreach (var resource in resourcesPositions)
        {
            //resource.Initialize();
        }
        
        // TODO set quest 
    }

    private Vector2 PickRandomPositionWithin(Transform spawnPos)
    {
        return (Vector2)spawnPos.position + Random.insideUnitCircle;
    }

    public float GetCurrentDifficulty()
    {
        return _currentStage.difficultyRate;
    }

    public void ShowStageEnterUI()
    {

    }
}
