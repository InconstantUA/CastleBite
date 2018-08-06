using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitHirePanel : MonoBehaviour
{
    public PartyUnit unitToHire;
    public enum Mode {Normal, FirstUnit};
    public Mode mode;

    // Use this for initialization
    void Start() {
        // populate panel with information from attached unitToHire
        transform.Find("Name").GetComponent<Text>().text = "[" + unitToHire.UnitName + "]";
        // pupulate additional unit information
        if (Mode.FirstUnit == mode)
        {
            // fill in first hero hire menu with hero's most important characteristics
            transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                unitToHire.UnitRole + "\r\n" +
                unitToHire.UnitBriefDescription;
        }
        else
        {
            // Normal mode
            if (unitToHire.UnitSize == UnitSize.Double)
            {
                // for double size units also indicate their size
                transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    unitToHire.UnitCost.ToString() + "\r\n" +
                    unitToHire.UnitLeadership.ToString() + "\r\n" +
                    unitToHire.UnitRole + "\r\n" + "Large";
            }
            else
            {
                transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    unitToHire.UnitCost.ToString() + "\r\n" +
                    unitToHire.UnitLeadership.ToString() + "\r\n" +
                    unitToHire.UnitRole;
            }
        }
    }

    public PartyUnit GetUnitToHire()
    {
        return unitToHire;
    }

}
