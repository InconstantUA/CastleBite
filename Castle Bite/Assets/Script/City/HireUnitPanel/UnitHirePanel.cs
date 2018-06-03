using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHirePanel : MonoBehaviour {
    public PartyUnit unitToHire;
    public enum Mode {Normal, FirstUnit};
    public Mode mode;

    // Use this for initialization
    void Start() {
        // populate panel with information from attached unitToHire
        transform.Find("Name").GetComponent<Text>().text = "[" + unitToHire.GetUnitName() + "]";
        // pupulate additional unit information
        if (Mode.FirstUnit == mode)
        {
            // fill in first hero hire menu with hero's most important characteristics
            transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                unitToHire.GetRole() + "\r\n" +
                unitToHire.GetBriefDescription();
        }
        else
        {
            // Normal mode
            if (unitToHire.GetUnitSize() == PartyUnit.UnitSize.Double)
            {
                // for double size units also indicate their size
                transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    unitToHire.GetCost().ToString() + "\r\n" +
                    unitToHire.GetLeadership().ToString() + "\r\n" +
                    unitToHire.GetRole() + "\r\n" + "Large";
            }
            else
            {
                transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    unitToHire.GetCost().ToString() + "\r\n" +
                    unitToHire.GetLeadership().ToString() + "\r\n" +
                    unitToHire.GetRole();
            }
        }
    }

    public PartyUnit GetUnitToHire()
    {
        return unitToHire;
    }

}
