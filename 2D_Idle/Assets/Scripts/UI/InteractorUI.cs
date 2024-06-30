using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public enum eResourceType
{
    ����,
    ����,
    �����,
    �ο�,
    ���ɱ�
}
public class InteractorUI : MonoBehaviour
{
    public static InteractorUI Instance;
    [SerializeField] private RectTransform orientation;
    [SerializeField] private Button interactBTN;
    [SerializeField] private Image btnIcon;
    [SerializeField] private DOTweenAnimation clickAnimation;

    [SerializeField] private Sprite mineSprite;
    [SerializeField] private Sprite axeSprite;
    [SerializeField] private Sprite fishingSprite;
    [SerializeField] private Sprite fightSprite;
    [SerializeField] private Sprite talkSprite;
    public bool disableButtonOnStart;

    private readonly string click = "click";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (disableButtonOnStart) DisableOrientation();
    }

    private void SetSprite(eResourceType resourceType)
    {
        switch (resourceType)
        {
            case eResourceType.����:
                btnIcon.sprite = mineSprite;
                break;
            case eResourceType.����:
                btnIcon.sprite = axeSprite;
                break;
            case eResourceType.�����:
                btnIcon.sprite = fishingSprite;
                break;
            case eResourceType.�ο�:
                btnIcon.sprite = fightSprite;
                break;
            case eResourceType.���ɱ�:
                btnIcon.sprite = talkSprite;
                break;
        }
    }
    public void SetInteractor(eResourceType resourceType, UnityAction onClickBTN=null)
    {
        SetSprite(resourceType);
        interactBTN.onClick.RemoveAllListeners();
        interactBTN.onClick.AddListener(onClickBTN);
        interactBTN.onClick.AddListener(() => clickAnimation.DORestartById(click));

    }

    public void SetTalk(UnityAction onTalk)
    {
        SetSprite(eResourceType.���ɱ�);
        interactBTN.onClick.RemoveAllListeners();
        interactBTN.onClick.AddListener(onTalk);

    }

    public void SetAttack()
    {

    }

    public void EnableOrientation() => orientation.gameObject.SetActive(true);
    public void DisableOrientation() => orientation.gameObject.SetActive(false);
}
