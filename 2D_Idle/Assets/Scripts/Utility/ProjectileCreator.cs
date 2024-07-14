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

        private static Pool<ProjectileBehavior> projectilePool;

        public static ProjectileBehavior CreateProjectile(Vector3 spawnPosition, StatContainer attackerStat, LayerMask enemyLayer)
        {
            if (projectilePool == null)
            {
                CheckPoolExists();
                if (projectilePool == null)
                {
                    // Handle the case where the pool could not be created
                    Debug.LogError("Failed to create the BarTemplate object pool.");
                    return null;
                }
            }

            var newProjectile = projectilePool.Get();

            if (newProjectile == null)
            {
                projectilePool.Clear();
                projectilePool = null;
                var bar = Resources.Load<ProjectileBehavior>(projectilePrefabPath);
                projectilePool = Pool.Create<ProjectileBehavior>(bar).NonLazy();
                newProjectile = projectilePool.Get();
            }

            newProjectile.transform.position = spawnPosition;
            newProjectile.Initialize();
            newProjectile.SetEnemyLayer(enemyLayer)
                         .SetStatContainer(attackerStat);


            return newProjectile;
        }

        public static void ReturnProjectile(ProjectileBehavior usedBar)
        {
            try
            {
                projectilePool.Take(usedBar);
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

        private static void CheckPoolExists()
        {
            if (projectilePool == null)
            {
                var bar = Resources.Load<ProjectileBehavior>(projectilePrefabPath);
                if (bar == null)
                {
                    Debug.LogError($"Failed to load the prefab from path: {projectilePrefabPath}");
                }
                else
                {
                    projectilePool = Pool.Create<ProjectileBehavior>(bar).NonLazy();
                }
            }
        }
    }
}
