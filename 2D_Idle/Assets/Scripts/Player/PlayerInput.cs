using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool PC;
    public bool Mobile;
    public bool TopDown;

    private PlayerController player;
    private TopdownPlayerController playerTopdown;

    [SerializeField] private MovementJoystick joystick;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerTopdown = GetComponent<TopdownPlayerController>();
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

    public void MoveJoyStick()
    {
        if (joystick == null)
        {
            Debug.LogError("Joystick not set up");
        }
        playerTopdown.moveDir = joystick.joystickVec;
        if (playerTopdown.moveDir == Vector2.zero)
        {
            playerTopdown.DoIdle();
            return;
        }
        playerTopdown.RunWithNoTarget();
        playerTopdown.DOSmoothRun();
    }

    // Update is called once per frame
    void Update()
    {
        if (PC) GetInputPC();

        if (TopDown) MoveJoyStick();

    }
}
