using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera ActiveCamera = null;

    private static CinemachineVirtualCamera PreviousCamera = null;
    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == ActiveCamera;
    }

    public static void SetBaseCamera(CinemachineVirtualCamera camera)
    {
        ActiveCamera = camera;
        camera.Priority = 10;

        foreach (CinemachineVirtualCamera c in cameras)
        {
            if (c != camera && c.Priority != 0)
            {
                c.Priority = 0;
            }
        }
    }

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        if (ActiveCamera != null) PreviousCamera = ActiveCamera;

        camera.Priority = 10;

        ActiveCamera = camera;

        foreach (CinemachineVirtualCamera c in cameras)
        {
            if (c != camera && c.Priority != 0)
            {
                c.Priority = 0;
            }
        }
    }

    public static void SwitchToPreviousCamera()
    {
        if (PreviousCamera == null)
        {
            Debug.LogError("There is no Previous Camera");
            return;
        }
        SwitchCamera(PreviousCamera);
    }

    public static void Register(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera);
        Debug.Log("Registered Camera: " + camera);
    }

    public static void Unregister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
        Debug.Log("Camera unregistered: " + camera);
    }
}
