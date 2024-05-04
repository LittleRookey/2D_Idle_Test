using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;
using UnityEngine.UI;
using TMPro;

public class MonsterNameLevelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLevelText;

    private StatContainer _statContainer;
    Health health;
    private EnemyAI enemyAI;
    [SerializeField] private Color weakColor = Color.green;
    [SerializeField] private Color similarColor = Color.white;
    [SerializeField] private Color StrongColor = Color.red;
    private void Awake()
    {
        _statContainer = transform.parent.GetComponent<StatContainer>();
        health = transform.parent.GetComponent<Health>();
        enemyAI = transform.parent.GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        health.OnReturnFromPool += SetMonsterNameLevel;
    }

    private void OnDisable()
    {
        health.OnReturnFromPool -= SetMonsterNameLevel;
    }
    public void SetMonsterNameLevel()
    {
        var playerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelSystem>().GetLevel();
        // 강함 나타내기
        Color tagColor;
        if (_statContainer.MonsterLevel > playerLevel) tagColor = StrongColor;
        else if (_statContainer.MonsterLevel == playerLevel) tagColor = similarColor;
        else tagColor = weakColor;
        
        nameLevelText.SetText(TMProUtility.GetColorText($"Lv.{_statContainer.MonsterLevel} {health.Name}", tagColor));
    }
}
