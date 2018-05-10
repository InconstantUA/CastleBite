using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuKeyboardControl : MonoBehaviour {
    Button btn; // currently selected button
    int currSelctdBtnID;
    int previouslySelectedMenuID;
    Color tmpColor;

    // Use this for initialization
    void Start() {
        // Init list of all buttons in the menu
        GameObject mainMenu = transform.root.Find("MainMenuPanel").gameObject;
        menuBtnsList = mainMenu.GetComponentsInChildren<Button>();
        // pre-select first element in the menu
        currSelctdBtnID = 0;
        previouslySelectedMenuID = 0;
        HighlightSelectedMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) == true)
        {
            // switch to the other menu
            // audio.PlayOneShot(blip);
            // Disable cursor, to not conflict with keyboard input
            // If cursor was enabled, then we should verify if highlighted menu has not changed
            // and we cannot rely on previoiusly set previouslySelectedMenuID, 
            // that is why we each time get it from scratch using GetCurrentlySelectedBtnID();
            Cursor.visible = false;
            currSelctdBtnID = GetCurrentlySelectedBtnID();
            previouslySelectedMenuID = currSelctdBtnID;
            if (currSelctdBtnID < menuBtnsList.Length - 1)
            {
                currSelctdBtnID += 1;
            }
            else
            {
                currSelctdBtnID = 0;
            }
            DimmPreviouslySelectedMenu();
            HighlightSelectedMenu();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) == true)
        {
            // switch to the other menu
            // audio.PlayOneShot(blip);
            Cursor.visible = false;
            currSelctdBtnID = GetCurrentlySelectedBtnID();
            previouslySelectedMenuID = currSelctdBtnID;
            if (currSelctdBtnID > 0)
            {
                currSelctdBtnID -= 1;
            }
            else
            {
                currSelctdBtnID = menuBtnsList.Length - 1;
            }
            DimmPreviouslySelectedMenu();
            HighlightSelectedMenu();
        }
        if (Input.GetKeyDown(KeyCode.Return) == true)
        {
            currSelctdBtnID = GetCurrentlySelectedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            SetPressedStatus();
        }
        if (Input.GetKeyUp(KeyCode.Return) == true)
        {
            // update btn object, which later will be used in act on click
            currSelctdBtnID = GetCurrentlyPressedOrHighlightedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            // release color back to highlighted
            HighlightSelectedMenu();
            ActOnClick();
        }
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
        GameObject newMenu = btn.transform.root.Find(mName).gameObject;
        newMenu.SetActive(true);
        // Update list of all buttons in the menu - required for keyboard control script
        menuBtnsList = newMenu.GetComponentsInChildren<Button>();
        #region Keyboard-specific
        // Set currently selected button id and highligh it
        if ((btnID >= 0) && (btnID < menuBtnsList.Length))
        {
            // reset all menus, because they might be still highlighed from previous activity
            ResetAllMenuButtonsToNormal();
            currSelctdBtnID = btnID;
        }
        else
        {
            // If btnID is not provided, or it is invalid then keep previously pressed button
            currSelctdBtnID = GetCurrentlySelectedBtnID();
        }
        HighlightSelectedMenu();
        Debug.Log("SetActiveMenuTo " + mName + " and button id to " + currSelctdBtnID);
        #endregion Keyboard-specific
    }

    void OnMainMenuClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "Start":
                SceneManager.LoadScene(1);
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

    void OnOptionsSubmenuL2Click()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheMainMenu":
                SetActiveMenuTo("MainMenuPanel");
                //// disable Options sub-menu and activate Main menu
                //transform.parent.gameObject.SetActive(false);
                //transform.root.Find("MainMenuPanel").gameObject.SetActive(true);
                break;
            case "Game":
                SetActiveMenuTo("OptionsGameSubmenuL3Panel", 1);
                break;
            case "Video":
                // activate Video sub-options
                break;
            case "Audio":
                // activate Audio sub-options
                break;
            case "KeyBindings":
                // activate KeyBindings sub-options
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
                //// disable Options sub-menu and activate Main menu
                //transform.parent.gameObject.SetActive(false);
                //transform.root.Find("MainMenuPanel").gameObject.SetActive(true);
                break;
            case "Autosave":
                // Revert autosave setting on click
                Text btnTxt = btn.GetComponentsInChildren<Text>()[1];
                if (btnTxt.text == "Off")
                {
                    btnTxt.text = "On";
                } else
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

    #endregion OnClick

    void SetPressedStatus()
    {
        if (menuBtnsList[currSelctdBtnID].interactable)
        {
            tmpColor = menuBtnsList[currSelctdBtnID].colors.pressedColor;
        }
        else
        {
            tmpColor = menuBtnsList[currSelctdBtnID].colors.disabledColor;
        }
        tmpColor.a = 1;
        menuBtnsList[currSelctdBtnID].GetComponentInChildren<Text>().color = tmpColor;
        Debug.Log("SetPressedStatus for id " + currSelctdBtnID + " name " + menuBtnsList[currSelctdBtnID].name);
    }

    //// take action according to the menu
    //Cursor.visible = false;
    //        currSelctdBtnID = GetCurrentlySelectedBtnID();
    //string selectedMBtnName = menuBtnsList[currSelctdBtnID].name;
    //        switch(selectedMBtnName)
    //        {
    //            case "Start":
    //                SceneManager.LoadScene(1);
    //                break;
    //            case "Quit":
    //                Application.Quit();
    //                break;
    //            default:
    //                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
    //                break;
    //        }

    int GetCurrentlySelectedBtnID()
    {
        // it is possible that id has changed, because there was mouse activity
        // that is why I need to get active menu id set by mouse and verify if it is the same as for keyboard
        int selectedBtnID = -1;
        // to get currently highlighted menu we compare its text color with highlighted color
        for (int id=0; id < menuBtnsList.Length; id++)
        {
            // do not compare alpha (transparancy) because it is different, because we set it to 1(255)
            Debug.Log("Comparing " + menuBtnsList[id] + " menu");
            Color btnHClr = menuBtnsList[id].colors.highlightedColor;
            Color txtClr = menuBtnsList[id].GetComponentInChildren<Text>().color;
            if (((int)(txtClr.r * 1000) == (int)(btnHClr.r * 1000)) || ((int)(txtClr.g * 1000) == (int)(btnHClr.g * 1000)) || ((int)(txtClr.b * 1000) == (int)(btnHClr.b * 1000)))
            {
                // found id
                Debug.Log("Currently highlighted button is " + menuBtnsList[id].name);
                selectedBtnID = id;
                break;
            }
        }
        // Verfiy if we found the button id
        if (selectedBtnID < 0)
        {
            // not found
            // reset to 0
            selectedBtnID = 0;
        }
        Debug.Log("GetCurrentlySelectedBtnID is " + selectedBtnID);
        return selectedBtnID;
    }

    int GetCurrentlyPressedOrHighlightedBtnID()
    {
        // it is possible that id has changed, because there was mouse activity
        // that is why I need to get active menu id set by mouse and verify if it is the same as for keyboard
        int pressedBtnID = -1;
        // to get currently highlighted menu we compare its text color with highlighted color
        for (int id = 0; id < menuBtnsList.Length; id++)
        {
            // do not compare alpha (transparancy) because it is different, because we set it to 1(255)
            Debug.Log("Comparing " + menuBtnsList[id] + " menu");
            Color btnHClr = menuBtnsList[id].colors.highlightedColor;
            Color btnPClr = menuBtnsList[id].colors.pressedColor;
            Color txtClr = menuBtnsList[id].GetComponentInChildren<Text>().color;
            if ((((int)(txtClr.r * 1000) == (int)(btnPClr.r * 1000)) && ((int)(txtClr.g * 1000) == (int)(btnPClr.g * 1000)) && ((int)(txtClr.b * 1000) == (int)(btnPClr.b * 1000)))
             || (((int)(txtClr.r * 1000) == (int)(btnHClr.r * 1000)) && ((int)(txtClr.g * 1000) == (int)(btnHClr.g * 1000)) && ((int)(txtClr.b * 1000) == (int)(btnHClr.b * 1000))) )
            {
                // found id
                Debug.Log("Currently highlighted button is " + menuBtnsList[id].name);
                pressedBtnID = id;
                break;
            }
        }
        // Verfiy if we found the button id
        if (pressedBtnID < 0)
        {
            // not found
            // reset to 0
            pressedBtnID = 0;
        }
        Debug.Log("GetCurrentlyPressedOrHighlightedBtnID is " + pressedBtnID);
        return pressedBtnID;
    }

    void DimmPreviouslySelectedMenu()
    {
        // Remove highlight for previously selected menu
        if (menuBtnsList[previouslySelectedMenuID].interactable)
        {
            tmpColor = menuBtnsList[previouslySelectedMenuID].colors.normalColor;
        }
        else
        {
            tmpColor = menuBtnsList[previouslySelectedMenuID].colors.disabledColor;
        }
        tmpColor.a = 1;
        menuBtnsList[previouslySelectedMenuID].GetComponentInChildren<Text>().color = tmpColor;
        Debug.Log("Dimm button " + menuBtnsList[previouslySelectedMenuID].name);
    }

    void HighlightSelectedMenu()
    {
        // Highlight newly selected menu
        Debug.Log("id " + currSelctdBtnID);
        foreach(Button dbg_btn in menuBtnsList)
        {
            Debug.Log("btn name " + dbg_btn.name);
        }
        if (menuBtnsList[currSelctdBtnID].interactable)
        {
            tmpColor = menuBtnsList[currSelctdBtnID].colors.highlightedColor;
        }
        else
        {
            tmpColor = menuBtnsList[currSelctdBtnID].colors.disabledColor;
        }
        tmpColor.a = 1;
        menuBtnsList[currSelctdBtnID].GetComponentInChildren<Text>().color = tmpColor;
        Debug.Log("Highlight button " + menuBtnsList[currSelctdBtnID].name);
    }
}
