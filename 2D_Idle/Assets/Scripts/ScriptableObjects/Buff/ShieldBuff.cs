using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Stat
{
    [CreateAssetMenu(menuName = "Litkey/Buff/ShieldBuff")]
    public class ShieldBuff : Buff
    {
        public enum ShieldType
        {
            FixedAmount,
            PercentOfMaxHP
        }

        [VerticalGroup("Buff/Right")]
        public ShieldType shieldType;

        [VerticalGroup("Buff/Right")]
        [ShowIf("shieldType", ShieldType.FixedAmount)]
        public float fixedShieldAmount;

        [VerticalGroup("Buff/Right")]
        [ShowIf("shieldType", ShieldType.PercentOfMaxHP)]
        [Range(0, 100)]
        public float shieldPercent;

        public float CalculateShieldAmount(StatContainer statContainer)
        {
            switch (shieldType)
            {
                case ShieldType.FixedAmount:
                    return fixedShieldAmount;
                case ShieldType.PercentOfMaxHP:
                    return statContainer.HP.GetFinalValueWithoutBuff() * (shieldPercent / 100f);
                default:
                    return 0f;
            }
        }
    }

    public class ShieldInstance
    {
        public float Amount { get; private set; }
        public float RemainingDuration { get; private set; }
        public bool IsPermanent { get; private set; }

        public ShieldInstance(float amount, float duration, bool isPermanent)
        {
            Amount = amount;
            RemainingDuration = duration;
            IsPermanent = isPermanent;
        }

        public void ReduceAmount(float damage)
        {
            Amount = Mathf.Max(0, Amount - damage);
        }

        public void UpdateDuration(float deltaTime)
        {
            if (!IsPermanent)
            {
                RemainingDuration = Mathf.Max(0, RemainingDuration - deltaTime);
            }
        }
    }

}