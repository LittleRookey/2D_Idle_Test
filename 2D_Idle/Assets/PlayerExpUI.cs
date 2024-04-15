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

        float div = expBar.fillAmount;
        expBar.fillAmount = expRate;
        DOTween.To(() => expBar.fillAmount, x => div = x, expRate, 0.2f);
        expText.SetText($"{(100f * div).ToString("F2")}%");
    }

    public void ResetExp()
    {
        expText.SetText($"{0f.ToString("F2")}%");
        DOTween.To(() => expBar.fillAmount, x => expBar.fillAmount = x, 0f, 0.2f);
        //expBar.fillAmount = 0f;
        //expText.SetText($"{expBar.fillAmount.ToString("F2")}%");
    }

    private void UpdateLevelText()
    {
        levelText.SetText($"Lv {levelSystem.unitLevel.level}");
    }

    private void Start()
    {
        UpdateExp(levelSystem.GetCurrentExp(), levelSystem.GetMaxExp());
        UpdateLevelText();
    }
    private void OnEnable()
    {
        levelSystem.unitLevel.OnGainExp += UpdateExp;
        levelSystem.unitLevel.OnLevelUp += ResetExp;
        levelSystem.unitLevel.OnLevelUp += UpdateLevelText;
    }

    private void OnDisable()
    {
        levelSystem.unitLevel.OnGainExp -= UpdateExp;
        levelSystem.unitLevel.OnLevelUp -= ResetExp;
        levelSystem.unitLevel.OnLevelUp -= UpdateLevelText;
    }
}
