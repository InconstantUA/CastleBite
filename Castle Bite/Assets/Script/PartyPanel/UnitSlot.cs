using UnityEngine;
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

    // For battle screen
    bool isAllowedToApplyPowerToThisUnit = false;
    string errorMessage = "Error message";

    public bool IsAllowedToApplyPowerToThisUnit
    {
        get
        {
            return isAllowedToApplyPowerToThisUnit;
        }
    }

    public PartyUnit GetUnit()
    {
        // verify if slot has unit in it
        if (transform.childCount > 0)
        {
            return unitSlot.GetComponentInChildren<PartyUnit>();
        }
        return null;
    }

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
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.LogWarning("OnPointerDown");
            // on left mouse click
            SetPressedStatus();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
            // show unit info
            transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(unitSlot.GetComponentInChildren<PartyUnit>());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            // Debug.LogWarning("OnPointerUp");
            SetHighlightedStatus();
            ActOnClick();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            transform.root.Find("MiscUI/UnitInfoPanel").gameObject.SetActive(false);
        }
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
        // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.parent.parent.GetComponent<City>();
    }

    BattleScreen GetParentBattleScreen()
    {
        // structure: 5[BattleScreen]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.parent.parent.GetComponent<BattleScreen>();
    }

    PartyPanel GetParentPartyPanel()
    {
        // structure: 3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
        return transform.parent.parent.parent.GetComponent<PartyPanel>();
    }

    void ActOnCityClick()
    {
        Debug.Log("UnitSlot ActOnClick in City");
        // presave state, because we are going to reset it here
        City.CityViewActiveState cityState = GetParentCity().GetActiveState();
        // disable dismiss mode and return to normal mode
        // this looks naturally
        GetParentCity().ReturnToNomalState();
        // act based on the city (and cursor) state
        switch (cityState)
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
            case City.CityViewActiveState.ActiveUnitDrag:
                // ??
                break;
            default:
                Debug.LogError("Unknown state");
                break;
        }
    }

    public void ActOnBattleScreenClick()
    {
        BattleScreen battleScreen = GetParentBattleScreen();
        // Verify if battle has not ended
        if (!battleScreen.GetBattleHasEnded())
        {
            //Debug.Log("UnitSlot ActOnClick in Battle screen");
            // act based on the previously set by SetOnClickAction by PartyPanel conditions
            if (isAllowedToApplyPowerToThisUnit)
            {
                // it is allowed to apply powers to the unit in this cell
                GetParentPartyPanel().ApplyPowersToUnit(unitSlot.GetComponentInChildren<PartyUnit>());
                // set unit has moved flag
                PartyUnit activeUnit = battleScreen.GetActiveUnit();
                activeUnit.SetHasMoved(true);
                // activate next unit
                battleScreen.ActivateNextUnit();
            }
            else
            {
                // it is not allowed to use powers on this cell
                // display error message
                transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errorMessage);
            }
        }
    }

    void ActOnClick()
    {
        // act based on where we are:
        //  - in city
        //  - in battle
        City city = GetParentCity();
        BattleScreen battleScreen = GetParentBattleScreen();
        if (city)
        {
            ActOnCityClick();
        }
        if (battleScreen)
        {
            ActOnBattleScreenClick();
        }
    }

    void TryToDismissUnit()
    {
        // this depends on the fact if unit is dismissable
        // for example Capital guard is not dimissable
        // all other units normally are dismissable
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        // if unit does not exist, then nothing to do
        if (unit)
        {
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
            }
            else
            {
                // display error message
                NotificationPopUp notificationPopup = transform.root.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
                notificationPopup.DisplayMessage("It is not possible to dismiss " + unit.GetGivenName() + " " + unit.GetUnitName() + ".");
            }
        }
    }

    void OnDismissYesConfirmation()
    {
        Debug.Log("Yes");
        // Ask city to dismiss unit
        GetParentCity().DimissUnit(unitSlot.GetComponent<UnitSlot>());
    }

    void OnDismissNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    public void SetOnClickAction(bool isAllowedToApplyPwrToThisUnit, string errMsg = "")
    {
        isAllowedToApplyPowerToThisUnit = isAllowedToApplyPwrToThisUnit;
        errorMessage = errMsg;
    }

    #endregion OnClick
}