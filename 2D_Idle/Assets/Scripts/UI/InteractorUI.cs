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
    [SerializeField] private Button interactBTN;
    [SerializeField] private Image btnIcon;
    [SerializeField] private DOTweenAnimation clickAnimation;

    [SerializeField] private Sprite mineSprite;
    [SerializeField] private Sprite axeSprite;
    [SerializeField] private Sprite fishingSprite;
    [SerializeField] private Sprite fightSprite;
    [SerializeField] private Sprite talkSprite;


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
    }

    public void SetInteractor(eResourceType resourceType, UnityAction onClickBTN=null)
    {
        switch(resourceType)
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
        interactBTN.onClick.RemoveAllListeners();
        interactBTN.onClick.AddListener(() => onClickBTN?.Invoke());
        interactBTN.onClick.AddListener(() => clickAnimation.DORestartById(click));

    }


}
