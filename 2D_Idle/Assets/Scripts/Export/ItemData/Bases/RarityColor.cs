using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Skill
{
    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/SkillRarity")]
    public class RarityColor : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<eSkillRank, Color> rankColor;

        public Color GetSkillColor(eSkillRank rank)
        {
            return rankColor[rank];
        }
    }
}
