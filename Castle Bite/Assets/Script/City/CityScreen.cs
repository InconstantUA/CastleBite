using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityScreen : MonoBehaviour {
    [SerializeField]
    City mCity;
    [SerializeField]
    CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;
    [SerializeField]
    GameObject unitCanvasTemplate;


    public City City
    {
        get
        {
            return mCity;
        }
    }

    public CityViewActiveState CityViewActiveState
    {
        get
        {
            return cityViewActiveState;
        }
    }

    public void SetCityScreenActive(City city)
    {
        // save link to the city
        mCity = city;
        // activate this city screen
        gameObject.SetActive(true);
    }

    void SetRequiredComponentsActive(bool doActivate)
    {
        // Activate/Deactivate background
        transform.root.Find("MiscUI/Background").gameObject.SetActive(doActivate);
        // Activate/Deactivate city controls
        transform.root.Find("MiscUI/BottomControlPanel").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(doActivate);
        // Activate/Deactivate hero Parties menus and Focus panels
        if (doActivate)
        {
            // get HeroParty
            HeroParty heroParty = City.GetHeroPartyByMode(PartyMode.Party);
            // verify if there is party in a city
            if (heroParty != null)
            {
                // assign party leader to left focus panel
                transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = heroParty.GetHeroPartyLeaderUnit().gameObject;
                // activate left Focus panel
                transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(doActivate);
                // assign HeroParty to left hero party UI
                transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>().LHeroParty = heroParty;
                // activate left hero party UI
                transform.root.Find("MiscUI/LeftHeroParty").gameObject.SetActive(doActivate);
            }
            else
            {
                // activate left Focus panel
                // without linked hero it will trigger no-party info
                transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(doActivate);
                // activate hire hero panel
                transform.root.Find("MiscUI/HireHeroPanel").gameObject.SetActive(doActivate);
            }
            // assign city to right focus panel
            transform.root.Find("MiscUI/RightFocus").GetComponent<FocusPanel>().focusedObject = City.gameObject;
            // activate right Focus panel
            transform.root.Find("MiscUI/RightFocus").gameObject.SetActive(doActivate);
            // assign City garnizon HeroParty to right hero party UI
            transform.root.Find("MiscUI/RightHeroParty").GetComponent<HeroPartyUI>().LHeroParty = City.GetHeroPartyByMode(PartyMode.Garnizon);
            // activate right hero party UI
            transform.root.Find("MiscUI/RightHeroParty").gameObject.SetActive(doActivate);
            // activate hire common units panel
            transform.root.Find("MiscUI/HireCommonUnitButtons").gameObject.SetActive(doActivate);
        }
        else
        {
            // deactivate left Focus panel
            transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(doActivate);
            // deactivate left hero party UI
            transform.root.Find("MiscUI/LeftHeroParty").gameObject.SetActive(doActivate);
            // deactivate right Focus panel
            transform.root.Find("MiscUI/RightFocus").gameObject.SetActive(doActivate);
            // deactivate right hero party UI
            transform.root.Find("MiscUI/RightHeroParty").gameObject.SetActive(doActivate);
            // deactivate hire hero panel
            transform.root.Find("MiscUI/HireHeroPanel").gameObject.SetActive(doActivate);
            // deactivate hire common units panel
            transform.root.Find("MiscUI/HireCommonUnitButtons").gameObject.SetActive(doActivate);
        }
    }

    void OnEnable()
    {
        SetRequiredComponentsActive(true);
    }

    void OnDisable()
    {
        //DeactivateActiveToggle();
        SetRequiredComponentsActive(false);
    }

    #region States

    public void SetActiveStateDismiss(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveDismiss, doActivate);
    }

    public void SetActiveStateHeal(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveHeal, doActivate);
    }

    public void SetActiveStateResurect(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveResurect, doActivate);
    }

    public void SetActiveState(CityViewActiveState requiredState, bool doActivate)
    {
        // if doActivate is true,
        // then we are really swiching to this new state
        // otherwise it means that we are deactivating old state and returning to Normal state
        // this should not happen for Normal state, so here is check, for this
        if (requiredState == CityViewActiveState.Normal)
        {
            Debug.LogError("Unexpected condition");
        }
        // 1st return previous state to Normal, if it is already not Normal
        // this only needed if we are activating new state, while previous state was not normal
        if ((cityViewActiveState != CityViewActiveState.Normal) && (requiredState != cityViewActiveState) && (doActivate == true))
        {
            SetActiveState(cityViewActiveState, false);
        }
        // Update 
        cityViewActiveState = requiredState;
        // instruct party and garnizon panels to highlight differently cells with and without units
        // loop through all active hero parties
        foreach (HeroPartyUI heroPartyUI in transform.parent.GetComponentsInChildren<HeroPartyUI>())
        {
            // Set active state (highlight)
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                    break;
                case CityViewActiveState.ActiveHeal:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                    break;
                case CityViewActiveState.ActiveResurect:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                    break;
                case CityViewActiveState.ActiveUnitDrag:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveUnitDrag(doActivate);
                    break;
                default:
                    Debug.LogError("Unknown condition");
                    break;
            }
        }
        // verfiy if there is a city garnizon == we are in city edit mode and not in hero party edit mode
        // and verify if there is already party in city
        if (transform.GetComponentInParent<UIManager>())
        {
            if ((transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false) != null)
                && (transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Party, false) == null))
            {
                // activate hire hero panel
                transform.parent.Find("HireHeroPanel").gameObject.SetActive(true);
            }
            else
            {
                // deactivate hire hero panel
                transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
            }
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

    public void DeactivateActiveToggle()
    {
        // Deactivate Toggle
        transform.root.Find("MiscUI/BottomControlPanel/ToggleGroup").GetComponent<TextToggleGroup>().GetSelectedToggle().OnTurnOff.Invoke();
        transform.root.Find("MiscUI/BottomControlPanel/ToggleGroup").GetComponent<TextToggleGroup>().DeselectToggle();
    }

    public void ActOnHeroEnteringCity()
    {
        // Instruct Focus panel to update info
        transform.parent.Find("LeftFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.Init);
        // Disable Hire leader panel
        transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
    }
    #endregion States

    #region Hire Unit

    bool VerifyIfPlayerHasEnoughGoldToBuyUnit(PartyUnit hiredUnitTemplate)
    {
        bool result = false;
        int requiredGold = hiredUnitTemplate.UnitCost;
        //  Verify if player has enough gold
        if (TurnsManager.Instance.GetActivePlayer().PlayerGold >= requiredGold)
        {
            result = true;
        }
        else
        {
            // display message that is not enough gold
            NotificationPopUp notificationPopup = transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>();
            if (hiredUnitTemplate.IsLeader)
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

    #region Hire Hero

    HeroPartyUI CreateNewPartyInCity()
    {
        // create and update Hero Party panel in UI, parent it to city UI
        GameObject heroPartyPanelTemplate = transform.root.Find("Templates/Obj/HeroParty").gameObject;
        // create new party instance in city
        HeroParty newHeroParty = Instantiate(heroPartyPanelTemplate, City.transform).GetComponent<HeroParty>();
        // Set party mode
        newHeroParty.PartyMode = PartyMode.Party;
        // Activate new party
        newHeroParty.gameObject.SetActive(true);
        // Get LeftHeroParty UI
        HeroPartyUI leftHeroPartyUI = transform.parent.Find("LeftHeroParty").GetComponent<HeroPartyUI>();
        // Set link to HeroParty
        leftHeroPartyUI.LHeroParty = newHeroParty;
        // return new party UI as result
        return leftHeroPartyUI;
    }

    void SetHeroPartyRepresentationOnTheMap(HeroParty newLeaderParty)
    {
        // Create Hero's object on the map
        // create and update Hero Party panel in UI, (no - parent it to city UI)
        //Transform parentCityOnMap = map.Find(transform.name);
        // Get map transform
        Transform map = transform.root.Find("MapScreen/Map");
        // Get hero party on map UI template
        GameObject heroPartyOnMapUITemplate = transform.root.Find("Templates/UI/HeroOnMap").gameObject;
        // Create new hero party on map ui
        MapHero newPartyOnMapUI = Instantiate(heroPartyOnMapUITemplate, map).GetComponent<MapHero>();
        // set it to the same position as the parent city
        newPartyOnMapUI.transform.position = City.LMapCity.transform.position;
        // activate new party on map UI
        newPartyOnMapUI.gameObject.SetActive(true);
        // Link HeroParty to the hero party on the map
        newPartyOnMapUI.LHeroParty = newLeaderParty;
        // Link hero on the map to HeroParty
        newLeaderParty.LMapHero = (newPartyOnMapUI);
        // Link hero on the map to city on the map
        City.LMapCity.GetComponent<MapCity>().LMapHero = newPartyOnMapUI.GetComponent<MapHero>();
        // Link city on the map to hero on the map
        newPartyOnMapUI.GetComponent<MapHero>().lMapCity = City.LMapCity;
        // move hero party to the back, so its UI label is not covering city's UI label
        newPartyOnMapUI.transform.SetAsFirstSibling();
    }

    string GetHiredLeaderDestinationSlotName(PartyUnit hiredUnitTemplate)
    {
        // verify if hired unit size is single or double
        if (hiredUnitTemplate.UnitSize == UnitSize.Double)
        {
            // Double unit size
            return "Wide";
        }
        else
        {
            // Single unit size
            // verify if hired unit is mele or ranged
            if (hiredUnitTemplate.UnitPowerDistance == UnitPowerDistance.Mele)
            {
                // Place mele units in front row
                return "Front";
            }
            else
            {
                // Place ranged unit into back row
                return "Back";
            }
        }
    }

    PartyUnit HireGenericUnit(PartyUnit hiredUnitTemplate, HeroParty heroParty, Transform destinationUnitSlotTransform, bool doCreateUI = true)
    {
        // create unit in HeroParty
        PartyUnit newPartyUnit = Instantiate(hiredUnitTemplate, heroParty.transform).GetComponent<PartyUnit>();
        // Set new unit cell address
        newPartyUnit.PartyUnitData.unitCellAddress = destinationUnitSlotTransform.GetComponent<UnitSlot>().GetUnitCellAddress();
        // take gold from player
        TurnsManager.Instance.GetActivePlayer().PlayerGold -= hiredUnitTemplate.UnitCost;
        if (doCreateUI)
        {
            // create unit canvas in unit slot
            PartyUnitUI partyUnitUI = Instantiate(unitCanvasTemplate, destinationUnitSlotTransform).GetComponent<PartyUnitUI>();
            // link unit to unit canvas
            partyUnitUI.LPartyUnit = newPartyUnit;
            // Activate PartyUnitUI (canvas)
            partyUnitUI.gameObject.SetActive(true);
        }
        // return result
        return newPartyUnit;
    }

    void HirePartyLeader(PartyUnit hiredUnitTemplate, bool doCreateUI = true)
    {
        // create new party
        HeroPartyUI newLeaderPartyUI = CreateNewPartyInCity();
        // Get unit's slot transform
        Transform newUnitParentSlot = newLeaderPartyUI.GetComponentInChildren<PartyPanel>(true).GetUnitSlotTr("Middle", GetHiredLeaderDestinationSlotName(hiredUnitTemplate));
        // Hire unit, but do not create canvas, because it will be done automatically by HeroParty on enable
        PartyUnit newPartyUnit = HireGenericUnit(hiredUnitTemplate, newLeaderPartyUI.LHeroParty, newUnitParentSlot, false);
        // Create hero's representation on the map
        // prerequisites: party leader should be created beforehand
        SetHeroPartyRepresentationOnTheMap(newLeaderPartyUI.LHeroParty);
        if (doCreateUI)
        {
            // Activate HeroPartyUI
            newLeaderPartyUI.gameObject.SetActive(true);
            // trigger onDisable focus panel function so it reset its state
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().OnDisable();
            // link party leader to the Left Focus panel so it can use it to fill in information
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = newPartyUnit.gameObject;
            // trigger onEnable focus panel so it get linked unit data and update its information
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().OnEnable();
            //// Instruct Focus panel to update info
            //transform.parent.Find("LeftFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HirePartyLeader);
            // Disable Hire leader panel
            transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
        }
    }

    #endregion Hire Hero

    #region Hire Single Unit

    bool VerifySingleUnitHire(PartyUnit selectedUnit)
    {
        bool result = true;
        // this is actually not required, because
        // if number of units in city reaches maximum, 
        // then hire unit button is disabled
        // but let it be, just in case
        return result;
    }

    void HireSingleUnit(Transform callerCell, PartyUnit hiredUnitTemplate, bool doCreateUI = true)
    {
        if (VerifySingleUnitHire(hiredUnitTemplate))
        {
            // create unit
            HireGenericUnit(hiredUnitTemplate, City.GetHeroPartyByMode(PartyMode.Garnizon), callerCell.Find("UnitSlot"), doCreateUI);
            if (doCreateUI)
            {
                // Update city garnizon panel to fill in required information and do required adjustments;
                transform.GetComponentInParent<UIManager>().GetHeroPartyUIByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true).OnChange(PartyPanel.ChangeType.HireSingleUnit, callerCell);
                // Instruct Right focus panel to update information
                transform.parent.Find("RightFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HireSingleUnit);
            }
        }
    }
    #endregion Hire Single Unit

    #region Hire Double unit

    bool VerifyDoubleUnitHire(Transform callerCell, PartyUnit selectedUnit)
    {
        bool result = false;
        PartyPanel partyPanel = transform.GetComponentInParent<UIManager>().GetHeroPartyUIByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true);
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

    void HireDoubleUnit(Transform callerCell, PartyUnit hiredUnitTemplate, bool doCreateUI = true)
    {
        if (VerifyDoubleUnitHire(callerCell, hiredUnitTemplate))
        {
            // get parent for new cell
            // if it is double size, then place it in the wide cell
            // if hired unit is double unit, then we actually need to change its parent to the wide
            // hierarchy: [Top/Middle/Bottom panel]-[Front/Back/Wide]-callerCell
            // create unit
            HireGenericUnit(hiredUnitTemplate, City.GetHeroPartyByMode(PartyMode.Garnizon), callerCell.parent.Find("Wide/UnitSlot"), doCreateUI);
            if (doCreateUI)
            {
                // update panel
                transform.GetComponentInParent<UIManager>().GetHeroPartyUIByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true).OnChange(PartyPanel.ChangeType.HireDoubleUnit, callerCell);
                // Instruct Right focus panel to update information
                transform.parent.Find("RightFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HireDoubleUnit);
            }
        }
    }

    #endregion Hire Double unit

    public void HireUnit(Transform callerCell, UnitType hiredUnitType, bool doCreateUI = true)
    {
        // get template for selected unit type
        PartyUnit hiredUnitTemplate = transform.root.Find("Templates").GetComponent<TemplatesManager>().GetPartyUnitTemplateByType(hiredUnitType).GetComponent<PartyUnit>();
        // 1 do basic verications, which are applicable to every hired unit
        if (VerifyIfPlayerHasEnoughGoldToBuyUnit(hiredUnitTemplate))
        {
            // 2 act based on the unit type: leader or normal unit
            if (hiredUnitTemplate.IsLeader)
            {
                HirePartyLeader(hiredUnitTemplate, doCreateUI);
            }
            else
            {
                // act based on the unit type
                if (UnitSize.Single == hiredUnitTemplate.UnitSize)
                {
                    HireSingleUnit(callerCell, hiredUnitTemplate, doCreateUI);
                }
                else
                {
                    HireDoubleUnit(callerCell, hiredUnitTemplate, doCreateUI);
                }
            }
        }
    }

    #endregion Hire Unit

    #region Dismiss unit

    PartyPanel GetUnitsParentPartyPanel(Transform unitCell)
    {
        // structure: 2PartyPanel-1[Top/Middle/Bottom]-[Front/Wide/Back]-UnitSlot-1UnitCanvas-unit
        return unitCell.transform.parent.parent.GetComponent<PartyPanel>();
    }

    public void DismissPartyLeader(UnitSlot unitSlot)
    {
        // Get hero Party UI
        // Structure: LeftHeroParty-3PartyPanel-2Top/Middle/Bottom(Row)-1Back/Front/Wide(Cell)-UnitSlot(this)
        HeroPartyUI heroPartyUI = unitSlot.transform.parent.parent.parent.parent.GetComponent<HeroPartyUI>();
        // Get Hero Party
        HeroParty heroParty = heroPartyUI.LHeroParty;
        // Get Focus Panel
        FocusPanel focusPanel = GetComponentInParent<UIManager>().GetFocusPanelByHeroParty(heroParty);
        // Get Map hero UI
        MapHero mapHero = heroPartyUI.LHeroParty.LMapHero;
        // Deactivate HeroParty UI
        heroPartyUI.gameObject.SetActive(false);
        // Deactivate Focus Panel
        focusPanel.gameObject.SetActive(false);
        // Destroy hero's represetnation on map
        Destroy(mapHero.gameObject);
        // Destroy hero's party
        Destroy(heroParty.gameObject);
        // Verify if we are in city and not in hero edit mode
        if (GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false) != null)
        {
            // Enable Hire leader panel
            transform.parent.Find("HireHeroPanel").gameObject.SetActive(true);
        }
        // Activate focus panel again to display NoPartyInfo
        focusPanel.gameObject.SetActive(true);
    }

    public void DismissGenericUnit(UnitSlot unitSlot)
    {
        // get PartyUnit UI
        PartyUnitUI unitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
        // get PartyUnit
        PartyUnit partyUnit = unitUI.LPartyUnit;
        // Get Unit size
        UnitSize unitSize = partyUnit.UnitSize;
        // Get Party Unit HeroParty
        //HeroParty heroParty = partyUnit.GetComponentInParent<HeroParty>();
        // 1 get all required variables, before removing unit
        Transform unitCell = unitSlot.transform.parent;
        PartyPanel partyPanel = GetUnitsParentPartyPanel(unitCell);
        // 2 destory unit canvas, where it is linked to
        Destroy(unitUI.gameObject);
        //unitSlot.transform.DetachChildren(); // this is needed otherwise child count will remain the same, because object is destroyed after Update()
        // 3 and put it to recycle bin, because otherwise city.GetNumberOfPresentUnits() will return wrong number of units, because because object is actually destroyed after Update()
        partyUnit.transform.SetParent(transform.root.GetComponentInChildren<RecycleBin>().transform);
        // and destory party unit itself
        Destroy(partyUnit.gameObject);
        // Update party panel
        // act based on the unit size
        if (unitSize == UnitSize.Single)
        {
            partyPanel.OnChange(PartyPanel.ChangeType.DismissSingleUnit, unitCell);
        }
        else
        {
            partyPanel.OnChange(PartyPanel.ChangeType.DismissDoubleUnit, unitCell);
        }
        // if parent Party panel is in Garnizon state, then update focus panel
        if (PartyMode.Garnizon == partyPanel.PartyMode)
        {
            // Instruct focus panel linked to a city to update information
            // act based on the unit size
            if (unitSize == UnitSize.Single)
            {
                GetComponentInParent<UIManager>().GetFocusPanelByCity(City).OnChange(FocusPanel.ChangeType.DismissSingleUnit);
            }
            else
            {
                GetComponentInParent<UIManager>().GetFocusPanelByCity(City).OnChange(FocusPanel.ChangeType.DismissDoubleUnit);
            }
            // Activate hire unit buttons again
            SetHireUnitPnlButtonActive(true);
        }
    }

    public void DimissUnit(UnitSlot unitSlot)
    {
        Debug.Log("Dismiss unit");
        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        bool wasLeader = unit.IsLeader;
        if (wasLeader)
        {
            DismissPartyLeader(unitSlot);
        }
        else
        {
            DismissGenericUnit(unitSlot);
        }
        // disable dismiss mode and return to normal mode
        //ReturnToNomalState();
    }
    #endregion Dismiss unit

    // todo: fix duplicate function in PartyPanel
    public void SetHireUnitButtonActiveByCell(bool doActivate, string cellAddress)
    {
        Debug.Log("Set hire unit button at " + cellAddress + " " + doActivate.ToString());
        transform.root.Find("MiscUI/HireCommonUnitButtons/" + cellAddress).GetComponentInChildren<HirePartyUnitButton>(true).gameObject.SetActive(doActivate);
    }

    void BringHireUnitPnlButtonToTheFront()
    {
        transform.root.GetComponentInChildren<UIManager>().transform.Find("HireCommonUnitButtons").SetAsLastSibling();
        // verify if notification pop-up window is active NotificationPopUp
        if (transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<NotificationPopUp>())
        {
            // set nofiction pop-up as last sibling
            transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<NotificationPopUp>().transform.SetAsLastSibling();
        }
    }

    public void SetHireUnitPnlButtonActive(bool activate)
    {
        // get garnizon UI
        HeroPartyUI garnizonUI = GetComponentInParent<UIManager>().GetHeroPartyUIByMode(PartyMode.Garnizon);
        // this is only needed in garnizon mode, only in this mode hire buttons are present
        // verify if garnizon is present
        if (garnizonUI)
        {
            Debug.Log("Activate or Deactivate Hire Unit Buttons in City Garnizon");
            foreach (UnitSlot unitSlot in garnizonUI.GetComponentsInChildren<UnitSlot>(true))
            {
                // verify if we are not checking wide cell, because there is no + button in wide cells
                if (unitSlot.GetComponent<UnitSlotDropHandler>().CellSize != UnitSize.Double)
                {
                    // verify if we need to activate
                    if (activate
                        // Verify if city capacity is enough
                        && (City.GetUnitsCapacity() > garnizonUI.LHeroParty.GetNumberOfPresentUnits())
                        // verify if drop slot doesn't have an !active! unit in it
                        && (!unitSlot.GetComponentInChildren<PartyUnitUI>(false))
                        // verify if wide cell is not active = occupied in the same row
                        && (!unitSlot.transform.parent.parent.Find("Wide").gameObject.activeSelf)
                        )
                    {
                        //Debug.Log("Activate + button in " + horisontalPanel + "/" + cell + " cell");
                        SetHireUnitButtonActiveByCell(true, unitSlot.GetUnitCellAddress());
                        // And bring panel to the front
                        BringHireUnitPnlButtonToTheFront();
                    }
                    else
                    {
                        //Debug.Log("Deactivate + button in " + horisontalPanel + "/" + cell + " cell");
                        SetHireUnitButtonActiveByCell(false, unitSlot.GetUnitCellAddress());
                    }
                }
            }
        }
    }
}
