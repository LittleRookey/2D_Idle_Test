using Litkey.Stat;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Weather
{
    public enum eWeatherType
    {
        �Ȱ�,
        ��,
        ��,
        ��,
        ���,
        �帲


    }
    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/Weather")]
    public class Weather : ScriptableObject
    {
        public ParticleSystem weatherFX;
        public eWeatherType weatherType;

        public StatModifier[] WeatherStatEffect;
        public int extraEnemyCount;
    }
}
