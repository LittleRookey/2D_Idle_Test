using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

namespace Litkey.Stat
{
    [CreateAssetMenu(menuName = "Litkey/Buff/DamageOverTimeBuff")]
    public class DamageOverTimeBuff : Buff
    {
        public enum DamageType
        {
            Fixed,
            PercentageOfMaxHP,
            PercentageOfCurrentHP,
        }

        [SerializeField] private DamageType damageType;

        [SerializeField] private float damageAmount;
        [SerializeField] private float damageInterval = 1f;
        public float DamageInterval => damageInterval;
        
        public float CalculateDamage(StatContainer targetStat)
        {
            switch (damageType)
            {
                case DamageType.Fixed:
                    return damageAmount;
                case DamageType.PercentageOfMaxHP:
                    return targetStat.HP.GetFinalValueWithoutBuff() * (damageAmount / 100f);
                case DamageType.PercentageOfCurrentHP:
                    return targetStat.GetComponent<Health>().CurrentHealth * (damageAmount / 100f);
                default:
                    return 0f;
            }
        }

        private IEnumerator DamageOverTime(StatContainer target, int stacks)
        {
            float elapsedTime = 0f;
            while (elapsedTime < Duration || PermanentDuration)
            {
                yield return new WaitForSeconds(damageInterval);
                float damage = CalculateDamage(target) * stacks;
                target.GetComponent<Health>().TakeDamage(null, damage, true);
                elapsedTime += damageInterval;
            }
        }

        public override void ApplyEffect(StatContainer target, int stacks)
        {
            base.ApplyEffect(target, stacks);
            target.StartCoroutine(DamageOverTime(target, stacks));
        }

    }
}