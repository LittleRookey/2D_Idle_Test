using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private LevelSystem levelSystem;


    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;
    public void UpdateExp(float current, float max)
    {
        float expRate = current / max;

        expBar.fillAmount = expRate;
        //DOTween.To(() => expBar.fillAmount, x => div = x, expRate, 0.2f);
        expText.SetText($"{(expRate * 100f).ToString("F2")}%");
    }



    private void UpdateLevelText(float current, float max)
    {
        levelText.SetText($"Lv {levelSystem.unitLevel.level}");
    }

    private void Start()
    {
        UpdateExp(levelSystem.GetCurrentExp(), levelSystem.GetMaxExp());
        UpdateLevelText(levelSystem.GetCurrentExp(), levelSystem.GetMaxExp());
    }
    private void OnEnable()
    {
        levelSystem.unitLevel.OnGainExp += UpdateExp;
        levelSystem.unitLevel.OnLevelUp += UpdateExp;
        levelSystem.unitLevel.OnLevelUp += UpdateLevelText;
    }

    private void OnDisable()
    {
        levelSystem.unitLevel.OnGainExp -= UpdateExp;
        levelSystem.unitLevel.OnLevelUp -= UpdateExp;
        levelSystem.unitLevel.OnLevelUp -= UpdateLevelText;
    }
}
