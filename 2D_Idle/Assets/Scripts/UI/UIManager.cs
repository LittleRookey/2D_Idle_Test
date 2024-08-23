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
    [SerializeField] private Canvas Menus; 
    bool isOn = true;
    [SerializeField] private Canvas skillCooldownCanvas;
    private void Awake()
    {
        Instance = this;
        HideSkillCooldownCanvas();
        ShowMenus();
    }

    public void ShowSkillCooldownCanvas() => skillCooldownCanvas.enabled = true;
    public void HideSkillCooldownCanvas() => skillCooldownCanvas.enabled = false;
    public void HideMenus() => Menus.enabled = false;

    public void ShowMenus() => Menus.enabled = true;


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
