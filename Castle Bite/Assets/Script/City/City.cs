using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {
    public string cityName;
    public string cityDescription;
    public int cityLevel;
    public enum CityType { Normal, Capital };
    public CityType cityType;
    int maxCityLevel = 5;
    public enum CityViewActiveState { Normal, ActiveHeal, ActiveResurect, ActiveDismiss, ActiveHeroEquipment };
    [SerializeField]
    CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;
    public enum CityOccupationState { NoHeroIn, HeroIn };
    [SerializeField]
    CityOccupationState cityOccupationState;

    // add player here, because it accessed by many functions
    PlayerObj player;

    // City view state is required to effectively change between different states
    // and do not forget to enable or disable something
    // we need to correct transition to normal state after leaving city view
    // to view it normally again after we enter it
    // and do not have heal/resurect/dismiss states active after we enter city next time
    //
    // For the same purpose we have City inventory state
    // If we leave city, then we should deactivate inventory view
    //
    // The same for city occupation state
    // If hero leaves city, the we should return city state o NoHeroIn
    // and activate hire hero button


    // Use this for initialization
    void Start()
    {
        player = transform.root.Find("PlayerObj").gameObject.GetComponent<PlayerObj>();
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public int GetDefence()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + bonus;
    }

    public int GetHealPerDay()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + 5 + bonus;
    }

    public int GetUnitsCapacity()
    {
        return cityLevel;
    }

    public bool HasCityReachedMaximumLevel()
    {
        bool result = false;
        // note city level for capital is 6, which is higher than for normal city
        if  (cityLevel >= maxCityLevel)
        {
            // city has reached its maximum level
            result = true;
        }
        return result;
    }

    #region Active city states: dismiss, heal, resurect unit and hero equipment

    public CityViewActiveState GetActiveState()
    {
        return cityViewActiveState;
    }

    public void SetActiveState(CityViewActiveState requiredState, bool doActivate)
    {
        // if doActivate is true,
        // then we are really swiching to this new state
        // otherwise it means that we are deactivating old state and returning to Normal state
        // this should not happen for Normal state, so here is check, for this
        if  (requiredState == CityViewActiveState.Normal)
        {
            Debug.LogError("Unexpected condition");
        }
        // 1st return previous state to Normal, if it is already not Normal
        // this only needed if we are activating new state, while previous state was not normal
        if ( (cityViewActiveState != CityViewActiveState.Normal) && (requiredState != cityViewActiveState) && (doActivate == true))
        {
            SetActiveState(cityViewActiveState, false);
        }
        // Update 
        cityViewActiveState = requiredState;
        // instruct party and garnizon panels to
        // highlight differently cells with and without units
        // and disable hire buttons
        switch (cityViewActiveState)
        {
            case CityViewActiveState.ActiveDismiss:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                break;
            case CityViewActiveState.ActiveHeal:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                break;
            case CityViewActiveState.ActiveResurect:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                break;
            case CityViewActiveState.ActiveHeroEquipment:
                // this is not applicable to garnizon, garnizon does not have hero or eqipment
                // it is only appicable to hero party
                break;
        }
        // if hero party is present,
        // then activate active state highligh
        // if not - then disable hire hero button
        if (transform.GetComponentInChildren<HeroParty>())
        {
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                    break;
                case CityViewActiveState.ActiveHeal:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                    break;
                case CityViewActiveState.ActiveResurect:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                    break;
                case CityViewActiveState.ActiveHeroEquipment:
                    SetActiveHeroEquipment(doActivate);
                    break;
            }
        }
        else
        {
            transform.Find("HireHeroPanel/HireHeroPanelBtn").gameObject.SetActive(!doActivate);
        }
        // Update cursor
        CursorController.Instance.SetCityActiveViewStateCursor(cityViewActiveState, doActivate);
        // if doActivate was false
        // this means that we need to return to normal state.
        // that what we did by passing doActivate variable to other functions
        // so we only need to set here state to normal
        if (false == doActivate)
        {
            cityViewActiveState = CityViewActiveState.Normal;
        }
    }

    void SetActiveHeroEquipment(bool doActivate)
    {
        // Disable Hero equipment menu if it was enabled and enable it otherwise
        // Also disable / enable hero party and city garnizon
        // Structure:   [city]->[HeroParty/CityGarnizon]
        Transform heroParty = transform.GetComponentInChildren<HeroParty>().transform;
        GameObject heroEquipmentMenu = heroParty.Find("HeroEquipment").gameObject;
        GameObject heroUnitsPanel = heroParty.Find("PartyPanel").gameObject;
        GameObject cityGarnizon = transform.Find("CityGarnizon").gameObject;
        heroEquipmentMenu.SetActive(doActivate);
        heroUnitsPanel.SetActive(!doActivate);
        cityGarnizon.SetActive(!doActivate);
    }

    public void ReturnToNomalState()
    {
        // return states to normal
        if (CityViewActiveState.Normal != cityViewActiveState)
        {
            // 1st deactivate button which is linked to this state
            // because this is not done by user
            // we should do it ourselves
            // we should do this before deactivating state,
            // because state is reset to normal by SetActiveState
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    // find and deactivate toggle
                    // simulate user mouse click
                    transform.Find("CtrlPnlCity/Dismiss").GetComponent<ActionToggle>().OnPointerDown(null);
                    // also we need to manually deactivate Toggle
                    transform.Find("CtrlPnlCity/Dismiss").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveHeal:
                    transform.Find("CtrlPnlCity/Heal").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.Find("CtrlPnlCity/Heal").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveResurect:
                    transform.Find("CtrlPnlCity/Resurect").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.Find("CtrlPnlCity/Resurect").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveHeroEquipment:
                    // SetActiveState(cityViewActiveState, false);
                    transform.GetComponentInChildren<HeroParty>().transform.Find("HeroEquipmentBtn").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.GetComponentInChildren<HeroParty>().transform.Find("HeroEquipmentBtn").GetComponent<Toggle>().isOn = false;
                    break;
            }
            // deactive currently active state
            // SetActiveState(cityViewActiveState, false);
        }
    }

    public void ExitCity()
    {
        ReturnToNomalState();
        // deactivate this component
        transform.gameObject.SetActive(false);
    }

    #endregion

    #region UnitHire

    bool VerifyIfPlayerHasEnoughGoldToBuyUnit(PartyUnit hiredUnitTemplate)
    {
        bool result = false;
        int requiredGold = hiredUnitTemplate.GetCost();
        //  Verify if player has enough gold
        if (player.GetTotalGold() >= requiredGold)
        {
            result = true;
        } else
        {
            // display message that is not enough gold
            NotificationPopUp notificationPopup = transform.root.Find("MiscUI").Find("NotificationPopUp").GetComponent<NotificationPopUp>();
            if (hiredUnitTemplate.GetIsLeader())
            {
                notificationPopup.DisplayMessage("More gold is needed to hire this party leader.");
            }
            else
            {
                notificationPopup.DisplayMessage("More gold is needed to hire this party member.");
            }
        }
        return result;
    }

    GameObject CreateNewPartyInCity()
    {
        // create and update Hero Party panel in UI, parent it to city UI
        GameObject heroPartyPanelTemplate = transform.root.Find("Templates").Find("UI").Find("HeroParty").gameObject;
        GameObject newPartyUIPanel = Instantiate(heroPartyPanelTemplate, transform);
        //  activate new party UI panel
        newPartyUIPanel.SetActive(true);
        //  set hero's equipment button to be part of city control panel ToggleGroup
        //  this should be unset on hero leaving city
        ToggleGroup toggleGroup = transform.Find("CtrlPnlCity").GetComponent<ToggleGroup>();
        Toggle heroEquipmentToggle = newPartyUIPanel.transform.Find("HeroEquipmentBtn").GetComponent<Toggle>();
        heroEquipmentToggle.group = toggleGroup;
        //  set HeroEquipmentBtn Toggle within CityControlPanel, so it can dimm or deselect it when other Toggles in group are activated
        //  this should be set to null on hero leaving or accessed outside of the city.
        toggleGroup.GetComponent<CityControlPanel>().SetHeroEquipmentToggle(heroEquipmentToggle);
        // return new party as result
        return newPartyUIPanel;
    }

    PartyUnit CreateUnit(Transform newUnitParentSlot, PartyUnit hiredUnitTemplate)
    {
        //  create new instance of unity draggable canvas and set it as unit's parent
        GameObject unitCanvasTemplate = transform.root.Find("Templates").Find("UI").Find("UnitCanvas").gameObject;
        Transform newUnitParentTr = Instantiate(unitCanvasTemplate, newUnitParentSlot).transform;
        // enable it
        newUnitParentTr.gameObject.SetActive(true);
        // Create new unit and place it in parent transform
        PartyUnit newPartyUnit = Instantiate(hiredUnitTemplate, newUnitParentTr);
        return newPartyUnit;
    }

    void UpdateLeftFocus(GameObject newPartyUnitObj)
    {
        // activate required UIs and also fill in city's left focus panels
        // link party leader to the Left Focus panel
        // so it can use it to fill in information
        transform.Find("LeftFocus").GetComponent<FocusPanel>().focusedObject = newPartyUnitObj;
        // fill in city's left focus with information from the hero
        // Focus panel wil automatically detect changes and update info
        transform.Find("LeftFocus").GetComponent<FocusPanel>().OnChange();
    }

    void HirePartyLeader(PartyUnit hiredUnitTemplate)
    {
        // create new party
        GameObject newLeaderParty = CreateNewPartyInCity();
        // set middle right panel as hero's parent transform. Place it to the canvas, which later will be dragg and droppable
        Transform newUnitParentSlot = newLeaderParty.GetComponentInChildren<PartyPanel>().GetUnitSlotTr("Middle", "Right");
        // create new unit
        PartyUnit newPartyUnit = CreateUnit(newUnitParentSlot, hiredUnitTemplate);
        // Update UI with information from new unit;
        UpdateLeftFocus(newPartyUnit.gameObject);
        // Disable Hire leader panel
        transform.Find("HireHeroPanel").gameObject.SetActive(false);
        // take gold from player
        player.SetTotalGold(player.GetTotalGold() - hiredUnitTemplate.GetCost());
    }

    bool VerifySingleUnitHire(PartyUnit selectedUnit)
    {
        bool result = true;
        // this is actually not required, because
        // if number of units in city reaches maximum, 
        // then hire unit button is disabled
        // but let it be, just in case
        return result;
    }

    void UpdateRightFocus()
    {
        // Focus panel wil automatically detect changes and update info
        transform.Find("RightFocus").GetComponent<FocusPanel>().OnChange();
    }

    void HireSingleUnit(Transform callerCell, PartyUnit hiredUnitTemplate)
    {
        if (VerifySingleUnitHire(hiredUnitTemplate))
        {
            // get parent for new cell
            Transform parentTransform = callerCell.Find("UnitSlot");
            // create unit
            PartyUnit newPartyUnit = CreateUnit(parentTransform, hiredUnitTemplate);
            // fill in city's right focus with information from the hero
            UpdateRightFocus();
            // Update city garnizon panel to fill in required information;
            transform.Find("CityGarnizon/PartyPanel").GetComponent<PartyPanel>().OnChange(PartyPanel.ChangeType.HireSingleUnit, callerCell);
            // disable hire unit button
            callerCell.Find("HireUnitPnlBtn").gameObject.SetActive(false);
            // take gold from player
            player.SetTotalGold(player.GetTotalGold() - hiredUnitTemplate.GetCost());
        }
    }

    bool VerifyDoubleUnitHire(Transform callerCell, PartyUnit selectedUnit)
    {
        bool result = false;
        PartyPanel partyPanel = transform.Find("CityGarnizon/PartyPanel").GetComponent<PartyPanel>();
        // if all checks passed, then verify result is success
        // verify that we do not get more units, then city can keep
        bool cityCapacityOverflowCheckIsOK = partyPanel.VerifyCityCapacityOverflowOnDoubleUnitHire();
        // verify if player is highring double unit near occupied single unit cell
        // this is not possible and will cause double unit to be displayed on top of single unit
        // we should avoid this
        bool doubleUnintIsNotHiredNearOccupiedSingleCell = partyPanel.VerifyDoubleHireNearOccupiedSingleCell(callerCell);
        // if all conditions are met, then set result to true;
        if (cityCapacityOverflowCheckIsOK && doubleUnintIsNotHiredNearOccupiedSingleCell)
        {
            result = true;
        }
        return result;
    }

    void HireDoubleUnit(Transform callerCell, PartyUnit hiredUnitTemplate)
    {
        if (VerifyDoubleUnitHire(callerCell, hiredUnitTemplate))
        {
            // get parent for new cell
            // if it is double size, then place it in the wide cell
            // if hired unit is double unit, then we actually need to change its parent to the wide
            // hierarchy: [Top/Middle/Bottom panel]-[Left/Right/Wide]-callerCell
            Transform newUnitParentSlot = callerCell.parent.Find("Wide").Find("UnitSlot");
            // update panel
            transform.Find("CityGarnizon/PartyPanel").GetComponent<PartyPanel>().OnChange(PartyPanel.ChangeType.HireDoubleUnit, callerCell);
            // create unit
            PartyUnit newPartyUnit = CreateUnit(newUnitParentSlot, hiredUnitTemplate);
            // fill in city's right focus with information from the hero
            UpdateRightFocus();
            // take gold from player
            player.SetTotalGold(player.GetTotalGold() - hiredUnitTemplate.GetCost());
        }
    }

    public void HireUnit(Transform callerCell, PartyUnit hiredUnitTemplate)
    {
        // 1 do basic verications, which are applicable to every hired unit
        if (VerifyIfPlayerHasEnoughGoldToBuyUnit(hiredUnitTemplate))
        {
            // 2 act based on the unit type: leader or normal unit
            if (hiredUnitTemplate.GetIsLeader())
            {
                HirePartyLeader(hiredUnitTemplate);
            }
            else
            {
                // act based on the unit type
                if (PartyUnit.UnitSize.Single == hiredUnitTemplate.GetUnitSize())
                {
                    HireSingleUnit(callerCell, hiredUnitTemplate);
                }
                else
                {
                    HireDoubleUnit(callerCell, hiredUnitTemplate);
                }
            }
        }
    }


    public void DismissPartyLeader(UnitSlot unitSlot)
    {
        // dismiss all units in party
        // dismiss party
        //
        // update UI
        UpdateLeftFocus(null);
    }

    PartyPanel GetUnitsParentPartyPanel(Transform unitCell)
    {
        // structure: 2PartyPanel-1[Top/Middle/Bottom]-[Left/Wide/Right]-UnitSlot-1UnitCanvas-unit
        return unitCell.transform.parent.parent.GetComponent<PartyPanel>();
    }

    public void DimissSingleUnit(UnitSlot unitSlot)
    {
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        // todo just manually update all fields
        // or create special onDismiss functions
        // 1 get all required variables, before removing unit
        Transform unitCell = unitSlot.transform.parent;
        PartyPanel partyPanel = GetUnitsParentPartyPanel(unitCell);
        GameObject unitCanvas = unit.transform.parent.gameObject;
        // 2 dismiss unit with its parent canvas
        Destroy(unitCanvas);
        // Update party panel
        partyPanel.OnChange(PartyPanel.ChangeType.DismissSingleUnit, unitCell);
        // if parent Party panel is in Garnizon state, then update right focus
        // no need to update left focus, because it is only updated on leader dismiss
        if (PartyPanel.PanelMode.Garnizon == partyPanel.GetPanelMode())
        {
            // fill in city's right focus with information from the hero
            // Focus panel wil automatically detect changes and update info
            transform.Find("RightFocus").GetComponent<FocusPanel>().OnChange();
        }

    }


    public void DimissDoubleUnit(UnitSlot unitSlot)
    {

    }

    public void DimissUnit(UnitSlot unitSlot)
    {
        Debug.Log("Dismiss unit");
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        bool wasLeader = unit.GetIsLeader();
        if (wasLeader)
        {
            DismissPartyLeader(unitSlot);
        }
        else
        {
            // act based on the unit size
            if (unit.GetUnitSize() == PartyUnit.UnitSize.Single)
            {
                DimissSingleUnit(unitSlot);
            }
            else
            {
                DimissDoubleUnit(unitSlot);
            }
        }
        // disable dismiss mode and return to normal mode
        ReturnToNomalState();
    }

    #endregion
}
