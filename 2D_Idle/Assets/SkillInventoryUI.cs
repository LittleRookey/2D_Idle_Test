using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.Utility;
using Redcode.Pools;
using DG.Tweening;


public class SkillInventoryUI : MonoBehaviour
{
    [SerializeField] private SkillInventory skillInventory;

    [Header("UIs")]
    [SerializeField] private RectTransform windowTransform;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillTitleText;
    [SerializeField] private TextMeshProUGUI skillRankText;
    [SerializeField] private TextMeshProUGUI skillExplanationText;
    [SerializeField] private UnityEngine.UI.Slider skillExpSlider;
    [SerializeField] private TextMeshProUGUI skillExpText;
    [SerializeField] private Button leftActiveSkillButton;
    [SerializeField] private Button rightPassiveSkillButton;
    
    // ��Ʈ��
    [SerializeField] private DOTweenAnimation leftBtnAnim;
    [SerializeField] private DOTweenAnimation rightBtnAnim;

    [Header("�ɷ�ġ������")]
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI skillDamageText;
    [SerializeField] private TextMeshProUGUI cooldownText; 

    [SerializeField] private RarityColor skillRarity;

    [Header("��ų ��� ������")]
    [SerializeField] private SkillDescriptionUI descriptionUIPrefab;
    [SerializeField] private RectTransform descriptionUIsParent;
    [SerializeField] private RectTransform skillSlotUIParent;
    
    [SerializeField] private SkillSlotUI skillSlotUIPrefab;


    private SkillDescriptionUI currentDescription;
    
    private Pool<SkillDescriptionUI> descriptionsPool;

    private List<SkillSlotUI> skillSlots;

    private Pool<SkillSlotUI> skillSlotPool;
    private void Awake()
    {
        skillSlots = new List<SkillSlotUI>();
        descriptions = new List<SkillDescriptionUI>();

        descriptionsPool = Pool.Create<SkillDescriptionUI>(descriptionUIPrefab);
        descriptionsPool.SetContainer(descriptionUIsParent);
        skillSlotPool = Pool.Create<SkillSlotUI>(skillSlotUIPrefab);
        skillSlotPool.SetContainer(skillSlotUIParent);
    }

    private void OnEnable()
    {
        skillInventory.OnAddSkill.AddListener(AddSlot);
    }

    private void OnDisable()
    {
        skillInventory.OnAddSkill.AddListener(AddSlot);
    }
    private void Start()
    {
        CloseInventory();
    }

    public void OpenInventory()
    {
        windowTransform.gameObject.SetActive(true);
        ActiveMode();
        ShowSkillInfo(skillInventory.GetSkill("����˹�"));
    }

    public void CloseInventory()
    {
        RemoveAllSkillSlots();
        passiveMode = true;
        windowTransform.gameObject.SetActive(false);

        if (currentSkill != null)
            currentSkill.Level.OnGainExp -= UpdateSkillExp;

        currentSkill = null;
    }

    public void AddSlot(Skill skill)
    {
        var slot = skillSlotPool.Get(); 
        
        skillSlots.Add(slot);
        slot.SetSlot(skill);
        //slot.OnClickSlot.AddListener((Skill mSkill) => ShowSkillInfo(mSkill));
        slot.AddListener(ShowSkillInfo);
    }

    private void RemoveAllSkillSlots()
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].ClearSlot();
            skillSlotPool.Take(skillSlots[i]);
        }
        skillSlots.Clear();
    }
    bool passiveMode;
    public void PassiveMode()
    {
        if (passiveMode) return;
        passiveMode = true;
        var passives = skillInventory.GetPassives();

        RemoveAllSkillSlots();

        for(int i = 0; i < passives.Count; i++)
        {
            AddSlot(passives[i]);
        }

        rightBtnAnim.DORestartById("Right");
    }

    public void ActiveMode()
    {
        if (!passiveMode) return;
        passiveMode = false;
        var actives = skillInventory.GetActives();

        RemoveAllSkillSlots();

        for (int i = 0; i < actives.Count; i++)
        {
            AddSlot(actives[i]);
        }
        leftBtnAnim.DORestartById("Left");
        
    }
    
    private Skill currentSkill;

    private void UpdateSkillExp(float current, float max)
    {
        skillExpSlider.value = current / max;
        skillExpText.SetText($"{current} / {max}");
    }

    private void UpdateSkillLevel(float current, float max)
    {
        skillTitleText.SetText($"{currentSkill.skillName} +{currentSkill.skillLevel}");
    }
    private List<SkillDescriptionUI> descriptions;
    public void ShowSkillInfo(Skill skill)
    {
        if (currentSkill == null) currentSkill = skill;

        ClearDescriptions();

        // ���� �մ� ��ų ��� ����
        currentSkill.Level.OnGainExp -= UpdateSkillExp;
        currentSkill.Level.OnLevelUp -= UpdateSkillLevel;
        // �� ��ų ���
        currentSkill = skill;


        var skillLevel = currentSkill.Level;
        skillLevel.OnGainExp += UpdateSkillExp;
        currentSkill.Level.OnLevelUp += UpdateSkillLevel;

        skillIcon.sprite = skill._icon;

        skillTitleText.SetText($"{skill.skillName} +{skill.skillLevel}");
        skillRankText.SetText($"{TMProUtility.GetColorText(skill.currentRank.ToString(), skillRarity.GetSkillColor(skill.currentRank))}");
        skillExplanationText.SetText(skill.skillExplanation);

        skillExpSlider.value = skillLevel.CurrentExp / skillLevel.MaxExp;
        skillExpText.SetText($"{skillLevel.CurrentExp} / {skillLevel.MaxExp}");

        skillTypeText.SetText(skill is ActiveSkill ? "��Ƽ��" : "�нú�");

        if (skill is ActiveSkill activeSkill)
        {
            skillDamageText.SetText($"{activeSkill.GetTotalDamage(activeSkill.currentRank)}%");
            cooldownText.SetText($"{activeSkill.cooldown}��");
            for (int i = (int)activeSkill.startRank; i < (int)activeSkill.GetMaxUpgradeRank(); i++)
            {
                var currentRank = (eSkillRank)i;
                if (!activeSkill.rankUpgrades.ContainsKey(currentRank)) continue;
                var descriptionUI = descriptionsPool.Get();
                descriptionUI.UpdateDescription(activeSkill, currentRank);
                Debug.Log("currentRank: " + currentRank);
                descriptionUI.gameObject.SetActive(true);
                descriptions.Add(descriptionUI);
            }
        }
        else if (skill is PassiveSkill passiveSkill)
        {
            skillDamageText.SetText($"{passiveSkill.finalDamage} %");
            cooldownText.SetText("�������");
            Debug.Log("startRank: " + passiveSkill.startRank);
            Debug.Log("maxRank: " + passiveSkill.GetMaxUpgradeRank());

            for (int i = (int)passiveSkill.startRank; i < (int)passiveSkill.GetMaxUpgradeRank(); i++)
            {
                var currentRank = (eSkillRank)i;
                if (!passiveSkill.LevelUpgrades.ContainsKey(currentRank)) continue;
                var descriptionUI = descriptionsPool.Get();
                descriptionUI.UpdateDescription(passiveSkill, currentRank);
                Debug.Log("currentRank: " + currentRank);
                descriptionUI.gameObject.SetActive(true);
                descriptions.Add(descriptionUI);
            }

        }

        // ��ų description ��ũ������ ���̱�
    }

    
    private void ClearDescriptions()
    {
        for (int i = 0; i < descriptions.Count; i++)
        {
            descriptionsPool.Take(descriptions[i]);
        }
        descriptions.Clear();
    }
}
