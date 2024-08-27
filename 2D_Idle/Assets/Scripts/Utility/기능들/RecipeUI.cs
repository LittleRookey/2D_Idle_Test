using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redcode.Pools;
using TMPro;
using Litkey.Interface;
using Sirenix.OdinInspector;
using Litkey.Utility;

namespace Litkey.InventorySystem
{
    public class RecipeUI : MonoBehaviour, ILoadable, ISavable
    {
        public static RecipeUI Instance;
        [SerializeField] private GameDatas gameData;
        [SerializeField] private Inventory inventory;
        [SerializeField] private Canvas recipeWindow;
        [SerializeField] private RecipeUISlot recipeSlotPrefab;
        [SerializeField] private RectTransform recipeSlotParent;
        [SerializeField] private RecipeDatabase recipeDB;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Button makeRecipeButton;
        
        List<RecipeUISlot> activeRecipeSlots;
        Pool<RecipeUISlot> recipeSlotPool;

        private void Awake()
        {
            Instance = this;
            recipeSlotPool = Pool.Create<RecipeUISlot>(recipeSlotPrefab);
            recipeSlotPool.SetContainer(recipeSlotParent);
            activeRecipeSlots = new List<RecipeUISlot>();
            goldText.SetText(0.ToString());
            gameData.OnGameDataLoaded.AddListener(Load);
            CloseRecipeWindow();

        }
        private RecipeUISlot prevSelectedSlot;
        public void SelectSlot(RecipeUISlot slot)
        {
            if (prevSelectedSlot == null) prevSelectedSlot = slot;
            makeRecipeButton.onClick.RemoveAllListeners();
            prevSelectedSlot.DeSelectSlot();

            slot.SelectSlot();
            makeRecipeButton.onClick.AddListener(() => MakeRecipeProduct(slot.Recipe));
            goldText.SetText(slot.Recipe.requiredGold.ToString("N0"));
            prevSelectedSlot = slot;
        }

        public void OpenRecipeWindow()
        {
            recipeWindow.enabled = true;

        }

        public void CloseRecipeWindow()
        {
            recipeWindow.enabled = false;
            if (prevSelectedSlot != null) prevSelectedSlot.DeSelectSlot();
            makeRecipeButton.onClick.RemoveAllListeners();
            prevSelectedSlot = null;
            goldText.SetText(0.ToString());
        }

        
        public void AddRecipeToInventoryOnLoad(int recipeID)
        {
            // 중복체크, 중복이면 더하지 않기 
            for (int i = 0; i < activeRecipeSlots.Count; i++)
            {
                if (activeRecipeSlots[i].Recipe.intID == recipeID) 
                    return;
            }
            var recipeSlot = recipeSlotPool.Get();
            var recipe = recipeDB.GetRecipe(recipeID);
            recipeSlot.SetSlot(recipe, Save);
            recipeSlot.AddListener(SelectSlot);
            activeRecipeSlots.Add(recipeSlot);

        }

        // 레시피들을 로드할떄 잠김상태 업데이트하기
        // 레시피 처음얻엇을때는 잠김상태
        [Button("Addrecipe")]
        public void AddRecipeToInventoryOnGain(int recipeID)
        {
            // 중복체크, 중복이면 더하지 않기 
            for (int i = 0; i < activeRecipeSlots.Count; i++)
            {
                if (activeRecipeSlots[i].Recipe.intID == recipeID)
                    return;
            }
            var recipeSlot = recipeSlotPool.Get();
            var recipe = recipeDB.GetRecipe(recipeID);
            recipe.SetLocked();
            recipeSlot.SetSlot(recipe, Save);
            recipeSlot.AddListener(SelectSlot);
            activeRecipeSlots.Add(recipeSlot);
            Save();
        }

        public bool MakeRecipeProduct(ItemRecipe recipe)
        {
            bool notEnoughItem = false;
            foreach (var required in recipe.requiredItems)
            {
                if (!inventory.ContainsItem(required.itemData, required.count))
                {
                    notEnoughItem = true;
                }
            }
            if (!ResourceManager.Instance.HasGold(recipe.requiredGold))
            {
                notEnoughItem = true;
            }

            if (notEnoughItem)
            {
                WarningMessageInvoker.Instance.ShowMessage("제작 재료가 충분하지 않습니다");
                return false;
            }

            // 소비하기
            foreach (var required in recipe.requiredItems)
            {
                inventory.UseItem(required.itemData, required.count);
            }
            ResourceManager.Instance.UseGold(recipe.requiredGold);

            // 아이템 만들기 (충분한재료가 있는시점)
            inventory.AddToInventory(recipe.resultItem.itemData, recipe.resultItem.count);
            // TODO 제작 성공 메시지 띄우기

            return true;
        }

        public void Load()
        {
            var serializedRecipeData = gameData.dataSettings.recipeData;
            foreach (var kVal in serializedRecipeData.gainedRecipes)
            {
                if (kVal.Value.IsUnlocked) recipeDB.GetRecipe(kVal.Key).SetUnlocked();
                else recipeDB.GetRecipe(kVal.Key).SetLocked();

                AddRecipeToInventoryOnLoad(kVal.Key);
            }
        }

        public void Save()
        {
            var recipeStatus = gameData.dataSettings.recipeData;
            recipeStatus.SaveRecipes(this.activeRecipeSlots);
            gameData.SaveDataLocal();
        }


    }
}
