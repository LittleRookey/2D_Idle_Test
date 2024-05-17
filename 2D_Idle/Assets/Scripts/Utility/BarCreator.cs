using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using System;

namespace Litkey.Utility 
{ 
    public class BarCreator 
    {
        private static readonly string barPath = "Prefabs/UI/BarTemplate";

        private static Pool<BarTemplate> barPool;

        public static BarTemplate CreateFillBar(Vector3 spawnPosition, bool startWithFullBar=true)
        {
            if (barPool == null)
            {
                CheckPoolExists();
                if (barPool == null)
                {
                    // Handle the case where the pool could not be created
                    Debug.LogError("Failed to create the BarTemplate object pool.");
                    return null;
                }
            }

            var newBar = barPool.Get();

            if (newBar == null)
            {
                barPool.Clear();
                barPool = null;
                var bar = Resources.Load<BarTemplate>(barPath);
                barPool = Pool.Create<BarTemplate>(bar).NonLazy();
                newBar = barPool.Get();
            }

            newBar.transform.position = spawnPosition;

            newBar.SetBar(startWithFullBar);

            return newBar;
        }

        public static BarTemplate CreateFillBar(Vector3 spawnPosition, Transform parent, bool startWithFullBar=true)
        {
            if (barPool == null)
            {
                CheckPoolExists();
                if (barPool == null)
                {
                    // Handle the case where the pool could not be created
                    Debug.LogError("Failed to create the BarTemplate object pool.");
                    return null;
                }
            }

            var newBar = barPool.Get();
            Debug.Log("New Bar is null?:: " +newBar == null);
            if (newBar == null)
            {
                barPool.Clear();
                barPool = null;
                var bar = Resources.Load<BarTemplate>(barPath);
                barPool = Pool.Create<BarTemplate>(bar).NonLazy();
                newBar = barPool.Get();
            }
            newBar.transform.position = spawnPosition;
            newBar.transform.parent = parent;

            newBar.SetBar(startWithFullBar);

            return newBar;
        }

        public static BarTemplate CreateFillBar(float width, float height, Vector3 spawnPosition, Transform parent, bool startWithFullBar=true)
        {
            if (barPool == null)
            {
                CheckPoolExists();
                if (barPool == null)
                {
                    // Handle the case where the pool could not be created
                    Debug.LogError("Failed to create the BarTemplate object pool.");
                    return null;
                }
            }

            var newBar = barPool.Get();

            if (newBar == null)
            {
                barPool.Clear();
                barPool = null;
                var bar = Resources.Load<BarTemplate>(barPath);
                barPool = Pool.Create<BarTemplate>(bar).NonLazy();
                newBar = barPool.Get();
            }

            newBar.transform.position = spawnPosition;
            newBar.transform.parent = parent;

            newBar.SetBar(width, height, startWithFullBar);

            return newBar;
        }

        public static void ReturnBar(BarTemplate usedBar)
        {
            try
            {
                barPool.Take(usedBar);
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
            if (barPool == null)
            {
                var bar = Resources.Load<BarTemplate>(barPath);
                if (bar == null)
                {
                    Debug.LogError($"Failed to load the prefab from path: {barPath}");
                }
                else
                {
                    barPool = Pool.Create<BarTemplate>(bar).NonLazy();
                }
            }
        }


    }

    public class ShapeCreator
    {
        private static readonly string shapePath = "Prefabs/UI/Circle";

        private static Pool<SpriteRenderer> shapePool;

        public static SpriteRenderer CreateCircle(Vector3 spawnPosition, Color baseColor=default)
        {
            CheckPoolExists();

            var newBar = shapePool.Get();

            Debug.Log("New Shape is null?:: " + newBar == null);
            if (newBar == null)
            {
                shapePool.Clear();
                shapePool = null;
                var bar = Resources.Load<SpriteRenderer>(shapePath);
                shapePool = Pool.Create<SpriteRenderer>(bar).NonLazy();
                newBar = shapePool.Get();
            }

            newBar.color = baseColor;
            newBar.transform.position = spawnPosition;

            return newBar;
        }

        public static void ReturnShape(SpriteRenderer usedBar)
        {
            try
            {
                shapePool.Take(usedBar);
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
            if (shapePool == null)
            {
                var bar = Resources.Load<SpriteRenderer>(shapePath);
                shapePool = Pool.Create<SpriteRenderer>(bar).NonLazy();
            }
        }
    }
}