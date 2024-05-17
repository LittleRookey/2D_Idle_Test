using Redcode.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Litkey.Utility
{
    public class ResourcePopupCreator 
    {
        private static readonly string barPath = "Prefabs/UI/ResourcePopupUI";

        private static Pool<ResourceInfo> barPool;

        public static ResourceInfo CreatePopup(Vector3 spawnPosition, Transform parent, Sprite resourceIcon, string resourceText)
        {
            CheckPoolExists();

            var newBar = barPool.Get();

            Debug.Log("New ResourceUI is null?:: " + newBar == null);

            if (newBar == null)
            {
                barPool.Clear();
                barPool = null;
                var bar = Resources.Load<ResourceInfo>(barPath);
                barPool = Pool.Create<ResourceInfo>(bar).NonLazy();
                newBar = barPool.Get();
            }


            newBar.transform.position = spawnPosition;
            newBar.transform.parent = parent;
            Debug.Log("Spawned at position: " + spawnPosition);

            newBar.SetResourceInfo(resourceIcon, resourceText);

            newBar.PlayAnim();
            return newBar;
        }

        public static ResourceInfo CreatePopup(Vector3 spawnPosition, Sprite resourceIcon, string resourceText, Color textColor)
        {
            CheckPoolExists();

            var newBar = barPool.Get();

            Debug.Log("New ResourceUI is null?:: " + newBar == null);

            if (newBar == null)
            {
                barPool.Clear();
                barPool = null;
                var bar = Resources.Load<ResourceInfo>(barPath);
                barPool = Pool.Create<ResourceInfo>(bar).NonLazy();
                newBar = barPool.Get();
            }

            newBar.transform.position = spawnPosition;

            newBar.SetResourceInfo(resourceIcon, resourceText, textColor);

            newBar.PlayAnim();
            return newBar;
        }

        public static void ReturnBar(ResourceInfo usedBar)
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
                var bar = Resources.Load<ResourceInfo>(barPath);
                barPool = Pool.Create<ResourceInfo>(bar, 3).NonLazy();
            }
        }
    }
}
