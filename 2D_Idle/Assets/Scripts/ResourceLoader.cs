using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResourceLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable()
    {
        ResourceManager.OnGainGold.AddListener(UpdateExtraGold);
        ResourceManager.Instance.OnResourceLoaded.AddListener(UpdateGold);
        
    }

    private void Start()
    {
        UpdateExtraGold(0);
    }

   
    public void UpdateExtraGold(int extraGold)
    {
        if (extraGold < 0) return;
        goldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }

    public void UpdateGold()
    {
        goldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }
}
