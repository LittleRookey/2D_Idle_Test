using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int Gold => gold;
    public int totalRunDistance;

    private int gold;
    private void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
        gold = 1000;
    }

    public void GainGold(int extraGold)
    {
        gold += extraGold;
    }

    public void UseGold(int usedGold)
    {
        gold -= usedGold;
    }

    public bool HasGold(int reqGold)
    {
        return gold >= reqGold;
    }
}
