using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.InventorySystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class RecipeUISlot : MonoBehaviour
{
    [SerializeField] private Button indexButton;
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private List<ItemSlotUI> itemDisplaySlots;
    [SerializeField] private ItemSlotUI resultDisplaySlot;
    [SerializeField] private Image arrow;

    [SerializeField] private Image highlight;
    [SerializeField] private Button selectButton;

    [SerializeField] private Button lockedButton;
    [SerializeField] private Image lockedImageIcon;
    [SerializeField] private DOTweenAnimation touchToUnlock;

    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    ItemRecipe recipe;
    public ItemRecipe Recipe => recipe;

    [Button("SetRecipe")]
    public void SetSlot(ItemRecipe recipe, UnityAction OnUnlocked=null)
    {
        this.recipe = recipe;
        DeSelectSlot();
        lockedImageIcon.DOFade(0.8f, 0.01f);
        lockedButton.image.DOFade(0.8f, 0.01f);
        lockedImageIcon.sprite = lockedSprite;

        for (int i = 0; i < itemDisplaySlots.Count; i++)
        {
            itemDisplaySlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < recipe.requiredItems.Count; i++)
        {
            itemDisplaySlots[i].SetSlot(recipe.requiredItems[i].itemData, recipe.requiredItems[i].count);
            itemDisplaySlots[i].gameObject.SetActive(true);
        }
        indexText.SetText(recipe.intID.ToString());

        resultDisplaySlot.SetSlot(recipe.resultItem.itemData, recipe.resultItem.count);


        
        
        highlight.gameObject.SetActive(false);
        if (recipe.isLocked)
        {
            lockedButton.gameObject.SetActive(true);
            touchToUnlock.gameObject.SetActive(true);
            touchToUnlock.DORestart();

            lockedButton.onClick.RemoveAllListeners();
            lockedButton.onClick.AddListener(() =>
            {
                recipe.SetUnlocked();
                OnUnlocked?.Invoke();
                touchToUnlock.DOPause();
                touchToUnlock.gameObject.SetActive(false);
                lockedImageIcon.GetComponent<DOTweenAnimation>()
                .tween.OnComplete(() =>
                {
                    lockedImageIcon.sprite = unlockedSprite;
                    lockedImageIcon.transform.DOScale(1.3f, 0.1f).SetEase(Ease.Linear);
                    lockedImageIcon.DOFade(0f, 2f)
                    .SetDelay(0.5f)
                    .OnPlay(() => lockedButton.image.DOFade(0f, 2f))
                    .OnComplete(() =>
                    {
                        lockedButton.gameObject.SetActive(false);
                        
                    });
                })
                .Restart();

            });
        }
        else
        {
            lockedButton.gameObject.SetActive(false);
        }
    }

    public void SelectSlot()
    {
        if (recipe == null) return;
        if (recipe.isLocked) return;
        highlight.gameObject.SetActive(true);
    }

    public void DeSelectSlot()
    {
        if (recipe == null) return;
        highlight.gameObject.SetActive(false);

    }

    public void ClearSlot()
    {
        this.recipe = null;
        lockedImageIcon.DOFade(0.8f, 0.1f);
        lockedButton.image.DOFade(0.8f, 0.1f);
        indexButton.onClick.RemoveAllListeners();
    }
}
