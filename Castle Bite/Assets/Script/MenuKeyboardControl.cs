using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuKeyboardControl : MonoBehaviour {
    Button btn; // currently selected button
    int currSelctdBtnID;
    int previouslySelectedMenuID;
    Color tmpColor;

    // Use this for initialization
    void Start() {
        // Init list of all buttons in the menu
        GameObject mainMenu = transform.root.Find("MainMenu").Find("MainMenuPanel").gameObject;
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
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
            // switch to the other menu
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
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
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
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
            Cursor.visible = false;
            currSelctdBtnID = GetCurrentlySelectedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            SetPressedStatus();
        }
        if (Input.GetKeyUp(KeyCode.Return) == true)
        {
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
            Cursor.visible = false;
            // update btn object, which later will be used in act on click
            currSelctdBtnID = GetCurrentlyPressedOrHighlightedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            // release color back to highlighted
            HighlightSelectedMenu();
            ActOnClick();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) == true)
        {
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
            Cursor.visible = false;
            // if selected menu control is one of the below then act on this
            currSelctdBtnID = GetCurrentlyPressedOrHighlightedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            string currActiveMenuName = btn.transform.parent.name;
            // move slider for the font size
            if ((currActiveMenuName == "OptionsVideoSubmenuL3Panel") & (btn.name == "FontSize"))
            {
                MoveSlider(btn, -1); // decrease font
                // ChangeFontSize();
            }
            if ((currActiveMenuName == "OptionsAudioSubmenuL3Panel") & (btn.name == "MusicVolume"))
            {
                MoveSlider(btn, -5); // decrease volume
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) == true)
        {
            // menu may be changed by mouse, that is why we get current menu and update menuBtnsList
            menuBtnsList = transform.root.Find("MainMenu").Find(GetActiveMenuName()).gameObject.GetComponentsInChildren<Button>();
            Cursor.visible = false;
            // if selected menu control is one of the below then act on this
            currSelctdBtnID = GetCurrentlyPressedOrHighlightedBtnID();
            btn = menuBtnsList[currSelctdBtnID];
            string currActiveMenuName = btn.transform.parent.name;
            // move slider for the font size
            if ((currActiveMenuName == "OptionsVideoSubmenuL3Panel") & (btn.name == "FontSize"))
            {
                MoveSlider(btn, 1); // increase font
                // ChangeFontSize();
            }
            if ((currActiveMenuName == "OptionsAudioSubmenuL3Panel") & (btn.name == "MusicVolume"))
            {
                MoveSlider(btn, 5); // inrease volume
            }
        }
    }

    string GetActiveMenuName()
    {
        string[] allMenuNames = {   "MainMenuPanel",
                                    "OptionsSubmenuL2Panel",
                                    "OptionsGameSubmenuL3Panel",
                                    "OptionsVideoSubmenuL3Panel",
                                    "OptionsAudioSubmenuL3Panel",
                                    "OptionsKeyboardAndMouseSubmenuL3Panel"};
        string activeMenuName = "unknown";
        foreach (string menuName in allMenuNames)
        {
            if (transform.root.Find("MainMenu").Find(menuName).gameObject.activeSelf)
            {
                activeMenuName = menuName;
            }
        }
        return activeMenuName;
    }

    #region from MenuOptionsVideoFontSizeControl.cs
    void MoveSlider(Button btn, int direction)
    {
        //  direction<0 <- move left
        //  direction>0 <- move right
        Slider sld = btn.GetComponentInChildren<Slider>();
        if (direction < 0)
        {
            // make sure that slider is not going to less than min value
            if ((sld.value + direction) > sld.minValue)
            {
                sld.value += direction; // direction is negative so we should not use - here otherwise it will become +
            }
            else
            {
                // reset to minimum value
                sld.value = sld.minValue;
            }
        }
        else
        {
            // make sure that slider is not going more than max value
            if ((sld.value + direction) < sld.maxValue)
            {
                sld.value += direction;
            } else
            {
                // reset to max value
                sld.value = sld.maxValue;
            }
        }
        // update text show in UI
        // Text txt = sld.transform.parent.gameObject.GetComponentInChildren<Text>();
        // txt.text = sld.value.ToString();

    }

    #endregion

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
                StartGame();
                break;
            case "Continue":
                ContinueGame();
                break;
            case "Save":
                SaveGame();
                break;
            case "Load":
                LoadGame();
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
        GameObject mainMenu = transform.root.Find("MainMenu").gameObject;
        mainMenu.SetActive(false);
        transform.root.Find("Cities").gameObject.SetActive(true);
        // As long as we are in game mode now, then Start button is not needed any more
        // instead activate Continue button
        GameObject mainMenuPanel = mainMenu.transform.Find("MainMenuPanel").gameObject;
        GameObject startButton = mainMenuPanel.transform.Find("Start").gameObject;
        GameObject continueButton = mainMenuPanel.transform.Find("Continue").gameObject;
        startButton.SetActive(false);
        continueButton.SetActive(true);
        // Also activate Save and Load buttons for future use
        GameObject saveButton = mainMenuPanel.transform.Find("Save").gameObject;
        GameObject loadButton = mainMenuPanel.transform.Find("Load").gameObject;
        saveButton.SetActive(true);
        loadButton.SetActive(true);
    }

    void ContinueGame()
    {
        // Activate map screen and deactivate menu
        GameObject mainMenu = transform.root.Find("MainMenu").gameObject;
        mainMenu.SetActive(false);
        transform.root.Find("MapScreen").gameObject.SetActive(true);
    }

    void SaveGame()
    {
    }

    void LoadGame()
    {
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
