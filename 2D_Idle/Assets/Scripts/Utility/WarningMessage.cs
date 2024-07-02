using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class WarningMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Image msgBG;
    [SerializeField] private Image icon;
    [SerializeField] private DOTweenAnimation fadeOutAnim;
    private string system = "[System] ";
    private float fadeDuration = 0.5f;
    WaitForSeconds sec;
    
    private void Awake()
    {
        sec = new WaitForSeconds(1f);        
    }
    public void SetMessage(string message, float messageRemainTime=1.0f, UnityAction onCompleted = null)
    {
        InitializeMessage();
        warningText.SetText(system + message);
        StartFadeOut(messageRemainTime, onCompleted);
    }

    private void StartFadeOut(float messageRemainTime = 1f, UnityAction onCompleted=null)
    {
        fadeOutAnim.tween.SetDelay(messageRemainTime);
        DOTween.Sequence()
        .Append(warningText.DOFade(0f, fadeDuration))
        .Join(msgBG.DOFade(0f, fadeDuration))
        .Join(icon.DOFade(0f, fadeDuration))
        .OnPlay(() => fadeOutAnim.DORestart())
        .SetDelay(messageRemainTime)
        .OnComplete(() => onCompleted?.Invoke())
        .Restart();
    }

    private void InitializeMessage()
    {
        warningText.color = Color.white;
        msgBG.color = Color.white;
        icon.color = Color.white;
    }
}
