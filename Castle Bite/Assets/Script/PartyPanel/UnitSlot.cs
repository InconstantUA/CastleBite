﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class UnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Text unitName;
    Button unitSlot;
    Color tmpColor;

    // reference to the Confirmation pop-up window
    private ConfirmationPopUp confirmationPopUp;
    // actions for Confrimation pop-up
    private UnityAction disableYesAction;
    private UnityAction disableNoAction;

    private void Awake()
    {
        confirmationPopUp = ConfirmationPopUp.Instance();
        // define actions
        disableYesAction = new UnityAction(OnDismissYesConfirmation);
        disableNoAction = new UnityAction(OnDismissNoConfirmation);
    }

    void Start()
    {
        unitSlot = gameObject.GetComponent<Button>();
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

    Text GetUnitName()
    {
        // it in unit slot can change on unit move or hire or dismiss or other reasons
        // this function is used to always return the latest information about unit.
        Text resultNameTxtComp = null;
        if (unitSlot.transform.childCount > 0)
        {
            // verify if unit has isLeader atrribute ON
            // PartyUnit unit = unitSlot.transform.GetComponentInChildren<PartyUnit>();
            // fill in highered object UI panel
            resultNameTxtComp = unitSlot.transform.GetChild(0).Find("Name").GetComponent<Text>();
        }
        return resultNameTxtComp;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
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
        SetHighlightedStatus();
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
        // first verify that unit name is present (not null)
        unitName = GetUnitName();
        if (unitName)
        {
            // avoid double job
            if (!CompareColors(unitSlot.colors.highlightedColor, unitName.color))
            {
                // change to highlighted color
                if (unitSlot.interactable)
                {
                    tmpColor = unitSlot.colors.highlightedColor;
                }
                else
                {
                    tmpColor = unitSlot.colors.disabledColor;
                }
                tmpColor.a = 1;
                unitName.color = tmpColor;
                // Debug.Log("SetHighlightedStatus " + unitSlot.name + " button");
            }
        }
    }

    void SetPressedStatus()
    {
        unitName = GetUnitName();
        if (unitName)
        {
            if (unitSlot.interactable)
            {
                tmpColor = unitSlot.colors.pressedColor;
            }
            else
            {
                tmpColor = unitSlot.colors.disabledColor;
            }
            tmpColor.a = 1;
            unitName.color = tmpColor;
        }
        // Debug.Log("SetPressedStatus " + unitSlot.name + " button");
    }

    void SetNormalStatus()
    {
        unitName = GetUnitName();
        if (unitName)
        {
            if (unitSlot.interactable)
            {
                tmpColor = unitSlot.colors.normalColor;
            }
            else
            {
                tmpColor = unitSlot.colors.disabledColor;
            }
            tmpColor.a = 1;
            unitName.color = tmpColor;
        }
        // Debug.Log("SetNormalStatus " + unitSlot.name + " button");
    }

    #region OnClick

    City GetParentCity()
    {
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Left/Right/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.parent.parent.GetComponent<City>();
    }

    PartyPanel GetParentParty()
    {
        return transform.parent.parent.parent.GetComponent<PartyPanel>();
    }

    void ActOnClick()
    {
        // act based on the city (and cursor) state
         switch (GetParentCity().GetActiveState())
        {
            case City.CityViewActiveState.Normal:
                // do nothing for now
                break;
            case City.CityViewActiveState.ActiveHeroEquipment:
                // do nothing for now
                break;
            case City.CityViewActiveState.ActiveDismiss:
                // try to dismiss unit, if it is possible
                TryToDismissUnit();
                break;
            case City.CityViewActiveState.ActiveHeal:
                // try to heal unit, if it is possible
                break;
            case City.CityViewActiveState.ActiveResurect:
                // try to resurect unit, if it is possible
                break;
            default:
                Debug.LogError("Unknown state");
                break;
        }
    }

    void TryToDismissUnit()
    {
        // this depends on the fact if unit is dismissable
        // for example Capital guard is not dimissable
        // all other units normally are dismissable
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        if (unit.GetIsDismissable())
        {
            // as for confirmation
            string confirmationMessage;
            // verify if this is party leader
            if (unit.GetIsLeader())
            {
                confirmationMessage = "Dismissing party leader will permanently dismiss whole party and all its members. Do you want to dismiss " + unit.GetGivenName() + " " + unit.GetUnitName() + " and whole party?";
            }
            else
            {
                confirmationMessage = "Do you want to dismiss " + unit.GetUnitName() + "?";
            }
            // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
            confirmationPopUp.Choice(confirmationMessage, disableYesAction, disableNoAction);
        } else
        {
            // display error message
            NotificationPopUp notificationPopup = transform.root.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
            notificationPopup.DisplayMessage("It is not possible to dismiss " + unit.GetGivenName() + " " + unit.GetUnitName() + ".");
        }
    }

    void OnDismissYesConfirmation()
    {
        Debug.Log("Yes");
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        bool wasLeader = unit.GetIsLeader();
        if (wasLeader)
        {
            // dismiss all units in party
            // dismiss party
        }
        else
        {
            // dismiss unit with its parent canvas
            Debug.Log("Dismiss uit");
            Destroy(unit.transform.parent.gameObject);
        }
        // update city state, party and focus panels
        Transform cityTr = GetParentCity().transform;
        if (wasLeader)
        {
            // remove link to party leader to the Left Focus panel
            cityTr.Find("LeftFocus").GetComponent<FocusPanel>().focusedObject = null;
            // fill in city's left focus with information from the hero
            // Focus panel wil automatically detect changes and update info
            cityTr.Find("LeftFocus").GetComponent<FocusPanel>().OnChange();
        }
        else
        {
            // Update party panel
            PartyPanel partyPanel = GetParentParty();
            partyPanel.OnChange();
            // if parent Party panel is in Garnizon state, then update right focus
            if (PartyPanel.PanelMode.Garnizon == partyPanel.GetPanelMode())
            {
                // fill in city's right focus with information from the hero
                // Focus panel wil automatically detect changes and update info
                cityTr.Find("RightFocus").GetComponent<FocusPanel>().OnChange();
            }
        }
        // disable dismiss mode and return to normal mode
        GetParentCity().ReturnToNomalState();
    }

    void OnDismissNoConfirmation()
    {
        Debug.Log("No");
        // disable dismiss mode and return to normal mode
        GetParentCity().ReturnToNomalState();
    }

    #endregion OnClick
}