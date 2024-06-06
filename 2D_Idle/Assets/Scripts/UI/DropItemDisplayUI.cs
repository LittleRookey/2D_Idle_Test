using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Litkey.InventorySystem;
using UnityEngine.Events;

public class DropItemDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Image BG;
    [SerializeField] private Image itemIcon;
    [SerializeField] private DOTweenAnimation tweenAnim;

    private Sprite goldSprite;
    private string goldPath = "Images/icons_items_coin";

    Color bgOriginColor;
    Color itemTextOriginColor;
    Color iconOriginColor;

    Color bgFadeColor;
    Color itemTextFadeColor;
    Color iconFadeColor;

    private UnityEvent<DropItemDisplayUI> OnFadeOut = new();

    private void Awake()
    {

        goldSprite = Resources.Load<Sprite>(goldPath);

        bgOriginColor = BG.color;
        itemTextOriginColor = itemText.color;
        iconOriginColor = itemIcon.color;

        bgFadeColor = new Color(BG.color.r, BG.color.g, BG.color.b, 0f);
        itemTextFadeColor = new Color(itemText.color.r, itemText.color.g, itemText.color.b, 0f);
        iconFadeColor = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0f);
    }
    public void SetItemUI(ItemData itemData, int count, UnityAction<DropItemDisplayUI> OnFadeOutEnd)
    {
        itemText.color = itemTextOriginColor;
        itemIcon.color = iconOriginColor;
        BG.color = bgOriginColor;

        itemText.SetText($"{itemData.Name} x{count}");
        itemIcon.sprite = itemData.IconSprite;
        tweenAnim.DORestart();

        OnFadeOut.AddListener(OnFadeOutEnd);

        StartCoroutine(FadeOut());
    }

    public void SetGoldUI(int count, UnityAction<DropItemDisplayUI> OnFadeOutEnd)
    {
        itemText.color = itemTextOriginColor;
        itemIcon.color = iconOriginColor;
        BG.color = bgOriginColor;

        itemIcon.sprite = goldSprite;
        itemText.SetText($"°ñµå x{count}");
        tweenAnim.DORestart();
        
        OnFadeOut.AddListener(OnFadeOutEnd);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f);
        FadeOutColor();
        yield return new WaitForSeconds(0.5f);
        OnFadeOut?.Invoke(this);
        OnFadeOut.RemoveAllListeners();
    }

    private void FadeOutColor()
    {
        BG.DOColor(bgFadeColor, 0.5f);
        itemText.DOColor(itemTextFadeColor, 0.5f);
        itemIcon.DOColor(iconFadeColor, 0.5f);

    }
}
