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
    private UnityAction dismissYesAction;
    private UnityAction dismissNoAction;

    // For battle screen
    bool isAllowedToApplyPowerToThisUnit = false;
    string errorMessage = "Error message";


    private void Awake()
    {
        // define actions
        dismissYesAction = new UnityAction(OnDismissYesConfirmation);
        dismissNoAction = new UnityAction(OnDismissNoConfirmation);
        // trigger changes
        OnTransformChildrenChanged();
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

    #region Button controls

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
            // verify is partyunit UI is present
            if (GetComponentInChildren<PartyUnitUI>())
                // show unit info
                transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(GetComponentInChildren<PartyUnitUI>().LPartyUnit);
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

    //bool CompareColors(Color a, Color b)
    //{
    //    bool result = false;
    //    if (((int)(a.r * 1000) == (int)(b.r * 1000)) || ((int)(a.g * 1000) == (int)(b.g * 1000)) || ((int)(a.b * 1000) == (int)(b.b * 1000)))
    //    {
    //        result = true;
    //    }
    //    return result;
    //}

    void SetHighlightedStatus()
    {
        // first verify that unit name is present (not null)
        unitName = GetUnitNameText();
        if (unitName)
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

    void SetPressedStatus()
    {
        unitName = GetUnitNameText();
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
        unitName = GetUnitNameText();
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

    #endregion Button controls

    public string GetUnitCellAddress()
    {
        // structure: PartyPanel-2[Top/Middle/Bottom]-1[Front/Back/Wide]-UnitSlot(this)
        return transform.parent.parent.name + "/" + transform.parent.name;
    }

    //IEnumerator UpdateUnitEquipmentControl()
    //{
    //    // .. remake it without coroutine, because it is visible how equipment button controls disappear
    //    // .. or add some delay before activation of other menus
    //    // verify if we are in city view and there is HeroParty parent, because this is not the case when party panel is copied to PartiesInfoPanel
    //    // .. adjust logic later
    //    if (GetParentPartyPanel().transform.parent.GetComponent<HeroParty>())
    //    {
    //        // verify if party is in party mode, because only in this mode it has leader and should have equipment button visible
    //        if (GetParentPartyPanel().transform.parent.GetComponent<HeroParty>().PartyMode == PartyMode.Party)
    //        {
    //            // verify if there is unit in slot
    //            PartyUnitUI partyUnitUI = GetPartyUnitUI();
    //            if (partyUnitUI != null)
    //            {
    //                // verify if unit is leader
    //                if (partyUnitUI.LPartyUnit.IsLeader)
    //                {
    //                    // activate equipment button
    //                    //Debug.LogWarning("Enable equipment button");
    //                    partyUnitUI.transform.Find("UnitEquipmentControl").gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    // deactivate equipment button
    //                    //Debug.LogWarning("Disable equipment button");
    //                    partyUnitUI.transform.Find("UnitEquipmentControl").gameObject.SetActive(false);
    //                }
    //            }
    //            //else
    //            //{
    //            //    // deactivate equipment button
    //            //    //Debug.LogWarning("Disable equipment button");
    //            //    transform.Find("UnitEquipmentControl").gameObject.SetActive(false);
    //            //}
    //        }
    //        else
    //        {
    //            //// deactivate equipment button
    //            ////Debug.LogWarning("Disable equipment button");
    //            //transform.Find("UnitEquipmentControl").gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        //// deactivate equipment button
    //        ////Debug.LogWarning("Disable equipment button");
    //        //transform.Find("UnitEquipmentControl").gameObject.SetActive(false);
    //    }
    //    // skip 1 frame untill child object is fully instantiated
    //    yield return null;
    //}

    //IEnumerator UpdateUpgradeUnitControl()
    //{
    //    // skip 1 frame untill child object is fully instantiated
    //    yield return null;
    //    // verify if there is unit in slot
    //    PartyUnit partyUnit = GetUnit();
    //    if (partyUnit != null)
    //    {
    //        //Debug.LogWarning("Enable upgrade unit + button");
    //        transform.parent.Find("UpgradeUnitControl").gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        //Debug.LogWarning("Disable upgrade unit + button");
    //        transform.parent.Find("UpgradeUnitControl").gameObject.SetActive(false);
    //    }
    //}

    void OnTransformChildrenChanged()
    {
        Debug.Log("Unit Slot: The list of children has changed");
        if (gameObject.activeInHierarchy)
        {
            //StartCoroutine(UpdateUnitEquipmentControl());
            //StartCoroutine(UpdateUpgradeUnitControl());
        }
    }

    public bool IsAllowedToApplyPowerToThisUnit
    {
        get
        {
            return isAllowedToApplyPowerToThisUnit;
        }
    }

    public PartyUnitUI GetPartyUnitUI()
    {
        return GetComponentInChildren<PartyUnitUI>();
    }

    public PartyUnit GetUnit()
    {
        //// verify if slot has unit in it
        //if (transform.childCount > 0)
        //{
        //    //Debug.Log(transform.parent.parent.name + " " + transform.parent.name + " has childs");
        //    // check whether we are in battle or in other mode
        //    if (transform.GetComponentInChildren<UnitOnBattleMouseHandler>(true))
        //    {
        //        // we are in battle mode
        //        return transform.GetComponentInChildren<UnitOnBattleMouseHandler>(true).GetComponentInChildren<PartyUnit>(true);
        //    }
        //    else
        //    {
        //        // we are in other mode
        //        return transform.GetComponentInChildren<UnitDragHandler>(true).GetComponentInChildren<PartyUnit>(true);
        //    }
        //}
        //else
        //{
        //    //Debug.Log(transform.parent.parent.name + " " + transform.parent.name + " is empty");
        //}
        if (GetPartyUnitUI())
        {
            return GetPartyUnitUI().LPartyUnit;
        }
        return null;
    }

    Text GetUnitNameText()
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

    #region OnClick

    //City GetParentCity()
    //{
    //    // structure: 5[City]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
    //    return transform.parent.parent.parent.parent.parent.GetComponent<City>();
    //}

    //BattleScreen GetParentBattleScreen()
    //{
    //    // structure: 5[BattleScreen]-4[HeroParty/CityGarnizon]-3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
    //    return transform.parent.parent.parent.parent.parent.GetComponent<BattleScreen>();
    //}

    //PartyPanel GetParentPartyPanel()
    //{
    //    // structure: 3PartyPanel-2[Top/Middle/Bottom]Panel-1[Front/Back/Wide]Panel-(this)UnitSlot
    //    return transform.parent.parent.parent.GetComponent<PartyPanel>();
    //}

    void ActOnCityClick()
    {
        Debug.Log("UnitSlot ActOnClick in City");
        // Get city screen
        EditPartyScreen cityScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>();
        // Get city state
        EditPartyScreenActiveState cityState = cityScreen.EditPartyScreenActiveState;
        // Verify if city state is not normal
        if (EditPartyScreenActiveState.Normal != cityState)
        {
            cityScreen.DeactivateActiveToggle();
        }
        // Get party unit UI in this slot
        PartyUnitUI unitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
        // Verify if unit is found
        if (unitUI)
        {
            // act based on the city (and cursor) state
            switch (cityState)
            {
                case EditPartyScreenActiveState.Normal:
                    // do nothing for now
                    break;
                case EditPartyScreenActiveState.ActiveDismiss:
                    // try to dismiss unit, if it is possible
                    TryToDismissUnit(unitUI.LPartyUnit);
                    break;
                case EditPartyScreenActiveState.ActiveHeal:
                    // try to heal unit, if it is possible
                    Debug.Log("Show Heal Unit confirmation box");
                    break;
                case EditPartyScreenActiveState.ActiveResurect:
                    // try to resurect unit, if it is possible
                    Debug.Log("Show Resurect Unit confirmation box");
                    break;
                case EditPartyScreenActiveState.ActiveUnitDrag:
                    // ??
                    break;
                case EditPartyScreenActiveState.ActiveItemDrag:
                    // this should not be triggered here, because it should be triggered in the unit slot drop handler when item is being dropped in it
                    Debug.LogWarning("Unpredicted condition");
                    break;
                default:
                    Debug.LogError("Unknown state");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Unit no found");
        }
        // Verify if city state is not normal
        if (EditPartyScreenActiveState.Normal != cityState)
        {
            // disable previous city state
            cityScreen.SetActiveState(cityState, false);
        }
    }

    public void ActOnBattleScreenClick()
    {
        BattleScreen battleScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>();
        // Verify if battle has not ended
        if (!battleScreen.GetBattleHasEnded())
        {
            //Debug.Log("UnitSlot ActOnClick in Battle screen");
            // act based on the previously set by SetOnClickAction by PartyPanel conditions
            if (isAllowedToApplyPowerToThisUnit)
            {
                // it is allowed to apply powers to the unit in this cell
                // GetParentPartyPanel().ApplyPowersToUnit(unitSlot.GetComponentInChildren<PartyUnitUI>());
                GetComponentInParent<PartyPanel>().ApplyPowersToUnit(unitSlot.GetComponentInChildren<PartyUnitUI>());
                // set unit has moved flag
                battleScreen.ActiveUnitUI.LPartyUnit.HasMoved = (true);
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
        //  - in hero edit screen
        //  - in battle
        UIManager uiManager = transform.root.GetComponentInChildren<UIManager>();
        // verify if EditPartyScreen is active
        if (uiManager.GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            ActOnCityClick();
        }
        // verify if Battle screen is active
        else if (uiManager.GetComponentInChildren<BattleScreen>(false) != null)
        {
            ActOnBattleScreenClick();
        }
        // verify if PartiesInfoPanel is active
        else if (uiManager.GetComponentInChildren<PartiesInfoPanel>(false) != null)
        {
            Debug.Log("Act on PartiesInfoPanel active");
        }
        else
        {
            Debug.LogError("No or unknown active screen");
        }
    }

    void TryToDismissUnit(PartyUnit unit)
    {
        // this depends on the fact if unit is dismissable
        // for example Capital guard is not dimissable
        // all other units normally are dismissable
        if (unit.IsDismissable)
        {
            // as for confirmation
            string confirmationMessage;
            // verify if this is party leader
            if (unit.IsLeader)
            {
                confirmationMessage = "Dismissing party leader will permanently dismiss whole party and all its members. Do you want to dismiss " + unit.GivenName + " " + unit.UnitName + " and whole party?";
            }
            else
            {
                confirmationMessage = "Do you want to dismiss " + unit.UnitName + "?";
            }
            // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
            ConfirmationPopUp.Instance().Choice(confirmationMessage, dismissYesAction, dismissNoAction);
        }
        else
        {
            // display error message
            NotificationPopUp notificationPopup = transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>();
            notificationPopup.DisplayMessage("It is not possible to dismiss " + unit.GivenName + " " + unit.UnitName + ".");
        }
    }

    void ActivateHireUnitButtonsIfNeeded()
    {
        // get city screen
        EditPartyScreen cityScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>();
        // verify if we are in city view mode
        if (cityScreen != null)
        {
            // activate hire unit pnl button
            cityScreen.SetHireUnitPnlButtonActive(true);
        }
    }

    void OnDismissYesConfirmation()
    {
        Debug.Log("Yes");
        // Check and activate hire units buttons if we are in city view
        ActivateHireUnitButtonsIfNeeded();
        // Ask city to dismiss unit
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().DimissUnit(this);
        // do not place code below, because this unit slot is destroyed
    }

    void OnDismissNoConfirmation()
    {
        Debug.Log("No");
        // Check and activate hire units buttons if we are in city view
        ActivateHireUnitButtonsIfNeeded();
        //// get party panel from slot
        //// and activate hire unit pannel button again
        //// structure: 3partypanel-2row-1cell-this(UnitSlot)
        //transform.parent.parent.parent.GetComponent<PartyPanel>().SetHireUnitPnlButtonActive(true);
    }

    public void SetOnClickAction(bool isAllowedToApplyPwrToThisUnit, string errMsg = "")
    {
        isAllowedToApplyPowerToThisUnit = isAllowedToApplyPwrToThisUnit;
        errorMessage = errMsg;
    }

    #endregion OnClick
}