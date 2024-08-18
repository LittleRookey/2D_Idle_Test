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
        public bool IsDebuff; // �����?
        [VerticalGroup("Buff/Right")]
        public bool Stackable; // ������ ���̴���
        [ShowIf("Stackable"), VerticalGroup("Buff/Right")] public int MaxStackableAmount; // ������ ����� ���̴���
        [VerticalGroup("Buff/Right")] public bool PermanentDuration; // ���� ����?
        [HideIf("PermanentDuration", true), VerticalGroup("Buff/Right")] public float Duration; // ������ ���ʵ��� ���̴���,
        public List<StatModifier> BuffStats;

    }
}