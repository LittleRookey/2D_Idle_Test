using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

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
            barPool.Take(usedBar);
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
}