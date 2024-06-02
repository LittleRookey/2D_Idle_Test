using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.InventorySystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using Litkey.Utility;

public class DropItem : MonoBehaviour
{
    public Image icon;
    public GameObject onDropEffect;
    public TextMeshProUGUI itemText;
    private Color textColor;
    private string itemName;
    private int itemCount;
    private ItemData itemData;

    [Header("Bounces")]
    public int maxBounce; // Maximum bounce height
    public float xForce; // Horizontal force
    public float yForce; // Vertical force
    public float gravity; // Gravity force

    [SerializeField] private EquipmentRarityColor equipmentColor;

    string num = " x";
    
    public IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(1.5f);

        DropItemCreator.ReturnDrop(this);
    }


    [Button("PlayBounce")]
    public void CreateBouncingEffect()
    {
        var randomCircle = Random.insideUnitCircle;
        DOTween.Sequence()
            .Append(transform.DOBlendableMoveBy(new Vector3(randomCircle.x, randomCircle.y, 0f) * xForce, xForce / 2f).SetEase(Ease.OutQuint))
            .Join(transform.DOBlendableMoveBy(Vector3.up * yForce, yForce / 2f).SetEase(Ease.OutExpo))
            .Insert(yForce / 2f, transform.DOBlendableMoveBy(Vector3.down * maxBounce, maxBounce / 2f).SetEase(Ease.OutBounce))
            .Play();
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount = 1)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
        StartCoroutine(ReturnToPool());
    }
    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity, ItemData itemData)
    {
        this.itemData = itemData;

        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        
        this.textColor = equipmentColor.GetColor(rarity);
        this.itemText.color = textColor;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
        StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
        StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor, bool showCountText = false)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;

        if (showCountText)
            this.itemText.text = itemName + num + itemCount;
        StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, bool showCountText = false)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;
        StartCoroutine(ReturnToPool());
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        if (showCountText)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, EquipmentRarity rarity, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        StartCoroutine(ReturnToPool());

        this.textColor = equipmentColor.GetColor(rarity);
        this.itemText.color = textColor;
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;

        StartCoroutine(ReturnToPool());
        onDropEffect.transform.SetParent(this.onDropEffect.transform);
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, Color textColor, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;
        
        StartCoroutine(ReturnToPool());

        onDropEffect.transform.SetParent(this.onDropEffect.transform);

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }
}
