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
        [SerializeField] private float _dayLength; // in seconds

        private TimeSpan _currentTime;
        [ShowInInspector] public float _minuteLength => _dayLength / WorldTimeConstants.MinutesInDay;

        [SerializeField] private bool stopTimer;
        WaitForSeconds waitTime;
        private void Start()
        {
            StartCoroutine(AddMinute());
            waitTime = new WaitForSeconds(_minuteLength);
        }
        private IEnumerator AddMinute()
        {
            _currentTime += TimeSpan.FromMinutes(1);
            WorldTimeChanged?.Invoke(this, _currentTime);

            yield return waitTime;
            StartCoroutine(AddMinute());
        }

        float Timer;
        private void Update()
        {
            if (stopTimer) return;

            Timer += Time.deltaTime;
            if (Timer >= _minuteLength)
            {
                _currentTime += TimeSpan.FromMinutes(1);
                WorldTimeChanged?.Invoke(this, _currentTime);
                Timer = 0f;
            }
        }

        public float PercentOfDay()
        {
            return (float)_currentTime.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
        }

        public void SetTime(TimeSpan newTime)
        {
            stopTimer = true;
            _currentTime = newTime;
            WorldTimeChanged?.Invoke(this, _currentTime);
            Timer = 0f; // Reset the timer to avoid immediate update
            stopTimer = false; 
        }

        public void SetTime(int hour, int minute, int sec)
        {
            stopTimer = true;
            _currentTime = new TimeSpan(hour, minute, sec);

            WorldTimeChanged?.Invoke(this, _currentTime);
            Timer = 0f; // Reset the timer to avoid immediate update
            stopTimer = false;
        }

        public void StartTimer() => stopTimer = false;

        public void StopTimer() => stopTimer = true;
    }


}
