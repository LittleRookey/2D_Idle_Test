using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private RectTransform joystick;
    [SerializeField] private RectTransform auto;
    [SerializeField] private RectTransform interaction;
    [SerializeField] private RectTransform menusUI;
    [SerializeField] private RectTransform timeUI;
    [SerializeField] private RectTransform playerInfo;
    [SerializeField] private RectTransform goldInfo;

    bool isOn = true;

    private void Awake()
    {
        Instance = this;
    }
    public void DisableUIs()
    {
        isOn = false;
        joystick.gameObject.SetActive(isOn);
        auto.gameObject.SetActive(isOn);
        interaction.gameObject.SetActive(isOn);
        menusUI.gameObject.SetActive(isOn);
        timeUI.gameObject.SetActive(isOn);
        playerInfo.gameObject.SetActive(isOn);
        goldInfo.gameObject.SetActive(isOn);
    }

    public void EnableUIs()
    {
        isOn = true;
        joystick.gameObject.SetActive(isOn);
        auto.gameObject.SetActive(isOn);
        interaction.gameObject.SetActive(isOn);
        menusUI.gameObject.SetActive(isOn);
        timeUI.gameObject.SetActive(isOn);
        playerInfo.gameObject.SetActive(isOn);
        goldInfo.gameObject.SetActive(isOn);
    }

    public void ToggleUIOnOff()
    {
        isOn = !isOn;
        joystick.gameObject.SetActive(isOn);
        auto.gameObject.SetActive(isOn);
        interaction.gameObject.SetActive(isOn);
        menusUI.gameObject.SetActive(isOn);
        timeUI.gameObject.SetActive(isOn);
        playerInfo.gameObject.SetActive(isOn);
        goldInfo.gameObject.SetActive(isOn);
    }

}
