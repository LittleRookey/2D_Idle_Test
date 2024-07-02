using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private float _minuteLength => _dayLength / WorldTimeConstants.MinutesInDay;

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
        
    }


}
