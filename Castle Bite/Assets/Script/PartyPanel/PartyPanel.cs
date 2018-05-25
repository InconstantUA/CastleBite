using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls all operations with child panels
public class PartyPanel : MonoBehaviour {
    public enum PanelMode { Party, Garnizon };
    public PanelMode panelMode;
    string[] horisontalPanels = { "Top", "Middle", "Bottom" };
    string[] singleUnitCells = { "Left", "Right" };
    string[] cells = { "Left", "Right", "Wide" };

    public Transform GetUnitSlotTr(string row, string cell)
    {
        Debug.Log(row + " " + cell);
        return transform.Find(row).Find(cell).Find("UnitSlot");
    }

    public Transform GetUnitSlotTr(Transform callerCell, PartyUnit selectedUnit = null)
    {
        // return most suitable cell for the unit, if it was specified
        // otherwise just return row and cell
        // if it is double size, then place it in the wide cell
        // if (PartyUnit.UnitSize.Single == unit.GetUnitSize())
        // if hired unit is double unit, then we actually need to change its parent to the wide
        Transform newUnitParentSlot = null;
        if (selectedUnit.GetUnitSize() == PartyUnit.UnitSize.Double)
        {
            // hierarchy: [Top/Middle/Bottom panel]-[Left/Right/Wide]-callerCell
            newUnitParentSlot = callerCell.parent.Find("Wide").Find("UnitSlot");
            // Also we need to enable Wide panel, because by defaut it is disabled
            callerCell.parent.Find("Wide").gameObject.SetActive(true);
            // And disable left and right panels
            callerCell.parent.Find("Left").gameObject.SetActive(false);
            callerCell.parent.Find("Right").gameObject.SetActive(false);
        }
        else if (selectedUnit.GetUnitSize() == PartyUnit.UnitSize.Single)
        {
            newUnitParentSlot = callerCell.Find("UnitSlot");
        }
        return newUnitParentSlot;
    }

    public void OnChange()
    {
        if (panelMode == PanelMode.Garnizon)
        {
            // this is needed to disable hire units button
            // hero party does not have this functionality
            VerifyCityCapacity();
        }
    }

    // Use this for initialization
    void Start()
    {
        // verify if city or hero capacity has not been reached
        // if number of units in city or hero party reaches maximum, 
        // then hire unit button is disabled
        if (panelMode == PanelMode.Garnizon)
        {
            // this is needed to disable hire units button
            // hero party does not have this functionality
            VerifyCityCapacity();
        }
    }

    PartyUnit GetPartyLeader()
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
                    if (unit.isLeader)
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
                    unitsNumber += 1;
                }
            }
        }
        return unitsNumber;
    }

    public void SetHireUnitPnlButtonActive(bool activate)
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

    //// Update is called once per frame
    //void Update () {

    //}
}
