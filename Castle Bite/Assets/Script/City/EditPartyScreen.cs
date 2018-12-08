using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EditPartyScreenActiveState
{
    Normal,
    ActiveHeal,
    ActiveResurect,
    ActiveDismiss,
    ActiveHeroEquipment,
    ActiveUnitDrag,
    ActiveItemDrag
};

public class EditPartyScreen : MonoBehaviour {
    [SerializeField]
    City lCity;
    [SerializeField]
    HeroParty[] lHeroParties;
    [SerializeField]
    EditPartyScreenActiveState editPartyScreenActiveState = EditPartyScreenActiveState.Normal;
    [SerializeField]
    GameObject unitCanvasTemplate;


    public City LCity
    {
        get
        {
            return lCity;
        }
    }

    public HeroParty[] LHeroParties
    {
        get
        {
            return lHeroParties;
        }
    }

    public EditPartyScreenActiveState EditPartyScreenActiveState
    {
        get
        {
            return editPartyScreenActiveState;
        }
    }

    public void SetEditPartyScreenActive(City city)
    {
        // save link to the city
        lCity = city;
        // activate this city screen
        gameObject.SetActive(true);
    }

    public void SetEditPartyScreenActive(HeroParty heroParty)
    {
        // save link to the city
        lHeroParties = new HeroParty[] { heroParty };
        // activate this city screen
        gameObject.SetActive(true);
    }

    public void SetEditPartyScreenActive(HeroParty heroParty1, HeroParty heroParty2)
    {
        // save link to the city
        lHeroParties = new HeroParty[] { heroParty1, heroParty2 };
        // activate this city screen
        gameObject.SetActive(true);
    }

    void ActivateCityView()
    {
        // get hero party in city, if it is present
        HeroParty heroParty = LCity.GetHeroPartyByMode(PartyMode.Party);
        HeroPartyUI heroPartyUI = null;
        // verify if there is party in a city
        if (heroParty != null)
        {
            // Get HeroParty UI
            heroPartyUI = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>();
            // assign HeroParty to left hero party UI
            heroPartyUI.GetComponent<HeroPartyUI>().LHeroParty = heroParty;
            // activate left hero party UI
            heroPartyUI.gameObject.SetActive(true);
            // assign party leader to left focus panel
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = heroPartyUI.GetPartyLeaderUI().gameObject;
            // activate left Focus panel
            transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(true);
        }
        else
        {
            // activate left Focus panel
            // without linked hero it will trigger no-party info
            transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(true);
            // activate hire hero panel
            transform.root.Find("MiscUI/HireHeroPanel").gameObject.SetActive(true);
        }
        // Get HeroParty UI
        heroPartyUI = transform.root.Find("MiscUI/RightHeroParty").GetComponent<HeroPartyUI>();
        // assign City garnizon HeroParty to right hero party UI
        heroPartyUI.LHeroParty = LCity.GetHeroPartyByMode(PartyMode.Garnizon);
        // activate right hero party UI
        heroPartyUI.gameObject.SetActive(true);
        // activate hire common units panel
        transform.root.Find("MiscUI/HireCommonUnitButtons").gameObject.SetActive(true);
        // assign city to right focus panel
        transform.root.Find("MiscUI/RightFocus").GetComponent<FocusPanel>().focusedObject = LCity.gameObject;
        // activate right Focus panel
        transform.root.Find("MiscUI/RightFocus").gameObject.SetActive(true);
    }

    void ActivateHeroPartiesView()
    {
        // check check if we have one party
        if (LHeroParties.Length <= 1)
        {
            // We have at least one party
            // Activate left view with this party
            // Get hero party
            HeroParty heroParty = LHeroParties[0];
            // we have only one party
            // Get HeroParty UI
            HeroPartyUI heroPartyUI = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>();
            // assign HeroParty to the left hero party UI
            heroPartyUI.LHeroParty = heroParty;
            // activate left hero party UI
            heroPartyUI.gameObject.SetActive(true);
            // assign party leader UI to the left focus panel
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = heroPartyUI.GetPartyLeaderUI().gameObject;
            // activate left Focus panel
            transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(true);
        }
        // Check if we have 2 parties
        if (LHeroParties.Length == 2)
        {
            // we have 2 parties
            // Activate right view with this party
            // Get hero party
            HeroParty heroParty = LHeroParties[1];
            // Get HeroParty UI
            HeroPartyUI heroPartyUI = transform.root.Find("MiscUI/RightHeroParty").GetComponent<HeroPartyUI>();
            // assign HeroParty to the right hero party UI
            heroPartyUI.LHeroParty = heroParty;
            // activate right hero party UI
            heroPartyUI.gameObject.SetActive(true);
            // assign party leader UI to the right focus panel
            transform.root.Find("MiscUI/RightFocus").GetComponent<FocusPanel>().focusedObject = heroPartyUI.GetPartyLeaderUI().gameObject;
            // activate right Focus panel
            transform.root.Find("MiscUI/RightFocus").gameObject.SetActive(true);
        }
    }

    void SetRequiredComponentsActive(bool doActivate)
    {
        // Activate/Deactivate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(doActivate);
        // Activate/Deactivate common controls
        //transform.root.Find("MiscUI/BottomControlPanel").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(doActivate);
        if (LCity != null)
        {
            // Activate/Deactivate city controls
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(doActivate);
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(doActivate);
        }
        // Activate/Deactivate hero Parties menus and Focus panels
        if (doActivate)
        {
            // check if we are in city edit mode
            if (LCity != null)
            {
                // Activate components needed for city view
                ActivateCityView();
            }
            else if (LHeroParties != null)
            {
                // Activate components needed for hero view
                ActivateHeroPartiesView();
            }
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
        // Unlink linked objects
        if (LCity != null)
        {
            lCity = null;
        }
        if (LHeroParties != null)
        {
            lHeroParties = null;
        }
    }

    #region States

    public void SetActiveStateDismiss(bool doActivate)
    {
        SetActiveState(EditPartyScreenActiveState.ActiveDismiss, doActivate);
    }

    public void SetActiveStateHeal(bool doActivate)
    {
        SetActiveState(EditPartyScreenActiveState.ActiveHeal, doActivate);
    }

    public void SetActiveStateResurect(bool doActivate)
    {
        SetActiveState(EditPartyScreenActiveState.ActiveResurect, doActivate);
    }

    public void SetActiveState(EditPartyScreenActiveState requiredState, bool doActivate)
    {
        // if doActivate is true,
        // then we are really swiching to this new state
        // otherwise it means that we are deactivating old state and returning to Normal state
        // this should not happen for Normal state, so here is check, for this
        if (requiredState == EditPartyScreenActiveState.Normal)
        {
            Debug.LogError("Unexpected condition");
        }
        // 1st return previous state to Normal, if it is already not Normal
        // this only needed if we are activating new state, while previous state was not normal
        if ((editPartyScreenActiveState != EditPartyScreenActiveState.Normal) && (requiredState != editPartyScreenActiveState) && (doActivate == true))
        {
            SetActiveState(editPartyScreenActiveState, false);
        }
        // Update 
        editPartyScreenActiveState = requiredState;
        // instruct party and garnizon panels to highlight differently cells with and without units
        // loop through all active hero parties
        foreach (HeroPartyUI heroPartyUI in transform.parent.GetComponentsInChildren<HeroPartyUI>())
        {
            // Set active state (highlight)
            switch (editPartyScreenActiveState)
            {
                case EditPartyScreenActiveState.ActiveDismiss:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                    break;
                case EditPartyScreenActiveState.ActiveHeal:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                    break;
                case EditPartyScreenActiveState.ActiveResurect:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                    break;
                case EditPartyScreenActiveState.ActiveUnitDrag:
                    heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveUnitDrag(doActivate);
                    break;
                case EditPartyScreenActiveState.ActiveItemDrag:
                    // verify if we are not in equipment view mode, when party panel is disabled
                    if (heroPartyUI.GetComponentInChildren<PartyPanel>())
                        heroPartyUI.GetComponentInChildren<PartyPanel>().SetActiveItemDrag(doActivate);
                    else if (transform.root.Find("MiscUI").GetComponentInChildren<HeroEquipment>())
                        transform.root.Find("MiscUI").GetComponentInChildren<HeroEquipment>().SetActiveItemDrag(doActivate);
                    else
                        Debug.LogWarning("Unknown condition");
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
        if (doActivate)
        {
            switch (requiredState)
            {
                case EditPartyScreenActiveState.ActiveDismiss:
                    CursorController.Instance.SetDismissUnitCursor();
                    break;
                case EditPartyScreenActiveState.ActiveHeal:
                    CursorController.Instance.SetHealUnitCursor();
                    break;
                case EditPartyScreenActiveState.ActiveResurect:
                    CursorController.Instance.SetResurectUnitCursor();
                    break;
                case EditPartyScreenActiveState.ActiveHeroEquipment:
                    CursorController.Instance.SetInvenotryUnitCursor();
                    break;
                case EditPartyScreenActiveState.ActiveUnitDrag:
                    CursorController.Instance.SetDragUnitCursor();
                    break;
                case EditPartyScreenActiveState.ActiveItemDrag:
                    CursorController.Instance.SetGrabHandCursor();
                    break;
                default:
                    Debug.LogError("Unknown condition");
                    break;
            }
        }
        else
        {
            CursorController.Instance.SetNormalCursor();
            // if doActivate was false
            // this means that we need to return to normal state.
            // that what we did by passing doActivate variable to other functions
            // so we only need to set here state to normal
            editPartyScreenActiveState = EditPartyScreenActiveState.Normal;
        }
    }

    public void DeactivateActiveToggle()
    {
        // Deactivate Toggle
        if (transform.root.Find("MiscUI/BottomControlPanel/ToggleGroup").GetComponent<TextToggleGroup>().GetSelectedToggle())
            transform.root.Find("MiscUI/BottomControlPanel/ToggleGroup").GetComponent<TextToggleGroup>().GetSelectedToggle().OnTurnOff.Invoke();
        transform.root.Find("MiscUI/BottomControlPanel/ToggleGroup").GetComponent<TextToggleGroup>().DeselectToggle();
    }

    //public void ActOnHeroEnteringCity()
    //{
    //    // Instruct Focus panel to update info
    //    transform.parent.Find("LeftFocus").GetComponent<FocusPanel>().OnChange(FocusPanel.ChangeType.Init);
    //    // Disable Hire leader panel
    //    transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
    //}
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

    HeroPartyUI CreateNewPartyInCity(City city = null)
    {
        // create and update Hero Party panel in UI, parent it to city UI
        // Set target city
        City targetCity = LCity;
        if (city != null)
        {
            targetCity = city;
        }
        // create new party instance in city
        HeroParty newHeroParty = Instantiate(ObjectsManager.Instance.HeroPartyTemplate, targetCity.transform).GetComponent<HeroParty>();
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

    void SetHeroPartyRepresentationOnTheMap(HeroParty newLeaderParty, City city = null)
    {
        // Create Hero's object on the map
        // create and update Hero Party panel in UI, (no - parent it to city UI)
        //Transform parentCityOnMap = map.Find(transform.name);
        // Create new hero party on map ui
        MapHero newPartyOnMapUI = Instantiate(ObjectsManager.Instance.HeroPartyOnMapTemplate, MapManager.Instance.transform).GetComponent<MapHero>();
        // Get city
        City targetCity = LCity;
        if (city != null)
        {
            targetCity = city;
        }
        // adjust position, because city and hero transform have differnet anchor points
        // Get ui world corners
        // 12
        // 03
        Vector3[] worldCorners = new Vector3[4];
        newPartyOnMapUI.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        // get map hero ui width and heigh
        float newPartyOnMapUIHeight = Mathf.Abs(worldCorners[3].x - worldCorners[0].x);
        float newPartyOnMapUIWidth = Mathf.Abs(worldCorners[1].y - worldCorners[0].y);
        Debug.Log(newPartyOnMapUIWidth + ":" + newPartyOnMapUIHeight);
        Vector3 positionAdjustment = new Vector3(newPartyOnMapUIHeight/2f, newPartyOnMapUIWidth/2f, 0);
        // set it to the same position as the parent city
        newPartyOnMapUI.transform.position = targetCity.LMapCity.transform.position + positionAdjustment;
        // activate new party on map UI
        newPartyOnMapUI.gameObject.SetActive(true);
        // Link HeroParty to the hero party on the map
        newPartyOnMapUI.LHeroParty = newLeaderParty;
        // Link hero on the map to HeroParty
        newLeaderParty.LMapHero = (newPartyOnMapUI);
        // Link hero on the map to city on the map
        targetCity.LMapCity.GetComponent<MapCity>().LMapHero = newPartyOnMapUI.GetComponent<MapHero>();
        // Link city on the map to hero on the map
        newPartyOnMapUI.GetComponent<MapHero>().lMapCity = targetCity.LMapCity;
        // move hero party to the back, so its UI label is not covering city's UI label
        // but it is in front of map slices
        newPartyOnMapUI.transform.SetSiblingIndex(1);
        // rename it
        newPartyOnMapUI.gameObject.name = newLeaderParty.GetPartyLeader().GivenName + " " + newLeaderParty.GetPartyLeader().UnitName + " Party";
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

    void HirePartyLeader(PartyUnit hiredUnitTemplate, bool doCreateUI = true, City city = null)
    {
        // create new party
        HeroPartyUI newHeroPartyUI = CreateNewPartyInCity(city);
        // Get unit's slot transform
        Transform newUnitParentSlot = newHeroPartyUI.GetComponentInChildren<PartyPanel>(true).GetUnitSlotTr("Middle", GetHiredLeaderDestinationSlotName(hiredUnitTemplate));
        // Hire unit, but do not create canvas, because it will be done automatically by HeroParty on enable
        PartyUnit newPartyLeader = HireGenericUnit(hiredUnitTemplate, newHeroPartyUI.LHeroParty, newUnitParentSlot, false);
        // Rename hero party, when we know the leader
        newHeroPartyUI.LHeroParty.gameObject.name = newPartyLeader.GivenName + " " + newPartyLeader.UnitName + " Party";
        // Create hero's representation on the map
        // prerequisites: party leader should be created beforehand
        SetHeroPartyRepresentationOnTheMap(newHeroPartyUI.LHeroParty, city);
        if (doCreateUI)
        {
            // Activate HeroPartyUI
            newHeroPartyUI.gameObject.SetActive(true);
            // trigger onDisable focus panel function so it reset its state
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().OnDisable();
            // link party leader to the Left Focus panel so it can use it to fill in information
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = newHeroPartyUI.GetPartyLeaderUI().gameObject;
            // trigger onEnable focus panel so it get linked unit data and update its information
            transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().OnEnable();
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
            HireGenericUnit(hiredUnitTemplate, LCity.GetHeroPartyByMode(PartyMode.Garnizon), callerCell.Find("UnitSlot"), doCreateUI);
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
            HireGenericUnit(hiredUnitTemplate, LCity.GetHeroPartyByMode(PartyMode.Garnizon), callerCell.parent.Find("Wide/UnitSlot"), doCreateUI);
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

    public void HireUnit(Transform callerCell, UnitType hiredUnitType, bool doCreateUI = true, City city = null)
    {
        // get template for selected unit type
        PartyUnit hiredUnitTemplate = transform.root.Find("Templates").GetComponent<TemplatesManager>().GetPartyUnitTemplateByType(hiredUnitType).GetComponent<PartyUnit>();
        // 1 do basic verications, which are applicable to every hired unit
        if (VerifyIfPlayerHasEnoughGoldToBuyUnit(hiredUnitTemplate))
        {
            // 2 act based on the unit type: leader or normal unit
            if (hiredUnitTemplate.IsLeader)
            {
                HirePartyLeader(hiredUnitTemplate, doCreateUI, city);
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

    IEnumerator ReturnToTheMapScreenWithAnimation()
    {
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Wait for all animations to finish
        yield return new WaitForSeconds(0.51f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate this screen
        gameObject.SetActive(false);
        // activate map screen
        MapMenuManager.Instance.gameObject.SetActive(true);
        MapManager.Instance.gameObject.SetActive(true);
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
        // verify if focus panel not deactivated already
        // .. find out why it is deactivated earlier and by whom
        if (focusPanel != null)
        {
            // Deactivate Focus Panel
            focusPanel.gameObject.SetActive(false);
        }
        // Destroy hero's represetnation on map
        Destroy(mapHero.gameObject);
        // Destroy hero's party
        Destroy(heroParty.gameObject);
        // Verify if we are in city and not in hero edit mode
        if (GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false) != null)
        {
            // Enable Hire leader panel
            transform.parent.Find("HireHeroPanel").gameObject.SetActive(true);
            // Activate focus panel again to display NoPartyInfo
            focusPanel.gameObject.SetActive(true);
        }
        else
        {
            // check if there was only one hero in this screen
            if (LHeroParties.Length == 1)
            {
                // return to the map view with animation
                StartCoroutine(ReturnToTheMapScreenWithAnimation());
            }
        }
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
                GetComponentInParent<UIManager>().GetFocusPanelByCity(LCity).OnChange(FocusPanel.ChangeType.DismissSingleUnit);
            }
            else
            {
                GetComponentInParent<UIManager>().GetFocusPanelByCity(LCity).OnChange(FocusPanel.ChangeType.DismissDoubleUnit);
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
        // Debug.Log("Set hire unit button at " + cellAddress + " " + doActivate.ToString());
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
            // Debug.Log("Activate or Deactivate Hire Unit Buttons in City Garnizon");
            foreach (UnitSlot unitSlot in garnizonUI.GetComponentsInChildren<UnitSlot>(true))
            {
                // verify if we are not checking wide cell, because there is no + button in wide cells
                if (unitSlot.GetComponent<UnitSlotDropHandler>().CellSize != UnitSize.Double)
                {
                    // verify if we need to activate
                    if (activate
                        // Verify if city capacity is enough
                        && (LCity.GetUnitsCapacity() > garnizonUI.LHeroParty.GetNumberOfPresentUnits())
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
