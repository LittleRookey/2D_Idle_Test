using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CinematicSetting : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera characterCamera;
    [SerializeField] private CinemachineVirtualCamera characterDeathCamera;

    private float shakeTimerTotal;
    public void OnDeath()
    {
        characterCamera.Priority = 10;
        characterDeathCamera.Priority = 11;
    }

    public void OnRevive()
    {
        characterCamera.Priority = 11;
        characterDeathCamera.Priority = 10;
    }

    [Button("ShakeCamera")]
    public void ShakeCamera()
    {
        ShakeCamera(1f, 0.1f);
    }
    private float shakeTimer;
    private float startingIntensity;
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            characterCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
        startingIntensity = intensity;
        shakeTimerTotal = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                // time over
                CinemachineBasicMultiChannelPerlin cinemachinePerlin = characterCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachinePerlin.m_AmplitudeGain = 0f;
                Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }

}
