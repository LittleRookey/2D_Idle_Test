using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Stat
{

[System.Serializable]
    public struct StatData
    {
        public eSubStatType statType;
        public Color color;
        public Sprite statIcon;
    }

    [CreateAssetMenu(fileName = "StatColor", menuName = "Litkey/StatColor")]
    public class StatColor : ScriptableObject
    {

        public List<StatData> statDatabase = new List<StatData>();


        [Button("InitStatColors")]
        public void InitStatColor()
        {
            statDatabase.Clear();
            statDatabase.Add(new StatData { statType = eSubStatType.체력, color = new Color32(255, 0, 0, 255) }); // Red
            statDatabase.Add(new StatData { statType = eSubStatType.물리공격력, color = new Color32(255, 128, 0, 255) }); // Orange
            statDatabase.Add(new StatData { statType = eSubStatType.마법공격력, color = new Color32(0, 128, 255, 255) }); // Blue
            statDatabase.Add(new StatData { statType = eSubStatType.물리방어력, color = new Color32(0, 255, 0, 255) }); // Green
            statDatabase.Add(new StatData { statType = eSubStatType.마법방어력, color = new Color32(128, 0, 255, 255) }); // Purple
            statDatabase.Add(new StatData { statType = eSubStatType.공격속도, color = new Color32(255, 255, 0, 255) }); // Yellow
            statDatabase.Add(new StatData { statType = eSubStatType.이동속도, color = new Color32(255, 128, 255, 255) }); // Pink
            statDatabase.Add(new StatData { statType = eSubStatType.크리확률, color = new Color32(255, 255, 255, 255) }); // White
            statDatabase.Add(new StatData { statType = eSubStatType.크리데미지, color = new Color32(128, 128, 128, 255) }); // Gray
            statDatabase.Add(new StatData { statType = eSubStatType.추가골드, color = new Color32(255, 215, 0, 255) }); // Gold
            statDatabase.Add(new StatData { statType = eSubStatType.추가경험치, color = new Color32(0, 255, 255, 255) }); // Cyan
            statDatabase.Add(new StatData { statType = eSubStatType.명중, color = new Color32(128, 255, 0, 255) }); // Lime
            statDatabase.Add(new StatData { statType = eSubStatType.회피, color = new Color32(255, 128, 128, 255) }); // Salmon
            statDatabase.Add(new StatData { statType = eSubStatType.물리저항, color = new Color32(128, 128, 0, 255) }); // Olive
            statDatabase.Add(new StatData { statType = eSubStatType.마법저항, color = new Color32(0, 128, 128, 255) }); // Teal
            statDatabase.Add(new StatData { statType = eSubStatType.물리관통력, color = new Color32(255, 0, 255, 255) }); // Magenta
            statDatabase.Add(new StatData { statType = eSubStatType.마법관통력, color = new Color32(0, 255, 128, 255) }); // Emerald
        }
        public Color GetColor(eSubStatType statType)
        {
            for (int i = 0; i < statDatabase.Count; i++)
            {
                if (statDatabase[i].statType == statType) return statDatabase[i].color;
            }
            return Color.white;
        }

        public Sprite GetStatIcon(eSubStatType statType)
        {
            for (int i = 0; i < statDatabase.Count; i++)
            {
                if (statDatabase[i].statType == statType) return statDatabase[i].statIcon;
            }
            return null;
        }
    }

}