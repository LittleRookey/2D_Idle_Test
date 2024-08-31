using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Litkey.InventorySystem;

namespace Litkey.Stat
{
    [System.Serializable]
    public class MinMaxStatModifier : StatModifier
    {
        [HorizontalGroup("MinMaxValue")]
        [LabelText("Min Value")]
        public float minValue;

        [HorizontalGroup("MinMaxValue")]
        [LabelText("Max Value")]
        public float maxValue;

        

        public MinMaxStatModifier(eSubStatType statType, OperatorType oper, float minValue, float maxValue)
        {
            this.statType = statType;
            this.oper = oper;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = 0; // Initialize value to 0, it will be set when GetRandomValue is called
        }

        public MinMaxStatModifier(eSubStatType statType, OperatorType oper, float minValue, float maxValue, float currentValue)
        {
            this.statType = statType;
            this.oper = oper;
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.value = currentValue; // Initialize value to 0, it will be set when GetRandomValue is called
        }

        [Button("Generate Random Value")]
        public float GetRandomValue()
        {
            if (value > float.Epsilon) return value;
            if (minValue > maxValue)
            {
                Debug.LogWarning($"MinMaxStatModifier: Min value ({minValue}) is greater than Max value ({maxValue}). Swapping values.");
                float temp = minValue;
                minValue = maxValue;
                maxValue = temp;
            }

            float randomValue = UnityEngine.Random.Range(minValue, maxValue);

            // Round to 2 decimal places
            randomValue = (float)Math.Round(randomValue, 2);

            // Update the base class's value
            this.value = randomValue;

            return randomValue;
        }

        public override string ToString()
        {
            return $"{statType} {oper} {minValue}-{maxValue}";
        }

        // Override the Compare method to include min and max values
        public bool Compare(MinMaxStatModifier other)
        {
            return this.statType == other.statType
                && this.oper == other.oper
                && Math.Abs(this.minValue - other.minValue) < float.Epsilon
                && Math.Abs(this.maxValue - other.maxValue) < float.Epsilon;
        }
    }
}