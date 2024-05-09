using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CinematicSetting : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera characterCamera;
    [SerializeField] private CinemachineVirtualCamera characterDeathCamera;

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

}
