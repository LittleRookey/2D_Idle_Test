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
            Debug.Log($"È®·ü ¼º°ø?: {Rand <= RandHitRange} / by {Rand} <= {RandHitRange}");
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

    public static class PolygonRandomPosition
    {
        public static Vector2 GetRandomPositionOf(PolygonCollider2D polygonCollider)
        {
            int maxAttempts = 100; // Maximum number of attempts to find a point within the polygon
            int attempts = 0;

            Vector2 randomPoint;

            do
            {
                // Create a random point within the bounds of the PolygonCollider2D
                randomPoint = new Vector2(
                    UnityEngine.Random.Range(polygonCollider.bounds.min.x, polygonCollider.bounds.max.x),
                    UnityEngine.Random.Range(polygonCollider.bounds.min.y, polygonCollider.bounds.max.y)
                );

                attempts++;

                // Check if the random point is within the PolygonCollider2D
                if (polygonCollider.OverlapPoint(randomPoint))
                {
                    // The random point is within the PolygonCollider2D
                    Debug.Log("Random point within collider: " + randomPoint);
                    return randomPoint;
                }
            } while (attempts < maxAttempts);

            // If no point is found within the maximum number of attempts
            Debug.LogWarning("Failed to find a random point within the collider after " + maxAttempts + " attempts.");
            return Vector2.zero;
        }
    }
}