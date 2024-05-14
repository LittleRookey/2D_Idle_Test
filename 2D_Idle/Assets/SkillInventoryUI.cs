using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SkillInventoryUI : MonoBehaviour
{
    [SerializeField] private SkillInventory skillInventory;

    [Header("UIs")]
    [SerializeField] private TextMeshProUGUI skillTitleText;
    [SerializeField] private TextMeshProUGUI skillRankText;
    [SerializeField] private TextMeshProUGUI skillExplanationText;
    [SerializeField] private Slider skillExpSlider;
    [SerializeField] private TextMeshProUGUI skillExpText;
    [Header("능력치상세정보")]
    [SerializeField] private TextMeshProUGUI skillTypeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
