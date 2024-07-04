using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldTime;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace WorldTime
{

    public class WorldTimeDisplay : MonoBehaviour
    {
        [SerializeField] private WorldTime _worldTime;

        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Sprite sun;
        [SerializeField] private Sprite moon;
        [SerializeField] private Image dayNightIcon;
        [SerializeField] private DOTweenAnimation OnSpriteChangedAnim;
        [SerializeField] private HorizontalLayoutGroup horGroup;
        private void Awake()
        {
            _worldTime.WorldTimeChanged += OnWorldTimeChanged;
            OnSpriteChangedAnim.tween.onPlay += () => horGroup.enabled = false;
            OnSpriteChangedAnim.tween.onComplete += () => horGroup.enabled = true;
        }

        private void OnDestroy()
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            _timeText.SetText(newTime.ToString(@"hh\:mm"));
        }

        public void Day()
        {
            dayNightIcon.sprite = sun;
            OnSpriteChangedAnim.DORestart();
        }

        public void Night()
        {
            dayNightIcon.sprite = moon;
            OnSpriteChangedAnim.DORestart();
        }
    }

}