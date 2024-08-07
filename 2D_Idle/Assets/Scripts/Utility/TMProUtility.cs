using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
    public static class TMProUtility
    {
        public static string GetColorText(string returnText, Color col)
        {

            return $"<color=#{ColorUtility.ToHtmlStringRGBA(col)}>{returnText}</color>";
        }

        public static string GetLinkText(string displayText, string linkID)
        {
            return $"<link=\"{linkID}\">{displayText}</link>";
        }
    }

}