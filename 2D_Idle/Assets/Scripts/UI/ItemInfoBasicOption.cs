using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;
using System.Text;

public enum eItemOptionType
{
    기본,
    추가,
    제련,
}
public class ItemInfoBasicOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionTitleText;
    [SerializeField] private TextMeshProUGUI optionText;

    private const string option = " 옵션";
    private const string plus = "+";
    private const string minus = "-";
    private const string arrow = "→ ";
    private const string durability = "내구도 ";

    private const string whitespace = " ";
    private const string openParenth = "(";
    private const string closeParenth = ")";

    private StringBuilder optionsBuilder = new StringBuilder();


    public void SetOption(eItemOptionType optionType, Dictionary<eSubStatType, float> stats)
    {

        optionTitleText.SetText(optionType.ToString() + option);

        optionsBuilder.Clear();
        string options = string.Empty;
        int i = 0;
        foreach (var kVal in stats)
        {
            if (i > 0)
            {
                optionsBuilder.AppendLine();
            }
            i++;
            optionsBuilder.Append(arrow)
                          .Append(kVal.Key.ToString())
                          .Append(whitespace)
                          .Append(kVal.Value > 0 ? plus : minus)
                          .Append(Mathf.Abs(kVal.Value));
        }

        optionText.SetText(optionsBuilder);
    }

    public void SetOption(eItemOptionType optionType, StatModifier[] baseStats, Dictionary<eSubStatType, float> upgradedStats)
    {

        optionTitleText.SetText(optionType.ToString() + option);

        optionsBuilder.Clear();
        string options = string.Empty;
        for (int i = 0; i < baseStats.Length; i++)
        {
            if (i > 0)
            {
                optionsBuilder.AppendLine();
            }
            optionsBuilder.Append(arrow)
                          .Append(baseStats[i].statType.ToString())
                          .Append(' ')
                          .Append(baseStats[i].value > 0 ? plus : minus)
                          .Append(Mathf.Abs(baseStats[i].value));
            Debug.Log($"Item contains upgrade Stat {upgradedStats.ContainsKey(baseStats[i].statType)} of type {baseStats[i].statType}");
            if (upgradedStats.ContainsKey(baseStats[i].statType))
            {
                optionsBuilder.Append(whitespace)
                              .Append(TMProUtility.GetColorText(openParenth, Color.green))
                              .Append(TMProUtility.GetColorText(plus, Color.green))
                              .Append(TMProUtility.GetColorText(upgradedStats[baseStats[i].statType].ToString(), Color.green))
                              .Append(TMProUtility.GetColorText(closeParenth, Color.green));
            }
                          
        }

        optionText.SetText(optionsBuilder);
    }

    public void SetOption(eItemOptionType optionType, StatModifier[] stats)
    {
        
        optionTitleText.SetText(optionType.ToString() + option);

        optionsBuilder.Clear();
        string options = string.Empty;
        for (int i = 0; i < stats.Length; i++)
        {
            if (i > 0)
            {
                optionsBuilder.AppendLine();
            }
            optionsBuilder.Append(arrow)
                          .Append(stats[i].statType.ToString())
                          .Append(' ')
                          .Append(stats[i].value > 0 ? plus : minus)
                          .Append(Mathf.Abs(stats[i].value));
        }

        optionText.SetText(optionsBuilder);
    }

    public void SetOption(eItemOptionType optionType, List<StatModifier> stats)
    {

        optionTitleText.SetText(optionType.ToString() + option);

        optionsBuilder.Clear();
        string options = string.Empty;
        for (int i = 0; i < stats.Count; i++)
        {
            if (i > 0)
            {
                optionsBuilder.AppendLine();
            }
            optionsBuilder.Append(arrow)
                          .Append(stats[i].statType.ToString())
                          .Append(' ')
                          .Append(stats[i].value > 0 ? plus : minus)
                          .Append(Mathf.Abs(stats[i].value));
        }

        optionText.SetText(optionsBuilder);
    }

    public void SetOption(eItemOptionType optionType, int MaxDurability)
    {

        optionTitleText.SetText(optionType.ToString() + option);

        optionsBuilder.Clear();
        string options = string.Empty;
        
        optionsBuilder.Append(arrow)
                        .Append(durability)
                        .Append(plus)
                        .Append(MaxDurability);
        

        optionText.SetText(optionsBuilder);
    }
}
