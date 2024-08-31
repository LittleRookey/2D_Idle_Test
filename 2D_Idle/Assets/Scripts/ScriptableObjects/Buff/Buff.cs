using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Litkey.Stat
{
    public interface IBuff
    {
        int BuffID { get; }
        string BuffName { get; }
        bool IsDebuff { get; }
        bool Stackable { get; }
        int MaxStackableAmount { get; }
        bool PermanentDuration { get; }
        float Duration { get; }

    }
    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/Buff/Buff")]
    public class Buff : ScriptableObject, IBuff
    {
        [SerializeField] private int buffID;
        public int BuffID => buffID;

        [PreviewField(100f)]
        public Sprite BuffIcon;

        [SerializeField] private string buffName;
        public string BuffName => buffName;

        [TextArea]
        public string Explanation;

        [SerializeField] private bool isDebuff;
        public bool IsDebuff => isDebuff;

        [SerializeField] private bool stackable;
        public bool Stackable => stackable;

        [ShowIf("stackable"), SerializeField]
        private int maxStackableAmount;
        public int MaxStackableAmount => maxStackableAmount;

        [SerializeField] private bool permanentDuration;
        public bool PermanentDuration => permanentDuration;

        [HideIf("permanentDuration"), SerializeField]
        private float duration;
        public float Duration => duration;

        public List<StatModifier> BuffStats;

        public virtual void ApplyEffect(StatContainer target, int stacks) { }
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
