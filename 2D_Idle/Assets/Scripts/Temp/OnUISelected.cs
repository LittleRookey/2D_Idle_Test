using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class OnUISelected : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Image selectedFrame;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deSelectedColor;
    
    private AnimationClip defaultAnimClip;
    private SpriteRenderer spriteRenderer;


    [SerializeField] private Button confirmButton;
    [SerializeField] private bool disableOnStart;
    private float deselectedSpeed = 0f;
    private float selectedSpeed = 0.8f;

    [SerializeField] private Color deselectedColorSprite = Color.white;
    [SerializeField] private Color selectedColorSprite = Color.white;

    public UnityEvent OnConfirmation;
    private void Awake()
    {
        spriteRenderer = playerAnim.GetComponent<SpriteRenderer>();
        defaultAnimClip = playerAnim.runtimeAnimatorController.animationClips[0];
        playerAnim.speed = deselectedSpeed;
        deselectedColorSprite = new Color(255f, 255f, 255f, 130f);
        selectedColorSprite = Color.white;
        if (disableOnStart)
        {
            gameObject.SetActive(false);
        }
        confirmButton.gameObject.SetActive(false);
    }
    public void SelectUI()
    {
        playerAnim.speed = selectedSpeed;
        selectedFrame.color = selectedColor;
        spriteRenderer.color = selectedColorSprite;

        confirmButton.gameObject.SetActive(true);
        confirmButton.onClick.AddListener(OnConfirm);
    }

    public void DeSelectUI()
    {
        playerAnim.speed = deselectedSpeed;
        selectedFrame.color = deSelectedColor;
        spriteRenderer.color = deselectedColorSprite;

        confirmButton.onClick.RemoveListener(OnConfirm);
    }

    public void DisableGOOnConfirm()
    {
        gameObject.SetActive(false);
    }

    public void OnConfirm()
    {
        OnConfirmation?.Invoke();
        confirmButton.gameObject.SetActive(false);
    }

}
