using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    private void Awake()
    {
        Instance = this;
        
    }


}
