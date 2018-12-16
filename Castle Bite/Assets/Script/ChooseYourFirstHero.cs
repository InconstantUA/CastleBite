using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseYourFirstHero : MonoBehaviour {
    [SerializeField]
    UnitType[] dominionFactionSelectableUnits;
    [SerializeField]
    UnitType[] greenskinFactionSelectableUnits;
    [SerializeField]
    UnitType[] undeadFactionSelectableUnits;

    City GetCityByTypeAndFaction(CityType cityType, Faction faction)
    {
        foreach (City city in transform.root.Find("Map/Cities").GetComponentsInChildren<City>())
        {
            if (city.CityType == cityType && city.CityFaction == faction)
            {
                return city;
            }
        }
        Debug.LogError("Failed to find city with " + cityType.ToString() + " type and " + faction.ToString() + " faction.");
        return null;
    }

    public void SetFaction(Faction faction)
    {
        // get higherable unit types
        UnitType[] highrableUnitTypes;
        switch (faction)
        {
            case Faction.Dominion:
                highrableUnitTypes = dominionFactionSelectableUnits;
                break;
            case Faction.Greenskin:
                highrableUnitTypes = greenskinFactionSelectableUnits;
                break;
            case Faction.Undead:
                highrableUnitTypes = undeadFactionSelectableUnits;
                break;
            default:
                Debug.LogError("Unknown faction " + faction);
                break;
        }
        Debug.Log("Set faction to " + faction);
    }

    public void SetActive(bool doActivate)
    {
        // Activate/Deactivate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(true);
        // Get hire unit menu
        HireUnitGeneric hireUnitGeneric = transform.root.Find("MiscUI").GetComponentInChildren<HireUnitGeneric>(true);
        // Activate/Deactivate Hire Unit menu
        if (doActivate)
        {
            // Get unit types to hire
            UnitType[] unitTypesToHire = new UnitType[] { UnitType.Knight, UnitType.Ranger, UnitType.Archmage, UnitType.Seraphim };
            // Change UnitTemplate (unit information) preferred height
            hireUnitGeneric.transform.Find("UnitTemplate").GetComponent<LayoutElement>().preferredHeight = 48;
            // Get Dominion capital
            City dominionCapitalCity = GetCityByTypeAndFaction(CityType.Capital, Faction.Dominion);
            // Activate hire unit menu
            hireUnitGeneric.SetActive(unitTypesToHire, null, UnitHirePanel.Mode.FirstUnit);
            // Deactivate hire unit Header, because we will replace it with our header
            hireUnitGeneric.transform.Find("Header").gameObject.SetActive(false);
            //// Deactivate hire unit background, because it is not needed and it will cover other menus
            //hireUnitGeneric.transform.Find("Background").gameObject.SetActive(false);
            // Get current units list rect transform
            RectTransform unitsListRT = hireUnitGeneric.transform.Find("UnitsToHire").GetComponent<RectTransform>();
            // Get new position placehoslder rect transform
            RectTransform unitsListPlaceholderRT = hireUnitGeneric.transform.Find("UnitsToHirePlaceholderMiddle").GetComponent<RectTransform>();
            // Change position of Units to hire list
            unitsListRT.offsetMin = new Vector2(unitsListPlaceholderRT.offsetMin.x, unitsListPlaceholderRT.offsetMin.y); // left, bottom
            unitsListRT.offsetMax = new Vector2(unitsListPlaceholderRT.offsetMax.x, unitsListPlaceholderRT.offsetMax.y); // -right, -top
            // Deactivate standard hire unit button
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/HireUnitBtn").gameObject.SetActive(false);
            // Activate replacement for hire unit button
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/ContinueAndHireFirstHeroBtn").gameObject.SetActive(true);
            // Deactivate standard close hire unit menu button
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/CloseHireUnitMenuBtn").gameObject.SetActive(false);
            // Deactivate activated by default top gold info panel
            transform.root.Find("MiscUI/TopInfoPanel/Middle/CurrentGold").gameObject.SetActive(false);
        }
        else
        {
            // Deactivate hire unit menu
            hireUnitGeneric.gameObject.SetActive(false);
            // Activate back hire unit Header
            hireUnitGeneric.transform.Find("Header").gameObject.SetActive(true);
            //// Activate back hire unit background
            //hireUnitGeneric.transform.Find("Background").gameObject.SetActive(true);
            // Get the list of units to hire transform
            Transform unitsListTr = hireUnitGeneric.transform.Find("UnitsToHire");
            // Get current units list rect transform
            RectTransform unitsListRT = unitsListTr.GetComponent<RectTransform>();
            // Get new position placehoslder rect transform
            RectTransform unitsListPlaceholderRT = hireUnitGeneric.transform.Find("UnitsToHirePlaceholderTop").GetComponent<RectTransform>();
            // Change position of Units to hire list
            unitsListRT.offsetMin = new Vector2(unitsListPlaceholderRT.offsetMin.x, unitsListPlaceholderRT.offsetMin.y); // left, bottom
            unitsListRT.offsetMax = new Vector2(unitsListPlaceholderRT.offsetMax.x, unitsListPlaceholderRT.offsetMax.y); // -right, -top
            // Change UnitTemplate (unit information) preferred height
            hireUnitGeneric.transform.Find("UnitTemplate").GetComponent<LayoutElement>().preferredHeight = 80;
            // Deactivate replacement for hire unit button
            transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/ContinueAndHireFirstHeroBtn").gameObject.SetActive(false);
        }
        // Activate/Dectivate this menu
        gameObject.SetActive(doActivate);
    }

    Toggle GetSelectedToggle()
    {
        // Find selected toggle and get attached to it unit template
        Toggle[] toggles = transform.root.Find("MiscUI").GetComponentInChildren<HireUnitGeneric>(true).GetComponentInChildren<ToggleGroup>(true).GetComponentsInChildren<Toggle>();
        // Get selected toggle
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                return toggle;
            }
        }
        Debug.LogError("No any toggle selected");
        return null;
    }

    PlayerUniqueAbility GetSelectedUniqueAbility()
    {
        // Get selected toggle
        Toggle selectedToggle = GetSelectedToggle();
        // verify if toggle is is selected
        if (selectedToggle != null)
        {
            return selectedToggle.GetComponent<ChooseAbility>().ability;
        }
        // Return hardcore by default
        return PlayerUniqueAbility.Hardcore;
    }

    UnitType GetSelectedUnitType()
    {
        // Get selected toggle
        Toggle selectedToggle = GetSelectedToggle();
        // verify if toggle is is selected
        if (selectedToggle != null)
        {
            return selectedToggle.GetComponent<UnitHirePanel>().UnitToHire.UnitType;
        }
        // Return Unknown type by default
        return UnitType.Unknown;
    }

    string GetPlayerName()
    {
        // get name from input
        string name = transform.root.Find("MiscUI/ChooseYourFirstHero/InputField").GetComponent<InputField>().text;
        // verify if name is not empty
        if ("" == name)
        {
            // reset name to default
            name = ChapterManager.Instance.PlayersData[0].givenName;
        }
        // return name
        return name;
    }

    City GetActivePlayerCapital()
    {
        foreach (City city in transform.root.Find("Map/Cities").GetComponentsInChildren<City>())
        {
            // verify if city faction match players faction and that it is capital city
            // if ((city.CityFaction == transform.root.GetComponentInChildren<TurnsManager>().GetActivePlayer().Faction)
            if ((city.CityFaction == TurnsManager.Instance.GetActivePlayer().Faction)
                && (city.CityType == CityType.Capital))
            {
                return city;
            }
        }
        return null;
    }

    public void HireFirstHero()
    {
        // Get Chosen race captial city link
        // Ask City to Hire chosen unit
        //GetCityTransform().GetComponent<City>().HireUnit(null, GetSelectedUnitType());
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).HireUnit(null, GetSelectedUnitType(), false, GetActivePlayerCapital());
        // Deactivate Choose your first hero menu
        transform.root.Find("MiscUI").GetComponentInChildren<ChooseYourFirstHero>(true).SetActive(false);
        // Activate Prolog
        transform.root.Find("MiscUI").GetComponentInChildren<Prolog>(true).SetActive(true);
    }
}
