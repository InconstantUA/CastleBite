using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseYourFirstHero : MonoBehaviour {
    //[SerializeField]
    //UnitType[] dominionFactionSelectableUnits;
    //[SerializeField]
    //UnitType[] greenskinFactionSelectableUnits;
    //[SerializeField]
    //UnitType[] undeadFactionSelectableUnits;
    [SerializeField]
    FactionSelectionGroup factionSelectionGroup;
    [SerializeField]
    HireUnitGeneric hireUnitGeneric;
    [SerializeField]
    TextToggleGroup uniqueAbilitiesToggleGroup;
    [SerializeField]
    TextToggle uniqueAbilityToggleTemplate;
    [SerializeField]
    Color factionSpecificAbilityNormalColor;
    [SerializeField]
    Color factionSpecificAbilityHighlightedColor;
    [SerializeField]
    Color factionSpecificAbilityPressedColor;
    [SerializeField]
    InputField playerNameInputField;
    [SerializeField]
    int unitTemplateHeight;
    [SerializeField]
    LoadingScreen loadingScreen;
    [SerializeField]
    StartGameConfig startGameConfig;
    [SerializeField]
    Prolog prolog;


    City GetCityByTypeAndFaction(CityType cityType, Faction faction)
    {
        foreach (City city in ObjectsManager.Instance.transform.Find("Map/Cities").GetComponentsInChildren<City>())
        {
            if (city.CityType == cityType && city.CityCurrentFaction == faction)
            {
                return city;
            }
        }
        Debug.LogError("Failed to find city with " + cityType.ToString() + " type and " + faction.ToString() + " faction.");
        return null;
    }

    public void SelectFaction(Faction faction = Faction.Unknown)
    {
        // Verify if faction is unknown
        if (faction == Faction.Unknown)
        {
            // Get currently active Faction
            faction = factionSelectionGroup.GetSelectedFaction();
        }
        Debug.Log("Set faction to " + faction);
        // get higherable unit types
        //UnitType[] highrableUnitTypes = null;
        //switch (faction)
        //{
        //    case Faction.Dominion:
        //        highrableUnitTypes = dominionFactionSelectableUnits;
        //        break;
        //    case Faction.Greenskin:
        //        highrableUnitTypes = greenskinFactionSelectableUnits;
        //        break;
        //    case Faction.Undead:
        //        highrableUnitTypes = undeadFactionSelectableUnits;
        //        break;
        //    default:
        //        Debug.LogError("Unknown faction " + faction);
        //        break;
        //}
        // Get hire unit menu
        // HireUnitGeneric hireUnitGeneric = transform.root.Find("MiscUI").GetComponentInChildren<HireUnitGeneric>(true);
        // activate hire unit menu
        hireUnitGeneric.SetActive(ConfigManager.Instance[faction].hireablePartyLeaders, null, UnitHirePanel.Mode.FirstUnit, unitTemplateHeight);
        //// Deactivate standard hire unit button
        //transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/HireUnitBtn").gameObject.SetActive(false);
        //// Deactivate standard close hire unit menu button
        //transform.root.Find("MiscUI/BottomControlPanel/RightControls/CloseHireUnitMenuBtn").gameObject.SetActive(false);
        // Reset unique abilities (it may be specific to faction)
        SetUniqueAbilitiesActive(false);
        SetUniqueAbilitiesActive(true);
        // Reset default player name, when needed
        if (playerNameInputField.text == "")
        {
            SetDefaultPlayerName();
        }
    }

    string GetDefaultPlayerName()
    {
        // get selected faction
        Faction selectedFaction = factionSelectionGroup.GetSelectedFaction();
        // get chapter data for the player with selected faction
        GameObject worldTemplate = ChapterManager.Instance.GetWorldTemplateByName(ChapterManager.Instance.ActiveChapter.ChapterData.chapterName);
        // get player name which is defined in the world template
        return worldTemplate.GetComponentInChildren<ObjectsManager>().GetPlayerByFaction(selectedFaction).GivenName;
    }

    void SetDefaultPlayerName()
    {
        // set in placeholder give name defined in the world template
        playerNameInputField.placeholder.GetComponent<Text>().text = GetDefaultPlayerName();
    }

    TextToggle CreateNewAbilityToggle(UniqueAbilityConfig uniqueAbilityConfig)
    {
        // create toggle in UI
        TextToggle uniqueAbilityToggle = Instantiate(uniqueAbilityToggleTemplate.gameObject, uniqueAbilitiesToggleGroup.transform).GetComponent<TextToggle>();
        // set toggle group
        uniqueAbilityToggle.toggleGroup = uniqueAbilitiesToggleGroup;
        // set unique ability name
        uniqueAbilityToggle.GetComponent<Text>().text = uniqueAbilityConfig.displayName;
        // Set unique ability description
        foreach (Text text in uniqueAbilityToggle.GetComponentsInChildren<Text>())
        {
            // verify if we are not getting toggle = we are getting description
            if (text.gameObject != uniqueAbilityToggle.gameObject)
            {
                text.text = uniqueAbilityConfig.description;
            }
        }
        // set unique ability reference
        uniqueAbilityToggle.GetComponent<UniqueAbilitySelector>().UniqueAbilityConfig = uniqueAbilityConfig;
        // return reference to the new toggle
        return uniqueAbilityToggle;
    }

    void SetUniqueAbilitiesActive(bool doActivate)
    {
        // verify if we need to activate or deactivate abilities list
        if (doActivate)
        {
            // loop through all possible unique abilities configs
            foreach (UniqueAbilityConfig uniqueAbilityConfig in ConfigManager.Instance.UniqueAbilityConfigs)
            {
                // verify this ability is applicable only to specific factions
                if (uniqueAbilityConfig.doLimitUsageByFaction)
                {
                    // verify if that ability is allowed for this Faction
                    if (Array.Exists(uniqueAbilityConfig.applicableToFactions, element => element == factionSelectionGroup.GetSelectedFaction()))
                    {
                        TextToggle uniqueAbilityToggle = CreateNewAbilityToggle(uniqueAbilityConfig);
                        // adjust text color and toggle color schema to represent Faction-specific ability
                        uniqueAbilityToggle.GetComponent<Text>().color = factionSpecificAbilityNormalColor;
                        uniqueAbilityToggle.NormalColor = factionSpecificAbilityNormalColor;
                        uniqueAbilityToggle.HighlightedColor = factionSpecificAbilityHighlightedColor;
                        uniqueAbilityToggle.PressedColor = factionSpecificAbilityPressedColor;
                    }
                }
                else
                {
                    CreateNewAbilityToggle(uniqueAbilityConfig);
                }
            }
            // preselect first toggle
            uniqueAbilitiesToggleGroup.GetComponentsInChildren<TextToggle>()[0].ActOnLeftMouseClick();
        }
        else
        {
            // remove all previus abilities
            RecycleBin.RecycleChildrenOf(uniqueAbilitiesToggleGroup.gameObject);
            //// loop through all abilities in UI
            //foreach(Transform t in uniqueAbilitiesToggleGroup.transform)
            //{
            //    // move toggle to recycle bin and destroy it (move is required, because actual destroy is being done at the end of the frame,
            //    // but we need to preselect new toggle on reset SetUniqueAbilitiesActive(false/true)
            //    t.SetParent(RecycleBin.Instance.transform);
            //    Debug.Log("Destroy " + t.name);
            //    Destroy(t.gameObject);
            //}
        }
    }

    public void SetActive(bool doActivate)
    {
        // Create world from template
        
        // Activate/Deactivate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(doActivate);
        // Activate/Dectivate this menu
        // Note: order of activation is important: it should be activated before HireUnit to activate factionSelectionGroup
        gameObject.SetActive(doActivate);
        // Get hire unit menu
        // HireUnitGeneric hireUnitGeneric = transform.root.Find("MiscUI").GetComponentInChildren<HireUnitGeneric>(true);
        // Activate/Deactivate Hire Unit menu
        if (doActivate)
        {
            // Get unit types to hire
            // UnitType[] unitTypesToHire = new UnitType[] { UnitType.Knight, UnitType.Ranger, UnitType.Archmage, UnitType.Seraphim };
            // Change UnitTemplate (unit information) preferred height
            // hireUnitGeneric.UnitUIToggleTemplate.GetComponent<LayoutElement>().preferredHeight = 48;
            // Get Dominion capital
            // City dominionCapitalCity = GetCityByTypeAndFaction(CityType.Capital, Faction.Dominion);
            // Activate hire unit menu
            // hireUnitGeneric.SetActive(unitTypesToHire, null, UnitHirePanel.Mode.FirstUnit);
            SelectFaction();
            //// Deactivate hire unit background, because it is not needed and it will cover other menus
            //hireUnitGeneric.transform.Find("Background").gameObject.SetActive(false);
            // Get current units list rect transform
            RectTransform unitsListRT = hireUnitGeneric.UnitsToHireList.GetComponent<RectTransform>();
            // Get new position placehoslder rect transform
            RectTransform unitsListPlaceholderRT = hireUnitGeneric.transform.Find("UnitsToHirePlaceholderMiddle").GetComponent<RectTransform>();
            // Change position of Units to hire list
            unitsListRT.offsetMin = new Vector2(unitsListPlaceholderRT.offsetMin.x, unitsListPlaceholderRT.offsetMin.y); // left, bottom
            unitsListRT.offsetMax = new Vector2(unitsListPlaceholderRT.offsetMax.x, unitsListPlaceholderRT.offsetMax.y); // -right, -top
            // Activate Back button
            transform.root.Find("MiscUI/BottomControlPanel/RightControls/HireFirstHeroBackBtn").gameObject.SetActive(true);
            // bring it to the front
            transform.SetAsLastSibling();
        }
        else
        {
            // Deactivate hire unit menu
            hireUnitGeneric.gameObject.SetActive(false);
            //// Activate back hire unit background
            //hireUnitGeneric.transform.Find("Background").gameObject.SetActive(true);
            // Get the list of units to hire transform
            // Transform unitsListTr = hireUnitGeneric.UnitsToHireList.transform;
            // Get current units list rect transform
            RectTransform unitsListRT = hireUnitGeneric.UnitsToHireList.GetComponent<RectTransform>();
            // Get new position placehoslder rect transform
            RectTransform unitsListPlaceholderRT = hireUnitGeneric.transform.Find("UnitsToHirePlaceholderTop").GetComponent<RectTransform>();
            // Change position of Units to hire list
            unitsListRT.offsetMin = new Vector2(unitsListPlaceholderRT.offsetMin.x, unitsListPlaceholderRT.offsetMin.y); // left, bottom
            unitsListRT.offsetMax = new Vector2(unitsListPlaceholderRT.offsetMax.x, unitsListPlaceholderRT.offsetMax.y); // -right, -top
            // Change UnitTemplate (unit information) preferred height
            // hireUnitGeneric.UnitUIToggleTemplate.GetComponent<LayoutElement>().preferredHeight = 80;
        }
        // Deactivate/Activate hire unit Header, because we will be replaced it with our header
        hireUnitGeneric.transform.Find("Header").gameObject.SetActive(!doActivate);
        // Activate/Deactivate replacement for hire unit button
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/ContinueAndHireFirstHeroBtn").gameObject.SetActive(doActivate);
        // Activate/Deactivate back button
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/HireFirstHeroBackBtn").gameObject.SetActive(doActivate);
        // verify if we are disabling this menu
        if (!doActivate)
        {
            // Deactivate unique abilities list
            SetUniqueAbilitiesActive(doActivate);
        }
        // ..
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void GoBack()
    {
        // deactivate this menu
        SetActive(false);
        // activate main menu
        MainMenuManager.Instance.gameObject.SetActive(true);
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

    //PlayerUniqueAbility GetSelectedUniqueAbility()
    //{
    //    // Get selected toggle
    //    Toggle selectedToggle = GetSelectedToggle();
    //    // verify if toggle is is selected
    //    if (selectedToggle != null)
    //    {
    //        return selectedToggle.GetComponent<ChooseAbility>().ability;
    //    }
    //    // Return hardcore by default
    //    return PlayerUniqueAbility.Hardcore;
    //}

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

    //string GetPlayerName()
    //{
    //    // get name from input
    //    string name = transform.root.Find("MiscUI/ChooseYourFirstHero/InputField").GetComponent<InputField>().text;
    //    // verify if name is not empty
    //    if ("" == name)
    //    {
    //        // reset name to default
    //        name = ChapterManager.Instance.PlayersData[0].givenName;
    //    }
    //    // return name
    //    return name;
    //}

    //City GetActivePlayerCapital()
    //{
    //    foreach (City city in ObjectsManager.Instance.transform.Find("Map/Cities").GetComponentsInChildren<City>())
    //    {
    //        // verify if city faction match players faction and that it is capital city
    //        // if ((city.CityFaction == transform.root.GetComponentInChildren<TurnsManager>().GetActivePlayer().Faction)
    //        if ((city.CityFaction == TurnsManager.Instance.GetActivePlayer().Faction)
    //            && (city.CityType == CityType.Capital))
    //        {
    //            return city;
    //        }
    //    }
    //    return null;
    //}

    string GetPlayerName()
    {
        string playerName = playerNameInputField.text;
        // verify if player name is empty
        if (playerName == "")
        {
            // use default player name for selected faction
            Debug.LogWarning("Using default player name");
            return GetDefaultPlayerName();
        }
        else
        {
            return playerName;
        }
    }

    IEnumerator StartGame()
    {
        // skip all actions until next frame
        yield return null;
        // enable main menu panel (it was disabled by ChooseChapter)
        MainMenuManager.Instance.MainMenuPanel.SetActive(true);
        // Disable Choose Chapter menu
        MainMenuManager.Instance.ChooseChapter.gameObject.SetActive(false);
        // Load currently set active chapter
        ChapterManager.Instance.LoadChapter(ChapterManager.Instance.ActiveChapter.ChapterData.chapterName);
        // change player name to the name which is set by the user
        TurnsManager.Instance.GetActivePlayer().PlayerData.givenName = GetPlayerName();
        // get selected faction
        Faction selectedFaction = factionSelectionGroup.GetSelectedFaction();
        // Activate and reset turns manager, set chosen faction as active player
        TurnsManager.Instance.Reset(selectedFaction);
        // Activate main menu in game mode
        MainMenuManager.Instance.MainMenuInGameModeSetActive(true);
        // Activate world map
        ChapterManager.Instance.ActiveChapter.GetComponentInChildren<MapManager>(true).gameObject.SetActive(true);
        // Set chosen Unique ability for chosen player
        // TurnsManager.Instance.GetActivePlayer().PlayerData.playerUniqueAbilityData.uniqueAbilityConfig = Array.Find(ConfigManager.Instance.UniqueAbilityConfigsMap, element => element.playerUniqueAbility == playerUniqueAbility).uniqueAbilityConfig;
        // Array.Find(ConfigManager.Instance.UniqueAbilityConfigs, element => element.playerUniqueAbility == playerUniqueAbility);
        TurnsManager.Instance.GetActivePlayer().PlayerData.playerUniqueAbilityData.playerUniqueAbility = uniqueAbilitiesToggleGroup.GetSelectedToggle().GetComponent<UniqueAbilitySelector>().UniqueAbilityConfig.playerUniqueAbility;
        // GetSelectedUnitType and Get Chosen race starting city and hire first hero
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).HireUnit(null, GetSelectedUnitType(), false, ObjectsManager.Instance.GetStartingCityByFaction(selectedFaction));
        // Wait a bit
        yield return new WaitForSeconds(startGameConfig.startingScreenExplicitDelaySeconds);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate Loading screen
        loadingScreen.SetActive(false);
        // Activate Prolog
        prolog.SetActive(true, ChapterManager.Instance.ActiveChapter.ChapterData);
        // [Should be last] Deactivate (this) Choose your first hero menu
        SetActive(false);
    }

    public void HireFirstHero()
    {
        // activate Starting Game loading screen
        loadingScreen.SetActive(true, startGameConfig.startingGameTextString);
        // Bring loading screen to front
        loadingScreen.transform.SetAsLastSibling();
        // Block mouse input
        InputBlocker.Instance.SetActive(true);
        // Start to load a game
        StartCoroutine(StartGame());
    }
}
