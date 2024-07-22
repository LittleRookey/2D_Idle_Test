using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private Shop shop;

    public void SetShop()
    {
        ShopUI.Instance.SetShop(shop);
    }
}
