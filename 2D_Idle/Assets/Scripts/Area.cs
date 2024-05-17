using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;

public class Area : MonoBehaviour
{
    public string areaName;
    public eRegion region;
    public MonsterTable monsterTable;

    public UnityAction<Area> OnPlayerEnterArea;
    public bool disableOnStart;
    [SerializeField] public UILineRenderer uiLineRenderer;

    public UnityEvent OnEnterArea;
    private int enterCounter = 0;
    private void Awake()
    {
        if (disableOnStart) enterCounter = 0;
        if (uiLineRenderer == null) uiLineRenderer = GetComponentInChildren<UILineRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMarker"))
        {
            enterCounter++;
            //if (enterCounter <= 1 && disableOnStart) return;

            //Debug.Log("OnTriggerEnter Area");
            OnPlayerEnterArea?.Invoke(this);
            OnEnterArea?.Invoke();
        }
    }

    public void TurnOutlineOn()
    {
        if(uiLineRenderer == null) uiLineRenderer = GetComponentInChildren<UILineRenderer>();
        Debug.Log("UILineRenderer: " + uiLineRenderer + "in area: " + areaName);
        if (uiLineRenderer == null) return;

        uiLineRenderer.gameObject.SetActive(true);
    }
    public void TurnOutlineOff()
    {
        if (uiLineRenderer == null) uiLineRenderer = GetComponentInChildren<UILineRenderer>();

        if (uiLineRenderer == null) return;
        uiLineRenderer.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        areaName = gameObject.name;
    }

}
