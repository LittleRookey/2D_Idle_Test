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

   
    public void UpdateExtraGold(int extraGold)
    {
        if (extraGold < 0) return;
        int prevGold = ResourceManager.Instance.Gold - extraGold;
        DOTween.To(() => prevGold, x => {
            prevGold = x;
            goldText.SetText(Mathf.Round(x).ToString("N0"));
        }, prevGold + extraGold, 0.5f).SetEase(Ease.OutQuad);
    }

    public void UpdateGold()
    {
        goldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }
}
