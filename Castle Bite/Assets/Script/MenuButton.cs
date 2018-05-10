using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        // interactableDelay = btn.interactable;
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
        //if (btn.interactable != interactableDelay)
        //{
        //    if (btn.interactable)
        //    {
        //        tmpColor = btn.colors.normalColor;
        //    }
        //    else
        //    {
        //        tmpColor = btn.colors.disabledColor;
        //    }
        //    tmpColor.a = 1;
        //    txt.color = tmpColor;
        //}
        //interactableDelay = btn.interactable;
        // enable mouse on its move, if it was disabled before
        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0) & (! Cursor.visible))
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
        Debug.Log("highlight " + btn.name + "button");
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
            // Debug.Log("dimm " + otherButton.name + "button");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // set state
        currentState = States.Highlighted;
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // set state
        currentState = States.Pressed;
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
            default:
                Debug.Log("Error: unknown selected menu name [" + currActiveMenuName + "]");
                break;
        }
    }

    void SetActiveMenuTo(string mName)
    {
        // Disable old menu and activate new menu
        btn.transform.parent.gameObject.SetActive(false);
        GameObject newMenu = btn.transform.root.Find(mName).gameObject;
        newMenu.SetActive(true);
        // Update list of all buttons in the menu - required for keyboard control script
        menuBtnsList = newMenu.GetComponentsInChildren<Button>();
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
                SetActiveMenuTo("OptionsSubmenuL2Panel");
                break;
            case "Quit":
                Application.Quit();
                break;
            default:
                Debug.Log("Error: unknown selected button name [" + selectedMBtnName + "]");
                break;
        }
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
                // activate Game sub-options
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