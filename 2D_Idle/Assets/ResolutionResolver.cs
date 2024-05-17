using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionResolver : MonoBehaviour
{
    public Vector2Int resolution = new Vector2Int(1200, 1920);
    public int baseRefreshRate = 60;
    private void Awake()
    {
        //Screen.SetResolution(resolution.x, resolution.y, FullScreenMode.);
    }

}
