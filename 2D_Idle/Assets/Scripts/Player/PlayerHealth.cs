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
        _statContainer.HP.OnValueChanged.AddListener(UpdateMaxHealth);
        playerController.OnRevive.AddListener(RefillToMaxHealth);

    }

    protected override void OnDisable()
    {
        _statContainer.HP.OnValueChanged.RemoveListener(UpdateMaxHealth);
        playerController.OnRevive.RemoveListener(RefillToMaxHealth);

    }

}
