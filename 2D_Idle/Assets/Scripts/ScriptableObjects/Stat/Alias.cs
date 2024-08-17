using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;

namespace Litkey.Stat
{
    [CreateAssetMenu(fileName = "Alias", menuName = "Litkey/Alias")]
    public class Alias : ScriptableObject
    {
        public string aliasName;
        public int level;
        public List<StatModifier> extraStats;

        
    }

}