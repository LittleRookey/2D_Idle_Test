using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Litkey.Stat
{
    public class Debuff : ScriptableObject, IBuff
    {
        [SerializeField] private int buffID;
        public int BuffID { get => buffID; }

        [HorizontalGroup("Buff", 100f), PreviewField(100f)]
        public Sprite BuffIcon;
        [HorizontalGroup("Buff"), VerticalGroup("Buff/Right")]
        public string BuffName;
        [TextArea, VerticalGroup("Buff/Right")]
        public string Explanation;

        [VerticalGroup("Buff/Right")]
        public bool IsDebuff; // 디버프?
        [VerticalGroup("Buff/Right")]
        public bool Stackable; // 버프가 쌓이는지
        [ShowIf("Stackable"), VerticalGroup("Buff/Right")] public int MaxStackableAmount; // 버프가 몇까지 쌓이는지
        [VerticalGroup("Buff/Right")] public bool PermanentDuration; // 영구 버프?
        [HideIf("PermanentDuration", true), VerticalGroup("Buff/Right")] public float Duration; // 버프가 몇초동안 쌓이는지,
        public List<StatModifier> BuffStats;

    }
}