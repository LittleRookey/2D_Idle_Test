using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResourceLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;


    private void Start()
    {
        ResourceManager.OnGainGold.AddListener(UpdateGold);
    }
    private void OnEnable()
    {
        UpdateGold(-1);
    }
   
    public void UpdateGold(int extraGold)
    {
        if (extraGold > 0) Debug.Log(1);
        goldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }
}
