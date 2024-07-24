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

    private StringBuilder optionsBuilder = new StringBuilder();

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
