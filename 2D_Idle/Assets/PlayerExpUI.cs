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
    public void UpdateExp(float current, float max)
    {
        float expRate = current / max;

        expBar.fillAmount = expRate;
        DOTween.To(() => expBar.fillAmount, x => expBar.fillAmount = x, expRate, 0.2f).OnUpdate(() =>
        {
            expText.SetText($"{expBar.fillAmount.ToString("F2")}%");
        });
    }

    public void ResetExp()
    {
        expBar.fillAmount = 0f;
        expText.SetText($"{expBar.fillAmount.ToString("F2")}%");
    }

    private void Start()
    {
        UpdateExp(levelSystem.GetCurrentExp(), levelSystem.GetMaxExp());
    }
    private void OnEnable()
    {
        levelSystem.unitLevel.OnGainExp += UpdateExp;
        levelSystem.unitLevel.OnLevelUp += ResetExp;
    }

    private void OnDisable()
    {
        levelSystem.unitLevel.OnGainExp -= UpdateExp;
        levelSystem.unitLevel.OnLevelUp -= ResetExp;
    }
}
