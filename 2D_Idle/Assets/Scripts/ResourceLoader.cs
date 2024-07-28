using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourceLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

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

   
    public void UpdateExtraGold(int usedGold)
    {
        // °ñµå´Â ÀÌ¹Ì »ç¿ëµÊ 100 -25 75, ÀÌ¹Ì ÇÕÃÄÁü 100, 25, 125
        int currentGold = ResourceManager.Instance.Gold;
        Debug.Log($"Current Gold: {currentGold} ////// ResultGold: {currentGold + usedGold}");
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
