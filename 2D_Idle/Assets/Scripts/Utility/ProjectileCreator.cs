using Redcode.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Projectile
{

    public static class ProjectileCreator
    {
        private static readonly string projectilePrefabPath = "Prefabs/Projectile";
        private static Dictionary<string, Pool<ProjectileBehavior>> projectilePools;


        public static ProjectileBehavior CreateProjectile(ProjectileBehavior projectilePrefab, Vector3 spawnPosition, StatContainer attackerStat, LayerMask enemyLayer)
        {
            if (projectilePools == null)
            {
                projectilePools = new Dictionary<string, Pool<ProjectileBehavior>>();
            }
            string poolKey = projectilePrefab.projectileID;
            if (!CheckPoolExists(projectilePrefab))
            {
                Debug.LogError($"Failed to create or find the ProjectileBehavior object pool for key: {poolKey}");
                return null;
            }

            var newProjectile = projectilePools[poolKey].Get();
            if (newProjectile == null)
            {
                projectilePools[poolKey].Clear();
                projectilePools.Remove(poolKey);
                if (!CheckPoolExists(poolKey))
                {
                    Debug.LogError($"Failed to recreate the ProjectileBehavior object pool for key: {poolKey}");
                    return null;
                }
                newProjectile = projectilePools[poolKey].Get();
            }

            newProjectile.transform.position = spawnPosition;
            newProjectile.Initialize();
            newProjectile.SetEnemyLayer(enemyLayer)
                         .SetStatContainer(attackerStat);
            return newProjectile;

        }
        public static ProjectileBehavior CreateProjectile(string poolKey, Vector3 spawnPosition, StatContainer attackerStat, LayerMask enemyLayer)
        {
            if (projectilePools == null)
            {
                projectilePools = new Dictionary<string, Pool<ProjectileBehavior>>();
            }

            if (!CheckPoolExists(poolKey))
            {
                Debug.LogError($"Failed to create or find the ProjectileBehavior object pool for key: {poolKey}");
                return null;
            }

            var newProjectile = projectilePools[poolKey].Get();
            if (newProjectile == null)
            {
                projectilePools[poolKey].Clear();
                projectilePools.Remove(poolKey);
                if (!CheckPoolExists(poolKey))
                {
                    Debug.LogError($"Failed to recreate the ProjectileBehavior object pool for key: {poolKey}");
                    return null;
                }
                newProjectile = projectilePools[poolKey].Get();
            }

            newProjectile.transform.position = spawnPosition;
            newProjectile.Initialize();
            newProjectile.SetEnemyLayer(enemyLayer)
                         .SetStatContainer(attackerStat);
            return newProjectile;
        }

        public static void ReturnProjectile(string poolKey, ProjectileBehavior usedProjectile)
        {
            if (projectilePools == null || !projectilePools.ContainsKey(poolKey))
            {
                Debug.LogError($"Attempted to return a projectile to a non-existent pool: {poolKey}");
                return;
            }

            try
            {
                projectilePools[poolKey].Take(usedProjectile);
            }
            catch (NullReferenceException nl)
            {
                Debug.Log($"Projectile is null for pool {poolKey}: {nl.Message}");
            }
            catch (ArgumentException arg)
            {
                Debug.Log($"Argument exception for pool {poolKey}: {arg.Message}");
            }
        }

        private static bool CheckPoolExists(string poolKey)
        {
            if (!projectilePools.ContainsKey(poolKey))
            {
                var projectilePrefab = Resources.Load<ProjectileBehavior>($"{projectilePrefabPath}/{poolKey}");
                if (projectilePrefab == null)
                {
                    Debug.LogError($"Failed to load the prefab from path: {projectilePrefabPath}/{poolKey}");
                    return false;
                }
                else
                {
                    projectilePools[poolKey] = Pool.Create<ProjectileBehavior>(projectilePrefab).NonLazy();
                }
            }
            return true;
        }

        private static bool CheckPoolExists(ProjectileBehavior projectile)
        {
            string poolKey = projectile.projectileID;
            if (!projectilePools.ContainsKey(poolKey))
            {
                var projectilePrefab = projectile;
                if (projectilePrefab == null)
                {
                    Debug.LogError($"Failed to load the prefab from path: {projectilePrefabPath}/{poolKey}");
                    return false;
                }
                else
                {
                    projectilePools[poolKey] = Pool.Create<ProjectileBehavior>(projectilePrefab).NonLazy();
                }
            }
            return true;
        }
    }
}
