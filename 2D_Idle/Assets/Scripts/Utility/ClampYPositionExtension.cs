using Cinemachine;
using UnityEngine;

public class ClampYPositionExtension : CinemachineExtension
{
    [Tooltip("The minimum Y position for the camera.")]
    public float minYPosition = -0.6f;

    [Tooltip("The maximum Y position for the camera.")]
    public float maxYPosition = 0f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            // Clamp the Y position of the camera after the body stage
            Vector3 clampedPosition = state.RawPosition;
            clampedPosition.y = Mathf.Clamp(state.RawPosition.y, minYPosition, maxYPosition);
            state.RawPosition = clampedPosition;
        }
    }
}