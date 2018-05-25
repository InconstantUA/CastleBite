using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls all operations with child panels
public class PartyPanel : MonoBehaviour {

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

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
