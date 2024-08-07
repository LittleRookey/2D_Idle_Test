using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ClickableText : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var text = GetComponent<TextMeshProUGUI>();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
            if (linkIndex > -1)
            {
                var linkInfo = text.textInfo.linkInfo[linkIndex];
                var linkID = linkInfo.GetLinkID(); // returns item name or type's name

                // TODO show linkID's explanation

            }
        }
    }
}
