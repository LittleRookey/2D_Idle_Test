using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace WorldTime
{

    public static class WorldTimeConstants
    {
        public const int MinutesInDay = 1440;

    }

    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;

        [SerializeField, Range(0f, 1f)]
        private float _percentOfDay;

        [ShowInInspector]
        public float PercentOfDay
        {
            get => _percentOfDay;
            set
            {
                _percentOfDay = Mathf.Clamp01(value);
                UpdateCurrentTime();
            }
        }

        [SerializeField] private float _dayLength = 24f * 60f; // day length in seconds
        [ShowInInspector] public float MinuteLength => _dayLength / WorldTimeConstants.MinutesInDay;

        [SerializeField] private bool _stopTimer;
        private TimeSpan _currentTime;

        private void OnValidate()
        {
            // This ensures changes in the inspector are immediately reflected
            PercentOfDay = _percentOfDay;
        }

        private void Start()
        {
            UpdateCurrentTime();
        }

        private void Update()
        {
            if (_stopTimer) return;

            _percentOfDay += Time.deltaTime / _dayLength;
            if (_percentOfDay >= 1f)
            {
                _percentOfDay -= 1f;
            }

            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            int totalMinutes = Mathf.FloorToInt(_percentOfDay * WorldTimeConstants.MinutesInDay);
            _currentTime = TimeSpan.FromMinutes(totalMinutes);
            WorldTimeChanged?.Invoke(this, _currentTime);
        }

        public void SetTime(TimeSpan newTime)
        {
            _stopTimer = true;
            PercentOfDay = (float)newTime.TotalMinutes / WorldTimeConstants.MinutesInDay;
            _stopTimer = false;
        }

        public void SetTime(int hour, int minute, int second)
        {
            SetTime(new TimeSpan(hour, minute, second));
        }

        public void StartTimer() => _stopTimer = false;
        public void StopTimer() => _stopTimer = true;
    }

}
