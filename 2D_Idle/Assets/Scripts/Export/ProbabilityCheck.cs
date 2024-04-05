using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;
using Litkey.Stat;
using System;

namespace Litkey.Utility
{
    public static class Effects
    {
        public static void ScaleUpMagicCircle(GameObject go, float finalScale, float duration)
        {
            go.gameObject.SetActive(true);
            go.transform.localScale = Vector3.zero;
            float scale = 0f;
            DOTween.To(() => scale, x => scale = x, finalScale, duration)
                .OnUpdate(() => {
                    go.transform.localScale = Vector3.one * scale;
                })
                .OnComplete(() => go.gameObject.SetActive(false));
        }
    }

    public static class ProbabilityCheck
    {
        /// <summary>
        /// Chance: float number between 0 and 1
        /// </summary>
        /// <param name="Chance"></param>
        /// <returns>returns the success based on the given chance number between 0 and 1</returns>
        public static bool GetThisChanceResult(float Chance)
        {
            if (Chance < 0.0000001f)
            {
                Chance = 0.0000001f;
            }

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }

        
        /// <summary>
        /// Percentage_Chance: number between 0 and 100.
        /// </summary>
        /// <param name="Percentage_Chance"></param>
        /// <returns>returns the success based on the given number percentage</returns>
        public static bool GetThisChanceResult_Percentage(float Percentage_Chance)
        {
            if (Percentage_Chance < 0.0000001f)
            {
                Percentage_Chance = 0.0000001f;
            }

            Percentage_Chance = Percentage_Chance / 100;

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Percentage_Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }

        //internal static bool GetThisChanceResult_Percentage(SubStat p_critChance)
        //{
        //    throw new NotImplementedException();
        //}
    }
}