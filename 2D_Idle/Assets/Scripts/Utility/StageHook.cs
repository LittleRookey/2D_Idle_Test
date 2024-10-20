using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHook : MonoBehaviour
{
    
    public void OpenMap()
    {
        MapManager.Instance.OpenMap();
    }
    public void CloseMap()
    {
        MapManager.Instance.CloseWindow();
    }
    public void OpenRecipeUI()
    {
        RecipeUI.Instance.OpenRecipeWindow();
    }
}
