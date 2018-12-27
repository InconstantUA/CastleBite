using UnityEngine;
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
        // Debug.Log("SetHighlightedStatus " + btn.name + " button");
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
        // Debug.Log("SetPressedStatus " + btn.name + " button");
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
        // Debug.Log("DimmAllOtherMenus");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        // set state
        currentState = States.Highlighted;
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        // set state
        currentState = States.Pressed;
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
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
                Debug.LogError("Error: unknown selected menu name [" + currActiveMenuName + "]");
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
            case "QuitToMainMenu":
                QuitToMainMenu();
                break;
            default:
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OnMainMenuClick on " + selectedMBtnName + " button");
    }

    void QuitToMainMenu()
    {
        // Activate main menu
        transform.root.Find("MainMenu").gameObject.SetActive(true);
        // Get main menu panel transform
        Transform mainMenuPanel = transform.root.Find("MainMenu/MainMenuPanel");
        // activate and deactivate required menus
        mainMenuPanel.Find("Start").gameObject.SetActive(true);
        mainMenuPanel.Find("Continue").gameObject.SetActive(false);
        mainMenuPanel.Find("Quit").gameObject.SetActive(true);
        mainMenuPanel.Find("QuitToMainMenu").gameObject.SetActive(false);
        // Also activate Save and Load buttons for future use
        mainMenuPanel.Find("Save").gameObject.SetActive(false);
        mainMenuPanel.Find("Load").gameObject.SetActive(true);
        // Remove all objects, except needed to start game
        Debug.LogError("Todo: Remove all objects, except needed to start game");
        // .. ideas:
        // - mark some objects and non-removable
        // - on game start preload other scene with all needed objects
        // Deactivate map screen
        MapManager.Instance.gameObject.SetActive(false);
        MapMenuManager.Instance.gameObject.SetActive(false);
    }

    public void OnGameStartMenuChanges()
    {
        // As long as we are in game mode now, then Start button is not needed any more
        // instead activate Continue button
        Transform mainMenuPanel = transform.root.Find("MainMenu/MainMenuPanel");
        mainMenuPanel.Find("Start").gameObject.SetActive(false);
        mainMenuPanel.Find("Continue").gameObject.SetActive(true);
        // Replace quit button with quit to main menu
        mainMenuPanel.Find("Quit").gameObject.SetActive(false);
        mainMenuPanel.Find("QuitToMainMenu").gameObject.SetActive(true);
        // Also activate Save and Load buttons for future use
        mainMenuPanel.Find("Save").gameObject.SetActive(true);
        mainMenuPanel.Find("Load").gameObject.SetActive(true);
    }

    void StartGame()
    {
        // Activate Game canvas and deactivate menu canvas
        transform.root.Find("MainMenu").gameObject.SetActive(false);
        transform.root.Find("MiscUI").gameObject.SetActive(true);
        // Activate and deactivate required menus
        OnGameStartMenuChanges();
        // Create game players, get players data from Chapter manager
        //foreach (PlayerData playerData in ChapterManager.Instance.PlayersData)
        //{
        //    // init tiles discovery state (I assume that by default it will init with 0)
        //    playerData.tilesDiscoveryState = new int[MapManager.Instance.TileMapWidth, MapManager.Instance.TileMapHeight];
        //    // create player using player data
        //    ObjectsManager.Instance.CreatePlayer(playerData);
        //}
        // Activate and reset turns manager, set Dominion as active player
        // TurnsManager.Instance.Reset(Faction.Dominion);
        // Activate ChooseYourFirstHero menu
        transform.root.Find("MiscUI").GetComponentInChildren<ChooseYourFirstHero>(true).SetActive(true);
    }

    void ContinueGame()
    {
        // Activate map screen and deactivate menu
        GameObject mainMenu = transform.root.Find("MainMenu").gameObject;
        mainMenu.SetActive(false);
        MapManager.Instance.gameObject.SetActive(true);
        MapMenuManager.Instance.gameObject.SetActive(true);
    }

    void SaveGame()
    {
        Debug.Log("Activate Save game menu");
        transform.root.Find("MainMenu/SaveGame").gameObject.SetActive(true);
        // deactivate main menu panel
        transform.root.Find("MainMenu/MainMenuPanel").gameObject.SetActive(false);
    }

    void LoadGame()
    {
        Debug.Log("Activate Load game menu");
        transform.root.Find("MainMenu/LoadGame").gameObject.SetActive(true);
        // deactivate main menu panel
        transform.root.Find("MainMenu/MainMenuPanel").gameObject.SetActive(false);
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
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OnOptionsSubmenuL2Click on " + selectedMBtnName + " button");
    }



    void OptionsGameSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                // go to previous menu
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "Autosave":
                // Revert autosave setting on click
                Text btnTxt = btn.GetComponentsInChildren<Text>()[1];
                if (btnTxt.text == "Off")
                {
                    btnTxt.text = "On";
                    GameOptions.Instance.gameOpt.autosave = 1;
                }
                else
                {
                    btnTxt.text = "Off";
                    GameOptions.Instance.gameOpt.autosave = 0;
                }
                break;
            default:
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OptionsGameSubmenuL3PanelClick on " + selectedMBtnName + " button");
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
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OptionsVideoSubmenuL3PanelClick on " + selectedMBtnName + " button");
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
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OptionsAudioSubmenuL3PanelClick on " + selectedMBtnName + " button");
    }

    void OptionsKeyboardAndMouseSubmenuL3PanelClick()
    {
        string selectedMBtnName = btn.name;
        switch (selectedMBtnName)
        {
            case "ReturnToTheOptionsMenu":
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "MoveUp":
                // Change key binding
                break;
            case "MoveDown":
                // Change key binding
                break;
            case "SaveAndReturn":
                // Save
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "ResetToDefault":
                break;
            default:
                Debug.LogError("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
        // Debug.Log("OptionsKeyboardAndMouseSubmenuL3PanelClick on " + selectedMBtnName + " button");
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