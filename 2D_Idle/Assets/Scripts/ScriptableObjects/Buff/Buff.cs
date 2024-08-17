using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Litkey.Stat
{
    public interface IBuff
    {
        public int BuffID { get;}
    }
    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/Buff/Buff")]
    public class Buff : ScriptableObject, IBuff
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
    

    public class BuffInfo
    {
        public Buff buff;
        public int stackCount;
        public BuffInfo(Buff buff, int initialStack)
        {
            this.buff = buff;
            this.stackCount = initialStack;
        }

        // returns actually applied stack based on maxStack
        // +3 but 8 -> 10, returns 2
        public int IncreaseStack(int stack)
        {
            if (!buff.Stackable)
            {
                Debug.LogError($"Buff {buff.BuffName} is not stackable");
                return 0;
            }
            int origin = stackCount;
            stackCount += stack;

            stackCount = Mathf.Clamp(stackCount, 0, buff.MaxStackableAmount);

            return stackCount - origin;
        }

        // if stackcount is less than or equal to 0 return true, else false
        public bool DecreaseStack(int stack)
        {
            if (!buff.Stackable)
            {
                Debug.LogError($"Buff {buff.BuffName} is not stackable");
                return true;
            }
            int origin = stackCount;
            stackCount -= stack;
            stackCount = Mathf.Clamp(stackCount, 0, buff.MaxStackableAmount);

            return stackCount <= 0;
        }
    }
}
