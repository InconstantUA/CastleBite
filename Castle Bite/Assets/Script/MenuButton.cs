﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text txt;
    Button btn;
    bool interactableDelay;
    Color tmpColor;
    public enum States
    {
        Normal = 1,
        Highlighted = 2,
        Pressed = 4,
        Disabled = 8,
        Unknown = 16
    }
    public States currentState;

    void Start()
    {
        // init text object
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
        // use the same color as set for button normal color
        // otherwise we will have to remember to change both values
        // because at the game start text will not have the same color as button
        // Update: if we do this, then it causes Start button not to be highlighted at the start
        // so it cannot be used by keyboard
        // tmpColor = btn.colors.normalColor;
        // tmpColor.a = 1;
        // txt.color = tmpColor;
        currentState = States.Unknown;
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            // highlight button if it was highlighted before
            if (currentState == States.Highlighted)
            {
                DimmAllOtherMenus();
                SetHighlightedStatus();
            }
        }
    }

    void SetHighlightedStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.highlightedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        Debug.Log("SetHighlightedStatus " + btn.name + " button");
    }

    void SetPressedStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    void DimmAllOtherMenus()
    {
        // to make it easier dimm just all menus
        foreach (Button otherButton in transform.parent.GetComponentsInChildren<Button>())
        {
            if (otherButton.interactable)
            {
                tmpColor = otherButton.colors.normalColor;
            }
            else
            {
                tmpColor = otherButton.colors.disabledColor;
            }
            tmpColor.a = 1;
            otherButton.GetComponentInChildren<Text>().color = tmpColor;
            // Debug.Log("dimm " + otherButton.name + " button");
        }
        Debug.Log("DimmAllOtherMenus");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        // set state
        currentState = States.Highlighted;
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        // set state
        currentState = States.Pressed;
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        currentState = States.Highlighted;
        SetHighlightedStatus();
        ActOnClick();
    }


    #region OnClick

    Button[] menuBtnsList; // required for MenuKeyboardControl.cs

    void ActOnClick()
    {
        // act depending on the parent menu
        string currActiveMenuName = btn.transform.parent.name;
        switch (currActiveMenuName)
        {
            case "MainMenuPanel":
                OnMainMenuClick();
                break;
            case "OptionsSubmenuL2Panel":
                OnOptionsSubmenuL2Click();
                break;
            case "OptionsGameSubmenuL3Panel":
                OptionsGameSubmenuL3PanelClick();
                break;
            case "OptionsVideoSubmenuL3Panel":
                OptionsVideoSubmenuL3PanelClick();
                break;
            case "OptionsAudioSubmenuL3Panel":
                OptionsAudioSubmenuL3PanelClick();
                break;
            case "OptionsKeyboardAndMouseSubmenuL3Panel":
                OptionsKeyboardAndMouseSubmenuL3PanelClick();
                break;
            default:
                Debug.Log("Error: unknown selected menu name [" + currActiveMenuName + "]");
                break;
        }
    }

    void ResetAllMenuButtonsToNormal()
    {
        foreach (Button tmpBtn in menuBtnsList)
        {
            if (tmpBtn.interactable)
            {
                tmpColor = tmpBtn.colors.normalColor;
            }
            else
            {
                tmpColor = tmpBtn.colors.disabledColor;
            }
            tmpColor.a = 1;
            tmpBtn.GetComponentInChildren<Text>().color = tmpColor;
            // Debug.Log("dimm " + tmpBtn.name + "button");
        }

    }

    void SetActiveMenuTo(string mName, int btnID = -1)
    {
        // Disable old menu and activate new menu
        btn.transform.parent.gameObject.SetActive(false);
        GameObject newMenu = btn.transform.root.Find("MainMenu").Find(mName).gameObject;
        newMenu.SetActive(true);
        // Update list of all buttons in the menu - required for keyboard control script
        // This is also required here because of the active menu change
        menuBtnsList = newMenu.GetComponentsInChildren<Button>();
    }

    void OnMainMenuClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "Start":
                StartGame();
                break;
            case "Options":
                SetActiveMenuTo("OptionsSubmenuL2Panel", 1);
                break;
            case "Quit":
                Application.Quit();
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OnMainMenuClick on " + selectedMBtnName + " button");
    }

    void StartGame()
    {
        // Activate Game canvas and deactivate menu canvas
        transform.root.Find("MainMenu").gameObject.SetActive(false);
        transform.root.Find("Game").gameObject.SetActive(true);
    }

    void OnOptionsSubmenuL2Click()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheMainMenu":
                SetActiveMenuTo("MainMenuPanel");
                break;
            case "Game":
                SetActiveMenuTo("OptionsGameSubmenuL3Panel", 1);
                break;
            case "Video":
                SetActiveMenuTo("OptionsVideoSubmenuL3Panel", 1);
                break;
            case "Audio":
                SetActiveMenuTo("OptionsAudioSubmenuL3Panel", 1);
                break;
            case "KeyboardAndMouse":
                SetActiveMenuTo("OptionsKeyboardAndMouseSubmenuL3Panel", 1);
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OnOptionsSubmenuL2Click on " + selectedMBtnName + " button");
    }

    void OptionsGameSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "Autosave":
                // Revert autosave setting on click
                Text btnTxt = btn.GetComponentsInChildren<Text>()[1];
                if (btnTxt.text == "Off")
                {
                    btnTxt.text = "On";
                }
                else
                {
                    btnTxt.text = "Off";
                }
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OptionsGameSubmenuL3PanelClick on " + selectedMBtnName + " button");
    }

    void OptionsVideoSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "FontSize":
                // it does not react on enter or return keyboard presses
                // instead it react on keyboard -> <- keys
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OptionsVideoSubmenuL3PanelClick on " + selectedMBtnName + " button");
    }
    void OptionsAudioSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "MusicVolume":
                // it does not react on enter or return keyboard presses
                // instead it react on keyboard -> <- keys
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OptionsAudioSubmenuL3PanelClick on " + selectedMBtnName + " button");
    }

    void OptionsKeyboardAndMouseSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "DoCustomAction":
                // Change key binding
                break;
            case "SaveAndReturn":
                // Save
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "ResetToDefault":
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        Debug.Log("OptionsKeyboardAndMouseSubmenuL3PanelClick on " + selectedMBtnName + " button");
    }

    #endregion OnClick

    public void OnPointerExit(PointerEventData eventData)
    {
        // set state
        currentState = States.Normal;
        //if (btn.interactable)
        //{
        //    tmpColor = btn.colors.normalColor;
        //}
        //else
        //{
        //    tmpColor = btn.colors.disabledColor;
        //}
        //tmpColor.a = 1;
        //txt.color = tmpColor;
    }
}