using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool PC;
    public bool Mobile;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    private void GetInputPC()
    {
        var x = Input.GetAxis("Horizontal");

        if (x > 0f)
        {
            player.Turn(true);
            player.DOSmoothWalk();
        } else if (x < 0f)
        {
            player.Turn(false);
            player.DOSmoothWalk();
        } else
        {
            player.DoIdle();
        }
    }

    private void Mobile_MoveRight()
    {
        player.Turn(true);
        player.DOSmoothWalk();
    }

    // Update is called once per frame
    void Update()
    {
        if (PC) GetInputPC();

    }
}
