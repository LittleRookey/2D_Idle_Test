using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using System;
using Litkey.InventorySystem;
using DamageNumbersPro;
using Litkey.Stat;

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


    public class DropItemCreator
    {
        private static readonly string dropItemPath = "Prefabs/UI/DropItem";

        private static Pool<DropItem> dropItemPool;

        private static readonly string goldName = "°ñµå";

        private static readonly string goldSpritePath = "Images/icons_items_coin";
        private static readonly string equipmentRarityColorPath = "ScriptableObject/EquipmentRarityColor";

        private static EquipmentRarityColor rarityColor;

        private static Sprite goldImage;

        private static readonly float returnTime = 1.5f;
        public static DropItem CreateDrop(Vector3 spawnPosition, ItemData item, int count, Color textColor)
        {
            CheckPoolExists();

            var newBar = dropItemPool.Get();

            Debug.Log("New Shape is null?:: " + newBar == null);
            if (newBar == null)
            {
                dropItemPool.Clear();
                dropItemPool = null;
                var bar = Resources.Load<DropItem>(dropItemPath);
                rarityColor = Resources.Load<EquipmentRarityColor>(equipmentRarityColorPath);
                dropItemPool = Pool.Create<DropItem>(bar).NonLazy();
                newBar = dropItemPool.Get();
            }
            newBar.SetDropItem(item.IconSprite, item.Name, count, textColor);
            newBar.transform.position = spawnPosition;
            
            newBar.gameObject.SetActive(true);
            
            newBar.CreateBouncingEffect();

            return newBar;
        }

        public static DropItem CreateDrop(Vector3 spawnPosition, ItemData item, int count)
        {
            CheckPoolExists();

            var newBar = dropItemPool.Get();

            Debug.Log("New Shape is null?:: " + newBar == null);
            if (newBar == null)
            {
                dropItemPool.Clear();
                dropItemPool = null;
                var bar = Resources.Load<DropItem>(dropItemPath);
                rarityColor = Resources.Load<EquipmentRarityColor>(equipmentRarityColorPath);
                dropItemPool = Pool.Create<DropItem>(bar).NonLazy();
                newBar = dropItemPool.Get();
            }
            newBar.SetDropItem(item.IconSprite, item, count, rarityColor.GetColor(item.rarity));
            newBar.transform.position = spawnPosition;

            newBar.gameObject.SetActive(true);

            newBar.CreateBouncingEffect();

            return newBar;

        }
        public static DropItem CreateGoldDrop(Vector3 spawnPosition, int count)
        {
            CheckPoolExists();

            var newBar = dropItemPool.Get();


            if (newBar == null)
            {
                dropItemPool.Clear();
                dropItemPool = null;
                var bar = Resources.Load<DropItem>(dropItemPath);
                goldImage = Resources.Load<Sprite>(goldSpritePath);
                rarityColor = Resources.Load<EquipmentRarityColor>(equipmentRarityColorPath);
                dropItemPool = Pool.Create<DropItem>(bar).NonLazy();
                newBar = dropItemPool.Get();
            }
            newBar.SetDropItem(goldImage, goldName, count, Color.white, true);
            newBar.transform.position = spawnPosition;

            newBar.gameObject.SetActive(true);

            newBar.CreateBouncingEffect();

            return newBar;
        }

        public static void ReturnDrop(DropItem usedBar)
        {
            try
            {
                dropItemPool.Take(usedBar);
            }
            catch (NullReferenceException nl)
            {
                Debug.Log("DropItem is null: " + nl.Message);
            }
            catch (ArgumentException arg)
            {
                Debug.Log("Argument exception: " + arg.Message);
            }
        }


        private static void CheckPoolExists()
        {
            if (dropItemPool == null)
            {
                var bar = Resources.Load<DropItem>(dropItemPath);
                goldImage = Resources.Load<Sprite>(goldSpritePath);
                rarityColor = Resources.Load<EquipmentRarityColor>(equipmentRarityColorPath);
                dropItemPool = Pool.Create<DropItem>(bar).NonLazy();
            }
        }
    }


    public class DamagePopup
    {
        private static readonly string baseDMGTextPath = "Prefabs/BaseDamage";
        private static readonly string missTextPath = "Prefabs/Miss";
        private static readonly string critDMGTextPath = "Prefabs/CriticalDamage";

        protected static DamageNumberMesh dmg;
        protected static DamageNumberMesh missText;
        protected static DamageNumberMesh critDamageText;
        protected static Vector3 dmgOffset = new Vector3(0f, 0.6f, 0f);


        public static IEnumerator ShowDmgText(Vector3 spawnPosition, List<Damage> damages, bool[] misses)
        {
            CheckPoolExists();
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            if (misses != null)
            {
                //Debug.Log("Show damage text count: " + damages.Count);
                for (int i = 0; i < damages.Count; i++)
                {
                    var spawnPos = spawnPosition + Vector3.up * 0.7f + dmgOffset * (i + 1);
                    if (!misses[i])
                    {
                        ShowMissText(spawnPos);
                        yield return delay;
                        continue;
                    }

                    if (damages[i].isCrit)
                    {
                        critDamageText.Spawn(spawnPos, damages[i].damage);
                    }
                    else
                    {
                        dmg.Spawn(spawnPos, damages[i].damage);
                    }
                    yield return delay;
                }
            }
        }

        protected static void ShowMissText(Vector3 spawnPosition)
        {
            missText.Spawn(spawnPosition);
        }

        internal static IEnumerator ShowDmgText(Vector3 position, List<float> damages, object p)
        {
            throw new NotImplementedException();
        }

        private static void CheckPoolExists()
        {
            if (dmg == null || missText == null || critDamageText == null)
            {
                dmg = Resources.Load<DamageNumberMesh>(baseDMGTextPath);
                missText = Resources.Load<DamageNumberMesh>(missTextPath);
                critDamageText = Resources.Load<DamageNumberMesh>(critDMGTextPath);
            }
        }
    }
}