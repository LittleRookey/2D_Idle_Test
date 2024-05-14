using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResourceInfo : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI resourceText;
    [SerializeField] private DOTweenAnimation anim;
    public void SetResourceInfo(Sprite resourceSprite, string resourceText)
    {
        resourceIcon.sprite = resourceSprite;
        this.resourceText.SetText(resourceText);
    }

    public void SetResourceInfo(Sprite resourceSprite, string resourceText, Color textColor)
    {
        resourceIcon.sprite = resourceSprite;
        this.resourceText.SetText(resourceText);
        this.resourceText.color = textColor;
    }

    public void PlayAnim()
    {
        anim.DORestartById("GoUp");
    }
}
