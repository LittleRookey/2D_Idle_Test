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
            Mobile_MoveRight();
        } else if (x < 0f)
        {
            Mobile_MoveLeft();
        } else
        {
            OnStop();
        }
    }

    public void Mobile_MoveRight()
    {
        player.Turn(true);
        player.DOSmoothWalk();
    }

    public void Mobile_MoveLeft()
    {
        player.Turn(false);
        player.DOSmoothWalk();
    }

    public void OnStop()
    {
        player.DoIdle();
    }

    // Update is called once per frame
    void Update()
    {
        if (PC) GetInputPC();

    }
}
