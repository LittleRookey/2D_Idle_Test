using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using Litkey.Stat;
using Litkey.InventorySystem;

namespace Litkey.Data
{


    public class BaseStatWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Datas Window")]
        private static void OpenWindow()
        {
            GetWindow<BaseStatWindow>().Show();
        }

        private CreateNewWeapon createNewWeapon;
        private CreateNewSubWeapon createNewSubweapon;
        private CreateNewHelmet createNewHelmet;
        private CreateNewTopArmor createNewTopArmor;
        private CreateNewBottomArmor createNewBottomArmor;
        private CreateNewShoeArmor createNewShoeArmor;
        private CreateNewGloveArmor createNewGloveArmor;

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewWeapon = new CreateNewWeapon();
            createNewSubweapon = new CreateNewSubWeapon();
            createNewHelmet = new CreateNewHelmet();
            createNewTopArmor = new CreateNewTopArmor();
            createNewBottomArmor = new CreateNewBottomArmor();
            createNewShoeArmor = new CreateNewShoeArmor();
            createNewGloveArmor = new CreateNewGloveArmor();

            tree.AddAllAssetsAtPath("BaseStat", "Assets/Resources/ScriptableObject/BaseStat/", typeof(BaseStat), true);

            tree.Add("Create Weapon", createNewWeapon, EditorIcons.Plus);
            tree.Add("Create SubWeapon", createNewSubweapon, EditorIcons.Plus);
            tree.Add("Create Helmet", createNewHelmet, EditorIcons.Plus);
            tree.Add("Create BodyArmor", createNewTopArmor, EditorIcons.Plus);
            tree.Add("Create Pants", createNewBottomArmor, EditorIcons.Plus);
            tree.Add("Create Shoe", createNewShoeArmor, EditorIcons.Plus);
            tree.Add("Create Glove", createNewGloveArmor, EditorIcons.Plus);

            tree.AddAllAssetsAtPath("Items", "Assets/Resources/ScriptableObject/Equipment/", typeof(ItemData), true);
            tree.AddAllAssetsAtPath("LootTables", "Assets/Resources/ScriptableObject/LootTable/", typeof(LootTable), true);
            return tree;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewWeapon != null)
                DestroyImmediate(createNewWeapon.itemData);
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = this.MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    ItemData asset = selected.SelectedValue as ItemData;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

    }

    public class CreateNewWeapon
    {

        public CreateNewWeapon()
        {
            itemData = ScriptableObject.CreateInstance<WeaponItemData>();
            itemData.SetParts(eEquipmentParts.Weapon);
        }
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public WeaponItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/Weapons/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<WeaponItemData>();

        }
    }

    public class CreateNewSubWeapon
    {

        public CreateNewSubWeapon()
        {
            itemData = ScriptableObject.CreateInstance<WeaponItemData>();
            itemData.SetParts(eEquipmentParts.Subweapon);
        }
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public WeaponItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/SubWeapons/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<WeaponItemData>();

        }
    }


    public class CreateNewHelmet
    {
        public CreateNewHelmet()
        {
            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
            itemData.SetParts(eEquipmentParts.helmet);
        }
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ArmorItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/Helmets/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ArmorItemData>();

        }
    }

    public class CreateNewTopArmor
    {
        public CreateNewTopArmor()
        {
            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
            itemData.SetParts(eEquipmentParts.body);
        }

        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ArmorItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/TopArmors/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
        }
    }

    public class CreateNewBottomArmor
    {
        public CreateNewBottomArmor()
        {
            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
            itemData.SetParts(eEquipmentParts.pants);
        }

        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ArmorItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/BottomArmors/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
        }
    }

    public class CreateNewShoeArmor
    {
        public CreateNewShoeArmor()
        {
            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
            itemData.SetParts(eEquipmentParts.shoe);
        }

        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ArmorItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/ShoeArmors/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
        }
    }

    public class CreateNewGloveArmor
    {
        public CreateNewGloveArmor()
        {
            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
            itemData.SetParts(eEquipmentParts.Glove);
        }

        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public ArmorItemData itemData;

        [Button("Add new ItemData")]
        private void CreateNew()
        {
            AssetDatabase.CreateAsset(itemData, "Assets/Resources/ScriptableObject/Equipment/Gloves/" + itemData.Name + ".asset");
            AssetDatabase.SaveAssets();

            itemData = ScriptableObject.CreateInstance<ArmorItemData>();
        }
    }


}