using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public RectTransform joyStick;
    public RectTransform joyStickBG;
    public Vector2 joystickVec;
    [SerializeField] private GameObject nw_highlight;
    [SerializeField] private GameObject ne_highlight;
    [SerializeField] private GameObject sw_highlight;
    [SerializeField] private GameObject se_highlight;

    private Vector2 joystickOriginalPos;
    [SerializeField] private float joystickRadius = 1f;
    RectTransform jt;
    RectTransform jtBG;
    Camera mainCam;

    Vector2 nextPos;
    private Vector2 pointA;
    private Vector2 pointB;
    public bool isUIOpen;

    public Vector2 movementDirection;

    public bool disalbeOnStart;
    public bool disableOnTouchRelease;

    public bool isMovingJoystick;
    void Start()
    {
        nextPos = Vector2.zero;
        mainCam = Camera.main;
        jt = joyStick.GetComponent<RectTransform>();
        jtBG = joyStickBG.GetComponent<RectTransform>();
        joystickOriginalPos = joyStickBG.anchoredPosition;
        //joystickRadius = jtBG.sizeDelta.y / joystickBGDivider;
        if (disalbeOnStart)
            joyStickBG.gameObject.SetActive(false);
    }

    Vector2 pos;
    public void OnDrag(PointerEventData eventData)
    {

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joyStickBG, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / joyStickBG.sizeDelta.x);
            pos.y = (pos.y / joyStickBG.sizeDelta.y);

            joystickVec = new Vector2(pos.x * 2, pos.y * 2);
            joystickVec = Vector2.ClampMagnitude(joystickVec, joystickRadius);
                //(joystickVec.magnitude > 1.0f) ? joystickVec.normalized : joystickVec;

            // Move the joystick handle
            joyStick.anchoredPosition = new Vector2(joystickVec.x * (joyStickBG.sizeDelta.x / 3), joystickVec.y * (joyStickBG.sizeDelta.y / 3));

            if (joystickVec.x > 0 && joystickVec.y > 0)
            {
                ne_highlight.gameObject.SetActive(true);
                nw_highlight.gameObject.SetActive(false);
                se_highlight.gameObject.SetActive(false);
                sw_highlight.gameObject.SetActive(false);
            }
            else if (joystickVec.x < 0 && joystickVec.y > 0)
            {
                nw_highlight.gameObject.SetActive(true);
                ne_highlight.gameObject.SetActive(false);
                se_highlight.gameObject.SetActive(false);
                sw_highlight.gameObject.SetActive(false);
            }
            else if (joystickVec.x < 0 && joystickVec.y < 0)
            {
                sw_highlight.gameObject.SetActive(true);
                nw_highlight.gameObject.SetActive(false);
                ne_highlight.gameObject.SetActive(false);
                se_highlight.gameObject.SetActive(false);
            }
            else if (joystickVec.x > 0 && joystickVec.y < 0)
            {
                se_highlight.gameObject.SetActive(true);
                nw_highlight.gameObject.SetActive(false);
                ne_highlight.gameObject.SetActive(false);
                sw_highlight.gameObject.SetActive(false);
            }
            else
            {
                se_highlight.gameObject.SetActive(false);
                nw_highlight.gameObject.SetActive(false);
                ne_highlight.gameObject.SetActive(false);
                sw_highlight.gameObject.SetActive(false);
            }
        }

      
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (isUIOpen) return;
        //if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId))
        //{
        //    return; // Do nothing if the touch is over a UI element
        //}
        se_highlight.gameObject.SetActive(false);
        nw_highlight.gameObject.SetActive(false);
        ne_highlight.gameObject.SetActive(false);
        sw_highlight.gameObject.SetActive(false);

        if (disableOnTouchRelease)
            joyStickBG.gameObject.SetActive(false);

        joystickVec = Vector2.zero;
        movementDirection = Vector2.zero;
        joyStick.anchoredPosition = Vector2.zero;
        joyStickBG.anchoredPosition = joystickOriginalPos;
        isMovingJoystick = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (isUIOpen) return;
        //if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId))
        //{
        //    return; // Do nothing if the touch is over a UI element
        //}

        joyStickBG.anchoredPosition = Vector2.zero;
        joyStick.anchoredPosition = joyStickBG.anchoredPosition;
        isMovingJoystick = true;
        joyStickBG.gameObject.SetActive(true);
    }
}
