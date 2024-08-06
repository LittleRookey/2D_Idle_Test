using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class StageTitleUI : MonoBehaviour
{
    public static StageTitleUI Instance;
    [SerializeField] private Transform TitleBGOrientation;
    [SerializeField] private Image topImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private DOTweenAnimation titleTextDotween;
    private Color transparent;
    public bool disableOnStart;
    private void Awake()
    {
        Instance = this;
        transparent = new Color(0f, 0f, 0f, 0f);
        if (disableOnStart) TitleBGOrientation.gameObject.SetActive(false);
    }

    [Button("SetStage")]
    public void SetStageTitleUI(string stageName)
    {
        //topImage.color = transparent;
        //titleText.color = transparent;
        TitleBGOrientation.gameObject.SetActive(true);
        //// Fade In
        //topImage.DOFade(255f, 1.5f);
        titleText.SetText(stageName);
        titleTextDotween.DORestartById("FadeIn");
        //titleText.DOFade(255f, 1.5f);

        //bottomImage.onComplete.RemoveAllListeners();

        //// fade out
        //bottomImage.onComplete.AddListener(() => bottomImage.DORestartById("Exit"));


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
