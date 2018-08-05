using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class HireFirstHero : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Transform city;
    Text txt;
    Button btn;
    Color tmpColor;
    public GameObject gameObjectToBeActivated;
    public GameObject gameObjectToBeDeactivated;
    [SerializeField]
    PlayerData[] players;

    void OnEnable()
    {
        // reset player name placeholder text to default hero name
        // use first defined in Editor player name as default name
        transform.root.Find("ChooseYourFirstHero/HireUnit/Panel/InputField/Placeholder").GetComponent<Text>().text = players[0].givenName;
    }

    void Start()
    {
        // init text object
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        // enable mouse on its move, if it was disabled before by keyboard activity
        if (((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0)) & (!Cursor.visible))
        {
            Cursor.visible = true;
            // Highlight button, if needed by triggering on point enter
            OnPointerEnter(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        // dimm all other menus
        DimmAllOtherMenus();
        // highlight this menu
        SetHighlightedStatus();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        // keep state On
        ActOnClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // return to previous toggle state
        SetNormalStatus();
    }

    bool CompareColors(Color a, Color b)
    {
        bool result = false;
        if (((int)(a.r * 1000) == (int)(b.r * 1000)) || ((int)(a.g * 1000) == (int)(b.g * 1000)) || ((int)(a.b * 1000) == (int)(b.b * 1000)))
        {
            result = true;
        }
        return result;
    }

    void SetHighlightedStatus()
    {
        // avoid double job
        if (!CompareColors(btn.colors.highlightedColor, txt.color))
        {
            // change to highlighted color
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

    void SetNormalStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.normalColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        // Debug.Log("SetNormalStatus " + btn.name + " button");
    }

    void DimmAllOtherMenus()
    {
        // to make it easier dimm just all menus
        GameObject[] allControls = GameObject.FindGameObjectsWithTag("HighlightableControlsHirePartyLeader");
        foreach (GameObject control in allControls)
        {
            Text tmpTxt = control.GetComponentInChildren<Text>();
            Button tmpBtn = control.GetComponentInChildren<Button>();
            tmpTxt.color = tmpBtn.colors.normalColor;
        }
        // Debug.Log("DimmAllOtherMenus");
    }

    #region OnClick

    PlayerUniqueAbility GetSelectedUniqueAbility()
    {
        //  Find selected toggle and get attached to it unit template
        //  Hierarchy HirePartyLeader-Panel-Controls-(this)HireBtn
        Toggle[] toggles = transform.parent.parent.GetComponentsInChildren<ToggleGroup>()[0].GetComponentsInChildren<Toggle>();
        PlayerUniqueAbility selectedAbility = PlayerUniqueAbility.Hardcore;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                selectedAbility = toggle.GetComponent<ChooseAbility>().ability;
            }
        }
        return selectedAbility;
    }

    PartyUnit GetSelectedUnit()
    {
        //  Find selected toggle and get attached to it unit template
        //  Hierarchy HirePartyLeader-Panel-Controls-(this)HireBtn
        Toggle[] toggles = transform.parent.parent.GetComponentsInChildren<ToggleGroup>()[1].GetComponentsInChildren<Toggle>();
        PartyUnit selectedUnit = null;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                selectedUnit = toggle.GetComponent<UnitHirePanel>().GetUnitToHire();
            }
        }
        return selectedUnit;
    }

    Transform GetCityTransform()
    {
        return city;
    }

    string GetPlayerName()
    {
        // get name from input
        string name = transform.root.Find("ChooseYourFirstHero/HireUnit/Panel/InputField").GetComponent<InputField>().text;
        // verify if name is not empty
        if ("" == name)
        {
            // reset name to default
            name = players[0].givenName;
        }
        // return name
        return name;
    }

    void ActOnClick()
    {
        // Create game players
        transform.root.Find("MainMenu/LoadGame").GetComponent<LoadGame>().RemoveAllPlayers();
        transform.root.Find("MainMenu/LoadGame").GetComponent<LoadGame>().CreateGamePlayers(players);
        // Ask City to Hire chosen unit
        GetCityTransform().GetComponent<City>().HireUnit(null, GetSelectedUnit());
        // Activate required object
        gameObjectToBeActivated.SetActive(true);
        // Deactivate required object
        gameObjectToBeDeactivated.SetActive(false);
    }

    #endregion OnClick
}