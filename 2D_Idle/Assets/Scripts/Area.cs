using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    public string areaName;
    public eRegion region;
    public MonsterTable monsterTable;

    public UnityAction<Area> OnPlayerEnterArea;
    public bool disableOnStart;

    public UnityEvent OnEnterArea;
    private int enterCounter = 0;
    private void Awake()
    {
        if (disableOnStart) enterCounter = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMarker"))
        {
            enterCounter++;
            //if (enterCounter <= 1 && disableOnStart) return;

            Debug.Log("OnTriggerEnter Area");
            OnPlayerEnterArea?.Invoke(this);
            OnEnterArea?.Invoke();
        }
    }
    private void OnValidate()
    {
        areaName = gameObject.name;
    }

}
