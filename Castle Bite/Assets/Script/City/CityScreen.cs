using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityScreen : MonoBehaviour {
    [SerializeField]
    City mCity;
    [SerializeField]
    CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;

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
        mCity = city;
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

    PartyUnit CreateUnit(Transform newUnitParentSlot, PartyUnit hiredUnitTemplate)
    {
        // Get city Garnizon
        // this can be only city Garnizon party
        HeroParty cityGarnizon = GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false);
        // Create new unit and place it in parent transform
        PartyUnit newPartyUnit = Instantiate(hiredUnitTemplate, cityGarnizon.transform);
        // Set new unit cell address
        newPartyUnit.PartyUnitData.unitCellAddress = newUnitParentSlot.GetComponent<UnitSlot>().GetUnitCellAddress();
        // Create new instance of unity draggable canvas and set it as unit's parent
        GameObject unitCanvasTemplate = transform.root.Find("Templates/UI/UnitCanvas").gameObject;
        Transform newUnitCanvasTr = Instantiate(unitCanvasTemplate, newUnitParentSlot).transform;
        // enable it
        newUnitCanvasTr.gameObject.SetActive(true);
        // Create new unit link to party unit in UI
        newUnitCanvasTr.GetComponent<PartyUnitUI>().LPartyUnit = newPartyUnit;
        // return new unit link
        return newPartyUnit;
    }

    #region Hire Hero

    GameObject CreateNewPartyInCity()
    {
        // create and update Hero Party panel in UI, parent it to city UI
        GameObject heroPartyPanelTemplate = transform.root.Find("Templates/Obj/HeroParty").gameObject;
        // create new party instance in city
        GameObject newParty = Instantiate(heroPartyPanelTemplate, City.transform);
        // Set party mode
        newParty.GetComponent<HeroParty>().PartyMode = PartyMode.Party;
        // Activate new party
        newParty.SetActive(true);
        // Get LeftHeroParty UI
        HeroPartyUI leftHeroPartyUI = transform.parent.Find("LeftHeroParty").GetComponent<HeroPartyUI>();
        // Set link to HeroParty
        leftHeroPartyUI.LHeroParty = newParty.GetComponent<HeroParty>();
        // Activate it
        leftHeroPartyUI.gameObject.SetActive(true);
        // return new party UI as result
        return leftHeroPartyUI.gameObject;
    }

    void SetHeroPartyRepresentationOnTheMap(HeroParty newLeaderParty)
    {
        // Create Hero's object on the map
        Transform map = transform.root.Find("MapScreen/Map");
        //  create and update Hero Party panel in UI, (no - parent it to city UI)
        GameObject heroPartyOnMapUITemplate = transform.root.Find("Templates/UI/HeroOnMap").gameObject;
        Transform parentCityOnMap = map.Find(transform.name);
        GameObject newPartyOnMapUI = Instantiate(heroPartyOnMapUITemplate, map);
        // set it to the same position as the parent city
        newPartyOnMapUI.transform.position = parentCityOnMap.position;
        // activate new party UI panel
        newPartyOnMapUI.SetActive(true);
        // Link hero to the hero on the map
        MapHero heroOnMap = newPartyOnMapUI.GetComponent<MapHero>();
        heroOnMap.LinkedPartyTr = newLeaderParty.transform;
        // Link hero on the map to hero
        newLeaderParty.SetLinkedPartyOnMap(heroOnMap);
        // Link hero on the map to city on the map
        parentCityOnMap.GetComponent<MapCity>().LinkedPartyOnMapTr = newPartyOnMapUI.transform;
        // And do the opposite 
        // Link city on the map to hero on the map
        newPartyOnMapUI.GetComponent<MapHero>().linkedCityOnMapTr = parentCityOnMap;
        // bring city to the front
        // . - this does not work as expected, if hero is highered in other city and move to other city, then we have a problem that hero is on top
        //parentCityOnMap.SetAsLastSibling();
        // move hero party to the back instead
        heroOnMap.transform.SetAsFirstSibling();
    }

    void HirePartyLeader(PartyUnit hiredUnitTemplate)
    {
        // create new party
        GameObject newLeaderPartyUI = CreateNewPartyInCity();
        // set middle right panel as hero's parent transform. Place it to the canvas, which later will be dragg and droppable
        Transform newUnitParentSlot = newLeaderPartyUI.GetComponentInChildren<PartyPanel>().GetUnitSlotTr("Middle", "Back");
        // create new unit
        PartyUnit newPartyUnit = CreateUnit(newUnitParentSlot, hiredUnitTemplate);
        // Update Left focus with information from new unit;
        // activate required UIs and also fill in city's left focus panels
        // link party leader to the Left Focus panel
        // so it can use it to fill in information
        transform.parent.Find("LeftFocus").GetComponent<FocusPanel>().focusedObject = newPartyUnit.gameObject;
        // fill in city's left focus with information from the hero
        // Instruct Focus panel to update info
        transform.parent.Find("LeftFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HirePartyLeader);
        // Disable Hire leader panel
        transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
        // take gold from player
        TurnsManager.Instance.GetActivePlayer().PlayerGold -= hiredUnitTemplate.UnitCost;
        // Create hero's representation on the map
        SetHeroPartyRepresentationOnTheMap(newLeaderPartyUI.GetComponent<HeroPartyUI>().LHeroParty);
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

    void HireSingleUnit(Transform callerCell, PartyUnit hiredUnitTemplate)
    {
        if (VerifySingleUnitHire(hiredUnitTemplate))
        {
            // get parent for new cell
            Transform parentTransform = callerCell.Find("UnitSlot");
            // create unit
            // PartyUnit newPartyUnit = CreateUnit(parentTransform, hiredUnitTemplate);
            CreateUnit(parentTransform, hiredUnitTemplate);
            // Update city garnizon panel to fill in required information and do required adjustments;
            transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true).OnChange(PartyPanel.ChangeType.HireSingleUnit, callerCell);
            // Instruct Right focus panel to update information
            transform.parent.Find("RightFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HireSingleUnit);
            // take gold from player
            TurnsManager.Instance.GetActivePlayer().PlayerGold -= hiredUnitTemplate.UnitCost;
        }
    }
    #endregion Hire Single Unit

    #region Hire Double unit

    bool VerifyDoubleUnitHire(Transform callerCell, PartyUnit selectedUnit)
    {
        bool result = false;
        PartyPanel partyPanel = transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true);
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
            // hierarchy: [Top/Middle/Bottom panel]-[Front/Back/Wide]-callerCell
            Transform newUnitParentSlot = callerCell.parent.Find("Wide/UnitSlot");
            // create unit
            // PartyUnit newPartyUnit = CreateUnit(newUnitParentSlot, hiredUnitTemplate);
            CreateUnit(newUnitParentSlot, hiredUnitTemplate);
            // update panel
            transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>(true).OnChange(PartyPanel.ChangeType.HireDoubleUnit, callerCell);
            // Instruct Right focus panel to update information
            transform.parent.Find("RightFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.HireDoubleUnit);
            // take gold from player
            TurnsManager.Instance.GetActivePlayer().PlayerGold -= hiredUnitTemplate.UnitCost;
        }
    }

    #endregion Hire Double unit

    public void HireUnit(Transform callerCell, UnitType hiredUnitType)
    {
        // get template for selected unit type
        PartyUnit hiredUnitTemplate = transform.root.Find("Templates").GetComponent<TemplatesManager>().GetPartyUnitTemplateByType(hiredUnitType).GetComponent<PartyUnit>();
        // 1 do basic verications, which are applicable to every hired unit
        if (VerifyIfPlayerHasEnoughGoldToBuyUnit(hiredUnitTemplate))
        {
            // 2 act based on the unit type: leader or normal unit
            if (hiredUnitTemplate.IsLeader)
            {
                HirePartyLeader(hiredUnitTemplate);
            }
            else
            {
                // act based on the unit type
                if (UnitSize.Single == hiredUnitTemplate.UnitSize)
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
        MapHero mapHero = heroPartyUI.LHeroParty.GetLinkedPartyOnMap();
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
            GetComponentInParent<UIManager>().GetHeroPartyUIByMode(PartyMode.Garnizon).GetComponentInChildren<PartyPanel>().SetHireUnitPnlButtonActive(true);
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

}
