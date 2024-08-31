using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using Litkey.Skill;
using Redcode.Pools;
using UnityEngine.Events;

public class SkillInformationWindowUI : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillTitleText;
    [SerializeField] private TextMeshProUGUI skillRankText;
    [SerializeField] private TextMeshProUGUI skillExplanationText;
    [SerializeField] private UnityEngine.UI.Slider skillExpSlider;
    [SerializeField] private TextMeshProUGUI skillExpText;
    [SerializeField] private Image skillRankBG;

    [Header("능력치상세정보")]
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI skillDamageText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("Dotween")]
    [SerializeField] private DOTweenAnimation openingAnimation;

    [SerializeField] private RarityColor skillRarity;

    [Header("스킬 등급 상세정보")]
    [SerializeField] private SkillDescriptionUI descriptionUIPrefab;
    [SerializeField] private RectTransform descriptionUIsParent;

    private Pool<SkillDescriptionUI> descriptionsPool;

    private Skill currentSkill;
    private List<SkillDescriptionUI> descriptions;

    public UnityAction OnCloseSkillWindow;
    private void Awake()
    {
        descriptions = new List<SkillDescriptionUI>();
        descriptionsPool = Pool.Create<SkillDescriptionUI>(descriptionUIPrefab);
        descriptionsPool.SetContainer(descriptionUIsParent);
    }

    public void OpenSkillWindow(Skill skill, bool useDotween=false)
    {
        if (currentSkill == null) currentSkill = skill;

        ClearDescriptions();

        // 전에 잇던 스킬 등록 해제
        currentSkill.Level.OnGainExp -= UpdateSkillExp;
        currentSkill.Level.OnLevelUp -= UpdateSkillLevel;
        currentSkill.Level.OnLevelUp -= UpdateSkillExp;
        currentSkill.Level.OnLevelUp -= UpdateSkillOnLevelUp;

        // 새 스킬 등록
        currentSkill = skill;


        var skillLevel = currentSkill.Level;
        skillLevel.OnGainExp += UpdateSkillExp;
        currentSkill.Level.OnLevelUp += UpdateSkillLevel;
        currentSkill.Level.OnLevelUp += UpdateSkillExp;
        currentSkill.Level.OnLevelUp += UpdateSkillOnLevelUp;

        gameObject.SetActive(true);

        UpdateSkillInfo(skill);

        if (useDotween)
        {
            openingAnimation.DORestart();
        }
    }

    private void UpdateSkillExp(float current, float max)
    {
        skillExpSlider.value = current / max;
        skillExpText.SetText($"{current} / {max}");
    }

    string levelText;
    private void UpdateSkillLevel(float current, float max)
    {
        levelText = string.Empty;
        if (currentSkill.skillLevel > 1) levelText += $"+{currentSkill.skillLevel - 1}";
        skillTitleText.SetText($"{currentSkill.skillName} {levelText}");
    }

    private void UpdateSkillOnLevelUp(float x, float y)
    {
        skillDamageText.SetText($"{currentSkill.GetTotalDamage()}%");
    }

    public void CloseSkillWindow()
    {
        ClearDescriptions();

        // 전에 잇던 스킬 등록 해제
        if (currentSkill != null)
        {
            currentSkill.Level.OnGainExp -= UpdateSkillExp;
            currentSkill.Level.OnLevelUp -= UpdateSkillLevel;
            currentSkill.Level.OnLevelUp -= UpdateSkillExp;
            currentSkill.Level.OnLevelUp -= UpdateSkillOnLevelUp;
        }

        OnCloseSkillWindow?.Invoke();
        gameObject.SetActive(false);
    }

    public void UpdateSkillInfo(Skill skill)
    {
        var skillLevel = skill.Level;

        skillRankBG.color = skillRarity.GetSkillColor(skill.currentRank);

        skillIcon.sprite = skill._icon;
        levelText = string.Empty;
        if (skill.skillLevel > 1) levelText += $"+{skill.skillLevel - 1}";

        skillTitleText.SetText($"{skill.skillName} {levelText}");
        skillRankText.SetText($"{TMProUtility.GetColorText(skill.currentRank.ToString(), skillRarity.GetSkillColor(skill.currentRank))}");
        skillExplanationText.SetText(skill.skillExplanation);

        skillExpSlider.value = skillLevel.CurrentExp / skillLevel.MaxExp;
        skillExpText.SetText($"{skillLevel.CurrentExp} / {skillLevel.MaxExp}");

        skillTypeText.SetText(skill is ActiveSkill ? "액티브" : "패시브");

        if (skill is ActiveSkill activeSkill)
        {
            skillDamageText.SetText($"{activeSkill.GetTotalDamage()}%");
            cooldownText.SetText($"{activeSkill.CooldownDuration}초");
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
        //else if (skill is PassiveSkill passiveSkill)
        //{
        //    skillDamageText.SetText($"{passiveSkill.finalDamage} %");
        //    cooldownText.SetText("상시적용");
        //    Debug.Log("startRank: " + passiveSkill.startRank);
        //    Debug.Log("maxRank: " + passiveSkill.GetMaxUpgradeRank());

        //    for (int i = (int)passiveSkill.startRank; i < (int)passiveSkill.GetMaxUpgradeRank(); i++)
        //    {
        //        var currentRank = (eSkillRank)i;
        //        if (!passiveSkill.LevelUpgrades.ContainsKey(currentRank)) continue;
        //        var descriptionUI = descriptionsPool.Get();
        //        descriptionUI.UpdateDescription(passiveSkill, currentRank);
        //        //Debug.Log("currentRank: " + currentRank);
        //        descriptionUI.gameObject.SetActive(true);
        //        descriptions.Add(descriptionUI);
        //    }

        //}
    }


    private void ClearDescriptions()
    {
        if (descriptions == null) return;
        for (int i = 0; i < descriptions.Count; i++)
        {
            descriptionsPool.Take(descriptions[i]);
        }
        descriptions.Clear();
    }
}
