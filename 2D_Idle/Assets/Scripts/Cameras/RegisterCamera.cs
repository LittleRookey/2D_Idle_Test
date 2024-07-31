using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class RegisterCamera : MonoBehaviour
{
    CinemachineVirtualCamera camera;
    public bool SetAsBaseCamera;

    public UnityEvent OnCameraActivated;

    private void Awake()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
    }
    private void OnEnable()
    {
        CameraSwitcher.Register(camera);
        // Subscribe to the OnCameraActivated event in CameraSwitcher
        CameraSwitcher.OnCameraActivated += CheckIfThisCamera;
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(camera);
        // Unsubscribe from the OnCameraActivated event in CameraSwitcher
        CameraSwitcher.OnCameraActivated -= CheckIfThisCamera;
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

    private void CheckIfThisCamera(CinemachineVirtualCamera activatedCamera)
    {
        if (activatedCamera == camera)
        {
            OnCameraActivated.Invoke();
        }
    }
}
