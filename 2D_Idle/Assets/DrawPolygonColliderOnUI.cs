using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class DrawPolygonColliderOnUI : MonoBehaviour
{
    public PolygonCollider2D polygonCollider;
    public UILineRenderer lineRenderer;
    public float mapScale = 0.04f;

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        DrawPolygonOnUI();
//    }

//#endif
    //private void DrawPolygonOnUI()
    //{
    //    // Get the points of the PolygonCollider2D in world space
    //    Vector2[] worldSpacePoints = polygonCollider.GetPath(0);

    //    // Convert world space points to screen space points
    //    Vector3[] screenSpacePoints = new Vector3[worldSpacePoints.Length];
    //    for (int i = 0; i < worldSpacePoints.Length; i++)
    //    {
    //        screenSpacePoints[i] = Camera.main.WorldToScreenPoint(worldSpacePoints[i]);
    //    }

    //    // Scale the screen space points based on the map scale
    //    for (int i = 0; i < screenSpacePoints.Length; i++)
    //    {
    //        screenSpacePoints[i] /= mapScale;
    //    }

    //    // Set the positions of the LineRenderer
    //    lineRenderer.positionCount = screenSpacePoints.Length;
    //    lineRenderer.SetPositions(screenSpacePoints);
    //}
    [Button("DrarLine")]
    public void DrawPolygonOnUI()
    {
        Vector2[] normalizedPoints = polygonCollider.GetPath(0);
        for (int i = 0; i < normalizedPoints.Length; i++)
        {
            normalizedPoints[i] = NormalizedToUISpace(normalizedPoints[i]);
        }

        // Set the points of the UILineRenderer
        lineRenderer.Points = normalizedPoints;
    }

    private Vector2 NormalizedToUISpace(Vector2 normalizedPosition)
    {
        Vector2 uiPosition = new Vector2(
            (normalizedPosition.x * lineRenderer.rectTransform.sizeDelta.x * mapScale) - (lineRenderer.rectTransform.sizeDelta.x * 0.5f * mapScale),
            (normalizedPosition.y * lineRenderer.rectTransform.sizeDelta.y * mapScale) - (lineRenderer.rectTransform.sizeDelta.y * 0.5f * mapScale));

        return uiPosition;
    }
}
