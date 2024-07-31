using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class UIChildConnector : MonoBehaviour
{
    public RectTransform parentObject;
    private List<RectTransform> childObjects = new List<RectTransform>();
    [SerializeField] private UILineRenderer lineRenderer;
    [SerializeField] private float radius = 1.5f;
    void Start()
    {
        lineRenderer.color = Color.white;
    }
    [Button("UpdateLines")]
    public void ConnectLines()
    {
        childObjects.Clear();
        foreach (RectTransform child in parentObject)
        {
            if (child.gameObject.activeInHierarchy)
                childObjects.Add(child);
        }

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < childObjects.Count - 1; i++)
        {
            Vector2 startPoint = GetEdgePoint(childObjects[i], childObjects[i + 1]);
            Vector2 endPoint = GetEdgePoint(childObjects[i + 1], childObjects[i]);

            points.Add(startPoint);
            points.Add(endPoint);
        }

        lineRenderer.Points = points.ToArray();
    }

    private Vector2 GetEdgePoint(RectTransform from, RectTransform to)
    {
        Vector2 direction = (to.anchoredPosition - from.anchoredPosition).normalized;
        //float radius = GetImageRadius(from);
        return from.anchoredPosition + direction * radius;
    }

    private float GetImageRadius(RectTransform rectTransform)
    {
        Image image = rectTransform.GetComponent<Image>();
        if (image != null)
        {
            return Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / 2f;
        }
        return 0f;
    }
}