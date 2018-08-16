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

    public void ShowHireUnitMenu()
    {
        // Get unit which we need to hire parameters from caller
        // verify what unit type we need to hire: leader or common unit
        // get city, where unit will be hired
        // and get required units to hire based HirePartyUnitButton mode and city settings
        // and get destination cell
        UnitType[] unitTypesToHire;
        Transform destinationCellTr;
        City destinationCity;
        switch (buttonMode)
        {
            case ButtonMode.HireCommonUnit:
                // get city
                //// structure: 4City-3HireCommonUnitButtons-2[Top/Middle/Bottom]Row-1[Front/Back]Cell-HireUnitButton
                //destinationCity = transform.parent.parent.parent.parent.GetComponent<City>();
                // structure: 4MiscUI-3HireCommonUnitButtons-2[Top/Middle/Bottom]Row-1[Front/Back]Cell-HireUnitButton
                //             MiscUI-CityScreen(link to City)
                UIManager uiManager = transform.root.GetComponentInChildren<UIManager>();
                destinationCity = uiManager.GetComponentInChildren<CityScreen>().City;
                // get unit types to hire
                unitTypesToHire = destinationCity.HireableCommonUnits;
                // get cell address (Row/Cell) of this party button
                string address = transform.parent.parent.name + "/" + transform.parent.name;
                // get destination cell transform in city garnizon party panel
                Debug.Log("City " + destinationCity.name);
                Debug.Log("Party " + uiManager.GetHeroPartyByMode(PartyMode.Garnizon).name);
                Debug.Log("PartyPanel " + uiManager.GetHeroPartyByMode(PartyMode.Garnizon).GetComponentInChildren<PartyPanel>().name);
                destinationCellTr = uiManager.GetHeroPartyByMode(PartyMode.Garnizon).GetComponentInChildren<PartyPanel>().transform.Find(address);
                Debug.Log("Hire common unit for " + destinationCellTr.parent.name + "/" + destinationCellTr.name + " cell with " + address + " address");
                break;
            case ButtonMode.HirePartyLeader:
                // get city
                // structure: 2City-1HireHeroPanel-HireUnitButton
                destinationCity = transform.parent.parent.GetComponent<City>();
                // get unit types to hire
                unitTypesToHire = destinationCity.HireablePartyLeaders;
                destinationCellTr = null; // it will be set later based on the type of leader selected and it is not needed here because party is not created yet
                break;
            default:
                unitTypesToHire = null;
                destinationCellTr = null;
                destinationCity = null;
                Debug.LogError("Unknown HirePartyUnitButton mode ");
                break;
        }
        // activate hire unit menu
        transform.root.Find("MiscUI/HireUnit").GetComponent<HireUnitGeneric>().ActivateHireUnitMenu(unitTypesToHire, destinationCellTr, destinationCity); // verfy if it will find disabled menus
    }
}
