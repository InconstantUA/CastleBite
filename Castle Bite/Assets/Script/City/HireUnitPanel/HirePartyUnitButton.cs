using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirePartyUnitButton : MonoBehaviour {
    public enum ButtonMode
    {
        HireCommonUnit,
        HirePartyLeader
    }
    [SerializeField]
    ButtonMode buttonMode;

    void OnEnable()
    {
        // Debug.LogWarning("HirePartyUnitButton has been Enabled");
    }

    void OnDisable()
    {
        // Debug.LogWarning("HirePartyUnitButton has been Disabled");
    }

    public void ShowHireUnitMenu()
    {
        // Get unit which we need to hire parameters from caller
        // verify what unit type we need to hire: leader or common unit
        // get city, where unit will be hired
        // and get required units to hire based HirePartyUnitButton mode and city settings
        // and get destination cell
        UnitType[] unitTypesToHire;
        Transform destinationCellTr;
        UIManager uiManager = transform.root.GetComponentInChildren<UIManager>();
        City destinationCity = uiManager.GetComponentInChildren<EditPartyScreen>().LCity;
        switch (buttonMode)
        {
            case ButtonMode.HireCommonUnit:
                // get cell address (Row/Cell) of this party button
                // structure: 4MiscUI-3HireCommonUnitButtons-2[Top/Middle/Bottom]Row-1[Front/Back]Cell-HireUnitButton
                string address = transform.parent.parent.name + "/" + transform.parent.name;
                // get destination cell transform in city garnizon party panel
                Debug.Log("City " + destinationCity.name);
                Debug.Log("Party " + uiManager.GetHeroPartyUIByMode(PartyMode.Garnizon, false).name);
                Debug.Log("PartyPanel " + uiManager.GetHeroPartyUIByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>().name);
                destinationCellTr = uiManager.GetHeroPartyUIByMode(PartyMode.Garnizon, false).GetComponentInChildren<PartyPanel>().transform.Find(address);
                Debug.Log("Hire common unit for " + destinationCellTr.parent.name + "/" + destinationCellTr.name + " cell with " + address + " address");
                // get unit types to hire
                unitTypesToHire = destinationCity.HireableCommonUnits;
                break;
            case ButtonMode.HirePartyLeader:
                destinationCellTr = null; // it will be set later based on the type of leader selected and it is not needed here because party is not created yet
                // get unit types to hire
                unitTypesToHire = destinationCity.HireablePartyLeaders;
                break;
            default:
                destinationCellTr = null;
                unitTypesToHire = null;
                Debug.LogError("Unknown HirePartyUnitButton mode ");
                break;
        }
        // activate hire unit menu
        transform.root.Find("MiscUI/HireUnit").GetComponent<HireUnitGeneric>().SetActive(unitTypesToHire, destinationCellTr); // verfy if it will find disabled menus
    }
}
