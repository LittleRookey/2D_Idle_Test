using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourceLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI plusGoldText;

    private void Awake()
    {
        plusGoldText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResourceManager.OnGainGold.AddListener(UpdateExtraGold);
        ResourceManager.OnUseGold.AddListener(UpdateExtraGold);
        ResourceManager.Instance.OnResourceLoaded.AddListener(UpdateGold);
        
    }

    private void Start()
    {
        UpdateExtraGold(0);
    }

    string plus = "+";
    public void UpdateExtraGold(int usedGold)
    {
        // ���� �̹� ���� 100 -25 75, �̹� ������ 100, 25, 125
        int currentGold = ResourceManager.Instance.Gold;
        Debug.Log($"Current Gold: {currentGold} ////// ResultGold: {currentGold + usedGold}");
        plusGoldText.gameObject.SetActive(true);
        plusGoldText.SetText(plus + usedGold.ToString("N0"));
        plusGoldText.transform.DOScale(1.1f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(()=>plusGoldText.gameObject.SetActive(false));

        DOTween.To(() => currentGold, x => {
            currentGold = x;
            goldText.SetText(Mathf.Round(x).ToString("N0"));
        }, currentGold + usedGold, 0.5f).SetEase(Ease.OutQuad);
    }

    public void UpdateGold()
    {
        goldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }
}
