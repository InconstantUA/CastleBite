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
    string[] singleUnitCells = { "Left", "Right" };
    string[] cells = { "Left", "Right", "Wide" };
    public enum ChangeType { Init, HireSingleUnit, HireDoubleUnit, HirePartyLeader, DismissSingleUnit, DismissDoubleUnit, DismissPartyLeader}

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
        changedCell.parent.Find("Left").gameObject.SetActive(false);
        changedCell.parent.Find("Right").gameObject.SetActive(false);
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
        changedCell.parent.Find("Left").gameObject.SetActive(true);
        changedCell.parent.Find("Right").gameObject.SetActive(true);
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
            changedCell.parent.Find("Left/HireUnitPnlBtn").gameObject.SetActive(true);
            changedCell.parent.Find("Right/HireUnitPnlBtn").gameObject.SetActive(true);

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
                    if ((PanelMode.Garnizon == panelMode) && (("Left" == cell) || ("Right" == cell)))
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
                    if ((PanelMode.Garnizon == panelMode) && (("Left" == cell) || ("Right" == cell)))
                    {
                        unitPanel.Find("HireUnitPnlBtn").gameObject.SetActive(true);
                    }
                    // it is possible that double unit was dismissed
                    if ("Wide" == cell)
                    {
                        // we need to disable Wide panel, because it is still enabled and placed on top of single panels
                        unitPanel.parent.Find("Wide").gameObject.SetActive(false);
                        // and enable left and right panels
                        unitPanel.parent.Find("Left").gameObject.SetActive(true);
                        unitPanel.parent.Find("Right").gameObject.SetActive(true);
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
        if (callerCell.name == "Left")
        {
            // check Right cell
            oppositeCell = callerCell.parent.Find("Right");
        } else
        {
            // check Left cell
            oppositeCell = callerCell.parent.Find("Left");
        }
        // check if cell is occupied
        // structure [Left/Right cell]-UnitSlot-unit
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
        // structure: 5[City]-4[HeroParty/CityGarnizon]Party-3PartyPanel-2[Top/Middle/Bottom]HorizontalPanelGroup-1[Left/Right/Wide]Cell-UnitSlot-(this)UnitCanvas
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
                                                    if ("Left" == unitCell.name)
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find("Right");
                                                    }
                                                    else
                                                    {
                                                        nearbySrcCellTr = horizontalPanelGroup.Find("Left");
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
                            UnitDragHandler isLeftCellOccupied; // if null - false, if other - true, we use it as bool
                            UnitDragHandler isRightCellOccupied; // if null - false, if other - true, we use it as bool
                            bool isLeftCellUnitInterPartyMovable = false;
                            bool isRightCellUnitInterPartyMovable = false;
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
                                    leftCell = otherPartyPanel.transform.Find(horisontalPanel + "/Left");
                                    rightCell = otherPartyPanel.transform.Find(horisontalPanel + "/Right");
                                    // gather input for possible states
                                    // UnitCanvas, which has UnitDragHandler component attached is present
                                    isLeftCellOccupied = leftCell.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                    isRightCellOccupied = rightCell.Find("UnitSlot").GetComponentInChildren<UnitDragHandler>();
                                    if (isLeftCellOccupied)
                                    {
                                        // occupied
                                        cellUnit = isLeftCellOccupied.GetComponentInChildren<PartyUnit>();
                                        isUnitInterPartyDraggable = cellUnit.GetIsInterpartyMovable();
                                        if (isUnitInterPartyDraggable)
                                        {
                                            isLeftCellUnitInterPartyMovable = true;
                                        }
                                        else
                                        {
                                            isLeftCellUnitInterPartyMovable = false;
                                        }
                                    }
                                    if (isRightCellOccupied)
                                    {
                                        // occupied
                                        cellUnit = isRightCellOccupied.GetComponentInChildren<PartyUnit>();
                                        isUnitInterPartyDraggable = cellUnit.GetIsInterpartyMovable();
                                        if (isUnitInterPartyDraggable)
                                        {
                                            isRightCellUnitInterPartyMovable = true;
                                        }
                                        else
                                        {
                                            isRightCellUnitInterPartyMovable = false;
                                        }
                                    }
                                    // verify conditions
                                    // 00
                                    if (!isLeftCellOccupied && !isRightCellOccupied)
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
                                    else if ( (!isLeftCellOccupied && (isRightCellOccupied && isRightCellUnitInterPartyMovable))
                                      || ( (isLeftCellOccupied && isLeftCellUnitInterPartyMovable) && !isRightCellOccupied) )
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
                                    else if ((isLeftCellOccupied && isLeftCellUnitInterPartyMovable) && (isRightCellOccupied && isRightCellUnitInterPartyMovable))
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
                    // verify if unit has moved or not
                    PartyUnit unit = unitSlot.GetComponentInChildren<PartyUnit>();
                    if (!unit.GetHasMoved())
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
        // structure: 5PartyPanel-4[Top/Middle/Bottom]HorizontalPanel-3[Left/Right/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
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

    void ApplyHealPower()
    {
        Debug.Log("ApplyHealPower");
    }

    void ApplyResurectPower()
    {
        Debug.Log("ApplyResurectPower");
    }

    void ApplyMelePower()
    {
        Debug.Log("ApplyMelePower");
    }

    void ApplyRangedPower()
    {
        Debug.Log("ApplyRangedPower");
    }

    void ApplyMagicPower()
    {
        Debug.Log("ApplyMagicPower");
    }

    void ApplyPurePower()
    {
        Debug.Log("ApplyPurePower");
    }

    void HighlightActiveUnitInBattle(PartyUnit unitToActivate)
    {
        Debug.Log(" HighlightActiveUnitInBattle");
        // highlight unit canvas with blue color
        Color highlightColor = Color.blue;
        // structure: 3[Left/Right/Wide]Cell-2UnitSlot-1UnitCanvas-(This)PartyUnit
        Transform cellTr = unitToActivate.transform.parent.parent.parent;
        Text canvasText = cellTr.Find("Br").GetComponent<Text>();
        canvasText.color = highlightColor;
    }

    public void SetActiveUnitInBattle(PartyUnit unitToActivate)
    {
        Debug.Log("SetActiveUnitInBattle " + unitToActivate.GetUnitName());
        // new unit became active in battle
        // highlight differently cells which this unit can or cannot interract and in which way
        // act based on activated unit relationships with this panel
        // verify if this is enemy unit or unit from this party
        if (GetIsUnitFriendly(unitToActivate))
        {
            // this is friendly unit
            // This unit belongs to this party highlight it here
            HighlightActiveUnitInBattle(unitToActivate);
            // act based on the unit powers
            switch (unitToActivate.GetPower()) {
                // Helping or buf powers
                case PartyUnit.UnitPower.Heal:
                    ApplyHealPower();
                    break;
                case PartyUnit.UnitPower.Resurect:
                    ApplyResurectPower();
                    break;
                // Mele attack powers
                case PartyUnit.UnitPower.BlowWithGreatSword:
                case PartyUnit.UnitPower.BlowWithMaul:
                case PartyUnit.UnitPower.CutWithAxe:
                case PartyUnit.UnitPower.CutWithDagger:
                case PartyUnit.UnitPower.SlashWithSword:
                case PartyUnit.UnitPower.StabWithDagger:
                case PartyUnit.UnitPower.StompWithFoot:
                    ApplyMelePower();
                    break;
                // Ranged attack powers
                case PartyUnit.UnitPower.ShootWithBow:
                case PartyUnit.UnitPower.ShootWithCompoudBow:
                case PartyUnit.UnitPower.ThrowSpear:
                case PartyUnit.UnitPower.ThrowRock:
                    ApplyRangedPower();
                    break;
                // Magic attack powers
                case PartyUnit.UnitPower.CastChainLightning:
                case PartyUnit.UnitPower.CastLightningStorm:
                    ApplyMagicPower();
                    break;
                // Pure attack powers
                case PartyUnit.UnitPower.HolyWord:
                    ApplyPurePower();
                    break;
                default:
                    Debug.LogError("Unknown unit power");
                    break;
            }
        }
        else
        {
            // this enemy unit
        }
    }

    #endregion For Battle Screen

    //// Update is called once per frame
    //void Update () {

    //}
}
