using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RegisterCamera : MonoBehaviour
{
    CinemachineVirtualCamera camera;
    public bool SetAsBaseCamera;

    private void Awake()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
    }
    private void OnEnable()
    {
        CameraSwitcher.Register(camera);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(camera);
    }

    private void Start()
    {
        if (SetAsBaseCamera) CameraSwitcher.SetBaseCamera(camera);
    }
    public void SwitchToCamera()
    {
        Debug.Log($"Switched to Camera: {camera} and is active camera? {CameraSwitcher.IsActiveCamera(camera)}");
        if (CameraSwitcher.IsActiveCamera(camera))
        {
            CameraSwitcher.SwitchToPreviousCamera();
        }
        else
        {
            CameraSwitcher.SwitchCamera(camera);
        }
    }
}
