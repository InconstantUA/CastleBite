using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitHirePanel : MonoBehaviour
{
    [SerializeField]
    GameObject characteristicTemplate;

    PartyUnit unitToHire;
    public enum Mode {Normal, FirstUnit};
    private Mode panelMode = Mode.Normal;

    public PartyUnit UnitToHire
    {
        get
        {
            return unitToHire;
        }

        set
        {
            unitToHire = value;
        }
    }

    public Mode PanelMode
    {
        get
        {
            return panelMode;
        }

        set
        {
            panelMode = value;
        }
    }

    void SetUnitName()
    {
        transform.Find("Name").GetComponent<Text>().text = unitToHire.UnitName;
    }

    void SetUnitCharacteristic(string name, string value)
    {
        // copy characteristic from template
        GameObject newCharacteristic = Instantiate(characteristicTemplate, transform.Find("Characteristics"));
        // set characterstic name
        newCharacteristic.transform.Find("Name").GetComponent<Text>().text = name;
        // set characterstic value
        newCharacteristic.transform.Find("Value").GetComponent<Text>().text = value;
        // enable it
        newCharacteristic.SetActive(true);
    }

    // Use this for initialization
    public void Start() {
        if (unitToHire != null)
        {
            // Set unit name
            SetUnitName();
            // populate panel with information from attached unitToHire
            // pupulate additional unit information
            if (Mode.FirstUnit == panelMode)
            {
                SetUnitCharacteristic("Leadership", unitToHire.UnitLeadership.ToString());
                SetUnitCharacteristic("Role", unitToHire.UnitRole);
                //// Fill in characteristic names
                //transform.Find("CharacteristicsNames").GetComponent<Text>().text =
                //    "Leadership" + "\r\n" +
                //    "Role" + "\r\n";
                //// fill in first hero hire menu with hero's most important characteristics
                //transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                //    unitToHire.UnitLeadership.ToString() + "\r\n" +
                //    unitToHire.UnitRole;
            }
            else
            {
                // Normal mode
                // verify if unit is Leader
                if (unitToHire.IsLeader)
                {
                    SetUnitCharacteristic("Cost", unitToHire.UnitCost.ToString());
                    SetUnitCharacteristic("Leadership", unitToHire.UnitLeadership.ToString());
                    SetUnitCharacteristic("Role", unitToHire.UnitRole);
                    //// Fill in characteristic names
                    //transform.Find("CharacteristicsNames").GetComponent<Text>().text =
                    //    "Cost" + "\r\n" +
                    //    "Leadership" + "\r\n" +
                    //    "Role";
                    //// Fill in characteristic values
                    //transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    //    unitToHire.UnitCost.ToString() + "\r\n" +
                    //    unitToHire.UnitLeadership.ToString() + "\r\n" +
                    //    unitToHire.UnitRole;
                }
                else
                {
                    SetUnitCharacteristic("Cost", unitToHire.UnitLeadership.ToString());
                    // SetUnitCharacteristic("Ability", unitToHire.UnitAbility.ToString());
                    SetUnitCharacteristic("Size", unitToHire.UnitSize.ToString());
                    SetUnitCharacteristic("Role", unitToHire.UnitRole);
                    //// Fill in characteristic names
                    //transform.Find("CharacteristicsNames").GetComponent<Text>().text =
                    //    "Cost" + "\r\n" +
                    //    "Ability" + "\r\n" +
                    //    "Size" + "\r\n" +
                    //    "Role";
                    //// Fill in characteristic values
                    //transform.Find("CharacteristicsValues").GetComponent<Text>().text =
                    //    unitToHire.UnitCost.ToString() + "\r\n" +
                    //    unitToHire.UnitAbility.ToString() + "\r\n" +
                    //    unitToHire.UnitSize.ToString() + "\r\n" +
                    //    unitToHire.UnitRole;
                }
            }
        }
        // Force layout update
        // Note: this should be done to force all fields to be adjusted to the text size
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

}
