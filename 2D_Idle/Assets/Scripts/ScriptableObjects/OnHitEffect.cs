using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitEffect", menuName = "Litkey/HitEffect")]
public class OnHitEffect : ScriptableObject
{
    [SerializeField] private float onHitTimeScale = 0.5f;
    [SerializeField] private float onHitEventRunTime = 0.1f;
    [SerializeField] private float cameraShakeIntensity = 1.0f;

    public void RunEvent()
    {
        //CinematicSetting.Instance.ShakeCamera();


        Time.timeScale = onHitTimeScale;
        Time.fixedDeltaTime = onHitTimeScale * 0.02f; // adjust deltatime accordingly
        
        // reset time scale after a short delay

    }

    
}

