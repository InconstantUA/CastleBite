using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls all operations with child panels
public class PartyPanel : MonoBehaviour {
    public enum PanelMode { Party, Garnizon };
    [SerializeField]
    PanelMode panelMode;
    string[] horisontalPanels = { "Top", "Middle", "Bottom" };
    string[] singleUnitCells = { "Front", "Back" };
    string[] cells = { "Front", "Back", "Wide" };
    string[] cellsFront = { "Front", "Wide" };
    string[] cellsBack = { "Back", "Wide" };
    public enum ChangeType { Init, HireSingleUnit, HireDoubleUnit, HirePartyLeader, DismissSingleUnit, DismissDoubleUnit, DismissPartyLeader}

    // for battle
    PartyUnit activeBattleUnit;

    public Transform GetUnitSlotTr(string row, string cell)
    {
        // Debug.Log(row + " " + cell);
        return transform.Find(row).Find(cell).Find("UnitSlot");
    }

    public PanelMode GetPanelMode()
    {
        return panelMode;
    }

    #region On Change: hire or dismiss unit, for unit edit mode

    public void SetOnEditClickHandler(bool doActivate)
    {
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // Unit canvas is present
                    if (doActivate)
                    {
                        // add UnitOnBattleMouseHandler Component
                        GameObject unitCanvas = unitSlot.GetChild(0).gameObject;
                        // UnitDragHandler sc = unitCanvas.AddComponent<UnitDragHandler>() as UnitDragHandler;
                        unitCanvas.AddComponent<UnitDragHandler>();
                    }
                    else
                    {
                        // remove UnitOnBattleMouseHandler Component
                        UnitDragHandler unitDragHandlerComponent = unitSlot.GetComponentInChildren<UnitDragHandler>();
                        Destroy(unitDragHandlerComponent);
                    }
                }
            }
        }
    }

    void OnHireSingleUnit(Transform changedCell)
    {
        // UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        // it is more reliable to use GetChild(0), because it is only one child there
        Transform unitCanvas = changedCell.Find("UnitSlot").GetChild(0);
        PartyUnit unit = unitCanvas.GetComponentInChildren<PartyUnit>();
        // fill in highered object UI panel
        unitCanvas.Find("Name").GetComponent<Text>().text = GetUnitDisplayName(unit);
        changedCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = unit.GetHealthCurr().ToString();
        changedCell.Find("HPPanel/HPmax").GetComponent<Text>().text = unit.GetHealthMax().ToString();
        // disable hire unit button
        changedCell.Find("HireUnitPnlBtn").gameObject.SetActive(false);
    }

    void OnHireDoubleUnit(Transform changedCell)
    {
        // Also we need to enable Wide panel, because by defaut it is disabled
        changedCell.parent.Find("Wide").gameObject.SetActive(true);
        // And disable left and right panels
        changedCell.parent.Find("Front").gameObject.SetActive(false);
        changedCell.parent.Find("Back").gameObject.SetActive(false);
        // Update name and health information
        // UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        // it is more reliable to use GetChild(0), because it is only one child there
        Transform parentCell = changedCell.parent.Find("Wide");
        Transform unitCanvas = parentCell.Find("UnitSlot").GetChild(0);
        PartyUnit unit = unitCanvas.GetComponentInChildren<PartyUnit>();
        // fill in highered object UI panel
        unitCanvas.Find("Name").GetComponent<Text>().text = GetUnitDisplayName(unit);
        parentCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = unit.GetHealthCurr().ToString();
        parentCell.Find("HPPanel/HPmax").GetComponent<Text>().text = unit.GetHealthMax().ToString();
    }

    void OnDismissSingleUnit(Transform changedCell)
    {
        // it is possile that unit was dismissed
        // clean health information
        changedCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = "";
        changedCell.Find("HPPanel/HPmax").GetComponent<Text>().text = "";
        // activate hire unit button if panel is in garnizon state
        if (PartyPanel.PanelMode.Garnizon == panelMode)
        {
            Debug.Log("Activate hire unit button");
            changedCell.Find("HireUnitPnlBtn").gameObject.SetActive(true);
        }
    }

    void OnDimissDoubleUnit(Transform changedCell)
    {
        // Disable Wide panel
        changedCell.parent.Find("Wide").gameObject.SetActive(false);
        // And enable left and right panels
        changedCell.parent.Find("Front").gameObject.SetActive(true);
        changedCell.parent.Find("Back").gameObject.SetActive(true);
        // Update name and health information
        // UnitCanvas name on instantiate will change to UnitCanvas(Clone), 
        // it is more reliable to use GetChild(0), because it is only one child there
        Transform parentCell = changedCell.parent.Find("Wide");
        // fill in highered object UI panel
        parentCell.Find("HPPanel/HPcurr").GetComponent<Text>().text = "";
        parentCell.Find("HPPanel/HPmax").GetComponent<Text>().text = "";
        // activate hire unit buttons on left and right cells if panel is in garnizon state
        if (PartyPanel.PanelMode.Garnizon == panelMode)
        {
            Debug.Log("Activate hire unit button");
            changedCell.parent.Find("Front/HireUnitPnlBtn").gameObject.SetActive(true);
            changedCell.parent.Find("Back/HireUnitPnlBtn").gameObject.SetActive(true);

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
        if (panelMode == PanelMode.Garnizon)
        {
            // this is needed to disable or enable hire units button
            // hero party does not have this functionality
            VerifyCityCapacity();
        }
    }

    #endregion

    // Use this for initialization
    void Start()
    {
        OnChange(ChangeType.Init, null);
    }

    string GetUnitDisplayName(PartyUnit unit)
    {
        string unitName;
        if (unit.GetIsLeader())
        {
            // start with Hero's given name information
            unitName = unit.GetGivenName().ToString() + "\r\n" + unit.GetUnitName().ToString();
        }
        else
        {
            unitName = unit.GetUnitName().ToString();
        }
        return unitName;
    }

    void IntitPartyPanel()
    {
        Transform unitPanel;
        Transform unitSlot;
        PartyUnit unit;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                unitPanel = transform.Find(horisontalPanel+"/"+cell);
                unitSlot = unitPanel.Find("UnitSlot");
                // if (unitSlot.childCount > 0)
                if (unitSlot.GetComponentInChildren<UnitDragHandler>())
                {
                    // verify if unit has isLeader atrribute ON
                    unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // fill in highered object UI panel
                    unitSlot.GetChild(0).Find("Name").GetComponent<Text>().text = GetUnitDisplayName(unit);
                    unitPanel.Find("HPPanel/HPcurr").GetComponent<Text>().text = unit.GetHealthCurr().ToString();
                    unitPanel.Find("HPPanel/HPmax").GetComponent<Text>().text = unit.GetHealthMax().ToString();
                    // deactivate hire unit button if panel is in garnizon state and this left or right single panel
                    if ((PanelMode.Garnizon == panelMode) && (("Front" == cell) || ("Back" == cell)))
                    {
                        unitPanel.Find("HireUnitPnlBtn").gameObject.SetActive(false);
                    }
                }
                else
                {
                    // it is possile that unit was dismissed
                    // clean health information
                    unitPanel.Find("HPPanel/HPcurr").GetComponent<Text>().text = "";
                    unitPanel.Find("HPPanel/HPmax").GetComponent<Text>().text = "";
                    // activate hire unit button if panel is in garnizon state and this left or right single panel
                    if ((PanelMode.Garnizon == panelMode) && (("Front" == cell) || ("Back" == cell)))
                    {
                        unitPanel.Find("HireUnitPnlBtn").gameObject.SetActive(true);
                    }
                    // it is possible that double unit was dismissed
                    if ("Wide" == cell)
                    {
                        // we need to disable Wide panel, because it is still enabled and placed on top of single panels
                        unitPanel.parent.Find("Wide").gameObject.SetActive(false);
                        // and enable left and right panels
                        unitPanel.parent.Find("Front").gameObject.SetActive(true);
                        unitPanel.parent.Find("Back").gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    #region Verify capacity

    public PartyUnit GetPartyLeader()
    {
        PartyUnit leader = null;
        // find the unit with 
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if unit has isLeader atrribute ON
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (unit.GetIsLeader())
                    {
                        leader = unit;
                    }
                }
            }
        }
        return leader;
    }

    int GetCapacity()
    {
        int capacity = 0;
        // return capacity based on the parent mode
        // in garnizon mode we get city capacity
        // in party mode we get hero leadership
        if (panelMode == PanelMode.Garnizon)
        {
            City city = transform.parent.parent.GetComponent<City>();
            capacity = city.GetUnitsCapacity();
        } else
        {
            capacity = GetPartyLeader().GetLeadership() + 1; // +1 because we do not count leader
        }
        return capacity;
    }

    public int GetNumberOfPresentUnits()
    {
        int unitsNumber = 0;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                if (transform.Find(horisontalPanel).Find(cell).Find("UnitSlot").childCount > 0)
                {
                    // if this is double unit, then count is as +2
                    if (cell == "Wide")
                    {
                        // double unit
                        unitsNumber += 2;
                    } else
                    {
                        // single unit
                        unitsNumber += 1;
                    }
                }
            }
        }
        return unitsNumber;
    }

    public void SetHireUnitPnlButtonActive(bool activate)
    {
        GameObject hireUnitPnlBtn;
        // this is only needed in garnizon mode, only in this mode hire buttons are present
        if (panelMode == PanelMode.Garnizon)
        {
            foreach (string horisontalPanel in horisontalPanels)
            {
                foreach (string cell in singleUnitCells)
                {
                    if (activate)
                    {
                        // activate - set in front of drop slot
                        // do not activate it if drop slot has an unit in it
                        if (!transform.Find(horisontalPanel).Find(cell).Find("UnitSlot").GetComponentInChildren<UnitDragHandler>())
                        {
                            // partyPanel.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").SetAsLastSibling();
                            hireUnitPnlBtn = transform.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").gameObject;
                            hireUnitPnlBtn.SetActive(true);
                            // and bring it to the front
                            hireUnitPnlBtn.transform.SetAsLastSibling();
                        }
                        else
                        {
                            Debug.Log("slot " + horisontalPanel + " " + cell + " has a unit");
                            hireUnitPnlBtn = transform.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").gameObject;
                            hireUnitPnlBtn.SetActive(false);
                        }
                    }
                    else
                    {
                        //// deactivate - set behind drop slot
                        //if (partyPanel.Find(horisontalPanel).Find(cell).Find("UnitSlot").childCount == 0)
                        //{
                        // partyPanel.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").SetAsFirstSibling();
                        hireUnitPnlBtn = transform.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").gameObject;
                        hireUnitPnlBtn.SetActive(false);
                        //}
                        // and bring it to the front
                        // hireUnitPnlBtn.transform.SetAsLastSibling();
                    }
                }
            }
        }
    }

    void VerifyCityCapacity()
    {
        if (GetCapacity() <= GetNumberOfPresentUnits())
        {
            // disable hire buttons
            SetHireUnitPnlButtonActive(false);
        } else
        {
            // enable hire buttons
            SetHireUnitPnlButtonActive(true);
        }
    }

    #endregion Verify capacity

    public bool VerifyCityCapacityOverflowOnDoubleUnitHire()
    {
        bool result = true;
        if (GetCapacity() < (GetNumberOfPresentUnits()+2))
        {
            // overflow on double unit hire
            result = false;
            // show error message
            // this depends if city has reached max level
            City city = transform.parent.parent.GetComponent<City>();
            string errMsg;
            if (city.GetCityLevel() == 1)
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Increase city level.";
            }
            else if (city.HasCityReachedMaximumLevel())
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Dismiss or move other units to Hero's party.";
            }
            else
            {
                errMsg = "Not enough city capacity, 2 free slots are required. Dismiss or move other units to Hero's party or increase city level.";
            }
            transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
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
        if (callerCell.name == "Front")
        {
            // check Back cell
            oppositeCell = callerCell.parent.Find("Back");
        } else
        {
            // check Front cell
            oppositeCell = callerCell.parent.Find("Front");
        }
        // check if cell is occupied
        // structure [Front/Back cell]-UnitSlot-unit
        if (oppositeCell.Find("UnitSlot").childCount > 0)
        {
            result = false;
            string errMsg = "Not enough free space to hire this large unit, 2 free nearby horisontal slots are required. " + oppositeCell.name + " unit slot is occupied. Move unit from " + oppositeCell.name + " slot to other free slot or to Hero's party or dismiss it to free up a slot or click on hire button where 2 free nearby horisontal slots are available.";
            transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
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
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if unit is damaged
                    unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is alive (health > 0) and that he is damaged (health < maxhealth)
                        if ( (unit.GetHealthCurr()>0) && (unit.GetHealthCurr() < unit.GetHealthMax()) )
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
                    unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
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
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if we need to activate or deactivate highlight
                    // get unit for future reference
                    unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is dismissable
                        if (unit.GetIsDismissable())
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
                    unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
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
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                unitCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = unitCell.Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if we need to activate or deactivate highlight
                    unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (activate)
                    {
                        // activate highlight
                        // make sure that unit is alive (health > 0) and that he is damaged (health < maxhealth)
                        if (0 == unit.GetHealthCurr())
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
                    unitCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
    }

    PartyPanel GetOtherPartyPanel(PartyPanel currentPartyPanel)
    {
        // if other party is not present, then return null
        // calling script should do null verification
        PartyPanel otherPartyPanel = null;
        // get city transform, this actually can be inter-hero exchange root transform
        Transform cityTr = currentPartyPanel.transform.parent.parent;
        if (PartyPanel.PanelMode.Garnizon == currentPartyPanel.GetComponent<PartyPanel>().GetPanelMode())
        {
            // verify if there is a HeroParty in City
            if (cityTr.GetComponentInChildren<HeroParty>())
            {
                otherPartyPanel = cityTr.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>();
            }
        } else
        {
            // verify if there is a city garnizon
            if (cityTr.Find("CityGarnizon"))
            {
                otherPartyPanel = cityTr.Find("CityGarnizon").GetComponentInChildren<PartyPanel>();
            } else
            {
                // verify if we are exchanging with other hero party
                foreach (HeroParty heroParty in cityTr.GetComponentsInChildren<HeroParty>())
                {
                    // find party panel of other party in city, if it is present there
                    // this will be the case if there are more then 1 hero party in the city
                    if (heroParty.GetComponentInChildren<PartyPanel>() != currentPartyPanel)
                    {
                        otherPartyPanel = heroParty.GetComponentInChildren<PartyPanel>();
                    }
                }
            }
        }
        return otherPartyPanel;
    }

    void SetCellIsDroppableStatus(bool isDroppable, Transform cellTr, string errorMessage = "")
    {
        // isDroppable means if we can drop units to this cell
        if (isDroppable)
        {
            // Change text box color
            cellTr.Find("Br").GetComponent<Text>().color = Color.green;
        }
        else
        {
            // Change text box color
            cellTr.Find("Br").GetComponent<Text>().color = Color.red;
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
        if (PartyPanel.PanelMode.Garnizon == partyPanel.GetPanelMode())
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
        // Todo:
        // I'm actually doing the same job in each party panel
        // fix it somehow
        // this needs to be done only once
        //
        // unit being dragged is static, so we do not need to assign it here
        // or send as and argument
        // structure: 5[City]-4[HeroParty/CityGarnizon]Party-3PartyPanel-2[Top/Middle/Bottom]HorizontalPanelGroup-1[Front/Back/Wide]Cell-UnitSlot-(this)UnitCanvas
        PartyUnit unitBeingDragged = UnitDragHandler.unitBeingDragged.GetComponentInChildren<PartyUnit>();
        Transform unitCell = UnitDragHandler.unitBeingDragged.transform.parent.parent;
        Transform horizontalPanelGroup = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent;
        Transform sourcePartyPanel = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent.parent;
        Transform party = UnitDragHandler.unitBeingDragged.transform.parent.parent.parent.parent.parent;
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
            if (sourcePartyPanel == gameObject.GetComponent<PartyPanel>().transform)
            {
                foreach (string horisontalPanel in horisontalPanels)
                {
                    foreach (string cell in cells)
                    {
                        // verify if slot is active
                        // here we highlight only party panel of the draggable unit
                        // not this party panel, where we are now
                        cellTr = sourcePartyPanel.Find(horisontalPanel + "/" + cell);
                        if (cellTr.gameObject.activeSelf)
                        {
                            SetCellIsDroppableStatus(true, cellTr);
                        }
                    }
                }
            }
            else
            {
                // there can be only 2 panels
                // if i'm not the source panel, then I'm other panel or destination panel
                // InterParty scope
                //  The 1st easiest check is if draggable unit is inter-party movable or not
                //  The 2nd problem may happen with moving unit to the other party
                //  Verify if other party panel is present, if no panel - no problems
                PartyPanel otherPartyPanel = GetOtherPartyPanel(sourcePartyPanel.GetComponent<PartyPanel>());
                if (otherPartyPanel)
                {
                    if (unitBeingDragged.GetIsInterpartyMovable())
                    {
                        // unit is movable
                        // find in other panel where it can or cannot be placed and highlight accordingly
                        // here we highlight other party if it is present,
                        // there are multiple blocking conditions present
                        // skip highlighting for the non-interparty-movable objects
                        // on double unit hire, skip also highlighting of nearby cell near non-interparty-movable
                        // act based on the unit size
                        if (PartyUnit.UnitSize.Single == unitBeingDragged.GetUnitSize())
                        {
                            // Single unit size
                            // act based on the destination cell type
                            // Single[Occupied[movable/non-interparty-movable]/Free]/Double
                            // loop through all cells in other party panel and highlight based on condition
                            bool isCellActive;
                            PartyUnit cellUnit;
                            UnitDragHandler unitCanvas;
                            bool isUnitInterPartyDraggable;
                            foreach (string horisontalPanel in horisontalPanels)
                            {
                                foreach (string cell in cells)
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
                                            cellUnit = unitCanvas.GetComponentInChildren<PartyUnit>();
                                            isUnitInterPartyDraggable = cellUnit.GetIsInterpartyMovable();
                                            if (isUnitInterPartyDraggable)
                                            {
                                                // unit can be dragged to other party
                                                // do additional checks
                                                // if occupied cell is single-unit, 
                                                // then we can easily drag here unit and exchange units between the cells
                                                // but if the occupied cell is double unit cells, then we need to do additional verification
                                                if ("Wide" == cell)
                                                {
                                                    // do additional verification
                                                    // possible states and tranistions
                                                    // src state    dst state   result
                                                    // 01/10        2           check for source overflow
                                                    // 11           2           ok
                                                    // 1x/x1        2           not ok
                                                    // get nearby cell
                                                    Transform nearbySrcCellTr;
                                                    if ("Front" == unitCell.name)
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find("Back");
                                                    }
                                                    else
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find("Front");
                                                    }
                                                    // verify if nearby source cell is occupied
                                                    UnitDragHandler nearbySrcCellUnitCanvas = nearbySrcCellTr.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                                    if (nearbySrcCellUnitCanvas)
                                                    {
                                                        // nearby cell is occupied
                                                        // check if it is occupied by non-inter-party-movable unit, 
                                                        // because we cannot swap those units
                                                        PartyUnit nearbySrcUnit = nearbySrcCellUnitCanvas.GetComponentInChildren<PartyUnit>();
                                                        if (nearbySrcUnit.GetIsInterpartyMovable())
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
                                                        if (sourcePartyPanel.GetComponent<PartyPanel>().GetCapacity() < (sourcePartyPanel.GetComponent<PartyPanel>().GetNumberOfPresentUnits() + 1))
                                                        {
                                                            // not enough capacity
                                                            isDroppable = false;
                                                            errorMessage = GetNotEnoughCapacityErrorMessage(sourcePartyPanel.GetComponent<PartyPanel>(), true);
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
                                            if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetNumberOfPresentUnits() + 1))
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
                            foreach (string horisontalPanel in horisontalPanels)
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
                                        cellUnit = isFrontCellOccupied.GetComponentInChildren<PartyUnit>();
                                        isUnitInterPartyDraggable = cellUnit.GetIsInterpartyMovable();
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
                                        cellUnit = isBackCellOccupied.GetComponentInChildren<PartyUnit>();
                                        isUnitInterPartyDraggable = cellUnit.GetIsInterpartyMovable();
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
                                        if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetNumberOfPresentUnits() + 2))
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
                                        if (otherPartyPanel.GetCapacity() < (otherPartyPanel.GetNumberOfPresentUnits() + 1))
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
                        foreach (string horisontalPanel in horisontalPanels)
                        {
                            // get state of active cells
                            foreach (string cell in cells)
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
            foreach (string horisontalPanel in horisontalPanels)
            {
                foreach (string cell in cells)
                {
                    // Change text box color
                    transform.Find(horisontalPanel+"/"+cell+"/Br").GetComponent<Text>().color = normalColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
    }

    #region For Battle Screen

    public bool CanFight()
    {
        // loop through all units in each party and verify if there is at least one unit, which can fight
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if unit is alive and it has not escaped yet
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (unit.GetIsAlive() && !unit.GetHasEscaped())
                    {
                        return true;
                    }
                }
            }
        }
        // if not unit can fight, return false;
        return false;
    }

    public PartyUnit GetActiveUnitWithHighestInitiative()
    {
        PartyUnit unitWithHighestInitiative = null;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // verify if 
                    //  - unit has moved or not
                    //  - unit is alive
                    //  - unit has not escaped from the battle
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (!unit.GetHasMoved() && unit.GetIsAlive() && !unit.GetHasEscaped())
                    {
                        // compare initiative with other unit, if it was found
                        if (unitWithHighestInitiative)
                        {
                            if (unit.GetInitiative() > unitWithHighestInitiative.GetInitiative())
                            {
                                // found unit with highest initiative, update unitWithHighestInitiative variable
                                unitWithHighestInitiative = unit;
                            }
                        }
                        else
                        {
                            // no other unit found yet, assume that this unit has the highest initiative
                            unitWithHighestInitiative = unit;
                        }
                    }
                }
            }
        }
        return unitWithHighestInitiative;
    }

    public void ResetHasMovedFlag()
    {
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // set unit has moved to false, so it can move again
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    unit.SetHasMoved(false);
                }
            }
        }
    }

    public void SetOnBattleClickHandler(bool doActivate)
    {
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // Unit canvas is present
                    if (doActivate)
                    {
                        // add UnitOnBattleMouseHandler Component
                        GameObject unitCanvas = unitSlot.GetChild(0).gameObject;
                        // UnitOnBattleMouseHandler sc = unitCanvas.AddComponent<UnitOnBattleMouseHandler>() as UnitOnBattleMouseHandler;
                        unitCanvas.AddComponent<UnitOnBattleMouseHandler>();
                    }
                    else
                    {
                        // remove UnitOnBattleMouseHandler Component
                        UnitOnBattleMouseHandler unitOnBattleMouseHandlerComponent = unitSlot.GetComponentInChildren<UnitOnBattleMouseHandler>();
                        Destroy(unitOnBattleMouseHandlerComponent);
                    }
                }
            }
        }
    }

    bool GetIsUnitFriendly(PartyUnit unitToActivate)
    {
        // method 1
        // structure: 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanel-3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
        GameObject unitToActivatePartyPanel = unitToActivate.transform.parent.parent.parent.parent.parent.gameObject;
        if (gameObject == unitToActivatePartyPanel)
        {
            return true;
        }
        // method 2
        //foreach (string horisontalPanel in horisontalPanels)
        //{
        //    foreach (string cell in cells)
        //    {
        //        // verify if slot has an unit in it
        //        Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
        //        if (unitSlot.childCount > 0)
        //        {
        //            // verify if unit has isLeader atrribute ON
        //            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
        //            if (unit.GetInstanceID() == unitToActivate.GetInstanceID())
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        return false;
    }

    void SetIfCellCanBeTargetedStatus(bool isTargetable, Transform cellTr, string errorMessage, Color positiveColor, Color negativeColor)
    {
        // isDroppable means if we can drop units to this cell
        if (isTargetable)
        {
            // Change text box color
            cellTr.Find("Br").GetComponent<Text>().color = positiveColor;
        }
        else
        {
            // Change text box color
            cellTr.Find("Br").GetComponent<Text>().color = negativeColor;
        }
        // set UnitSlot in cell as droppable or not
        cellTr.Find("UnitSlot").GetComponent<UnitSlot>().SetOnClickAction(isTargetable, errorMessage);
    }

    void PrepareBattleFieldForHealPower(bool activeUnitIsFromThisParty)
    {
        Debug.Log("PrepareBattleFieldForHealPower");
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "";
        Color positiveColor = Color.green;
        // Color negativeColor = new Color32(221, 24, 24, 255); // dark red
        Color negativeColor = Color.grey;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // Unit canvas (and unit) is present
                    if (activeUnitIsFromThisParty)
                    {
                        // highlight units which can be healed
                        // verify if unit is damaged
                        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                        if ( ( unit.GetHealthCurr() < unit.GetHealthMax() ) && unit.GetIsAlive())
                        {
                            // unit can be healed
                            isAllowedToApplyPwrToThisUnit = true;
                            errorMessage = "";
                        }
                        else if ((unit.GetHealthCurr() == unit.GetHealthMax()) && unit.GetIsAlive())
                        {
                            // unit cannot be healed
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot heal this unit. Unit health is already full.";
                        } else if (!unit.GetIsAlive())
                        {
                            // unit is dead
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot heal dead units. This unit should be first resurected.";
                        }
                    }
                    else
                    {
                        // this is actions for enemy party
                        // set is as not unit to which we can apply powers
                        isAllowedToApplyPwrToThisUnit = false;
                        errorMessage = "Cannot heal enemy units.";
                    }
                    SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
                }
            }
        }
    }

    void PrepareBattleFieldForResurectPower(bool activeUnitIsFromThisParty)
    {
        Debug.Log("PrepareBattleFieldForResurectPower");
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "";
        Color positiveColor = Color.green;
        Color negativeColor = Color.grey;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // Unit canvas (and unit) is present
                    if (activeUnitIsFromThisParty)
                    {
                        // highlight units which can be healed
                        // verify if unit is dead
                        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                        if (unit.GetIsAlive())
                        {
                            // unit is alive and cannot be resurected
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot resurect alive units.";
                        }
                        else
                        {
                            // unit is dead and can be resurected
                            isAllowedToApplyPwrToThisUnit = true;
                            errorMessage = "";
                        }
                    }
                    else
                    {
                        // this is actions for enemy party
                        // set is as not unit to which we can apply powers
                        isAllowedToApplyPwrToThisUnit = false;
                        errorMessage = "Cannot resurect enemy units.";
                    }
                    SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
                }
            }
        }
    }

    bool DoesCellHasUnitsWhichCanFight(string horisontalPanel, string cell)
    {
        // verify if slot has an unit in it
        Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
        if (unitSlot.childCount > 0)
        {
            // front raw has at least one unit in it
            // verify if unit can act: alive and did not escape (flee from) the battle
            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
            if (unit.GetIsAlive() && !unit.GetHasEscaped())
            {
                return true;
            }
        }
        return false;
    }

    bool FrontRowHasUnitsWhichCanFight()
    {
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cellsFront)
            {
                if (DoesCellHasUnitsWhichCanFight(horisontalPanel, cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool BackRowHasUnitsWhichCanFight()
    {
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cellsBack)
            {
                if (DoesCellHasUnitsWhichCanFight(horisontalPanel, cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool HorizontalPanelHasUnitsWhichCanFight(string horisontalPanel)
    {
        foreach (string cell in cells)
        {
            if (DoesCellHasUnitsWhichCanFight(horisontalPanel, cell))
            {
                return true;
            }
        }
        return false;
    }

    bool IsActiveMeleUnitBlockedByItsPartyMembers()
    {
        string activeMeleUnitFrontBackWideRowPosition = activeBattleUnit.transform.parent.parent.parent.name;
        PartyPanel activeUnitPartyPanel = activeBattleUnit.transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
        bool isBlocked = true;
        bool frontRowHasUnitsWhichCanFight = activeUnitPartyPanel.FrontRowHasUnitsWhichCanFight();
        if ("Back" == activeMeleUnitFrontBackWideRowPosition)
        {

            if (frontRowHasUnitsWhichCanFight)
            {
                // mele unit is blocked by its own party units
                isBlocked = true;
            }
            else
            {
                // mele unit is not blocked and can fight
                isBlocked = false;
            }
        } else
        {
            // active unit is in a front row and cannot be blocked
            isBlocked = false;
        }
        // Debug.LogWarning(activeMeleUnitFrontBackWideRowPosition);
        // Debug.LogWarning(isBlocked.ToString());
        // Debug.LogWarning(frontRowHasUnitsWhichCanFight.ToString());
        return isBlocked;
    }

    void ResetAllCells()
    {
        Color positiveColor = Color.yellow;
        Color negativeColor = Color.grey;
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "This cannot be targeted";
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
            }
        }
    }

    void PrepareBattleFieldForMelePower(bool activeUnitIsFromThisParty)
    {
        Debug.Log("PrepareBattleFieldForMelePower");
        Color positiveColor = Color.yellow;
        Color negativeColor = Color.grey;
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "";
        bool activeMeleUnitIsBlocked = IsActiveMeleUnitBlockedByItsPartyMembers();
        string activeMeleUnitTopMiddleBottomPosition = activeBattleUnit.transform.parent.parent.parent.parent.name;
        bool enemyUnitIsPotentialTarget = false;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if destination slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // Unit canvas (and unit) is present
                    if (activeUnitIsFromThisParty)
                    {
                        // cannot attack friendly units
                        isAllowedToApplyPwrToThisUnit = false;
                        errorMessage = "Cannot attack friendly units.";
                    }
                    else
                    {
                        // this is actions for enemy party
                        // first filter out dead units
                        // get unit for later checks
                        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                        if (!unit.GetIsAlive())
                        {
                            // cannot attack dead units
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot attack dead units.";
                        }
                        else
                        {
                            // if active mele unit is blocked, then it cannot attack anything
                            if (activeMeleUnitIsBlocked)
                            {
                                // blocked
                                // set cannot attack error messages depending on friendly or enemy unit types
                                if (unitSlot.childCount > 0)
                                {
                                    // Unit canvas (and unit) is present
                                    if (activeUnitIsFromThisParty)
                                    {
                                        // cannot attack friendly units
                                        isAllowedToApplyPwrToThisUnit = false;
                                        errorMessage = "Cannot attack friendly units.";
                                    }
                                    else
                                    {
                                        // this is actions for enemy party
                                        isAllowedToApplyPwrToThisUnit = false;
                                        errorMessage = activeBattleUnit.GetUnitName() + " is mele unit and can attack only adjacent units. At this moment it is blocked by front row party members and cannot attack this enemy unit.";
                                    }
                                }
                                else
                                {
                                    // this is an empty cell
                                    isAllowedToApplyPwrToThisUnit = false;
                                    errorMessage = "No target.";
                                }
                            }
                            else
                            {
                                // not blocked
                                // act based on the mele unit position (cell)
                                // 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanelGroup-3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(this)Unit
                                // if mele unit is in back row, then verify if it is not blocked by front row units
                                // verify if this enemy unit it from front row or from back row
                                if ("Back" == cell)
                                {
                                    // unit is from back row and it may be protected by front row units
                                    // If front row is empty, then unit in back row potentially be reached
                                    // verify if enemy front row is not empty
                                    if (FrontRowHasUnitsWhichCanFight())
                                    {
                                        // front row has units which can fight
                                        // this means that this unit is protected from mele atack
                                        enemyUnitIsPotentialTarget = false;
                                    }
                                    else
                                    {
                                        // front does not have units which can fight
                                        // it means that active mele unit can potentially reach enemy unit
                                        enemyUnitIsPotentialTarget = true;
                                    }
                                }
                                else
                                {
                                    // unit is from front row and potentially can be targeted
                                    enemyUnitIsPotentialTarget = true;
                                }
                                if (enemyUnitIsPotentialTarget)
                                {
                                    // verify if mele unit can reach enemy unit depending on active mele unit and enemy positions
                                    switch (activeMeleUnitTopMiddleBottomPosition)
                                    {
                                        case "Top":
                                            // can reach closest 2 (top and middle) units
                                            // and also farest unit (if it is not protected by top and middle) units
                                            if (("Top" == horisontalPanel) || ("Middle" == horisontalPanel))
                                            {
                                                isAllowedToApplyPwrToThisUnit = true;
                                                errorMessage = "";
                                            }
                                            else
                                            {
                                                // Bottom horisontalPanel
                                                // verify if top or middle has units, which can fight
                                                // which means that they can protect bottom unit from mele attacks
                                                if (HorizontalPanelHasUnitsWhichCanFight("Top")
                                                    || HorizontalPanelHasUnitsWhichCanFight("Middle"))
                                                {
                                                    // unit is protected
                                                    isAllowedToApplyPwrToThisUnit = false;
                                                    errorMessage = "This unit cannot be targeted by mele attack. It is protected by unit above.";
                                                }
                                                else
                                                {
                                                    // unit is not protected and can be targeted
                                                    isAllowedToApplyPwrToThisUnit = true;
                                                    errorMessage = "";
                                                }
                                            }
                                            break;
                                        case "Middle":
                                            // Middle mele unit can reach any unit in front of it
                                            isAllowedToApplyPwrToThisUnit = true;
                                            errorMessage = "";
                                            break;
                                        case "Bottom":
                                            // can reach closest 2 (bottom and middle) units
                                            // and also farest unit (if it is not protected by bottom and middle) units
                                            if (("Bottom" == horisontalPanel) || ("Middle" == horisontalPanel))
                                            {
                                                isAllowedToApplyPwrToThisUnit = true;
                                                errorMessage = "";
                                            }
                                            else
                                            {
                                                // Top horisontalPanel
                                                // verify if bottom or middle has units, which can fight
                                                // which means that they can protect top unit from mele attacks
                                                if (HorizontalPanelHasUnitsWhichCanFight("Bottom")
                                                    || HorizontalPanelHasUnitsWhichCanFight("Middle"))
                                                {
                                                    // unit is protected
                                                    isAllowedToApplyPwrToThisUnit = false;
                                                    errorMessage = "This unit cannot be targeted by mele attack. It is protected by unit below.";
                                                }
                                                else
                                                {
                                                    // unit is not protected and can be targeted
                                                    isAllowedToApplyPwrToThisUnit = true;
                                                    errorMessage = "";
                                                }
                                            }
                                            break;
                                        default:
                                            Debug.LogError("Unknown unit position [" + activeMeleUnitTopMiddleBottomPosition + "]");
                                            break;
                                    }
                                }
                                else
                                {
                                    // unit cannot be targeted
                                    isAllowedToApplyPwrToThisUnit = false;
                                    errorMessage = "This enemy unit cannot be targeted, because it is protected by units in a front row.";
                                }
                            }
                        }
                    }
                    SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
                }
            }
        }
    }

    void PrepareBattleFieldForRangedPower(bool activeUnitIsFromThisParty)
    {
        Debug.Log("PrepareBattleFieldForRangedPower");
        Color positiveColor = Color.yellow;
        // Color negativeColor = new Color32(221, 24, 24, 255); // dark red
        Color negativeColor = Color.grey;
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "";
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // get unit for later checks
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // Unit canvas (and unit) is present
                    if (activeUnitIsFromThisParty)
                    {
                        // cannot attack friendly units
                        isAllowedToApplyPwrToThisUnit = false;
                        errorMessage = "Cannot attack friendly units.";
                    } else
                    {
                        // these are actions for enemy party
                        if (!unit.GetIsAlive())
                        {
                            // cannot attack dead units
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot attack dead units.";
                        }
                        else
                        {
                            // alive enemy unit
                            // ranged units can reach any unit
                            // so all enemy units can be targeted
                            isAllowedToApplyPwrToThisUnit = true;
                            errorMessage = "";
                        }
                    }
                    SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
                }
            }
        }
    }

    void PrepareBattleFieldForMagicPower(bool activeUnitIsFromThisParty)
    {
        Debug.Log("PrepareBattleFieldForMagicPower");
        Color positiveColor = Color.yellow;
        Color negativeColor = Color.grey;
        bool isAllowedToApplyPwrToThisUnit = false;
        string errorMessage = "";
        // highlight all cells based on friendly/enemy principle
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // Unit canvas (and unit) is present
                if (activeUnitIsFromThisParty)
                {
                    // cannot attack friendly units
                    isAllowedToApplyPwrToThisUnit = false;
                    errorMessage = "Cannot attack friendly units.";
                }
                else
                {
                    // verify if slot has an unit in it
                    Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                    if (unitSlot.childCount > 0)
                    {
                        // get unit for later checks
                        PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                        // these are actions for enemy party
                        if (!unit.GetIsAlive())
                        {
                            // cannot attack dead units
                            isAllowedToApplyPwrToThisUnit = false;
                            errorMessage = "Cannot attack dead units.";
                        }
                        else
                        {
                            // alive enemy unit
                            // ranged units can reach any unit
                            // so all enemy units can be targeted
                            isAllowedToApplyPwrToThisUnit = true;
                            errorMessage = "";
                        }
                    }
                }
                SetIfCellCanBeTargetedStatus(isAllowedToApplyPwrToThisUnit, transform.Find(horisontalPanel + "/" + cell), errorMessage, positiveColor, negativeColor);
            }
        }
    }

    void HighlightActiveUnitInBattle(PartyUnit unitToActivate)
    {
        Debug.Log(" HighlightActiveUnitInBattle");
        // highlight unit canvas with blue color
        Color highlightColor = Color.blue;
        // structure: 3[Front/Back/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
        Transform cellTr = unitToActivate.transform.parent.parent.parent;
        Text canvasText = cellTr.Find("Br").GetComponent<Text>();
        canvasText.color = highlightColor;
    }

    public void SetActiveUnitInBattle(PartyUnit unitToActivate)
    {
        Debug.Log("SetActiveUnitInBattle " + unitToActivate.GetUnitName());
        // first reset all cells do default values
        ResetAllCells();
        // save it locally for later use
        activeBattleUnit = unitToActivate;
        // new unit became active in battle
        // highlight differently cells which this unit can or cannot interract and in which way
        // act based on activated unit relationships with this panel
        // verify if this is enemy unit or unit from this party
        bool activeUnitIsFromThisParty = GetIsUnitFriendly(unitToActivate);
        // defined below how actions applied to the friendly and enemy units
        // based on the active unit powers
        switch (unitToActivate.GetAbility())
        {
            // Helping or buf powers
            case PartyUnit.UnitAbility.HealingWord:
            case PartyUnit.UnitAbility.HealingSong:
                PrepareBattleFieldForHealPower(activeUnitIsFromThisParty);
                break;
            case PartyUnit.UnitAbility.Resurect:
                PrepareBattleFieldForResurectPower(activeUnitIsFromThisParty);
                break;
            // Mele attack powers
            case PartyUnit.UnitAbility.BlowWithGreatSword:
            case PartyUnit.UnitAbility.BlowWithMaul:
            case PartyUnit.UnitAbility.CutWithAxe:
            case PartyUnit.UnitAbility.CutWithDagger:
            case PartyUnit.UnitAbility.SlashWithSword:
            case PartyUnit.UnitAbility.StabWithDagger:
            case PartyUnit.UnitAbility.StompWithFoot:
                PrepareBattleFieldForMelePower(activeUnitIsFromThisParty);
                break;
            // Ranged attack powers
            case PartyUnit.UnitAbility.ShootWithBow:
            case PartyUnit.UnitAbility.ShootWithCompoudBow:
            case PartyUnit.UnitAbility.ThrowSpear:
            case PartyUnit.UnitAbility.ThrowRock:
                PrepareBattleFieldForRangedPower(activeUnitIsFromThisParty);
                break;
            // Magic (including pure) attack powers
            case PartyUnit.UnitAbility.CastChainLightning:
            case PartyUnit.UnitAbility.CastLightningStorm:
            case PartyUnit.UnitAbility.HolyWord:
                PrepareBattleFieldForMagicPower(activeUnitIsFromThisParty);
                break;
            default:
                Debug.LogError("Unknown unit power");
                break;
        }
        // Highlight active unit itself, this should be done after previous highlights
        // to override their logic
        if (activeUnitIsFromThisParty)
        {
            // This unit belongs to this party highlight it here
            HighlightActiveUnitInBattle(unitToActivate);
        }
    }

    void ApplyHealPowerToSingleUnit(PartyUnit dstUnit)
    {
        Debug.Log("ApplyHealPowerToSingleUnit");
        // heal destination unit
        int healthAfterHeal = dstUnit.GetHealthCurr() + activeBattleUnit.GetPower();
        // make sure that we do not heal to more than maximum health
        if (healthAfterHeal > dstUnit.GetHealthMax())
        {
            healthAfterHeal = dstUnit.GetHealthMax();
        }
        dstUnit.SetHealthCurr(healthAfterHeal);
        // update current health in UI
        // structure: 3[Front/Back/Wide]cell-2UnitSlot/HPPanel-1UnitCanvas-dstUnit
        // structure: [Front/Back/Wide]cell-UnitSlot/HPPanel-HPcurr
        Transform cell = dstUnit.transform.parent.parent.parent;
        Text currentHealth = cell.Find("HPPanel/HPcurr").GetComponent<Text>();
        currentHealth.text = healthAfterHeal.ToString();
    }

    void ApplyHealPowerToMultipleUnits()
    {
        Debug.Log("ApplyHealPowerToMultipleUnits");
    }

    void ApplyResurectPower(PartyUnit dstUnit)
    {
        Debug.Log("ApplyResurectPower");
    }

    void ApplyDestructivePowerToSingleUnit(PartyUnit dstUnit)
    {
        Debug.Log("ApplyDestructivePowerToSingleUnit");
        // damage destination unit
        int healthAfterDamage = dstUnit.GetHealthCurr() - activeBattleUnit.GetPower();
        // make sure that we do not heal to more than maximum health
        if (healthAfterDamage <= 0)
        {
            healthAfterDamage = 0;
        }
        dstUnit.SetHealthCurr(healthAfterDamage);
        // update current health in UI
        // structure: 3[Front/Back/Wide]cell-2UnitSlot/HPPanel-1UnitCanvas-dstUnit
        // structure: [Front/Back/Wide]cell-UnitSlot/HPPanel-HPcurr
        Transform cell = dstUnit.transform.parent.parent.parent;
        Text currentHealth = cell.Find("HPPanel/HPcurr").GetComponent<Text>();
        currentHealth.text = healthAfterDamage.ToString();
        // verify if unit is dead
        if (0 == healthAfterDamage)
        {
            // set unit is dead attribute
            dstUnit.SetIsAlive(false);
            // set color ui more darker
            Color32 deadColor = new Color32(64, 64, 64, 255);
            currentHealth.color = deadColor;
            Text maxHealth = cell.Find("HPPanel/HPmax").GetComponent<Text>();
            maxHealth.color = deadColor;
            // set cell canvas to be more darker
            Text cellCanvas = cell.Find("Br").GetComponent<Text>();
            cellCanvas.color = deadColor;
        }
    }

    void ApplyDestructivePowerToMultipleUnits()
    {
        Debug.Log("ApplyDestructivePowerToMultipleUnits");
        // get all alive units in enemy party and apply damage to them
        // find enemy party based on activeBattleUnit
        // structure: 7BattleScreen-6Party-5PartyPanel-4[Top/Middle/Bottom]-3[Front/Back/Wide]cell-2UnitSlot-1UnitCanvas-activeBattleUnit
        Transform activeBattleUnitPartyTr = activeBattleUnit.transform.parent.parent.parent.parent.parent.parent;
        Transform battleScreenTr = activeBattleUnitPartyTr.parent;
        HeroParty[] allHeroesParties = battleScreenTr.GetComponentsInChildren<HeroParty>();
        // find party which does not match activeBattleUnit Party
        foreach (HeroParty heroParty in allHeroesParties)
        {
            if (heroParty.transform.GetInstanceID() != activeBattleUnitPartyTr.GetInstanceID())
            {
                // we found other (enemy hero party)
                // apply damage to all party members
                foreach (string horisontalPanel in horisontalPanels)
                {
                    foreach (string cell in cells)
                    {
                        // verify if slot has an unit in it
                        Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                        if (unitSlot.childCount > 0)
                        {
                            // get unit for later checks
                            PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                            // verify if unit is alive and has not escaped the battle
                            if (unit.GetIsAlive() && !unit.GetHasEscaped())
                            {
                                // apply damage to the unit
                                ApplyDestructivePowerToSingleUnit(unit);
                            }
                        }
                    }
                }

            }
        }
    }

    public void ApplyPowersToUnit(PartyUnit dstUnit)
    {
        // in case of applying magic powers it is possible to click on the unit slot, where there is no unit
        // but still the power should be applied
        if (dstUnit)
        {
            Debug.Log(activeBattleUnit.GetUnitName() + " acting upon " + dstUnit.GetUnitName() + " or whole party");
            switch (activeBattleUnit.GetAbility())
            {
                // Helping or buf powers
                case PartyUnit.UnitAbility.HealingWord:
                    ApplyHealPowerToSingleUnit(dstUnit);
                    break;
                case PartyUnit.UnitAbility.HealingSong:
                    ApplyHealPowerToMultipleUnits();
                    break;
                case PartyUnit.UnitAbility.Resurect:
                    ApplyResurectPower(dstUnit);
                    break;
                // Mele attack powers
                case PartyUnit.UnitAbility.BlowWithGreatSword:
                case PartyUnit.UnitAbility.BlowWithMaul:
                case PartyUnit.UnitAbility.CutWithAxe:
                case PartyUnit.UnitAbility.CutWithDagger:
                case PartyUnit.UnitAbility.SlashWithSword:
                case PartyUnit.UnitAbility.StabWithDagger:
                case PartyUnit.UnitAbility.StompWithFoot:
                // Ranged attack powers
                case PartyUnit.UnitAbility.ShootWithBow:
                case PartyUnit.UnitAbility.ShootWithCompoudBow:
                case PartyUnit.UnitAbility.ThrowSpear:
                case PartyUnit.UnitAbility.ThrowRock:
                    ApplyDestructivePowerToSingleUnit(dstUnit);
                    break;
                // Magic (including pure) attack powers
                case PartyUnit.UnitAbility.CastChainLightning:
                case PartyUnit.UnitAbility.CastLightningStorm:
                case PartyUnit.UnitAbility.HolyWord:
                    ApplyDestructivePowerToMultipleUnits();
                    break;
                default:
                    Debug.LogError("Unknown unit power");
                    break;
            }
        }
        else
        {
            // in case of magic power - apply it to all units in enemy party
            Debug.Log(activeBattleUnit.GetUnitName() + " acting upon whole party");
            switch (activeBattleUnit.GetAbility())
            {
                // Helping or buf powers
                case PartyUnit.UnitAbility.HealingSong:
                    ApplyHealPowerToMultipleUnits();
                    break;
                // Magic (including pure) attack powers
                case PartyUnit.UnitAbility.CastChainLightning:
                case PartyUnit.UnitAbility.CastLightningStorm:
                case PartyUnit.UnitAbility.HolyWord:
                    ApplyDestructivePowerToMultipleUnits();
                    break;
                default:
                    Debug.LogError("Unknown unit power");
                    break;
            }
        }
    }

    public bool HasEscapedBattle()
    {
        // verify if at least one unit has escaped the battle
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // get unit for later checks
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // verify if unit has escaped
                    if (unit.GetHasEscaped())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    int GetExperienceForDestroyedUnits()
    {
        int experiencePool = 0;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // get unit for later checks
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // verify if unit is dead
                    if (!unit.GetIsAlive())
                    {
                        // unit is dead, add his experience reward to experiencePool
                        experiencePool += unit.GetExperienceReward();
                    }
                }
            }
        }
        return experiencePool;
    }

    int GetNumberOfAfterBattleLeftUnit()
    {
        int unitsLeft = 0;
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // get unit for later checks
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // verify if unit is alive and has not escaped
                    if (unit.GetIsAlive() && !unit.GetHasEscaped())
                    {
                        // unit is alive and has not escaped
                        // increment units left counter
                        unitsLeft += 1;
                    }
                }
            }
        }
        return unitsLeft;
    }

    bool UnitCanBeUpgraded()
    {
        return true;
    }

    bool UnitHasReachedUpgradeLimit()
    {
        return true;
    }

    void OfferPartyLeaderToLearnNewAbility()
    {

    }

    void IncrementUnitMaxStats(PartyUnit unit)
    {
        // this is done on lvl up
        unit.SetExperienceReward(unit.GetExperienceReward() + unit.GetExperienceRewardIncrementOnLevelUp());
        unit.SetHealthMax(unit.GetHealthMax() + unit.GetHealthMaxIncrementOnLevelUp());
        unit.SetPower(unit.GetPower() + unit.GetPowerIncrementOnLevelUp());
    }

    void ResetUnitStatsToMax(PartyUnit unit)
    {
        // this is done on lvl up
        unit.SetHealthCurr(unit.GetHealthMax());
    }

    void UpgradeUnitClass(PartyUnit unit)
    {
        // this is done on lvl up

    }

    void UpgradeUnit(PartyUnit unit)
    {
        Debug.Log("UpgradeUnit");
        // unit has reached new level
        // verify if this is party leader
        if (unit.GetIsLeader())
        {
            // this party leader
            // reset his experience to 0
            unit.SetExperience(0);
            // and increment his level
            unit.SetLevel(unit.GetLevel() + 1);
            // offer party leader to learn new ability
            OfferPartyLeaderToLearnNewAbility();
        }
        else
        {
            // this common unit
            // verify if conditions which allow unit to upgrade are met
            if (UnitCanBeUpgraded())
            {
                // and upgrade unit to the next class 
                UpgradeUnitClass(unit);
            }
            else if (UnitHasReachedUpgradeLimit())
            {
                // increment max unit stats
                IncrementUnitMaxStats(unit);
                // reset unit stats to maximum and reset experience counter
                ResetUnitStatsToMax(unit);
                // reset his experience to 0
                unit.SetExperience(0);
                // and increment his level
                unit.SetLevel(unit.GetLevel() + 1);
            }
            else
            {
                // wait for upgrade condition to be fulfilled
                // keep unit's experience at max
                unit.SetExperience(unit.GetExperienceRequiredToReachNewLevel());
            }
        }

    }

    public void GrantAndShowExperienceGained(PartyPanel enemyPartyPanel)
    {
        // get all experience gained for destroyed enemy units
        int gainedExperiencePool = enemyPartyPanel.GetExperienceForDestroyedUnits();
        // distribute experience between all units, which are alive and not escaped
        int unitsLeftAfterBattle = GetNumberOfAfterBattleLeftUnit();
        int experiencePerUnit = gainedExperiencePool / unitsLeftAfterBattle;
        // grant experience and show gained experience
        foreach (string horisontalPanel in horisontalPanels)
        {
            foreach (string cell in cells)
            {
                // verify if slot has an unit in it
                Transform unitSlot = transform.Find(horisontalPanel).Find(cell).Find("UnitSlot");
                if (unitSlot.childCount > 0)
                {
                    // get unit for later checks
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    // verify if unit is alive and has not escaped
                    if (unit.GetIsAlive() && !unit.GetHasEscaped())
                    {
                        // add experience to the unit
                        int newUnitExperienceValue = unit.GetExperience() + experiencePerUnit;
                        // verify if unit has not reached new level
                        if (newUnitExperienceValue < unit.GetExperienceRequiredToReachNewLevel())
                        {
                            // unit has not reached new level
                            // just update hist current experience value
                            unit.SetExperience(newUnitExperienceValue);
                            // show gained experience
                        }
                        else
                        {
                            UpgradeUnit(unit);
                        }
                    }
                }
            }
        }
    }

    #endregion For Battle Screen

    //// Update is called once per frame
    //void Update () {

    //}
}
