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

    #region On Change: hire or dismiss unit

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
            capacity = GetPartyLeader().GetLeadership();
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
                        if (transform.Find(horisontalPanel).Find(cell).Find("UnitSlot").childCount == 0)
                        {
                            // partyPanel.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").SetAsLastSibling();
                            transform.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        //// deactivate - set behind drop slot
                        //if (partyPanel.Find(horisontalPanel).Find(cell).Find("UnitSlot").childCount == 0)
                        //{
                        // partyPanel.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").SetAsFirstSibling();
                        transform.Find(horisontalPanel).Find(cell).Find("HireUnitPnlBtn").gameObject.SetActive(false);
                        //}
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
            if (city.cityLevel == 1)
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
        Transform untCell;
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
                untCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = untCell.Find("UnitSlot");
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
                    untCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
    }


    public void SetActiveDismiss(bool activate)
    {
        Transform untCell;
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
                untCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = untCell.Find("UnitSlot");
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
                    untCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
    }


    public void SetActiveResurect(bool activate)
    {
        Transform untCell;
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
                untCell = transform.Find(horisontalPanel).Find(cell);
                unitSlot = untCell.Find("UnitSlot");
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
                    untCell.Find("Br").GetComponent<Text>().color = hightlightColor;
                }
            }
        }
        // and disable hire buttons
        SetHireUnitPnlButtonActive(!activate);
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
