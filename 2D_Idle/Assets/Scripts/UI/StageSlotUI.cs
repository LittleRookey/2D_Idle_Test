using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AssetKits.ParticleImage;
using TMPro;
using Litkey.Utility;
using System;

public class StageSlotUI : MonoBehaviour
{
    [SerializeField] private Button stageClickButton;
    [SerializeField] private DOTweenAnimation onSelectedAnim;
    [Header("Childs")]
    [SerializeField] private Image bottomStage;
    [SerializeField] private DOTweenAnimation pointer;
    [SerializeField] private ParticleImage onSelectedParticle;
    [SerializeField] private Image stageBalloon;
    [SerializeField] private TextMeshProUGUI stageTitleText;

    public bool isLocked = true;
    [SerializeField] private Color lockedColor;

    private void Awake()
    {
        pointer.onComplete.AddListener(() =>
        {
            stageBalloon.gameObject.SetActive(true);
        });
        Deselect();
    }

    public void SetStage(bool isLocked)
    {

        this.isLocked = isLocked;
        if (this.isLocked)
        {
            bottomStage.color = lockedColor;
        }
    }

    public void Select()
    {
        if (isLocked)
        {
            WarningMessageInvoker.Instance.ShowMessage("해당 스테이지는 잠겨있습니다.");
            return;
        }

        pointer.gameObject.SetActive(true);

        onSelectedParticle.gameObject.SetActive(true);
        onSelectedParticle.Play();

        foreach(var tween in pointer.GetTweens())
        {
            tween.Restart();
        }
    }

    public void Deselect()
    {
        pointer.gameObject.SetActive(false);

        onSelectedParticle.gameObject.SetActive(false);
        onSelectedParticle.Stop();

        stageBalloon.gameObject.SetActive(false);

        foreach (var tween in pointer.GetTweens())
        {
            tween.Pause();
        }
    }

    public void Unlock()
    {
        isLocked = false;
        bottomStage.color = Color.white;
    }
}
