using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerController.OnRevive.AddListener(RefillToMaxHealth);

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerController.OnRevive.RemoveListener(RefillToMaxHealth);

    }

}
