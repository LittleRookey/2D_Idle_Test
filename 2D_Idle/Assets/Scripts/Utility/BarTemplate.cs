using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class BarTemplate : MonoBehaviour
{
    [SerializeField] private Image outerBar;
    [SerializeField] private Image innerBar;
    [SerializeField] private Image BGBar;

    [SerializeField] private Color defaultInnerBarColor;
    [SerializeField] private Color defaultOuterBarColor;
    [SerializeField] private RectTransform parentBar;
    [SerializeField] private float defaultWidth = 0.6f;
    [SerializeField] private float defaultHeight = 0.12f;

    private bool startWithFullBar;

    public UnityEvent OnExitBehavior;

    public void SetBar(bool startWithFullBar)
    {
        SetSize(defaultWidth, defaultHeight);

        this.startWithFullBar = startWithFullBar;

        InitializeBar(startWithFullBar);
    }

    public void SetBar(float width, float height, bool startWithFullBar)
    {
        SetSize(width, height);

        this.startWithFullBar = startWithFullBar;

        InitializeBar(startWithFullBar);
    }


    public void SetBar(float width, float height, bool startWithFullBar, Color innerColor, Color outerColor)
    {
        SetSize(width, height);
        innerBar.color = innerColor;
        outerBar.color = outerColor;

        this.startWithFullBar = startWithFullBar;

        InitializeBar(startWithFullBar);
    }


    public void SetBar(float width, float height, bool startWithFullBar, Color innerColor = default, Color outerColor = default, Color bgColor = default)
    {
        SetSize(width, height);
        innerBar.color = innerColor;
        outerBar.color = outerColor;
        BGBar.color = bgColor;

        this.startWithFullBar = startWithFullBar;

        InitializeBar(startWithFullBar);
    }


    private void SetSize(float width, float height)
    {
        parentBar.rect.Set(0f, 0f, width, height);
    }


    private void InitializeBar(bool startWithFullBar)
    {
        innerBar.fillMethod = Image.FillMethod.Horizontal;
        float initFill = startWithFullBar ? 1f : 0f;
        innerBar.fillAmount = initFill;
    }

    public DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> StartFillBar(float timeTilFull, UnityAction endAction=null)
    {
        OnExitBehavior.AddListener(endAction);
        float goalFillValue = startWithFullBar ? 0f : 1f;
        var tween = DOTween.To(() => innerBar.fillAmount, x => innerBar.fillAmount = x, goalFillValue, timeTilFull).OnComplete(()=>
        {
            OnExitBehavior?.Invoke();
            OnExitBehavior.RemoveListener(endAction);
        });
        return tween;
    }

    public DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> StartFillBar(float startBarValue, float timeTilFull, UnityAction endAction = null)
    {
        OnExitBehavior.AddListener(endAction);

        innerBar.fillAmount = startBarValue;
        float goalFillValue = startWithFullBar ? 0f : 1f;
        var tween = DOTween.To(() => innerBar.fillAmount, x => innerBar.fillAmount = x, goalFillValue, timeTilFull).OnComplete(() =>
        {
            OnExitBehavior?.Invoke();
            OnExitBehavior.RemoveListener(endAction);
        });
        return tween;
    }

    public void SetOuterColor(Color col)
    {
        outerBar.color = col;
    }

    public void SetInnerColor(Color col)
    {
        innerBar.color = col;
    }
}
