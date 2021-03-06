﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[Serializable]
//public class PartyPanelData : System.Object
//{
//    public PartyPanelMode partyPanelMode;
//    public PartyUnitData[] partyUnitsData; // initialized and used only during game save and load
//}

public enum MeleUnitBlockingCondition
{
    None,
    AttackerIsBlockedByFriendlyFrontRowUnits,
    TargetUnitIsProtectedByUnitAbove,
    TargetUnitIsProtectedByUnitBelow,
    TargetUnitIsProtectedByFriendlyUnitsInFrontRow
}

// Controls all operations with child panels
public class PartyPanel : MonoBehaviour {
    [SerializeField]
    GameEvent triggerUnitAbility;

    // Party panel row
    public enum Row
    {
        Top,
        Middle,
        Bottom
    }

    // Party panel cell
    public enum Cell
    {
        Front,
        Back,
        Wide
    }

    //[SerializeField]
    //PartyPanelData partyPanelData;
    Row[] horisontalPanels = { Row.Top, Row.Middle, Row.Bottom };
    Cell[] cells = { Cell.Front, Cell.Back, Cell.Wide };
    Cell[] cellsFront = { Cell.Front, Cell.Wide };
    Cell[] cellsBack = { Cell.Back, Cell.Wide };
    public enum ChangeType { Init, HireSingleUnit, HireDoubleUnit, HirePartyLeader, DismissSingleUnit, DismissDoubleUnit, DismissPartyLeader}

    // for battle
    //PartyUnitUI activeBattleUnitUI;
    public string deadStatusText = "Dead";
    public string levelUpStatusText = "Level up";
    bool isAIControlled = false;

    public bool IsAIControlled
    {
        get
        {
            return isAIControlled;
        }

        set
        {
            isAIControlled = value;
        }
    }

    //public PartyPanelData PartyPanelData
    //{
    //    get
    //    {
    //        return partyPanelData;
    //    }

    //    set
    //    {
    //        partyPanelData = value;
    //    }
    //}

    public Transform GetUnitSlotTr(Row row, Cell cell)
    {
        // Debug.Log(row + " " + cell);
        return transform.Find(row.ToString()).Find(cell.ToString()).Find("UnitSlot");
    }

    public PartyMode PartyMode
    {
        get
        {
            //return GetComponentInParent<HeroPartyUI>().LHeroParty.PartyMode;
            return transform.parent.GetComponent<HeroPartyUI>().LHeroParty.PartyMode;
        }
    }


    //public PartyPanelMode GetPanelMode()
    //{
    //    return panelMode;
    //}

    void OnEnable()
    {
        OnChange(ChangeType.Init, null);
    }

    public HeroParty GetHeroParty()
    {
        return GetComponentInParent<HeroPartyUI>().LHeroParty;
    }

    public City GetCity()
    {
        return GetHeroParty().GetComponentInParent<City>();
    }

    #region On Change: hire or dismiss unit, for unit edit mode

    //public void SetOnEditClickHandler(bool doActivate)
    //{
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                // Unit canvas is present
    //                if (doActivate)
    //                {
    //                    // add UnitOnBattleMouseHandler Component
    //                    GameObject unitCanvas = unitSlot.GetChild(0).gameObject;
    //                    // UnitDragHandler sc = unitCanvas.AddComponent<UnitDragHandler>() as UnitDragHandler;
    //                    unitCanvas.AddComponent<UnitDragHandler>();
    //                }
    //                else
    //                {
    //                    // remove UnitOnBattleMouseHandler Component
    //                    UnitDragHandler unitDragHandlerComponent = unitSlot.GetComponentInChildren<UnitDragHandler>();
    //                    Destroy(unitDragHandlerComponent);
    //                }
    //            }
    //        }
    //    }
    //}

    //public void SetOnBattleClickHandler(bool doActivate)
    //{
    //    foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
    //    {
    //        // Unit canvas is present
    //        if (doActivate)
    //        {
    //            // add UnitOnBattleMouseHandler Component
    //            partyUnitUI.gameObject.AddComponent<UnitOnBattleMouseHandler>();
    //        }
    //        else
    //        {
    //            // remove UnitOnBattleMouseHandler Component
    //            Destroy(partyUnitUI.GetComponent<UnitOnBattleMouseHandler>());
    //        }
    //    }
    //    //foreach (Row horisontalPanel in horisontalPanels)
    //    //{
    //    //    foreach (Cell cell in cells)
    //    //    {
    //    //        // verify if slot has an unit in it
    //    //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //    //        if (unitSlot.childCount > 0)
    //    //        {
    //    //            // Unit canvas is present
    //    //            if (doActivate)
    //    //            {
    //    //                // add UnitOnBattleMouseHandler Component
    //    //                GameObject unitCanvas = unitSlot.GetChild(0).gameObject;
    //    //                // UnitOnBattleMouseHandler sc = unitCanvas.AddComponent<UnitOnBattleMouseHandler>() as UnitOnBattleMouseHandler;
    //    //                unitCanvas.AddComponent<UnitOnBattleMouseHandler>();
    //    //            }
    //    //            else
    //    //            {
    //    //                // remove UnitOnBattleMouseHandler Component
    //    //                UnitOnBattleMouseHandler unitOnBattleMouseHandlerComponent = unitSlot.GetComponentInChildren<UnitOnBattleMouseHandler>();
    //    //                Destroy(unitOnBattleMouseHandlerComponent);
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //}

    string GetCellAddressStringByTransform(Transform cellTransform)
    {
        Debug.Log("Cell address string is " + cellTransform.parent.name + "/" + cellTransform.name);
        return cellTransform.parent.name + "/" + cellTransform.name;
    }

    // todo: fix duplicate function in cityscreen
    public void SetHireUnitButtonActiveByCell(bool doActivate, string cellAddress)
    {
        // verify if we are in city view by checking if there is HireCommonUnitButtons panel active
        if (transform.root.GetComponentInChildren<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon) != null)
        {
            Debug.Log("Set hire unit button at " + cellAddress + " " + doActivate.ToString());
            transform.root.GetComponentInChildren<UIManager>().transform.Find("HireCommonUnitButtons/" + cellAddress).GetComponentInChildren<HirePartyUnitButton>(true).gameObject.SetActive(doActivate);
        }
    }

    void OnHireSingleUnit(Transform changedCell)
    {
        // UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        // it is more reliable to use GetChild(0), because it is only one child there
        //Transform unitCanvas = changedCell.Find("UnitSlot").GetChild(0);
        //PartyUnitUI unitUI = changedCell.GetComponentInChildren<UnitSlot>().GetComponentInChildren<PartyUnitUI>();
        //// fill in highered object UI panel
        //unitUI.SetUnitCellInfoUI();
        //unitUI.transform.Find("Name").GetComponent<Text>().text = unitUI.LPartyUnit.GetUnitDisplayName();
        //changedCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = unitUI.LPartyUnit.UnitHealthCurr.ToString();
        //changedCell.Find("HPPanel/HPmax").GetComponent<Text>().text = unitUI.LPartyUnit.GetUnitEffectiveMaxHealth().ToString();
        // disable hire unit button
        SetHireUnitButtonActiveByCell(false, GetCellAddressStringByTransform(changedCell));
    }

    void OnHireDoubleUnit(Transform changedCell)
    {
        // Also we need to enable Wide panel, because by defaut it is disabled
        changedCell.parent.Find(Cell.Wide.ToString()).gameObject.SetActive(true);
        // And disable left and right panels
        changedCell.parent.Find(Cell.Front.ToString()).gameObject.SetActive(false);
        changedCell.parent.Find(Cell.Back.ToString()).gameObject.SetActive(false);
        // Disable hire unit buttons
        SetHireUnitButtonActiveByCell(false, changedCell.parent.name + "/Front");
        SetHireUnitButtonActiveByCell(false, changedCell.parent.name + "/Back");
        // Update name and health information
        // UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        //// it is more reliable to use GetChild(0), because it is only one child there
        //Transform parentCell = changedCell.parent.Find(Cell.Wide);
        //PartyUnitUI unitUI = parentCell.GetComponentInChildren<UnitSlot>().GetComponentInChildren<PartyUnitUI>();
        // fill in highered object UI panel
        //unitUI.SetUnitCellInfoUI();
        //unitUI.transform.Find("Name").GetComponent<Text>().text = unitUI.LPartyUnit.GetUnitDisplayName();
        //parentCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = unitUI.LPartyUnit.UnitHealthCurr.ToString();
        //parentCell.Find("HPPanel/HPmax").GetComponent<Text>().text = unitUI.LPartyUnit.GetUnitEffectiveMaxHealth().ToString();
    }

    //void CleanHealthUI(Transform targetCell)
    //{
    //    Color defaultColor = new Color32(128, 128, 128, 255);
    //    targetCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = "";
    //    targetCell.Find("HPPanel/HPcurr").GetComponent<Text>().color = defaultColor;
    //    targetCell.Find("HPPanel/HPmax").GetComponent<Text>().text = "";
    //    targetCell.Find("HPPanel/HPmax").GetComponent<Text>().color = defaultColor;
    //}

    void OnDismissSingleUnit(Transform changedCell)
    {
        // it is possile that unit was dismissed
        //// Clean health information
        //CleanHealthUI(changedCell);
        //// Clean info
        //ClearInfoPanel(changedCell);
        //// Clean status
        //ClearUnitCellStatus(changedCell);
        // activate hire unit button if panel is in garnizon state
        if (PartyMode.Garnizon == PartyMode)
        {
            Debug.Log("Activate hire unit button");
            SetHireUnitButtonActiveByCell(true, GetCellAddressStringByTransform(changedCell));
        }
    }

    void OnDimissDoubleUnit(Transform changedCell)
    {
        //// Clean info
        //ClearInfoPanel(changedCell);
        //// Clean status
        //ClearUnitCellStatus(changedCell);
        // Disable Wide panel
        changedCell.parent.Find(Cell.Wide.ToString()).gameObject.SetActive(false);
        // And enable left and right panels
        changedCell.parent.Find(Cell.Front.ToString()).gameObject.SetActive(true);
        changedCell.parent.Find(Cell.Back.ToString()).gameObject.SetActive(true);
        //// Update name and health information
        //// UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        //// it is more reliable to use GetChild(0), because it is only one child there
        //Transform parentCell = changedCell.parent.Find(Cell.Wide);
        //CleanHealthUI(parentCell);
        // activate hire unit buttons on left and right cells if panel is in garnizon state
        if (PartyMode.Garnizon == PartyMode)
        {
            Debug.Log("Activate hire unit button");
            SetHireUnitButtonActiveByCell(true, changedCell.parent.name + "/Front");
            SetHireUnitButtonActiveByCell(true, changedCell.parent.name + "/Back");
        }
    }

    public void OnChange(ChangeType changeType, Transform changedCell)
    {
        // Debug.Log("PartyPanel OnChange");
        switch (changeType)
        {
            case ChangeType.Init:
                IntitPartyPanel();
                break;
            case ChangeType.HirePartyLeader:
                // we do not need to do anything here
                // because all actions are done by IntitPartyPanel() 
                // which is initiated by Start() function on party panel creation
                break;
            case ChangeType.HireSingleUnit:
                OnHireSingleUnit(changedCell);
                break;
            case ChangeType.HireDoubleUnit:
                OnHireDoubleUnit(changedCell);
                break;
            case ChangeType.DismissPartyLeader:
                break;
            case ChangeType.DismissSingleUnit:
                OnDismissSingleUnit(changedCell);
                break;
            case ChangeType.DismissDoubleUnit:
                OnDimissDoubleUnit(changedCell);
                break;
            default:
                Debug.LogError("Unknown condition");
                break;
        }
        // verify if city or hero capacity has not been reached
        // if number of units in city or hero party reaches maximum, 
        // then hire unit button is disabled
        if (PartyMode == PartyMode.Garnizon)
        {
            // Get city screen
            EditPartyScreen cityScreen = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>();
            // verify if we are in City Screen view
            if (cityScreen != null)
            {
                // this is needed to disable or enable hire units button
                // hero party does not have this functionality
                if (GetCapacity() <= GetLeadershipConsumedByPartyUnits())
                {
                    // disable hire buttons
                    cityScreen.SetHireUnitPnlButtonActive(false);
                }
                else
                {
                    // enable hire buttons
                    cityScreen.SetHireUnitPnlButtonActive(true);
                }
            }
            else
            {
                // hire buttons already disabled
            }
        }
    }

    #endregion

    void IntitPartyPanel()
    {
        InitPartyPanelCells();
        InitHireUnitsButtons();
    }

    void InitHireUnitsButtons()
    {
        // verify if panel is in garnizon state
        if (PartyMode.Garnizon == PartyMode)
        {
            PartyUnitUI partyUnitUI;
            foreach (Row horisontalPanel in horisontalPanels)
            {
                foreach (Cell cell in cells)
                {
                    // Get party unit UI
                    partyUnitUI = transform.Find(horisontalPanel + "/" + cell).GetComponentInChildren<PartyUnitUI>(true);
                    // verify if party unit is present
                    if (partyUnitUI != null)
                    {
                        // verify if this is left or right single panel
                        if ((Cell.Front == cell) || (Cell.Back == cell))
                        {
                            // deactivate hire unit button for this cell
                            SetHireUnitButtonActiveByCell(false, horisontalPanel + "/" + cell);
                        }
                        // verify if this is wide cell
                        else if (Cell.Wide == cell)
                        {
                            // deactivate hire unit button for single-unit cells
                            SetHireUnitButtonActiveByCell(false, horisontalPanel + "/Front");
                            SetHireUnitButtonActiveByCell(false, horisontalPanel + "/Back");
                        }
                    }
                    else
                    {
                        // verify if this is single-unit cells
                        if (((Cell.Front == cell) || (Cell.Back == cell))
                            // verify if wide (double) cell is not occupied in the same row
                            && (!transform.Find(horisontalPanel.ToString()).Find("Wide/UnitSlot").GetComponentInChildren<PartyUnitUI>(true))
                            )
                        {
                            // activate hire unit button in this cell
                            SetHireUnitButtonActiveByCell(true, horisontalPanel + "/" + cell);
                        }
                    }
                }
            }
        }
    }

    void SetGenericUnitCellActive(Transform unitCell, PartyUnitUI partyUnitUI)
    {
        if (partyUnitUI != null)
        {
            // Set unit cell Info in UI
            partyUnitUI.SetUnitCellInfoUI();
        }
        // activate/deactivate this cell
        unitCell.gameObject.SetActive((partyUnitUI != null));
    }

    void SetSingleUnitCellActive(Transform unitCell, PartyUnitUI partyUnitUI)
    {
        SetGenericUnitCellActive(unitCell, partyUnitUI);
        if (partyUnitUI != null)
        {
            // deactivate double-unit cell in the same raw
            SetDoubleUnitCellActive(unitCell.parent.Find(Cell.Wide.ToString()), null);
        }
    }

    void SetDoubleUnitCellActive(Transform unitCell, PartyUnitUI partyUnitUI)
    {
        SetGenericUnitCellActive(unitCell, partyUnitUI);
        // activate singe-unit cells if party unit UI is null
        // deactivate singe-unit cells if party unit UI is not null
        unitCell.parent.Find(Cell.Front.ToString()).gameObject.SetActive(partyUnitUI == null);
        unitCell.parent.Find(Cell.Back.ToString()).gameObject.SetActive(partyUnitUI == null);
    }

    void InitPartyPanelCells()
    {
        Transform unitCell;
        //Transform unitSlot;
        PartyUnitUI partyUnitUI;
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cells)
            {
                unitCell = transform.Find(horisontalPanel + "/" + cell);
                //unitSlot = unitCell.Find("UnitSlot");
                // Get party unit UI
                partyUnitUI = transform.Find(horisontalPanel + "/" + cell).GetComponentInChildren<PartyUnitUI>(true);
                // verify if this is single- or double-unit cell
                if ((Cell.Front == cell) || (Cell.Back == cell))
                {
                    SetSingleUnitCellActive(unitCell, partyUnitUI);
                }
                if (Cell.Wide == cell)
                {
                    SetDoubleUnitCellActive(unitCell, partyUnitUI);
                }
                //// verify if party unit is present
                //if (partyUnitUI != null)
                //{
                //    // verify if this is single- or double-unit cell
                //    if ((Cell.Front == cell) || (Cell.Back == cell))
                //    {
                //        SetSingleUnitCellActive(partyUnitUI, unitCell, true);
                //    }
                //    if (Cell.Wide == cell)
                //    {
                //        SetDoubleUnitCellActive(partyUnitUI, unitCell, true);
                //    }
                //}
                //else
                //{
                //    // cell does not have a unit
                //    // verify if this is single-unit cell
                //    if ((Cell.Front == cell) || (Cell.Back == cell))
                //    {
                //        // keep it enabled, for UI visibility
                //    }
                //    // verify if this is wide cell
                //    else if (Cell.Wide == cell)
                //    {
                //        // no unit in wide cell
                //        // it is possible that double unit was dismissed
                //        // we need to disable Wide panel, because it is still enabled and placed on top of single panels
                //        unitCell.parent.Find(Cell.Wide).gameObject.SetActive(false);
                //        // and enable Front and Back panels
                //        unitCell.parent.Find(Cell.Front).gameObject.SetActive(true);
                //        unitCell.parent.Find(Cell.Back).gameObject.SetActive(true);
                //    }
                //}
            }
        }
    }

    #region Verify capacity

    int GetCapacity()
    {
        int capacity = 0;
        // return capacity based on the parent mode
        // in garnizon mode we get city capacity
        // in party mode we get hero leadership
        if (PartyMode == PartyMode.Garnizon)
        {
            // this can be triggered by clonned party panel,
            // when user right clicks on a city
            // but clonned panel does not have city attached
            // that is why we need to check if return city is not null
            City city = GetCity();
            if (city)
            {
                capacity = city.GetUnitsCapacity();
            }
            else
            {
                Debug.LogError("City not found");
            }
        } else
        {
            capacity = GetComponentInParent<HeroPartyUI>().LHeroParty.GetPartyLeader().GetEffectiveLeadership() + 1; // +1 because we do not count leader
        }
        return capacity;
    }

    public int GetLeadershipConsumedByPartyUnits()
    {
        int consumedLeadership = 0;
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // if this is double unit, then count is as +2
            if (partyUnitUI.LPartyUnit.UnitSize == UnitSize.Double)
            {
                // double unit
                consumedLeadership += 2;
            }
            else
            {
                // single unit
                consumedLeadership += 1;
            }
        }
        return consumedLeadership;
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        if (transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot").childCount > 0)
        //        {
        //            // if this is double unit, then count is as +2
        //            if (cell == Cell.Wide)
        //            {
        //                // double unit
        //                consumedLeadership += 2;
        //            } else
        //            {
        //                // single unit
        //                consumedLeadership += 1;
        //            }
        //        }
        //    }
        //}
        //return consumedLeadership;
    }

    public List<UnitSlot> GetAllPowerTargetableUnitSlots()
    {
        List<UnitSlot> unitSlots = new List<UnitSlot> { };
        foreach(UnitSlot unitSlot in GetComponentsInChildren<UnitSlot>())
        {
            // verify if slot has an unit in it and it is allowed to apply power to this unit
            if ((unitSlot.GetComponentInChildren<PartyUnitUI>() != null) 
                && (unitSlot.IsAllowedToApplyPowerToThisUnit))
            {
                unitSlots.Add(unitSlot);
            }
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        Transform unitSlotTr = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        UnitSlot unitSlot = unitSlotTr.GetComponent<UnitSlot>();
        //        // verify if slot has an unit in it and it is allowed to apply power to this unit
        //        if ((unitSlotTr.childCount > 0) && (unitSlot.IsAllowedToApplyPowerToThisUnit))
        //        {
        //            unitSlots.Add(unitSlot);
        //        }
        //    }
        //}
        return unitSlots;
    }

    #endregion Verify capacity

    public bool VerifyCityCapacityOverflowOnDoubleUnitHire()
    {
        bool result = true;
        if (GetCapacity() < (GetLeadershipConsumedByPartyUnits()+2))
        {
            // overflow on double unit hire
            result = false;
            // show error message
            // this depends if city has reached max level
            City city = GetCity();
            string errMsg;
            if (city.CityLevelCurrent == 1)
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Increase city level.";
            }
            else if (city.CityLevelCurrent >= city.CityLevelMax) // note city level for capital is 6, which is higher than for normal city
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Dismiss or move other units to Hero's party.";
            }
            else
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Dismiss or move other units to Hero's party or increase city level.";
            }
            NotificationPopUp.Instance().DisplayMessage(errMsg);
        }
        return result;
    }

    public bool VerifyDoubleHireNearOccupiedSingleCell(Transform callerCell)
    {
        bool result = true;
        // structure PartyPanel-[Top/Middle/Bottom]-(this)callerCell
        // verify cell, which is located nearby
        // if it is occuped, then show an error message
        Transform oppositeCell;
        if (callerCell.name == Cell.Front.ToString())
        {
            // check Back cell
            oppositeCell = callerCell.parent.Find(Cell.Back.ToString());
        } else
        {
            // check Front cell
            oppositeCell = callerCell.parent.Find(Cell.Front.ToString());
        }
        // check if cell is occupied
        // structure [Front/Back cell]-UnitSlot-unit
        if (oppositeCell.Find("UnitSlot").childCount > 0)
        {
            result = false;
            string errMsg = "Not enough free space to hire this large unit, 2 free nearby horisontal slots are required. " + oppositeCell.name + " unit slot is occupied. Move unit from " + oppositeCell.name + " slot to other free slot or to Hero's party or dismiss it to free up a slot or click on hire button where 2 free nearby horisontal slots are available.";
            NotificationPopUp.Instance().DisplayMessage(errMsg);
        }
        return result;
    }

    public void SetActiveHeal(bool activate)
    {
        Transform unitCell;
        Transform unitSlot;
        PartyUnit unit;
        Color greenHighlight = Color.green;
        Color redHighlight = Color.red;
        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
        Color hightlightColor;
        // highlight differently cells with and without units
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel + "/" + cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if unit is damaged
                    unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is alive (health > 0) and that he is damaged (health < maxhealth)
                        if ( (unit.UnitHealthCurr>0) && (unit.UnitHealthCurr < unit.GetUnitEffectiveMaxHealth()) )
                        {
                            // unit can be healed
                            // highlight it with green
                            hightlightColor = greenHighlight;
                        }
                        else
                        {
                            // unit cannot be healed
                            // highlight with red
                            hightlightColor = redHighlight;
                        }
                    }
                    else
                    {
                        // deactivate highlight
                        // get color from 
                        hightlightColor = normalColor;
                    }
                    // Change text box color
                    // unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                    unitCell.GetComponent<PartyPanelCell>().CanvasText.color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    }


    public void SetActiveDismiss(bool activate)
    {
        Transform unitCell;
        Transform unitSlot;
        PartyUnit unit;
        Color greenHighlight = Color.green;
        Color redHighlight = Color.red;
        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
        Color hightlightColor;
        // highlight differently cells with and without units
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel + "/" + cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if we need to activate or deactivate highlight
                    // get unit for future reference
                    unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is dismissable
                        if (unit.IsDismissable)
                        {
                            // unit can be dismissed
                            // highlight it with green
                            hightlightColor = greenHighlight;
                        }
                        else
                        {
                            // unit cannot be dismissed
                            // highlight with red
                            hightlightColor = redHighlight;
                        }
                    }
                    else
                    {
                        // deactivate highlight
                        // get color from 
                        hightlightColor = normalColor;
                    }
                    // Change text box color
                    //unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                    unitCell.GetComponent<PartyPanelCell>().CanvasText.color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    }


    public void SetActiveResurect(bool activate)
    {
        Transform unitCell;
        Transform unitSlot;
        PartyUnit unit;
        Color greenHighlight = Color.green;
        Color redHighlight = Color.red;
        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
        Color hightlightColor;
        // highlight differently cells with and without units
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel + "/" + cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if we need to activate or deactivate highlight
                    unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is alive (health > 0) and that he is damaged (health < maxhealth)
                        if (0 == unit.UnitHealthCurr)
                        {
                            // unit is dead and can be resurected
                            // highlight it with green
                            hightlightColor = greenHighlight;
                        }
                        else
                        {
                            // unit is alive and cannot be resurected
                            // highlight with red
                            hightlightColor = redHighlight;
                        }
                    }
                    else
                    {
                        // deactivate highlight
                        // get color from 
                        hightlightColor = normalColor;
                    }
                    // Change text box color
                    //unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                    unitCell.GetComponent<PartyPanelCell>().CanvasText.color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    }

    PartyPanel GetOtherPartyPanel(PartyPanel currentPartyPanel)
    {
        // structure: 2MiscUI-1HeroParty-PartyPanel(this)
        UIManager uiManager = transform.root.GetComponentInChildren<UIManager>();
        foreach (HeroPartyUI heroPartyUI in uiManager.GetComponentsInChildren<HeroPartyUI>())
        {
            // find party panel of other party, if it is present there
            // this will be the case if there are more then 1 hero party in the city
            if (heroPartyUI.GetComponentInChildren<PartyPanel>() != currentPartyPanel)
            {
                return heroPartyUI.GetComponentInChildren<PartyPanel>();
            }
        }
        // if other party is not present, then return null
        // calling script should do null verification
        return null;
    }

    void SetCellIsDroppableStatus(bool isDroppable, Transform cellTr, string errorMessage = "")
    {
        // isDroppable means if we can drop units to this cell
        if (isDroppable)
        {
            // Change text box color
            //cellTr.Find("Br").GetComponent<Text>().color = Color.green;
            cellTr.GetComponent<PartyPanelCell>().CanvasText.color = Color.green;
        }
        else
        {
            // Change text box color
            //cellTr.Find("Br").GetComponent<Text>().color = Color.red;
            cellTr.GetComponent<PartyPanelCell>().CanvasText.color = Color.red;
        }
        // set UnitSlot in cell as droppable or not
        cellTr.Find("UnitSlot").GetComponent<UnitSlotDropHandler>().SetOnDropAction(isDroppable, errorMessage);
    }

    string GetNotEnoughCapacityErrorMessage(PartyPanel partyPanel, bool direction)
    {
        string errorMessage = "";
        // return error message based on the party panel mode and direction
        // direction = true -> source
        // direction = false -> target
        string directionStr = "source";
        if (!direction)
        {
            directionStr = "target";
        }
        if (PartyMode.Garnizon == partyPanel.PartyMode)
        {
            errorMessage = "Not enough " + directionStr + " city capacity.";
        }
        else
        {
            errorMessage = "Not enough " + directionStr + " hero leadership.";
        }
        return errorMessage;
    }

    public void SetActiveUnitDrag(bool activate)
    {
        // unit being dragged is static, so we do not need to assign it here
        // or send as and argument
        // structure: 6[MiscUI]-5[HeroPartyUI/CityGarnizonUI]Party-4PartyPanel-3[Top/Middle/Bottom]HorizontalPanelGroup-2[Front/Back/Wide]Cell-1UnitSlot-UnitCanvas(unitBeingDraggedUI)
        PartyUnitUI partyUnitUI = UnitDragHandler.unitBeingDraggedUI.GetComponent<PartyUnitUI>();
        PartyUnit unitBeingDragged = partyUnitUI.LPartyUnit;
        Transform unitCell = partyUnitUI.GetUnitCell();
        Transform horizontalPanelGroup = partyUnitUI.GetUnitRow().transform;
        PartyPanel sourcePartyPanel = partyUnitUI.GetUnitPartyPanel();
        Transform cellTr;
        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
        string errorMessage = "";
        bool isDroppable = false;
        if (activate)
        {
            // highlight differently cells where units can be and cannot be dropped
            // gradually go from the lowest possible scope
            // to the highest possible
            // Scopes: InterParty[CityGarnizon/HeroParty]-PartyPanel-HorizontalPanelGroup[Top/Middle/Bottom]
            // HorizontalPanelGroup[Top/Middle/Bottom] PartyPanel scopes
            //  there cannot be any possible failures
            //  so this can be safely highlighted as OK
            //  highlight all active cells
            // panel should understand who it is
            //  - source of the draggable object
            //  - or destination or other panel
            // find out if current panel is the source panel
            // Debug.Log(sourcePartyPanel.gameObject.GetInstanceID());
            // Debug.Log(gameObject.GetInstanceID());
            if (sourcePartyPanel.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                // We are in a source panel
                // Debug.Log("Highlight source panel");
                foreach (Row horisontalPanel in horisontalPanels)
                {
                    foreach (Cell cell in cells)
                    {
                        // verify if slot is active
                        // here we highlight only party panel of the draggable unit
                        // not this party panel, where we are now
                        cellTr = sourcePartyPanel.transform.Find(horisontalPanel + "/" + cell);
                        if (cellTr.gameObject.activeSelf)
                        {
                            SetCellIsDroppableStatus(true, cellTr);
                        }
                    }
                }
            }
            else
            {
                // Debug.Log("Highlight destination panel");
                // there can be only 2 panels
                // if i'm not the source panel, then I'm other panel or destination panel
                // InterParty scope
                //  The 1st easiest check is if draggable unit is inter-party movable or not
                //  The 2nd problem may happen with moving unit to the other party
                //  Verify if other party panel is present, if no panel - no problems
                PartyPanel otherPartyPanel = GetOtherPartyPanel(sourcePartyPanel);
                if (otherPartyPanel)
                {
                    if (unitBeingDragged.IsInterpartyMovable)
                    {
                        // unit is movable
                        // find in other panel where it can or cannot be placed and highlight accordingly
                        // here we highlight other party if it is present,
                        // there are multiple blocking conditions present
                        // skip highlighting for the non-interparty-movable objects
                        // on double unit hire, skip also highlighting of nearby cell near non-interparty-movable
                        // act based on the unit size
                        if (UnitSize.Single == unitBeingDragged.UnitSize)
                        {
                            // Single unit size
                            // act based on the destination cell type
                            // Single[Occupied[movable/non-interparty-movable]/Free]/Double
                            // loop through all cells in other party panel and highlight based on condition
                            bool isCellActive;
                            PartyUnit cellUnit;
                            UnitDragHandler unitCanvas;
                            bool isUnitInterPartyDraggable;
                            foreach (Row horisontalPanel in horisontalPanels)
                            {
                                foreach (Cell cell in cells)
                                {
                                    // verify if slot is active
                                    // wo do not need to do anything with inacive cells
                                    isCellActive = otherPartyPanel.transform.Find(horisontalPanel + "/" + cell).gameObject.activeSelf;
                                    if (isCellActive)
                                    {
                                        // verify if cell is occupied
                                        // UnitCanvas, which has UnitDragHandler component attached is present
                                        unitCanvas = otherPartyPanel.transform.Find(horisontalPanel + "/" + cell + "/UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                        if (unitCanvas)
                                        {
                                            // occupied
                                            cellUnit = unitCanvas.GetComponent<PartyUnitUI>().LPartyUnit;
                                            isUnitInterPartyDraggable = cellUnit.IsInterpartyMovable;
                                            if (isUnitInterPartyDraggable)
                                            {
                                                // unit can be dragged to other party
                                                // do additional checks
                                                // if occupied cell is single-unit, 
                                                // then we can easily drag here unit and exchange units between the cells
                                                // but if the occupied cell is double unit cells, then we need to do additional verification
                                                if (Cell.Wide == cell)
                                                {
                                                    // do additional verification
                                                    // possible states and tranistions
                                                    // src state    dst state   result
                                                    // 01/10        2           check for source overflow
                                                    // 11           2           ok
                                                    // 1x/x1        2           not ok
                                                    // get nearby cell
                                                    Transform nearbySrcCellTr;
                                                    if (Cell.Front.ToString() == unitCell.name)
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find(Cell.Back.ToString());
                                                    }
                                                    else
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find(Cell.Front.ToString());
                                                    }
                                                    // verify if nearby source cell is occupied
                                                    UnitDragHandler nearbySrcCellUnitCanvas = nearbySrcCellTr.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                                    if (nearbySrcCellUnitCanvas)
                                                    {
                                                        // nearby cell is occupied
                                                        // check if it is occupied by non-inter-party-movable unit, 
                                                        // because we cannot swap those units
                                                        PartyUnit nearbySrcUnit = nearbySrcCellUnitCanvas.GetComponent<PartyUnitUI>().LPartyUnit;
                                                        if (nearbySrcUnit.IsInterpartyMovable)
                                                        {
                                                            // it is inter-party movable unit
                                                            // we can safely swap between src horizontal panel and destination panel with double unit
                                                            isDroppable = true;
                                                        }
                                                        else
                                                        {
                                                            // it is not inter-party movable unit
                                                            // we cannot swap it
                                                            isDroppable = false;
                                                            errorMessage = "Unit in nearby slot cannot be moved to other party.";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // nearby cell is free
                                                        // check for overflow in source cell
                                                        // +1 is we are simulating final capacity with swapped single and double units
                                                        if (sourcePartyPanel.GetCapacity() < (sourcePartyPanel.GetLeadershipConsumedByPartyUnits() + 1))
                                                        {
                                                            // not enough capacity
                                                            isDroppable = false;
                                                            errorMessage = GetNotEnoughCapacityErrorMessage(sourcePartyPanel, true);
                                                        }
                                                        else
                                                        {
                                                            // enough capacity
                                                            isDroppable = true;
                                                        }
                                                    }
                                                } else
                                                {
                                                    // we can swap units
                                                    isDroppable = true;
                                                }
                                            }
                                            else
                                            {
                                                isDroppable = false;
                                                errorMessage = "Unit in target slot cannot be moved to other party.";
                                            }
                                        }
                                        else
                                        {
                                            // free
                                            // verify if there is no overflow of city or hero capacity if unit move to free cell
                                            // +1 is we are simulating final capacity with moved single unit
                                            if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetLeadershipConsumedByPartyUnits() + 1))
                                            {
                                                // not enough capacity
                                                isDroppable = false;
                                                errorMessage = GetNotEnoughCapacityErrorMessage(otherPartyPanel, false);
                                            }
                                            else
                                            {
                                                // enough capacity
                                                isDroppable = true;
                                            }
                                            // Change text box color
                                        }
                                        // per-cell hightlight
                                        SetCellIsDroppableStatus(isDroppable, otherPartyPanel.transform.Find(horisontalPanel + "/" + cell), errorMessage);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Double unit size
                            // do the same as for single
                            // act based on destination horizontal panel state
                            // possible states:
                            // src cell     dst cell    result
                            // 2            00          check destination panel overflow +2
                            // 2            01/10       check destination panel overflow +1
                            // 2            x*/*x       not possible, x - non interparty movable unit
                            // 2            2/11        ok
                            PartyUnit cellUnit;
                            bool isUnitInterPartyDraggable;
                            GameObject wideCell;
                            Transform leftCell;
                            Transform rightCell;
                            UnitDragHandler isFrontCellOccupied; // if null - false, if other - true, we use it as bool
                            UnitDragHandler isBackCellOccupied; // if null - false, if other - true, we use it as bool
                            bool isFrontCellUnitInterPartyMovable = false;
                            bool isBackCellUnitInterPartyMovable = false;
                            foreach (Row horisontalPanel in horisontalPanels)
                            {
                                // verify if wide slot is active
                                // if wide is active, then we have unit there
                                // there is no need to additionally check this
                                // we can swap with other wide units
                                // we assume that all wide units are inter-party movable
                                wideCell = otherPartyPanel.transform.Find(horisontalPanel + "/Wide").gameObject;
                                if (wideCell.activeSelf)
                                {
                                    // wide slot is active
                                    // per-horizontal panel hightlight
                                    SetCellIsDroppableStatus(true, otherPartyPanel.transform.Find(horisontalPanel + "/Wide"));
                                }
                                else
                                {
                                    // wide slot is not active
                                    // single unit slot should be active
                                    // verify their state
                                    leftCell = otherPartyPanel.transform.Find(horisontalPanel + "/Front");
                                    rightCell = otherPartyPanel.transform.Find(horisontalPanel + "/Back");
                                    // gather input for possible states
                                    // UnitCanvas, which has UnitDragHandler component attached is present
                                    isFrontCellOccupied = leftCell.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                    isBackCellOccupied = rightCell.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                    if (isFrontCellOccupied)
                                    {
                                        // occupied
                                        cellUnit = isFrontCellOccupied.GetComponent<PartyUnitUI>().LPartyUnit;
                                        isUnitInterPartyDraggable = cellUnit.IsInterpartyMovable;
                                        if (isUnitInterPartyDraggable)
                                        {
                                            isFrontCellUnitInterPartyMovable = true;
                                        }
                                        else
                                        {
                                            isFrontCellUnitInterPartyMovable = false;
                                        }
                                    }
                                    if (isBackCellOccupied)
                                    {
                                        // occupied
                                        cellUnit = isBackCellOccupied.GetComponent<PartyUnitUI>().LPartyUnit;
                                        isUnitInterPartyDraggable = cellUnit.IsInterpartyMovable;
                                        if (isUnitInterPartyDraggable)
                                        {
                                            isBackCellUnitInterPartyMovable = true;
                                        }
                                        else
                                        {
                                            isBackCellUnitInterPartyMovable = false;
                                        }
                                    }
                                    // verify conditions
                                    // 00
                                    if (!isFrontCellOccupied && !isBackCellOccupied)
                                    {
                                        // check destination overflow +2
                                        if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetLeadershipConsumedByPartyUnits() + 2))
                                        {
                                            // not enough capacity
                                            isDroppable = false;
                                            errorMessage = GetNotEnoughCapacityErrorMessage(otherPartyPanel, false);
                                        }
                                        else
                                        {
                                            // enough capacity
                                            isDroppable = true;
                                        }
                                    }
                                    // 01/10
                                    else if ( (!isFrontCellOccupied && (isBackCellOccupied && isBackCellUnitInterPartyMovable))
                                      || ( (isFrontCellOccupied && isFrontCellUnitInterPartyMovable) && !isBackCellOccupied) )
                                    {
                                        // check destination overflow +1
                                        if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetLeadershipConsumedByPartyUnits() + 1))
                                        {
                                            // not enough capacity
                                            isDroppable = false;
                                            errorMessage = GetNotEnoughCapacityErrorMessage(otherPartyPanel, false);
                                        }
                                        else
                                        {
                                            // enough capacity
                                            isDroppable = true;
                                        }
                                    }
                                    // 11
                                    else if ((isFrontCellOccupied && isFrontCellUnitInterPartyMovable) && (isBackCellOccupied && isBackCellUnitInterPartyMovable))
                                    {
                                        // just swap
                                        isDroppable = true;
                                    }
                                    // x0
                                    // 0x
                                    // x1
                                    // x1
                                    // xx
                                    else
                                    {
                                        // all other conditions lead to error
                                        isDroppable = false;
                                        errorMessage = "Unit(s) in target slot(s) cannot be moved to other party.";
                                    }
                                    // per-horizontal panel hightlight
                                    SetCellIsDroppableStatus(isDroppable, leftCell, errorMessage);
                                    SetCellIsDroppableStatus(isDroppable, rightCell, errorMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        // unit is not inter-party movable
                        // highlight other panel with (red) - unit cannot be dropped there
                        foreach (Row horisontalPanel in horisontalPanels)
                        {
                            // get state of active cells
                            foreach (Cell cell in cells)
                            {
                                // Change text box color
                                errorMessage = "This unit cannot be moved to other party.";
                                SetCellIsDroppableStatus(false, transform.Find(horisontalPanel + "/" + cell), errorMessage);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // reset all colors to normal
            foreach (Row horisontalPanel in horisontalPanels)
            {
                foreach (Cell cell in cells)
                {
                    // Change text box color
                    //transform.Find(horisontalPanel+"/"+cell+"/Br").GetComponent<Text>().color = normalColor;
                    transform.Find(horisontalPanel+"/"+cell).GetComponent<PartyPanelCell>().CanvasText.color = normalColor;
                }
            }
        }
        // and disable hire buttons
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    }



    #region For Battle Screen

    public bool CanFight()
    {
        // loop through all units in each party and verify if there is at least one unit, which can fight
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if unit can fight
            if (partyUnitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle())
            {
                return true;
            }
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            if (GetUnitUIWhichCanFight(horisontalPanel, cell))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        // if not unit can fight, return false;
        return false;
    }

    // todo: fix duplicate function in HeroParty
    //public PartyUnitUI GetActiveUnitWithHighestInitiative(BattleScreen.TurnPhase turnPhase)
    //{
    //    PartyUnitUI unitWithHighestInitiative = null;
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                // verify if 
    //                //  - unit has moved or not
    //                //  - unit is alive
    //                //  - unit has not escaped from the battle
    //                PartyUnitUI unitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                if (!unitUI.LPartyUnit.HasMoved)
    //                {
    //                    //// during main phase check for Active units
    //                    //bool doProceed = false;
    //                    //if ( (BattleScreen.TurnPhase.Main == turnPhase)
    //                    //    && ( (unitUI.LPartyUnit.UnitStatus == UnitStatus.Active) 
    //                    //       || (unitUI.LPartyUnit.UnitStatus == UnitStatus.Escaping) ) )
    //                    //{
    //                    //    doProceed = true;
    //                    //}
    //                    //// during post wait phase check for units which are in Waiting status
    //                    //if ((BattleScreen.TurnPhase.PostWait == turnPhase)
    //                    //    && (unitUI.LPartyUnit.UnitStatus == UnitStatus.Waiting))
    //                    //{
    //                    //    doProceed = true;
    //                    //}
    //                    //if (doProceed)
    //                    // verify if unit can be given turn in battle based on its status and turn phase
    //                    if (unitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle(turnPhase))
    //                    {
    //                        // compare initiative with other unit, if it was found
    //                        if (unitWithHighestInitiative)
    //                        {
    //                            if (unitUI.LPartyUnit.GetEffectiveInitiative() > unitWithHighestInitiative.LPartyUnit.GetEffectiveInitiative())
    //                            {
    //                                // found unit with highest initiative, update unitWithHighestInitiative variable
    //                                unitWithHighestInitiative = unitUI;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            // no other unit found yet, assume that this unit has the highest initiative
    //                            unitWithHighestInitiative = unitUI;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return unitWithHighestInitiative;
    //}

    public PartyUnitUI GetActiveUnitUIWithHighestInitiativeWhichHasNotMoved(BattleTurnPhase turnPhase)
    {
        PartyUnitUI unitUIWithHighestInitiative = null;
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if 
            //  - unit has moved or not
            //  - unit is alive
            //  - unit has not escaped from the battle
            if (!partyUnitUI.LPartyUnit.HasMoved)
            {
                // verify if unit can be given turn in battle based on its status and turn phase
                if (partyUnitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle(turnPhase))
                {
                    // verify if unit with the initiative has been found
                    if (unitUIWithHighestInitiative)
                    {
                        // compare initiative with previously found unit
                        if (partyUnitUI.LPartyUnit.GetEffectiveInitiative() > unitUIWithHighestInitiative.LPartyUnit.GetEffectiveInitiative())
                        {
                            // found unit with highest initiative, update unitWithHighestInitiative variable
                            unitUIWithHighestInitiative = partyUnitUI;
                        }
                        else if (partyUnitUI.LPartyUnit.GetEffectiveInitiative() == unitUIWithHighestInitiative.LPartyUnit.GetEffectiveInitiative())
                        {
                            // equal initiative
                            // randomly choose between units
                            // Random.value resturns a value between 0 and 1, 
                            // so by shifting 0.5 you could also modify the probability of the two numbers.
                            if (UnityEngine.Random.value < 0.5f)
                            {
                                return partyUnitUI;
                            }
                            else
                            {
                                return unitUIWithHighestInitiative;
                            }
                        }
                    }
                    else
                    {
                        // no other unit found yet, assume that this unit has the highest initiative
                        unitUIWithHighestInitiative = partyUnitUI;
                    }
                }
            }
        }
        return unitUIWithHighestInitiative;
    }

    public void ResetHasMovedFlag()
    {
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // set unit has moved to false, so it can move again in new turn
            partyUnitUI.LPartyUnit.HasMoved = false;
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // set unit has moved to false, so it can move again
        //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        //            unit.HasMoved = false;
        //        }
        //    }
        //}
    }


    bool GetIsUnitFriendly(PartyUnitUI unitToActivateUI)
    {
        // method 1
        // structure: 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanel-3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
        PartyPanel unitToActivatePartyPanel = unitToActivateUI.GetUnitPartyPanel(); //.transform.parent.parent.parent.parent.parent.gameObject;
        // compare this game object of party panel to the unit's party panel game object
        if (gameObject.GetInstanceID() == unitToActivatePartyPanel.gameObject.GetInstanceID())
        {
            return true;
        }
        // method 2
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // verify if unit has isLeader atrribute ON
        //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        //            if (unit.GetInstanceID() == unitToActivate.GetInstanceID())
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        return false;
    }

    //void SetIfCellCanBeTargetedStatus(bool isTargetable, Transform cellTr, string errorMessage, Color positiveColor, Color negativeColor)
    //{
    //    // isDroppable means if we can drop units to this cell
    //    if (isTargetable)
    //    {
    //        // Unit can be targeted
    //        // Change text box color
    //        cellTr.Find("Br").GetComponent<Text>().color = positiveColor;
    //    }
    //    else
    //    {
    //        // Unit can't be targeted
    //        // Change text box color
    //        // Skip "Br" modify for dead and units which has escaped from battle
    //        // Get unit if it is present in the cell
    //        Transform unitSlot = cellTr.Find("UnitSlot");
    //        if (unitSlot.childCount > 0)
    //        {
    //            // Unit present in cell
    //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //            // Verify if unit has not escaped or dead
    //            if (   (unit.UnitStatus == UnitStatus.Active)
    //                || (unit.UnitStatus == UnitStatus.Waiting)
    //                || (unit.UnitStatus == UnitStatus.Escaping) )
    //            {
    //                // Unit is alive and has not escaped
    //                // Apply default negative Color
    //                cellTr.Find("Br").GetComponent<Text>().color = negativeColor;
    //            }
    //            else
    //            {
    //                // do not change status of dead or escaped unit, because it is already set correctly
    //            }
    //        }
    //        else
    //        {
    //            // No unit
    //            // Apply default negative Color
    //            cellTr.Find("Br").GetComponent<Text>().color = negativeColor;
    //        }
    //    }
    //    // set UnitSlot in cell as droppable or not
    //    cellTr.Find("UnitSlot").GetComponent<UnitSlot>().SetOnClickAction(isTargetable, errorMessage);
    //}

    PartyUnitUI GetUnitUIWhichCanFight(Row horisontalPanel, Cell cell)
    {
        // verify if slot has an unit in it
        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        if (unitSlot.childCount > 0)
        {
            // cell has a unit in it
            // Get party unit UI
            PartyUnitUI unitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
            // verify if unit in its state can be given a turn in battle
            if (unitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle())
            {
                return unitUI;
            }
            // verify if unit can act: alive and did not escape (flee from) the battle
            //if (  (unitUI.LPartyUnit.UnitStatus == UnitStatus.Active) 
            //   || (unitUI.LPartyUnit.UnitStatus == UnitStatus.Waiting)
            //   || (unitUI.LPartyUnit.UnitStatus == UnitStatus.Escaping) )
            //{
            //    return unitUI;
            //}
        }
        return null;
    }

    bool FrontRowHasUnitsWhichCanFight()
    {
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cellsFront)
            {
                if (GetUnitUIWhichCanFight(horisontalPanel, cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool BackRowHasUnitsWhichCanFight()
    {
        foreach (Row horisontalPanel in horisontalPanels)
        {
            foreach (Cell cell in cellsBack)
            {
                if (GetUnitUIWhichCanFight(horisontalPanel, cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CellsHaveUnit(Row horisontalPanel, Cell[] cells)
    {
        foreach (Cell cell in cells)
        {
            if (GetUnitUIWhichCanFight(horisontalPanel, cell))
            {
                return true;
            }
        }
        return false;
    }

    bool HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row horisontalPanel, Cell cell)
    {
        switch (cell)
        {
            case Cell.Front:
                return CellsHaveUnit(horisontalPanel, cellsFront);
            case Cell.Back:
                return CellsHaveUnit(horisontalPanel, cellsBack);
            case Cell.Wide:
                return CellsHaveUnit(horisontalPanel, cellsFront);
            default:
                Debug.LogError("Unknown row");
                break;
        }
        return false;
    }

    //public void ResetAllCellsCanBeTargetedStatus()
    //{
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    Color positiveColor = Color.white; // it doesn't matter, because isAllowedToApplyPwrToThisUnit is false
    //    Color negativeColor = new Color32(32, 32, 32, 255);
    //    string errorMessage = "This cannot be targeted";
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //        }
    //    }
    //}

    bool IsActiveMeleUnitBlockedByItsPartyMembers(PartyPanelCell srcPartyPanelCell)
    {
        // structure: PartyUnitUI(activeBattleUnitUI)
        //string activeMeleUnitFrontBackWideRowPosition = activeBattleUnitUI.GetUnitCell().name; // .transform.parent.parent.parent.name;
        //PartyPanel activeUnitPartyPanel = activeBattleUnitUI.GetUnitPartyPanel(); // .transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
        bool isBlocked = true;
        //bool frontRowHasUnitsWhichCanFight = activeUnitPartyPanel.FrontRowHasUnitsWhichCanFight();
        //if (Cell.Back.ToString() == activeMeleUnitFrontBackWideRowPosition)
        if (Cell.Back == srcPartyPanelCell.Cell)
        {

            if (srcPartyPanelCell.GetComponentInParent<PartyPanel>().FrontRowHasUnitsWhichCanFight())
            {
                // mele unit is blocked by its own party units
                isBlocked = true;
            }
            else
            {
                // mele unit is not blocked and can fight
                isBlocked = false;
            }
        }
        else
        {
            // active unit is in a front row and cannot be blocked
            isBlocked = false;
        }
        // Debug.LogWarning(activeMeleUnitFrontBackWideRowPosition);
        // Debug.LogWarning(isBlocked.ToString());
        // Debug.LogWarning(frontRowHasUnitsWhichCanFight.ToString());
        return isBlocked;
    }

    bool GetMeleUnitIsProtectedByFriendlyUnitsInFrontRow(Cell cell)
    {
        // if mele unit is in back row, then verify if it is not blocked by front row units
        // verify if Target unit is from front row or from back row
        if (Cell.Back == cell)
        {
            // unit is from back row and it may be protected by front row units
            // If front row is empty, then unit in back row potentially be reached
            // verify if enemy front row is not empty
            if (FrontRowHasUnitsWhichCanFight())
            {
                // front row has units which can fight
                // this means that this unit is protected from mele atack
                // return protected
                return true;
            }
            else
            {
                // front does not have units which can fight
                // it means that active mele unit can potentially reach enemy unit
                // return not protected
                return false;
            }
        }
        else
        {
            // unit is from front row and potentially can be targeted
            // return not protected
            return false;
        }
    }

    public MeleUnitBlockingCondition GetMeleUnitBlockingCondition(PartyPanelCell srcPartyPanelCell, PartyPanelCell dstPartyPanelCell)
    {
        Cell targetUnitCell = dstPartyPanelCell.Cell;
        Row targetUnitRow = dstPartyPanelCell.GetComponentInParent<PartyPanelRow>().Row;
        Row activeMeleUnitRow = srcPartyPanelCell.GetComponentInParent<PartyPanelRow>().Row;
        // if Active mele unit is blocked, then it cannot attack anything
        if (IsActiveMeleUnitBlockedByItsPartyMembers(srcPartyPanelCell))
        {
            return MeleUnitBlockingCondition.AttackerIsBlockedByFriendlyFrontRowUnits;
        }
        else
        {
            // not blocked
            // act based on the mele unit position (cell)
            // 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanelGroup-3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(this)Unit
            if (GetMeleUnitIsProtectedByFriendlyUnitsInFrontRow(targetUnitCell))
            {
                // This enemy unit cannot be targeted, because it is protected by units in a front row.
                return MeleUnitBlockingCondition.TargetUnitIsProtectedByFriendlyUnitsInFrontRow;
            }
            else
            {
                // verify if mele unit can reach enemy unit depending on active mele unit and enemy positions
                switch (activeMeleUnitRow)
                {
                    case Row.Top:
                        // can reach closest 2 (top and middle) units
                        // and also farest unit (if it is not protected by top and middle) units
                        if ((Row.Top == targetUnitRow) || (Row.Middle == targetUnitRow))
                        {
                            // Target unit is not protected and can be targeted
                            return MeleUnitBlockingCondition.None;
                        }
                        else
                        {
                            // Bottom row
                            // verify if top or middle has units, which can fight
                            // which means that they can protect bottom unit from mele attacks
                            if (HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Top, targetUnitCell)
                                || HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Middle, targetUnitCell))
                            {
                                // Target unit cannot be targeted by mele attack. It is protected by unit above.
                                return MeleUnitBlockingCondition.TargetUnitIsProtectedByUnitAbove;
                            }
                            else
                            {
                                // Target unit is not protected and can be targeted
                                return MeleUnitBlockingCondition.None;
                            }
                        }
                    case Row.Middle:
                        // Middle mele unit can reach any unit in front of it
                        return MeleUnitBlockingCondition.None;
                    case Row.Bottom:
                        // can reach closest 2 (bottom and middle) units
                        // and also farest unit (if it is not protected by bottom and middle) units
                        if ((Row.Bottom == targetUnitRow) || (Row.Middle == targetUnitRow))
                        {
                            // Target unit is not protected and can be targeted
                            return MeleUnitBlockingCondition.None;
                        }
                        else
                        {
                            // Top row
                            // verify if bottom or middle has units, which can fight
                            // which means that they can protect top unit from mele attacks
                            if (HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Bottom, targetUnitCell)
                                || HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Middle, targetUnitCell))
                            {
                                // This unit cannot be targeted by mele attack. It is protected by unit below
                                return MeleUnitBlockingCondition.TargetUnitIsProtectedByUnitBelow;
                            }
                            else
                            {
                                // Target unit is not protected and can be targeted
                                return MeleUnitBlockingCondition.None;
                            }
                        }
                    default:
                        Debug.LogError("Unknown unit position [" + activeMeleUnitRow + "]");
                        // assume that Target unit is not protected and can be targeted
                        return MeleUnitBlockingCondition.None;
                }
            }
        }
    }

    //void PrepareBattleFieldForMelePower(bool activeUnitIsFromThisParty)
    //{
    //    //Debug.Log("PrepareBattleFieldForMelePower");
    //    Color positiveColor = Color.yellow;
    //    Color negativeColor = Color.grey;
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    string errorMessage = "";
    //    //bool activeMeleUnitIsBlocked = IsActiveMeleUnitBlockedByItsPartyMembers();
    //    Row activeMeleUnitRow = activeBattleUnitUI.GetUnitRow().Row;
    //    //bool enemyUnitIsPotentialTarget = false;
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if destination slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                //// Unit canvas (and unit) is present
    //                //if (activeUnitIsFromThisParty)
    //                //{
    //                //    // cannot attack friendly units
    //                //    isAllowedToApplyPwrToThisUnit = false;
    //                //    errorMessage = "Cannot attack friendly units.";
    //                //}
    //                //else
    //                //{
    //                //    // these are actions for enemy party
    //                //    // first filter out dead units
    //                //    // Get party unit UI
    //                //    PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                //    // get party unit
    //                //    PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                //    switch (partyUnit.UnitStatus)
    //                //    {
    //                //        case UnitStatus.Active:
    //                //        case UnitStatus.Waiting:
    //                //        case UnitStatus.Escaping:
    //                //            // if active mele unit is blocked, then it cannot attack anything
    //                //            if (activeMeleUnitIsBlocked)
    //                //            {
    //                //                // blocked
    //                //                // set cannot attack error messages depending on friendly or enemy unit types
    //                //                //if (unitSlot.childCount > 0)
    //                //                //{
    //                //                //    // Unit canvas (and unit) is present
    //                //                //    if (activeUnitIsFromThisParty)
    //                //                //    {
    //                //                //        // cannot attack friendly units
    //                //                //        isAllowedToApplyPwrToThisUnit = false;
    //                //                //        errorMessage = "Cannot attack friendly units.";
    //                //                //    }
    //                //                //    else
    //                //                //    {
    //                //                //        // this is actions for enemy party
    //                //                //        isAllowedToApplyPwrToThisUnit = false;
    //                //                //        errorMessage = activeBattleUnitUI.LPartyUnit.UnitName + " is mele unit and can attack only adjacent units. At this moment it is blocked by front row party members and cannot attack this enemy unit.";
    //                //                //    }
    //                //                //}
    //                //                //else
    //                //                //{
    //                //                //    // this is an empty cell
    //                //                //    isAllowedToApplyPwrToThisUnit = false;
    //                //                //    errorMessage = "No target.";
    //                //                //}
    //                //                // cannot attack, because unit is blocked
    //                //                isAllowedToApplyPwrToThisUnit = false;
    //                //                errorMessage = activeBattleUnitUI.LPartyUnit.UnitName + " is mele unit and can attack only adjacent units. At this moment it is blocked by front row party members and cannot attack this enemy unit.";
    //                //            }
    //                //            else
    //                //            {
    //                //                // not blocked
    //                //                // act based on the mele unit position (cell)
    //                //                // 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanelGroup-3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(this)Unit
    //                //                // if mele unit is in back row, then verify if it is not blocked by front row units
    //                //                // verify if this enemy unit is from front row or from back row
    //                //                if (Cell.Back == cell)
    //                //                {
    //                //                    // unit is from back row and it may be protected by front row units
    //                //                    // If front row is empty, then unit in back row potentially be reached
    //                //                    // verify if enemy front row is not empty
    //                //                    if (FrontRowHasUnitsWhichCanFight())
    //                //                    {
    //                //                        // front row has units which can fight
    //                //                        // this means that this unit is protected from mele atack
    //                //                        enemyUnitIsPotentialTarget = false;
    //                //                    }
    //                //                    else
    //                //                    {
    //                //                        // front does not have units which can fight
    //                //                        // it means that active mele unit can potentially reach enemy unit
    //                //                        enemyUnitIsPotentialTarget = true;
    //                //                    }
    //                //                }
    //                //                else
    //                //                {
    //                //                    // unit is from front row and potentially can be targeted
    //                //                    enemyUnitIsPotentialTarget = true;
    //                //                }
    //                //                if (enemyUnitIsPotentialTarget)
    //                //                {
    //                //                    // verify if mele unit can reach enemy unit depending on active mele unit and enemy positions
    //                //                    switch (activeMeleUnitRow)
    //                //                    {
    //                //                        case Row.Top:
    //                //                            // can reach closest 2 (top and middle) units
    //                //                            // and also farest unit (if it is not protected by top and middle) units
    //                //                            if ((Row.Top == horisontalPanel) || (Row.Middle == horisontalPanel))
    //                //                            {
    //                //                                isAllowedToApplyPwrToThisUnit = true;
    //                //                                errorMessage = "";
    //                //                            }
    //                //                            else
    //                //                            {
    //                //                                // Bottom horisontalPanel
    //                //                                // verify if top or middle has units, which can fight
    //                //                                // which means that they can protect bottom unit from mele attacks
    //                //                                if (HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Top, cell)
    //                //                                    || HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Middle, cell))
    //                //                                {
    //                //                                    // unit is protected
    //                //                                    isAllowedToApplyPwrToThisUnit = false;
    //                //                                    errorMessage = "This unit cannot be targeted by mele attack. It is protected by unit above.";
    //                //                                }
    //                //                                else
    //                //                                {
    //                //                                    // unit is not protected and can be targeted
    //                //                                    isAllowedToApplyPwrToThisUnit = true;
    //                //                                    errorMessage = "";
    //                //                                }
    //                //                            }
    //                //                            break;
    //                //                        case Row.Middle:
    //                //                            // Middle mele unit can reach any unit in front of it
    //                //                            isAllowedToApplyPwrToThisUnit = true;
    //                //                            errorMessage = "";
    //                //                            break;
    //                //                        case Row.Bottom:
    //                //                            // can reach closest 2 (bottom and middle) units
    //                //                            // and also farest unit (if it is not protected by bottom and middle) units
    //                //                            if ((Row.Bottom == horisontalPanel) || (Row.Middle == horisontalPanel))
    //                //                            {
    //                //                                isAllowedToApplyPwrToThisUnit = true;
    //                //                                errorMessage = "";
    //                //                            }
    //                //                            else
    //                //                            {
    //                //                                // Top horisontalPanel
    //                //                                // verify if bottom or middle has units, which can fight
    //                //                                // which means that they can protect top unit from mele attacks
    //                //                                if (HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Bottom, cell)
    //                //                                    || HorizontalPanelHasUnitsWhichCanFightInTheSameRow(Row.Middle, cell))
    //                //                                {
    //                //                                    // unit is protected
    //                //                                    isAllowedToApplyPwrToThisUnit = false;
    //                //                                    errorMessage = "This unit cannot be targeted by mele attack. It is protected by unit below.";
    //                //                                }
    //                //                                else
    //                //                                {
    //                //                                    // unit is not protected and can be targeted
    //                //                                    isAllowedToApplyPwrToThisUnit = true;
    //                //                                    errorMessage = "";
    //                //                                }
    //                //                            }
    //                //                            break;
    //                //                        default:
    //                //                            Debug.LogError("Unknown unit position [" + activeMeleUnitRow + "]");
    //                //                            break;
    //                //                    }
    //                //                }
    //                //                else
    //                //                {
    //                //                    // unit cannot be targeted
    //                //                    isAllowedToApplyPwrToThisUnit = false;
    //                //                    errorMessage = "This enemy unit cannot be targeted, because it is protected by units in a front row.";
    //                //                }
    //                //            }
    //                //            break;
    //                //        case UnitStatus.Dead:
    //                //            // cannot attack dead units
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot attack dead units.";
    //                //            break;
    //                //        case UnitStatus.Escaped:
    //                //            // cannot attack dead units
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot attack units which escaped from the battle field.";
    //                //            break;
    //                //        default:
    //                //            Debug.LogError("Unknown unit status");
    //                //            break;
    //                //    }
    //                //}
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // get active mele unit blocking condition
    //                MeleUnitBlockingCondition meleUnitBlockingCondition = GetMeleUnitBlockingCondition(cell, horisontalPanel, activeMeleUnitRow);
    //                // verify if unit can be be attacked
    //                isAllowedToApplyPwrToThisUnit = partyUnit.UnitStatusConfig.GetCanBeAttacked(activeUnitIsFromThisParty, meleUnitBlockingCondition);
    //                // set message based on the unit status
    //                errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToAttackMessage(activeUnitIsFromThisParty, meleUnitBlockingCondition);
    //                // set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //            }
    //        }
    //    }
    //}

    //void PrepareBattleFieldForRangedPower(bool activeUnitIsFromThisParty)
    //{
    //    //Debug.Log("PrepareBattleFieldForRangedPower");
    //    Color positiveColor = Color.yellow;
    //    // Color negativeColor = new Color32(221, 24, 24, 255); // dark red
    //    Color negativeColor = Color.grey;
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    string errorMessage = "";
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                //// get unit for later checks
    //                //PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //                //// Unit canvas (and unit) is present
    //                //if (activeUnitIsFromThisParty)
    //                //{
    //                //    // cannot attack friendly units
    //                //    isAllowedToApplyPwrToThisUnit = false;
    //                //    errorMessage = "Cannot attack friendly units.";
    //                //} else
    //                //{
    //                //    // these are actions for enemy party
    //                //    switch (unit.UnitStatus)
    //                //    {
    //                //        case UnitStatus.Active:
    //                //        case UnitStatus.Waiting:
    //                //        case UnitStatus.Escaping:
    //                //            // alive enemy unit
    //                //            // ranged units can reach any unit
    //                //            // so all enemy units can be targeted
    //                //            isAllowedToApplyPwrToThisUnit = true;
    //                //            errorMessage = "";
    //                //            break;
    //                //        case UnitStatus.Dead:
    //                //            // cannot attack dead units
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot attack dead units.";
    //                //            break;
    //                //        case UnitStatus.Escaped:
    //                //            // unit has escaped from the battle field and can't be attacked
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot attack units which escaped from the battle field.";
    //                //            break;
    //                //        default:
    //                //            Debug.LogError("Unknown unit status");
    //                //            break;
    //                //    }
    //                //}
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // verify if unit can be be attacked
    //                isAllowedToApplyPwrToThisUnit = partyUnit.UnitStatusConfig.GetCanBeAttacked(activeUnitIsFromThisParty);
    //                // set message based on the unit status
    //                errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToAttackMessage(activeUnitIsFromThisParty);
    //                // set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //            }
    //        }
    //    }
    //}

    //void PrepareBattleFieldForMagicPower(bool activeUnitIsFromThisParty)
    //{
    //    Debug.Log("PrepareBattleFieldForMagicPower");
    //    Color positiveColor = Color.yellow;
    //    Color negativeColor = Color.grey;
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    string errorMessage = "";
    //    // highlight all cells based on friendly/enemy principle
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            //// Unit canvas (and unit) is present
    //            //if (activeUnitIsFromThisParty)
    //            //{
    //            //    // cannot attack friendly units
    //            //    isAllowedToApplyPwrToThisUnit = false;
    //            //    errorMessage = "Cannot attack friendly units.";
    //            //}
    //            //else
    //            //{
    //            //    // verify if slot has an unit in it
    //            //    Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            //    if (unitSlot.childCount > 0)
    //            //    {
    //            //        // get unit for later checks
    //            //        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //            //        // these are actions for enemy party
    //            //        switch (unit.UnitStatus)
    //            //        {
    //            //            case UnitStatus.Active:
    //            //            case UnitStatus.Waiting:
    //            //            case UnitStatus.Escaping:
    //            //                // alive enemy unit
    //            //                // ranged units can reach any unit
    //            //                // so all enemy units can be targeted
    //            //                isAllowedToApplyPwrToThisUnit = true;
    //            //                errorMessage = "";
    //            //                break;
    //            //            case UnitStatus.Dead:
    //            //                // cannot attack dead units
    //            //                isAllowedToApplyPwrToThisUnit = false;
    //            //                errorMessage = "Cannot attack dead units.";
    //            //                break;
    //            //            case UnitStatus.Escaped:
    //            //                // unit has escaped from the battle field and can't be attacked
    //            //                isAllowedToApplyPwrToThisUnit = false;
    //            //                errorMessage = "Cannot attack units which escaped from the battle field.";
    //            //                break;
    //            //            default:
    //            //                Debug.LogError("Unknown unit status");
    //            //                break;
    //            //        }
    //            //    }
    //            //}
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // verify if unit can be be attacked
    //                isAllowedToApplyPwrToThisUnit = partyUnit.UnitStatusConfig.GetCanBeAttacked(activeUnitIsFromThisParty);
    //                // set message based on the unit status
    //                errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToAttackMessage(activeUnitIsFromThisParty);
    //                // set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //            }
    //        }
    //    }
    //}

    //void PrepareBattleFieldForHealPower(bool activeUnitIsFromThisParty)
    //{
    //    Debug.Log("PrepareBattleFieldForHealPower");
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    string errorMessage = "";
    //    Color positiveColor = Color.green;
    //    // Color negativeColor = new Color32(221, 24, 24, 255); // dark red
    //    Color negativeColor = Color.grey;
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                // Unit canvas (and unit) is present
    //                //if (activeUnitIsFromThisParty)
    //                //{
    //                //    // highlight units which can be healed
    //                //    // Get party unit UI
    //                //    PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                //    // Get party unit
    //                //    PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                //    switch (partyUnit.UnitStatus)
    //                //    {
    //                //        case UnitStatus.Active:
    //                //        case UnitStatus.Waiting:
    //                //        case UnitStatus.Escaping:
    //                //            if (partyUnit.UnitHealthCurr < partyUnit.GetUnitEffectiveMaxHealth())
    //                //            {
    //                //                // unit can be healed
    //                //                isAllowedToApplyPwrToThisUnit = true;
    //                //                errorMessage = "";
    //                //            }
    //                //            else if (partyUnit.UnitHealthCurr == partyUnit.GetUnitEffectiveMaxHealth())
    //                //            {
    //                //                // unit cannot be healed
    //                //                isAllowedToApplyPwrToThisUnit = false;
    //                //                errorMessage = "Cannot heal this unit. Unit health is already full.";
    //                //            }
    //                //            break;
    //                //        case UnitStatus.Dead:
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot heal dead units. This unit should be first resurected.";
    //                //            break;
    //                //        case UnitStatus.Escaped:
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot heal units which escaped from the battle field.";
    //                //            break;
    //                //        default:
    //                //            Debug.LogError("Unknown unit status");
    //                //            break;
    //                //    }
    //                //}
    //                //else
    //                //{
    //                //    // this is actions for enemy party
    //                //    // set is as not unit to which we can apply powers
    //                //    isAllowedToApplyPwrToThisUnit = false;
    //                //    errorMessage = "Cannot heal enemy units.";
    //                //}
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // Get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // Get if unit health is full
    //                bool unitHealthIsFull = partyUnit.UnitHealthCurr == partyUnit.GetUnitEffectiveMaxHealth();
    //                // verify if unit can be be healed
    //                isAllowedToApplyPwrToThisUnit = partyUnit.UnitStatusConfig.GetCanBeHealed(activeUnitIsFromThisParty, unitHealthIsFull);
    //                // set message based on the unit status and current health
    //                errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToHealMessage(activeUnitIsFromThisParty, unitHealthIsFull);
    //                // Set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //            }
    //        }
    //    }
    //}

    //void PrepareBattleFieldForResurectPower(bool activeUnitIsFromThisParty)
    //{
    //    Debug.Log("PrepareBattleFieldForResurectPower");
    //    bool isAllowedToApplyPwrToThisUnit = false;
    //    string errorMessage = "";
    //    Color positiveColor = Color.green;
    //    Color negativeColor = Color.grey;
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                //// Unit canvas (and unit) is present
    //                //if (activeUnitIsFromThisParty)
    //                //{
    //                //    // highlight units which can be healed
    //                //    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //                //    switch (unit.UnitStatus)
    //                //    {
    //                //        case UnitStatus.Active:
    //                //        case UnitStatus.Waiting:
    //                //        case UnitStatus.Escaping:
    //                //        case UnitStatus.Escaped:
    //                //            // unit is alive and cannot be resurected
    //                //            isAllowedToApplyPwrToThisUnit = false;
    //                //            errorMessage = "Cannot resurect alive units.";
    //                //            break;
    //                //        case UnitStatus.Dead:
    //                //            // unit is dead and can be resurected
    //                //            isAllowedToApplyPwrToThisUnit = true;
    //                //            errorMessage = "";
    //                //            break;
    //                //        default:
    //                //            Debug.LogError("Unknown unit status");
    //                //            break;
    //                //    }
    //                //}
    //                //else
    //                //{
    //                //    // this is actions for enemy party
    //                //    // set is as not unit to which we can apply powers
    //                //    isAllowedToApplyPwrToThisUnit = false;
    //                //    errorMessage = "Cannot resurect enemy units.";
    //                //}
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // Get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // verify if unit can be be healed
    //                isAllowedToApplyPwrToThisUnit = partyUnit.UnitStatusConfig.GetCanBeResurected(activeUnitIsFromThisParty);
    //                // set message based on the unit status and current health
    //                errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToResurectMessage(activeUnitIsFromThisParty);
    //                // Set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
    //            }
    //        }
    //    }
    //}

    //void PrepareBattleFieldForActiveUnit(bool activeUnitIsFromThisParty, PartyUnitUI unitToActivateUI)
    //{
    //    // init variables, which are used in a loop
    //    bool isAllowedToApplyPwrToTargetUnit = false;
    //    string errorMessage = "";
    //    // those will be needed only for mele units
    //    MeleUnitBlockingCondition meleUnitBlockingCondition = MeleUnitBlockingCondition.None;
    //    Row activeMeleUnitRow = activeBattleUnitUI.GetUnitRow().Row;
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // verify if destination slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                // Get party unit UI
    //                PartyUnitUI partyUnitUI = unitSlot.GetComponentInChildren<PartyUnitUI>();
    //                // get party unit
    //                PartyUnit partyUnit = partyUnitUI.LPartyUnit;
    //                // reset is allowed to apply power to target unit
    //                isAllowedToApplyPwrToTargetUnit = false;
    //                // verify if unit's ability is mele or ranged
    //                if (unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbilityRange == UnitAbilityRange.Mele)
    //                {
    //                    // get active mele unit blocking condition
    //                    meleUnitBlockingCondition = GetMeleUnitBlockingCondition(cell, horisontalPanel, activeMeleUnitRow);
    //                }
    //                // verify if ability type is damaging
    //                if ((unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbilityTypes & UnitAbilityTypes.Damaging) == UnitAbilityTypes.Damaging)
    //                {
    //                    // verify if unit can be be attacked (return true if isAllowedToApplyPwrToTargetUnit is already set to true or if it unit can be attacked)
    //                    isAllowedToApplyPwrToTargetUnit = isAllowedToApplyPwrToTargetUnit | partyUnit.UnitStatusConfig.GetCanBeAttacked(activeUnitIsFromThisParty, meleUnitBlockingCondition);
    //                    // set message based on the unit status (Note: if isAllowedToApplyPwrToTargetUnit is true, then message anyway will not be shown)
    //                    errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToAttackMessage(activeUnitIsFromThisParty, meleUnitBlockingCondition);
    //                }
    //                // verify if ability type is healing
    //                if ((unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbilityTypes & UnitAbilityTypes.Healing) == UnitAbilityTypes.Healing)
    //                {
    //                    // Get if unit health is full
    //                    bool unitHealthIsFull = partyUnit.UnitHealthCurr == partyUnit.GetUnitEffectiveMaxHealth();
    //                    // verify if unit can be be healed (return true if isAllowedToApplyPwrToTargetUnit is already set to true or if it unit can be healed)
    //                    isAllowedToApplyPwrToTargetUnit = isAllowedToApplyPwrToTargetUnit | partyUnit.UnitStatusConfig.GetCanBeHealed(activeUnitIsFromThisParty, unitHealthIsFull);
    //                    // set message based on the unit status and current health (Note: if isAllowedToApplyPwrToTargetUnit is true, then message anyway will not be shown)
    //                    errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToHealMessage(activeUnitIsFromThisParty, unitHealthIsFull);
    //                }
    //                // verify if ability type is resurecting
    //                if ((unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbilityTypes & UnitAbilityTypes.Resurecting) == UnitAbilityTypes.Resurecting)
    //                {
    //                    // verify if unit can be be resurected (return true if isAllowedToApplyPwrToTargetUnit is already set to true or if it unit can be resurected)
    //                    isAllowedToApplyPwrToTargetUnit = isAllowedToApplyPwrToTargetUnit | partyUnit.UnitStatusConfig.GetCanBeResurected(activeUnitIsFromThisParty);
    //                    Debug.Log("Unit: " + partyUnit.UnitName + ". Allowed to resurect is " + isAllowedToApplyPwrToTargetUnit);
    //                    // set message based on the unit status (Note: if isAllowedToApplyPwrToTargetUnit is true, then message anyway will not be shown)
    //                    errorMessage = partyUnitUI.UnitStatusUIConfig.GetOnTryToResurectMessage(activeUnitIsFromThisParty);
    //                }
    //                // set if cell can be targeted
    //                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToTargetUnit, 
    //                    transform.Find(horisontalPanel + "/" + cell), 
    //                    errorMessage, // Note: if isAllowedToApplyPwrToTargetUnit is true, then message will not be shown
    //                    unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbility.abilityIsApplicableColor,
    //                    unitToActivateUI.LPartyUnit.UnitAbilityConfig.unitAbility.abilityNotApplicableColor);
    //            }
    //            else
    //            {
    //                // no unit in this slot
    //                // verify if this slot is active (to not apply things to disabled unit slots)
    //                if (unitSlot.gameObject.activeSelf)
    //                {
    //                    // prohibit applying powers to inactive slots
    //                    isAllowedToApplyPwrToTargetUnit = false;
    //                    // set error message
    //                    errorMessage = "There is no unit in target cell.";
    //                    // No unit in cell
    //                    SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToTargetUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, Color.green, new Color32(32, 32, 32, 255));
    //                }
    //            }
    //        }
    //    }
    //}
    //public void HighlightActiveUnitInBattle(PartyUnit unitToActivate, bool doHighlight = true)
    //{
    //    Color highlightColor;
    //    if (doHighlight)
    //    {
    //        Debug.Log(" HighlightActiveUnitInBattle");
    //        highlightColor = Color.white;
    //    }
    //    else
    //    {
    //        Debug.Log(" Remove highlight from active unit in battle");
    //        highlightColor = Color.grey;
    //    }
    //    // highlight unit canvas with required color
    //    // structure: 3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
    //    Transform cellTr = unitToActivate.transform.parent.parent.parent;
    //    Text canvasText = cellTr.Find("Br").GetComponent<Text>();
    //    canvasText.color = highlightColor;
    //}

    //public IEnumerator SetActiveUnitInBattle(PartyUnitUI unitToActivateUI)
    //{
    //    //Debug.Log("SetActiveUnitInBattle " + unitToActivate.UnitName);
    //    // save it locally for later use
    //    activeBattleUnitUI = unitToActivateUI;
    //    // new unit became active in battle
    //    // highlight differently cells which this unit can or cannot interract and in which way
    //    // act based on activated unit relationships with this panel
    //    // verify if this is enemy unit or unit from this party
    //    bool activeUnitIsFromThisParty = GetIsUnitFriendly(unitToActivateUI);
    //    // If active unit is from this party
    //    //// Then trigger buffs and debuffs before applying highlights
    //    //if (activeUnitIsFromThisParty)
    //    //{
    //    //    // Verify if unit has buffs which should be removed, example: defense
    //    //    DeactivateExpiredBuffs(unitToActivate);
    //    //    // Verify if unit has debuffs which should be applied, example: poison
    //    //    TriggerAppliedDebuffs(unitToActivate);
    //    //    // Deactivate debuffs which has expired, example: poison duration may last 2 turns
    //    //    // This is checked and done after debuff trigger
    //    //    //DeactivateExpiredDebuffs(unitToActivate);
    //    //}
    //    // defined below how actions applied to the friendly and enemy units
    //    // based on the active unit powers
    //    //switch (unitToActivateUI.LPartyUnit.UnitAbilityID)
    //    //{
    //    //    // Helping or buf powers
    //    //    case UnitAbilityID.HealingWord:
    //    //    case UnitAbilityID.HealingSong:
    //    //    case UnitAbilityID.SacrificingEcho:
    //    //        PrepareBattleFieldForHealPower(activeUnitIsFromThisParty);
    //    //        break;
    //    //    case UnitAbilityID.Resurect:
    //    //        PrepareBattleFieldForResurectPower(activeUnitIsFromThisParty);
    //    //        break;
    //    //    // Mele attack powers
    //    //    case UnitAbilityID.BlowWithGreatSword:
    //    //    case UnitAbilityID.BlowWithMaul:
    //    //    case UnitAbilityID.CutWithAxe:
    //    //    case UnitAbilityID.CutWithDagger:
    //    //    case UnitAbilityID.SlashWithSword:
    //    //    case UnitAbilityID.StabWithDagger:
    //    //    case UnitAbilityID.StompWithFoot:
    //    //        PrepareBattleFieldForMelePower(activeUnitIsFromThisParty);
    //    //        break;
    //    //    // Ranged attack powers
    //    //    case UnitAbilityID.ShootWithBow:
    //    //    case UnitAbilityID.ShootWithCompoudBow:
    //    //    case UnitAbilityID.ThrowSpear:
    //    //    case UnitAbilityID.ThrowRock:
    //    //    case UnitAbilityID.DrainLife:
    //    //        PrepareBattleFieldForRangedPower(activeUnitIsFromThisParty);
    //    //        break;
    //    //    // Magic (including pure or whole-party) attack powers
    //    //    case UnitAbilityID.CastChainLightning:
    //    //    case UnitAbilityID.CastLightningStorm:
    //    //    case UnitAbilityID.HolyWord:
    //    //    case UnitAbilityID.EarthShatteringLeap:
    //    //    case UnitAbilityID.Malediction:
    //    //        PrepareBattleFieldForMagicPower(activeUnitIsFromThisParty);
    //    //        break;
    //    //    default:
    //    //        Debug.LogError("Unknown unit power");
    //    //        break;
    //    //}
    //    // prepare battle field for active unit
    //    // PrepareBattleFieldForActiveUnit(activeUnitIsFromThisParty, unitToActivateUI);
    //    // Highlight active unit itself, this should be done after previous highlights
    //    // to override their logic
    //    if (activeUnitIsFromThisParty)
    //    {
    //        // This unit belongs to this party highlight it here
    //        // without adding to a queue
    //        unitToActivateUI.HighlightActiveUnitInBattle(true);
    //    }
    //    yield return null;
    //}

    //public void OnBattleNewUnitHasBeenActivated(System.Object context)
    //{
    //    // verify if context is PartyUnitUI
    //    if (context is PartyUnitUI)
    //    {
    //        // init party unit UI from context
    //        PartyUnitUI partyUnitUI = (PartyUnitUI)context;
    //        // save it locally for later use
    //        activeBattleUnitUI = partyUnitUI;
    //    }
    //    else
    //    {
    //        Debug.Log("Context doesn't match");
    //    }
    //}

    //void DeactivateExpiredBuffs(PartyUnit unit)
    //{
    //    // Deactivate expired buffs in UI
    //    UnitBuffIndicator[] buffsUI = unit.GetUnitBuffsPanel().GetComponentsInChildren<UnitBuffIndicator>();
    //    foreach (UnitBuffIndicator buffUI in buffsUI)
    //    {
    //        // First decrement buff current duration
    //        buffUI.DecrementCurrentDuration();
    //        // Verify if it has timed out;
    //        if (buffUI.GetCurrentDuration() == 0)
    //        {
    //            // buff has timed out
    //            // deactivate it (it will be destroyed at the end of animation)
    //            buffUI.SetActiveAdvance(false);
    //            // deactivate it in unit properties too
    //            unit.GetUnitBuffs()[(int)buffUI.GetUnitBuff()] = UnitBuff.None;
    //        }
    //    }
    //}

    //void TriggerAppliedDebuffs(PartyUnit unit)
    //{
    //    UnitDebuffIndicator[] debuffsIndicators = unit.GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
    //    //UnitDebuffsUI unitDebuffsUI = unit.GetUnitDebuffsPanel().GetComponent<UnitDebuffsUI>();
    //    foreach (UnitDebuffIndicator debuffIndicator in debuffsIndicators)
    //    {
    //        Debug.Log(unit.name);
    //        // as long as we cannot initiate all debuffs at the same time
    //        // we add debuffs to the queue and they will be triggered one after another
    //        // CoroutineQueue queue = unitDebuffsUI.GetQueue();
    //        CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).GetQueue();
    //        if (queue == null)
    //        {
    //            Debug.LogError("No queue");
    //        }
    //        if (debuffIndicator == null)
    //        {
    //            Debug.LogError("No debuffIndicator");
    //        }
    //        IEnumerator coroutine = debuffIndicator.TriggerDebuff(unit);
    //        if (coroutine == null)
    //        {
    //            Debug.LogError("No coroutine");
    //        }
    //        queue.Run(coroutine);
    //        // Trigger debuff against player
    //        // Decrement buff current duration
    //        debuffIndicator.DecrementCurrentDuration();
    //    }
    //}

    //void DeactivateExpiredDebuffs(PartyUnit unit)
    //{
    //    // Deactivate expired buffs in UI
    //    UnitDebuffIndicator[] debuffsIndicators = unit.GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
    //    foreach (UnitDebuffIndicator debuffIndicator in debuffsIndicators)
    //    {
    //        // Verify if it has timed out;
    //        if (debuffIndicator.GetCurrentDuration() == 0)
    //        {
    //            // buff has timed out
    //            // deactivate it (it will be destroyed at the end of animation)
    //            debuffIndicator.SetActiveAdvance(false);
    //            // deactivate it in unit properties too
    //            unit.GetUnitBuffs()[(int)debuffIndicator.GetUnitBuff()] = UnitBuff.None;
    //        }
    //    }
    //}

    //void ApplyHealingPowerToSingleUnit(PartyUnitUI dstUnitUI)
    //{
    //    Debug.Log("ApplyHealPowerToSingleUnit");
    //    //// heal destination unit
    //    //int healthAfterHeal = dstUnitUI.LPartyUnit.UnitHealthCurr + activeBattleUnitUI.LPartyUnit.UnitAbilityCurrentPower;
    //    //// make sure that we do not heal to more than maximum health
    //    //if (healthAfterHeal > dstUnitUI.LPartyUnit.GetUnitEffectiveMaxHealth())
    //    //{
    //    //    healthAfterHeal = dstUnitUI.LPartyUnit.GetUnitEffectiveMaxHealth();
    //    //}
    //    //dstUnitUI.LPartyUnit.UnitHealthCurr = (healthAfterHeal);
    //    //// update current health in UI
    //    //Text currentHealth = dstUnitUI.GetUnitCurrentHealthText();
    //    //currentHealth.text = healthAfterHeal.ToString();
    //    //// update info panel
    //    //dstUnitUI.UnitInfoPanelText.text = "+" + activeBattleUnitUI.LPartyUnit.UnitAbilityCurrentPower.ToString();
    //    //dstUnitUI.UnitInfoPanelText.color = Color.green;
    //    ApplyUnitAbility(dstUnitUI);
    //}

    //void ApplyHealingPowerToMultipleUnits()
    //{
    //    Debug.Log("ApplyHealPowerToMultipleUnits");
    //    // get all alive units in enemy party and apply damage to them
    //    // find enemy party based on activeBattleUnit
    //    // structure: 7BattleScreen-6Party-5PartyPanel-4[Top/Middle/Bottom]-3[Front/Back/Wide]cell-2UnitSlot-1UnitCanvas-activeBattleUnit
    //    HeroParty activeBattleUnitParty = activeBattleUnitUI.LPartyUnit.GetUnitParty(); //.transform.parent.parent.parent.parent.parent.parent;
    //    // find party which does not match activeBattleUnit Party
    //    foreach (HeroPartyUI heroPartyUI in transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<HeroPartyUI>())
    //    {
    //        // verify if this is the same (friendly) hero party where active unit is lcoated
    //        if (heroPartyUI.LHeroParty.gameObject.GetInstanceID() == activeBattleUnitParty.gameObject.GetInstanceID())
    //        {
    //            // heal all party members to which ability is applicable
    //            foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
    //            {
    //                // verify if ability is applicable
    //                if (partyUnitUI.LPartyUnit.UnitAbilityConfig.IsApplicableToUnit(partyUnitUI.LPartyUnit))
    //                {
    //                    ApplyHealingPowerToSingleUnit(partyUnitUI);
    //                }
    //            }
    //        }
    //    }
    //}

    //void ApplyResurectPower(PartyUnitUI dstUnitUI)
    //{
    //    Debug.Log("ApplyResurectPower");
    //}

    //void ClearInfoPanel(Transform changedCell)
    //{
    //    Color32 defaultColor = new Color32(180, 180, 180, 255);
    //    Text infoPanelTxt = changedCell.Find("InfoPanel").GetComponent<Text>();
    //    infoPanelTxt.text = "";
    //    infoPanelTxt.color = defaultColor;
    //}

    //void ClearInfoPanel(Transform partyPanelTr, Row horisontalPanel, Cell cell)
    //{
    //    ClearInfoPanel(partyPanelTr.Find(horisontalPanel + "/" + cell));
    //}

    public void ResetUnitCellInfoPanel()
    {
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            partyUnitUI.ClearUnitInfoPanel();
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // Unit canvas (and unit) is present
        //        // verify if slot has an unit in it
        //        Transform unitSlot = partyPanel.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            //// get unit for later checks
        //            //PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        //            //// verify if unit is alive and did not flee from battle
        //            //if (unit.GetIsAlive() && !unit.GetHasEscaped())
        //            //{
        //                // clear both info panels
        //                //ClearInfoPanel(partyPanel, horisontalPanel, cell);
        //            //}
        //        }
        //    }
        //}
    }

    public void ResetUnitCellHighlight()
    {
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        transform.Find(horisontalPanel + "/" + cell).Find("Br").GetComponent<Text>().color = new Color32(128, 128, 128, 255);
        //    }
        //}
        foreach(PartyPanelCell partyPanelCell in GetComponentsInChildren<PartyPanelCell>(true))
        {
            partyPanelCell.CanvasText.color = new Color32(128, 128, 128, 255);
        }
    }

    //public void RemoveDeadUnits()
    //{
    //    //City city = GetCity();
    //    foreach (Row horisontalPanel in horisontalPanels)
    //    {
    //        foreach (Cell cell in cells)
    //        {
    //            // Unit canvas (and unit) is present
    //            // verify if slot has an unit in it
    //            Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            if (unitSlot.childCount > 0)
    //            {
    //                PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //                if (unit.UnitStatus == UnitStatus.Dead)
    //                {
    //                    // destroy unit canvas
    //                    Debug.Log("Verify: destroy dead unit " + unit.name);
    //                    //city.DismissGenericUnit(unitSlot.GetComponent<UnitSlot>());
    //                    transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().DismissGenericUnit(unitSlot.GetComponent<UnitSlot>());
    //                    // Destroy(unitSlot.GetChild(0).gameObject);
    //                }
    //            }
    //        }
    //    }
    //}

    //void ClearUnitCellStatus(Transform targetCell)
    //{
    //    Color32 defaultColor = new Color32(180, 180, 180, 255);
    //    Text infoPanelTxt = targetCell.Find("UnitStatus").GetComponent<Text>();
    //    infoPanelTxt.text = "";
    //    infoPanelTxt.color = defaultColor;
    //}

    //void ClearUnitCellStatus(Transform partyPanelTr, Row horisontalPanel, Cell cell)
    //{
    //    ClearUnitCellStatus(partyPanelTr.Find(horisontalPanel + "/" + cell));
    //}

    public void ResetUnitCellStatus(string[] exceptions)
    {
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // Do not remove some statuses, because they should persist
            // Verify if it is in exceptions
            bool itIsException = false;
            string partyUnitStatusText = partyUnitUI.UnitStatusText.text;
            foreach (string exception in exceptions)
            {
                if (exception == partyUnitStatusText)
                {
                    itIsException = true;
                }
            }
            if (!itIsException)
            {
                // Clear status in UI, because it is not in exceptions list
                partyUnitUI.ClearPartyUnitStatusUI();
                // Reset status in unit
                partyUnitUI.LPartyUnit.UnitStatus = UnitStatus.Active;
            }
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // Unit canvas (and unit) is present
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // Do not remove some statuses, because they should persist
        //            // Verify if it is in exceptions
        //            Text unitStatus = unitSlot.parent.Find("UnitStatus").GetComponent<Text>();
        //            bool itIsException = false;
        //            foreach (string exception in exceptions)
        //            {
        //                if (exception == unitStatus.text)
        //                {
        //                    itIsException = true;
        //                }
        //            }
        //            if (!itIsException)
        //            {
        //                // Clear status in UI, because it is not in exceptions list
        //                ClearUnitCellStatus(transform, horisontalPanel, cell);
        //                // Reset status in unit
        //                unitSlot.GetComponentInChildren<PartyUnitUI>().SetUnitStatus(UnitStatus.Active);
        //            }
        //        }
        //    }
        //}
    }

    //void ClearUnitCellBuffsOrDebuffs(Transform partyPanelTr, Row horisontalPanel, Cell cell, string whatToRemove)
    //{
    //    UnitBuffIndicator[] allBuffs = partyPanelTr.Find(horisontalPanel + "/" + cell + "/" + "UnitStatus/" + whatToRemove).GetComponentsInChildren<UnitBuffIndicator>();
    //    foreach (UnitBuffIndicator buff in allBuffs)
    //    {
    //        Destroy(buff.gameObject);
    //    }
    //}

    public void RemoveAllBuffsAndDebuffs()
    {
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            partyUnitUI.RemoveAllBuffsAndDebuffs();
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // Unit canvas (and unit) is present
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // Remove all buffs and debuffs from unit
        //            unitSlot.GetComponentInChildren<PartyUnitUI>().RemoveAllBuffsAndDebuffs();
        //        }
        //    }
        //}
        //yield return null;
    }

    //void ApplyUnitPowerModifiersToSingleUnit(PartyUnitUI dstUnitUI)
    //{
    //    // get active unit
    //    PartyUnit activeUnit = activeBattleUnitUI.LPartyUnit;
    //    // get destination unit
    //    PartyUnit destinationUnit = dstUnitUI.LPartyUnit;
    //    // loop through all Unit Power Modifiers
    //    foreach (UnitPowerModifier unitPowerModifier in activeUnit.UnitAbilityConfig.unitPowerModifiers)
    //    {
    //        // get and apply unit PowerModifier
    //        unitPowerModifier.Apply(activeUnit, destinationUnit);
    //    }
    //}

    //void ApplyUnitAbility(PartyUnitUI dstUnitUI)
    //{
    //    // apply unit ability
    //    activeBattleUnitUI.LPartyUnit.UnitAbilityConfig.unitAbility.Apply(activeBattleUnitUI.LPartyUnit, dstUnitUI.LPartyUnit);
    //}

    //void ApplyDestructivePowerToSingleUnitUI(PartyUnitUI dstUnitUI)
    //{
    //    //Debug.Log("ApplyDestructivePowerToSingleUnit");
    //    // Note: order is important
    //    // ApplyUnitPowerModifiersToSingleUnit - in case of LifeLeech, we need to get unit health before main ability bein applied, 
    //    // because if unit has 35 life left and vampire has 60 damange, then on attack he will heal itself for 35 life, not for 60
    //    ApplyUnitPowerModifiersToSingleUnit(dstUnitUI);
    //    ApplyUnitAbility(dstUnitUI);
    //    // dstUnitUI.ApplyDestructiveAbility(dstUnitUI.LPartyUnit.GetAbilityDamageDealt(activeBattleUnitUI.LPartyUnit));
    //    ApplyUniquePowerModifiersToSingleUnit(dstUnitUI);
    //}


    //public void SetUnitDebuffActive(PartyUnitUI partyUnitUI, UniquePowerModifier uniquePowerModifier, bool doActivate)
    //{
    //    // get unit debuffs panel
    //    Transform debuffsPanel = partyUnitUI.GetUnitDebuffsPanel();
    //    if (doActivate)
    //    {
    //        // verify if unit already has this debuf
    //        if (uniquePowerModifier.upmAppliedDebuff == partyUnitUI.LPartyUnit.UnitDebuffs[(int)uniquePowerModifier.upmAppliedDebuff])
    //        {
    //            // the same debuff is already applied
    //            // reset its counter to max
    //            // .. fix, verify if it is there
    //            if (debuffsPanel.Find(uniquePowerModifier.upmAppliedDebuff.ToString()))
    //            {
    //                UnitDebuffIndicator unitDebuffIndicator = debuffsPanel.Find(uniquePowerModifier.upmAppliedDebuff.ToString()).GetComponent<UnitDebuffIndicator>();
    //                if (unitDebuffIndicator)
    //                {
    //                    unitDebuffIndicator.CurrentDuration = unitDebuffIndicator.TotalDuration;
    //                }
    //                else
    //                {
    //                    Debug.LogError("No unitDebuffIndicator");
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // debuff is not applied yet
    //            // add debuff to unit
    //            //Debug.Log(((int)UnitBuff.DefenseStance).ToString());
    //            //Debug.Log(partyUnit.GetUnitBuffs().Length.ToString());
    //            partyUnitUI.LPartyUnit.UnitDebuffs[(int)uniquePowerModifier.upmAppliedDebuff] = uniquePowerModifier.upmAppliedDebuff;
    //            // create debuff by duplicating from template
    //            // Note: debuff name in template should be the same as in AppliedDebuff
    //            Transform debuffTemplate = transform.root.Find("Templates/UI/Debuffs/" + uniquePowerModifier.upmAppliedDebuff.ToString());
    //            Debug.Log("Applying " + uniquePowerModifier.upmAppliedDebuff.ToString() + " debuff");
    //            Transform newDebuff = Instantiate(debuffTemplate, debuffsPanel);
    //            // activate buff
    //            newDebuff.GetComponent<UnitDebuffIndicator>().SetActiveAdvance(true, uniquePowerModifier);
    //            // rename it so it can be later found by name
    //            newDebuff.name = uniquePowerModifier.upmAppliedDebuff.ToString();
    //        }
    //    }
    //    else
    //    {
    //        // remove buff
    //        partyUnitUI.LPartyUnit.UnitDebuffs[(int)uniquePowerModifier.upmAppliedDebuff] = UnitDebuff.None;
    //        Destroy(debuffsPanel.Find(uniquePowerModifier.upmAppliedDebuff.ToString()).gameObject);
    //    }
    //}

    //void ApplyUniquePowerModifierToSingleUnit(PartyUnit dstUnit, UniquePowerModifier uniquePowerModifier)
    //{
    //    // Set debuff in UI
    //    // Set debuff in unit properties
    //    dstUnit.AddDebuff(uniquePowerModifier);
    //}

    //void ApplyUniquePowerModifiersToSingleUnit(PartyUnitUI dstUnitUI)
    //{
    //    foreach (UniquePowerModifier uniquePowerModifier in activeBattleUnitUI.LPartyUnit.UniquePowerModifiers)
    //    {
    //        SetUnitDebuffActive(dstUnitUI, uniquePowerModifier, true);
    //        // ApplyUniquePowerModifierToSingleUnit(dstUnit, uniquePowerModifier);
    //    }
    //}

    //void ApplyDestructivePowerToMultipleUnits()
    //{
    //    Debug.Log("ApplyDestructivePowerToMultipleUnits");
    //    // get all alive units in enemy party and apply damage to them
    //    // find enemy party based on activeBattleUnit
    //    // structure: 7BattleScreen-6Party-5PartyPanel-4[Top/Middle/Bottom]-3[Front/Back/Wide]cell-2UnitSlot-1UnitCanvas-activeBattleUnit
    //    HeroParty activeBattleUnitParty = activeBattleUnitUI.LPartyUnit.GetUnitParty(); //.transform.parent.parent.parent.parent.parent.parent;
    //    // find party which does not match activeBattleUnit Party
    //    foreach (HeroPartyUI heroPartyUI in transform.root.GetComponentInChildren<UIManager>().GetComponentsInChildren<HeroPartyUI>())
    //    {
    //        if (heroPartyUI.LHeroParty.gameObject.GetInstanceID() != activeBattleUnitParty.gameObject.GetInstanceID())
    //        {
    //            // we found other (enemy hero party)
    //            // apply damage to all party members who can fight
    //            foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
    //            {
    //                // verify if unit can fight
    //                if (partyUnitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle())
    //                {
    //                    ApplyDestructivePowerToSingleUnitUI(partyUnitUI);
    //                }
    //            }
    //            //foreach (Row horisontalPanel in horisontalPanels)
    //            //{
    //            //    foreach (Cell cell in cells)
    //            //    {
    //            //        PartyUnitUI unitUI = GetUnitUIWhichCanFight(horisontalPanel, cell);
    //            //        if (unitUI)
    //            //        {
    //            //            ApplyDestructivePowerToSingleUnit(unitUI);
    //            //        }
    //            //        //// verify if slot has an unit in it
    //            //        //Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
    //            //        //if (unitSlot.childCount > 0)
    //            //        //{
    //            //        //    // get unit for later checks
    //            //        //    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //            //        //    // verify if unit is alive and has not escaped the battle
    //            //        //    if (unit.UnitStatus == UnitStatus.Active)
    //            //        //    {
    //            //        //        // apply damage to the unit
    //            //        //        ApplyDestructivePowerToSingleUnit(unit);
    //            //        //    }
    //            //        //}
    //            //    }
    //            //}
    //        }
    //    }
    //}

    //public void ApplyPowersToUnit(PartyUnitUI dstUnitUI)
    //{
    //    // Trigger Event
    //    triggerUnitAbility.Raise(activeBattleUnitUI.gameObject, dstUnitUI.gameObject);
    //    // reset cell info panel beforehand, for both parties, to clean up previous information
    //    //ResetUnitCellInfoPanel();
    //    //activeBattleUnitUI.GetUnitPartyPanel().ResetUnitCellInfoPanel();
    //    // in case of applying magic powers it is possible to click on the unit slot, where there is no unit
    //    // but still the power should be applied
    //    //if (dstUnitUI)
    //    //{
    //    //    //Debug.Log(activeBattleUnit.UnitName + " acting upon " + dstUnit.UnitName + " or whole party");
    //    //    switch (activeBattleUnitUI.LPartyUnit.UnitAbilityID)
    //    //    {
    //    //        // Helping or buf powers
    //    //        case UnitAbilityID.HealingWord:
    //    //            ApplyHealingPowerToSingleUnit(dstUnitUI);
    //    //            break;
    //    //        case UnitAbilityID.HealingSong:
    //    //        case UnitAbilityID.SacrificingEcho:
    //    //            ApplyHealingPowerToMultipleUnits();
    //    //            break;
    //    //        case UnitAbilityID.Resurect:
    //    //            ApplyResurectPower(dstUnitUI);
    //    //            break;
    //    //        // Mele attack powers
    //    //        case UnitAbilityID.BlowWithGreatSword:
    //    //        case UnitAbilityID.BlowWithMaul:
    //    //        case UnitAbilityID.CutWithAxe:
    //    //        case UnitAbilityID.CutWithDagger:
    //    //        case UnitAbilityID.SlashWithSword:
    //    //        case UnitAbilityID.StabWithDagger:
    //    //        case UnitAbilityID.StompWithFoot:
    //    //        // Ranged attack powers
    //    //        case UnitAbilityID.ShootWithBow:
    //    //        case UnitAbilityID.ShootWithCompoudBow:
    //    //        case UnitAbilityID.ThrowSpear:
    //    //        case UnitAbilityID.ThrowRock:
    //    //        case UnitAbilityID.DrainLife:
    //    //            ApplyDestructivePowerToSingleUnitUI(dstUnitUI);
    //    //            break;
    //    //        // Magic (including pure or whole-party) attack powers
    //    //        case UnitAbilityID.CastChainLightning:
    //    //        case UnitAbilityID.CastLightningStorm:
    //    //        case UnitAbilityID.HolyWord:
    //    //        case UnitAbilityID.EarthShatteringLeap:
    //    //        case UnitAbilityID.Malediction:
    //    //            ApplyDestructivePowerToMultipleUnits();
    //    //            break;
    //    //        default:
    //    //            Debug.LogError("Unknown unit power");
    //    //            break;
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    // in case of magic power - apply it to all units in enemy party
    //    //    Debug.Log(activeBattleUnitUI.LPartyUnit.UnitName + " acting upon whole party");
    //    //    switch (activeBattleUnitUI.LPartyUnit.UnitAbilityID)
    //    //    {
    //    //        // Helping or buf powers
    //    //        case UnitAbilityID.HealingSong:
    //    //        case UnitAbilityID.SacrificingEcho:
    //    //            ApplyHealingPowerToMultipleUnits();
    //    //            break;
    //    //        // Magic (including pure or whole-party) attack powers
    //    //        case UnitAbilityID.CastChainLightning:
    //    //        case UnitAbilityID.CastLightningStorm:
    //    //        case UnitAbilityID.HolyWord:
    //    //        case UnitAbilityID.EarthShatteringLeap:
    //    //        case UnitAbilityID.Malediction:
    //    //            ApplyDestructivePowerToMultipleUnits();
    //    //            break;
    //    //        default:
    //    //            Debug.LogError("Unknown unit power");
    //    //            break;
    //    //    }
    //    //}
    //    // Gradually fade away unit cell information
    //    // CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).Queue;
    //    // CoroutineQueueManager.Run(FadeUnitsCellInfo());
    //    // StartCoroutine("FadeUnitCellInfo");
    //}

    //public void SetUnitDefenseBuffActive(PartyUnitUI partyUnitUI)
    //{
    //    // get unit buffs panel
    //    Transform buffsPanel = partyUnitUI.GetUnitBuffsPanel();
    //    if (doActivate)
    //    {
    //        // add buff to unit
    //        // Debug.Log(((int)UnitBuff.DefenseStance).ToString());
    //        // Debug.Log(partyUnit.GetUnitBuffs().Length.ToString());
    //        partyUnitUI.LPartyUnit.UnitBuffs[(int)UnitBuff.DefenseStance] = UnitBuff.DefenseStance;
    //        // create buff by duplicating from template
    //        Transform buffTemplate = transform.root.Find("Templates/UI/Buffs/Defense");
    //        Transform defenseBuff = Instantiate(buffTemplate, buffsPanel);
    //        // activate buff
    //        defenseBuff.GetComponent<UnitBuffIndicator>().SetActiveAdvance(true);
    //        // rename it so it can be later found by name
    //        defenseBuff.name = "Defense";
    //    } else
    //    {
    //        // remove buff
    //        partyUnitUI.LPartyUnit.UnitBuffs[(int)UnitBuff.DefenseStance] = UnitBuff.None;
    //        Destroy(buffsPanel.Find("Defense").gameObject);
    //    }
    //}

    public bool HasEscapedBattle()
    {
        // verify if at least one unit has escaped the battle
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if unit statis is Escaped
            if (partyUnitUI.LPartyUnit.UnitStatus == UnitStatus.Escaped)
            {
                // found at least one unit which has escaped the battle
                return true;
            }
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // get unit for later checks
        //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        //            // verify if unit has escaped
        //            // if (unit.GetHasEscaped())
        //            if (unit.UnitStatus == UnitStatus.Escaped)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        return false;
    }

    int GetExperienceForDestroyedUnits()
    {
        int experiencePool = 0;
        foreach(PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if unit is dead
            if (partyUnitUI.LPartyUnit.UnitStatus == UnitStatus.Dead)
            {
                // unit is dead, add his experience reward to experiencePool
                experiencePool += partyUnitUI.LPartyUnit.UnitExperienceReward;
            }
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // get unit for later checks
        //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
        //            // verify if unit is dead
        //            // if (!unit.GetIsAlive())
        //            if (unit.UnitStatus == UnitStatus.Dead)
        //            {
        //                // unit is dead, add his experience reward to experiencePool
        //                experiencePool += unit.UnitExperienceReward;
        //            }
        //        }
        //    }
        //}
        return experiencePool;
    }

    List<PartyUnitUI> GetRemainingAlivePartyUnitUIs()
    {
        //int unitsLeft = 0;
        List<PartyUnitUI> remainingAlivePartyUnitUIs = new List<PartyUnitUI>();
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if unit can fight
            if (partyUnitUI.LPartyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle())
            {
                // unit is alive and has not escaped
                // add it to the list of remaining UIs
                remainingAlivePartyUnitUIs.Add(partyUnitUI);
                //// increment units left counter
                //unitsLeft += 1;
            }
        }
        // Return result
        return remainingAlivePartyUnitUIs;
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            if (GetUnitUIWhichCanFight(horisontalPanel, cell))
        //            {
        //                // unit is alive and has not escaped
        //                // increment units left counter
        //                unitsLeft += 1;
        //            }
        //        }
        //    }
        //}
        //return unitsLeft;
    }

    //bool UnitCanBeUpgraded()
    //{
    //    return false;
    //}

    //bool UnitHasReachedUpgradeLimit()
    //{
    //    return true;
    //}

    //void OfferPartyLeaderToLearnNewAbility()
    //{

    //}

    //void IncrementUnitMaxStats(PartyUnit unit)
    //{
    //    // this is done on lvl up
    //    unit.UnitExperienceReward = (unit.UnitExperienceReward + unit.UnitExperienceRewardIncrementOnLevelUp);
    //    unit.GetUnitEffectiveMaxHealth() = (unit.GetUnitEffectiveMaxHealth() + unit.UnitHealthMaxIncrementOnLevelUp);
    //    unit.UnitPower = (unit.UnitPower + unit.UnitPowerIncrementOnLevelUp);
    //}

    //void UpgradeUnitClass(PartyUnit unit)
    //{
    //    // this is done on lvl up

    //}

    void UpgradeUnit(PartyUnitUI unitUI)
    {
        Debug.Log("UpgradeUnit");
        // unit has reached new level
        // add upgrade point to the unit
        unitUI.LPartyUnit.UnitUpgradePoints += 1;
        // reset unit stats to maximum and reset experience counter
        unitUI.ResetUnitHealthToMax();
        // reset his experience to 0
        unitUI.LPartyUnit.UnitExperience = 0;
        // and increment his level
        unitUI.LPartyUnit.UnitLevel += 1;
        //// verify if this is party leader
        //if (unit.IsLeader)
        //{
        //    // this party leader
        //    // reset his experience to 0
        //    unit.UnitExperience = (0);
        //    // and increment his level
        //    unit.UnitLevel = (unit.UnitLevel + 1);
        //    // offer party leader to learn new ability
        //    OfferPartyLeaderToLearnNewAbility();
        //}
        //else
        //{
        //    // this common unit
        //    // verify if conditions which allow unit to upgrade are met
        //    if (UnitCanBeUpgraded())
        //    {
        //        // and upgrade unit to the next class 
        //        UpgradeUnitClass(unit);
        //    }
        //    else if (UnitHasReachedUpgradeLimit())
        //    {
        //        // increment max unit stats
        //        IncrementUnitMaxStats(unit);
        //        // reset unit stats to maximum and reset experience counter
        //        ResetUnitStatsToMax(unit);
        //        // reset his experience to 0
        //        unit.UnitExperience = (0);
        //        // and increment his level
        //        unit.UnitLevel = (unit.UnitLevel + 1);
        //    }
        //    else
        //    {
        //        // wait for upgrade condition to be fulfilled
        //        // keep unit's experience at max
        //        unit.UnitExperience = (unit.UnitExperienceRequiredToReachNewLevel);
        //    }
        //}
    }

    void SetGainedExperienceAndInformationInPartyUnitUI(PartyUnitUI partyUnitUI, int experiencePerUnit)
    {
        // add experience to the unit
        int newUnitExperienceValue = partyUnitUI.LPartyUnit.UnitExperience + experiencePerUnit;
        // verify if unit has not reached new level
        if (newUnitExperienceValue < partyUnitUI.LPartyUnit.UnitExperienceRequiredToReachNextLevel)
        {
            // unit has not reached new level
            // just update hist current experience value
            partyUnitUI.LPartyUnit.UnitExperience = (newUnitExperienceValue);
        }
        else
        {
            UpgradeUnit(partyUnitUI);
            // update status panel to indicate level up
            partyUnitUI.UnitStatusText.text = levelUpStatusText;
            partyUnitUI.UnitStatusText.color = Color.green;
        }
        // show gained experience
        partyUnitUI.UnitInfoPanelText.text = "+" + experiencePerUnit.ToString() + " Exp";
        partyUnitUI.UnitInfoPanelText.color = Color.green;
    }

    public void GrantAndShowExperienceGained(PartyPanel enemyPartyPanel)
    {
        // Get the list of all remaining alive units in party
        List<PartyUnitUI> remainingAlivePartyUnitUIs = GetRemainingAlivePartyUnitUIs();
        // distribute experience between all units, which are alive and not escaped
        // get all experience gained for destroyed enemy units
        // and divide it by the number of alive units in the party
        int experiencePerUnit = enemyPartyPanel.GetExperienceForDestroyedUnits() / remainingAlivePartyUnitUIs.Count;
        // grant experience and show gained experience
        foreach(PartyUnitUI remainingAlivePartyUnitUI in remainingAlivePartyUnitUIs)
        {
            SetGainedExperienceAndInformationInPartyUnitUI(remainingAlivePartyUnitUI, experiencePerUnit);
        }
        //foreach (Row horisontalPanel in horisontalPanels)
        //{
        //    foreach (Cell cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel + "/" + cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            PartyUnitUI unitUI = GetUnitUIWhichCanFight(horisontalPanel, cell);
        //            if (unitUI)
        //            {
        //                SetGainedExperienceAndInformationInPartyUnitUI(unitUI, experiencePerUnit);
        //            }
        //        }
        //    }
        //}
    }

    //// Note: animation should be identical to the function with the same name in PartyUnit
    //IEnumerator FadeUnitsCellInfo()
    //{
    //    // Block mouse input
    //    // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
    //    InputBlocker.SetActive(true);
    //    // Init all party units Texts variable, so we don't do it on each for loop iteration
    //    PartyUnitUI[] partyUnitUIs = GetComponentsInChildren<PartyUnitUI>();
    //    // Fade
    //    for (float f = 1f; f >= 0; f -= 0.1f)
    //    {
    //        foreach(PartyUnitUI partyUnitUI in partyUnitUIs)
    //        {
    //            // change infoPanel transparancy
    //            Color c = partyUnitUI.UnitInfoPanelText.color;
    //            c.a = f;
    //            partyUnitUI.UnitInfoPanelText.color = c;
    //        }
    //        yield return new WaitForSeconds(.1f);
    //    }
    //    // Unblock mouse input
    //    InputBlocker.SetActive(false);
    //}

    #endregion For Battle Screen

    //public void SetActiveItemDrag(bool activate)
    //{
    //    Debug.LogWarning("SetActiveItemDrag: " + activate.ToString());
    //    // verify if item has active modifiers or usages
    //    if (InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HasActiveModifiers())
    //    {
    //        Transform unitCell;
    //        Transform unitSlot;
    //        PartyUnit unit;
    //        Color greenHighlight = Color.green;
    //        Color redHighlight = Color.red;
    //        Color normalColor = new Color(0.5f, 0.5f, 0.5f);
    //        Color hightlightColor;
    //        // highlight differently cells with and without units
    //        foreach (Row horisontalPanel in horisontalPanels)
    //        {
    //            foreach (Cell cell in cells)
    //            {
    //                // verify if slot has an unit in it
    //                unitCell = transform.Find(horisontalPanel + "/" + cell);
    //                unitSlot = unitCell.Find("UnitSlot");
    //                if (unitSlot.childCount > 0)
    //                {
    //                    // verify if we need to activate or deactivate highlight
    //                    unit = unitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
    //                    if (activate)
    //                    {
    //                        // activate highlight
    //                        // try to consume item in preview mode without actually doing anything
    //                        if (unit.UseItem(InventoryItemDragHandler.itemBeingDragged.LInventoryItem, true))
    //                        {
    //                            // highlight it with green
    //                            // hightlightColor = greenHighlight;
    //                            hightlightColor = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsApplicableColor;
    //                        }
    //                        else
    //                        {
    //                            // highlight with red
    //                            // hightlightColor = redHighlight;
    //                            hightlightColor = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.InventoryItemConfig.inventoryItemUIConfig.itemIsNotApplicableColor;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        // deactivate highlight
    //                        hightlightColor = normalColor;
    //                    }
    //                    // Change text box color
    //                    unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        // item is not consumable, do not highlight anything
    //    }
    //    // verify if we are in edit party screen and not in battle screen mode
    //    if (transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>() != null)
    //    {
    //        // and disable/enable hire buttons
    //        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>().SetHireUnitPnlButtonActive(!activate);
    //    }
    //}

}
