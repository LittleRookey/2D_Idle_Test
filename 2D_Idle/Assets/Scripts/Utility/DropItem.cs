using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.InventorySystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using Litkey.Utility;
using UnityEngine.Events;
using System;

public class DropItem : MonoBehaviour
{
    public Image icon;
    public ParticleSystem onDropEffect;
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


    [Header("Movement")]
    public float lerpDuration = 1f;
    public AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public UnityEvent OnReachedDestination;
    string num = " x";

    public void MoveToBag(Transform target, bool useSlerp=false, UnityAction OnReachedDestination = null)
    {
        Debug.Log("DropItem MoveToBag");
        StartCoroutine(MoveToPosition(target, useSlerp, OnReachedDestination));
    }
    public void MoveToCoinPosition(Transform target, bool useSlerp=false, UnityAction OnReachedDestination=null)
    {
        Debug.Log("DropItem MoveToCoin");
        StartCoroutine(MoveToPosition(target, useSlerp, OnReachedDestination));
    }

    private IEnumerator MoveToPosition(Transform target, bool useSlerp = false, UnityAction OnReachedDestination = null)
    {
        Debug.Log("DropItem MoveToPosition1111");
        yield return new WaitForSeconds(1f);
        Debug.Log("DropItem MoveToPosition2222");
        StartCoroutine(MoveCoroutine(target, useSlerp, OnReachedDestination));
    }

    private IEnumerator MoveCoroutine(Transform target, bool useSlerp, UnityAction OnReachedDestination = null)
    {

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        Debug.Log("Entered Movecoroutine DropItem");
        while (elapsedTime < lerpDuration)
        {
            float t = lerpCurve.Evaluate(elapsedTime / lerpDuration);

            if (useSlerp)
            {
                transform.position = Vector3.Slerp(startPosition, target.position, t);
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, target.position, t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;

        yield return null;

        OnReachedDestination?.Invoke();
        Debug.Log("Entered ReachedDestination DropItem");
        var dotween = target.GetComponent<DOTweenAnimation>();
        if (dotween != null)
        {
            Debug.Log("Entered Dotween");
            dotween.DORestart();
        }

        ReturnPoolNoCoroutine();
    }

    public IEnumerator ReturnToPool(UnityAction OnReturned=null)
    {
        yield return new WaitForSeconds(3f);
        OnReturned?.Invoke();
        onDropEffect.gameObject.SetActive(false);
        DropItemCreator.ReturnDrop(this);
    }

    private void ReturnPoolNoCoroutine(UnityAction OnReturned=null)
    {
        OnReturned?.Invoke();
        onDropEffect.gameObject.SetActive(false);
        DropItemCreator.ReturnDrop(this);
    }

    [Button("PlayBounce")]
    public void CreateBouncingEffect(UnityAction onCompleted = null)
    {
        Debug.Log("DropItem Started Bouncing effect");
        var randomCircle = UnityEngine.Random.insideUnitCircle;
        DOTween.Sequence()
            .Append(transform.DOBlendableMoveBy(new Vector3(randomCircle.x, randomCircle.y, 0f) * xForce, xForce / 2f).SetEase(Ease.OutQuint))
            .Join(transform.DOBlendableMoveBy(Vector3.up * yForce, yForce / 2f).SetEase(Ease.OutExpo))
            .Insert(yForce / 2f, transform.DOBlendableMoveBy(Vector3.down * maxBounce, maxBounce / 2f).SetEase(Ease.OutBounce))
            .OnComplete(() =>
            {
                Debug.Log("DropItem OnComplete callback started");
                try
                {
                    ShowDropEffect();
                    Debug.Log("DropItem Completed Bouncing");
                    onCompleted?.Invoke();
                    Debug.Log("DropItem entered onCompleted Bouncing => onCompleted null? " + (onCompleted == null));
                }
                catch (Exception e)
                {
                    Debug.LogError("Error in OnComplete callback: " + e.Message);
                }
            })
            .Play();
    }

    private void ShowDropEffect()
    {
        Debug.Log("Starting ShowDropEffect");
        try
        {
            onDropEffect.gameObject.SetActive(true);
            var main = onDropEffect.main;
            main.startColor = equipmentColor.GetColor(this.itemData.rarity);
            onDropEffect.Play();
            Debug.Log("Finished ShowDropEffect");
        }
        catch (Exception e)
        {
            Debug.LogError("Error in ShowDropEffect: " + e.Message);
        }
    }
    public void SetDropItem(Sprite icon, string itemName, int itemCount = 1)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
        //StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, ItemRarity rarity, ItemData itemData)
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
        //StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, ItemData itemData, int itemCount, Color textColor)
    {
        this.icon.sprite = icon;
        this.itemName = itemData.Name;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.textColor = textColor;
        this.itemText.color = this.textColor;

        if (itemCount > 1)
            this.itemText.SetText($"{TMProUtility.GetColorText($"[{itemData.rarity.ToString()}]", textColor)} {itemData.Name} x{itemCount}");
        else
            this.itemText.SetText($"{TMProUtility.GetColorText($"[{itemData.rarity.ToString()}]", textColor)} {itemData.Name}");
        //StartCoroutine(ReturnToPool(onReturned));
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
        //StartCoroutine(ReturnToPool());
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, bool showCountText = false)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        this.itemText.color = this.textColor;
        //StartCoroutine(ReturnToPool());
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        if (showCountText)
            this.itemText.text = itemName + num + itemCount;
    }

    public void SetDropItem(Sprite icon, string itemName, int itemCount, ItemRarity rarity, GameObject onDropEffect)
    {
        this.icon.sprite = icon;
        this.itemName = itemName;
        this.itemCount = itemCount;
        this.itemText.text = itemName;
        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;

        //StartCoroutine(ReturnToPool());

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

        //StartCoroutine(ReturnToPool());
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
        
        //StartCoroutine(ReturnToPool());

        onDropEffect.transform.SetParent(this.onDropEffect.transform);

        if (itemCount > 1)
            this.itemText.text = itemName + num + itemCount;
    }

}
