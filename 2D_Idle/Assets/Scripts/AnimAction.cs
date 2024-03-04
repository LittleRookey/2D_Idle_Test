using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAction : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void AttackAction()
    {
        playerController.DamageAction();
    }
}
