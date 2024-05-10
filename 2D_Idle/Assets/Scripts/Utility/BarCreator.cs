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
            CheckPoolExists();

            var newBar = barPool.Get();

            newBar.transform.position = spawnPosition;

            newBar.SetBar(startWithFullBar);

            return newBar;
        }

        public static BarTemplate CreateFillBar(Vector3 spawnPosition, Transform parent, bool startWithFullBar=true)
        {
            CheckPoolExists();

            var newBar = barPool.Get();

            newBar.transform.position = spawnPosition;
            newBar.transform.parent = parent;

            newBar.SetBar(startWithFullBar);

            return newBar;
        }

        public static BarTemplate CreateFillBar(float width, float height, Vector3 spawnPosition, Transform parent, bool startWithFullBar=true)
        {
            CheckPoolExists();

            var newBar = barPool.Get();

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
            }catch (ArgumentException)
            {
            }
        }

        private static void CheckPoolExists()
        {
            if (barPool == null)
            {
                var bar = Resources.Load<BarTemplate>(barPath);
                barPool = Pool.Create<BarTemplate>(bar, 3).NonLazy();
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
            catch (ArgumentException)
            {
            }
        }

        private static void CheckPoolExists()
        {
            if (shapePool == null)
            {
                var bar = Resources.Load<SpriteRenderer>(shapePath);
                shapePool = Pool.Create<SpriteRenderer>(bar, 3).NonLazy();
            }
        }
    }
}