using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeAdjust : MonoBehaviour
{
    private void OnEnable()
    {
        ScaleUIElement();

    }
    void ScaleUIElement()
    {
        // Get the current resolution
        Resolution currentResolution = Screen.currentResolution;

        // Calculate the desired size for the UI element
        float desiredWidth = currentResolution.width * 2 + 100;
        float desiredHeight = currentResolution.height * 2 + 100;

        // Set the size of the UI element
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(desiredWidth, desiredHeight);
    }
}
