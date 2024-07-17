using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;


public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [InlineEditor]
    [SerializeField] private Stage _currentStage;

    private int currentStageIndex = 0;

    [SerializeField] private SpawnPoint[]  spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupStage()
    {
        //spawnPoints[0].
    }

    public float GetCurrentDifficulty()
    {
        return _currentStage.difficultyRate;
    }

    public void ShowStageEnterUI()
    {

    }
}
