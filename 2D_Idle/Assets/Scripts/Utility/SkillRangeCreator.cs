using Redcode.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Skill 
{

    public static class SkillRangeCreator 
    {
        private static readonly string skillRangePrefabPath_Circle = "Prefabs/SkillRange/Circle";
        private static readonly string skillRangePrefabPath_Square = "Prefabs/SkillRange/Square";
        private static readonly string skillRangePrefabPath_Arc = "Prefabs/SkillRange/Arc";

        private static Pool<RangeSkillArea> skillRangePool_Circle;
        private static Pool<RangeSkillArea> skillRangePool_Square;
        private static Pool<RangeSkillArea> skillRangePool_Arc;

        private static Dictionary<eSkillRangeType, Pool<RangeSkillArea>> skillPools;
        public static RangeSkillArea CreateSquareSkillRange(Vector3 spawnPosition, float width, float height, float duration, UnityAction OnEnd=null)
        {
            
            CheckPoolExists(eSkillRangeType.square, skillRangePrefabPath_Square);
            
            var skillRangePool = skillPools[eSkillRangeType.square];

            var skillRange = skillRangePool.Get();

            skillRange.transform.position = spawnPosition;
            skillRange.SetRange(duration, width, height, OnEnd);

            return skillRange;
        }

        public static RangeSkillArea CreateArcSkillRange(Vector3 spawnPosition, float width, float height, float duration, UnityAction OnEnd=null)
        {

            CheckPoolExists(eSkillRangeType.arc, skillRangePrefabPath_Arc);

            var skillRangePool = skillPools[eSkillRangeType.arc];

            var skillRange = skillRangePool.Get();

            skillRange.transform.position = spawnPosition;
            skillRange.SetRange(duration, width, height, OnEnd);

            return skillRange;
        }

        public static RangeSkillArea CreateCircleSkillRange(Vector3 spawnPosition, float radius, float duration, UnityAction OnEnd=null)
        {

            CheckPoolExists(eSkillRangeType.circle, skillRangePrefabPath_Circle);

            var skillRangePool = skillPools[eSkillRangeType.circle];

            var skillRange = skillRangePool.Get();

            skillRange.transform.position = spawnPosition;
            skillRange.SetRange(duration, radius, OnEnd);

            return skillRange;
        }

        public static void ReturnSkillRange(RangeSkillArea usedBar)
        {
            try
            {
                skillPools[usedBar.SkillRangeType].Take(usedBar);
            }
            catch (NullReferenceException nl)
            {
                Debug.Log("Bar is null: " + nl.Message);
            }
            catch (ArgumentException arg)
            {
                Debug.Log("Argument exception: " + arg.Message);
            }
        }
        public static IEnumerator ReturnSkillRange(RangeSkillArea usedBar, float returnTime)
        {
            yield return new WaitForSeconds(returnTime);
            try
            {
                skillPools[usedBar.SkillRangeType].Take(usedBar);
            }
            catch (NullReferenceException nl)
            {
                Debug.Log("Bar is null: " + nl.Message);
            }
            catch (ArgumentException arg)
            {
                Debug.Log("Argument exception: " + arg.Message);
            }
        }
        private static void CheckPoolExists(eSkillRangeType skillRangeType, string skillRangePrefabPath)
        {
            if (skillPools == null)
            {
                skillPools = new Dictionary<eSkillRangeType, Pool<RangeSkillArea>>();
            }

            if (!skillPools.ContainsKey(skillRangeType))
            {
                var bar = Resources.Load<RangeSkillArea>(skillRangePrefabPath);
                if (bar == null)
                {
                    Debug.LogError($"Failed to load the prefab from path: {skillRangePrefabPath}");
                }
                else
                {
                    skillPools.Add(skillRangeType, Pool.Create<RangeSkillArea>(bar).NonLazy());
                }
            }
                
            
        }
    }
}
