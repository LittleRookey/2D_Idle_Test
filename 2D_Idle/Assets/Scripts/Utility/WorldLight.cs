using FunkyCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace WorldTime
{
    public class WorldLight : MonoBehaviour
    {
        [SerializeField] private LightCycle _light;


        [SerializeField] private WorldTime _worldTime;

        //[SerializeField] private Gradient _gradient;

        private void Awake()
        {
            _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan e)
        {
            _light.SetTime(PercentOfDay(e));
        }

        public float PercentOfDay(TimeSpan timeSpan)
        {
            return (float)timeSpan.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
        }
        private void OnDestroy()
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }

        //private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        //{
        //    _light.color = _gradient.Evaluate(PercentOfDay(newTime));
        //}


        

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            _light.SetTime(_worldTime.PercentOfDay());
        }
    }
}
