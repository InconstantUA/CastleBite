using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour {

    public static MapMenuManager Instance { get; private set; }

    [SerializeField]
    bool keepEnabledAfterStart;
    [SerializeField]
    MapFocusPanel mapFocusPanel;
    [SerializeField]
    MapOptions mapOptions;
    [SerializeField]
    PlayerIncomeInfo playerIncomeInfo;
    [SerializeField]
    ColorPicker colorPicker;

    void Awake()
    {
        Instance = this;
        // disable it after initial start
        gameObject.SetActive(keepEnabledAfterStart);
    }

    void OnEnable()
    {
        LoadMapUIOptions();
    }

    void OnDisable()
    {
        // SaveMapUIOptions();
    }

    void SaveMapUIOptions()
    {
        Debug.Log("Save Map UI Options");
        // verify if map options has not been destroyed
        if (MapOptions != null)
        {
            SaveCitiesNamesToggleOptions();
            SaveHeroesNamesToggleOptions();
            SavePlayerIncomeToggleOptions();
            SaveManaSourcesToggleOptions();
            SaveTreasureChestsToggleOptions();
        }
        else
        {
            Debug.LogWarning("Cannot save map options. MapOptions transform has been destoyed");
        }
    }

    void LoadMapUIOptions()
    {
        if (GameOptions.Instance != null)
        {
            Debug.Log("Load Map UI Options");
            LoadCitiesNamesToggleOptions();
            LoadHeroesNamesToggleOptions();
            LoadPlayerIncomeToggleOptions();
            LoadManaSourcesToggleOptions();
            LoadTreasureChestsToggleOptions();
        }
        else
        {
            Debug.LogWarning("GameOptions.Instance is null. Initialize it before enabling Map Menu. Or wait in couroutine untill it is initialized.");
        }
    }

    #region Save Options
    public void SaveCitiesNamesToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (MapOptions.transform.Find("ToggleCitiesNames").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleCitiesNames = 1;
        }
        else
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleCitiesNames = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowCityNames", GameOptions.Instance.mapUIOpt.toggleCitiesNames); // 0 - disable, 1 - enable
    }

    public void SaveHeroesNamesToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (MapOptions.transform.Find("ToggleHeroesNames").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleHeroesNames = 1;
        }
        else
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleHeroesNames = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowHeroesNames", GameOptions.Instance.mapUIOpt.toggleHeroesNames); // 0 - disable, 1 - enable
    }

    public void SaveManaSourcesToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (MapOptions.transform.Find("ToggleManaSources").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleManaSources = 1;
        }
        else
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleManaSources = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowManaSources", GameOptions.Instance.mapUIOpt.toggleManaSources); // 0 - disable, 1 - enable
    }

    public void SaveTreasureChestsToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (MapOptions.transform.Find("ToggleTreasureChests").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleTreasureChests = 1;
        }
        else
        {
            // set option
            GameOptions.Instance.mapUIOpt.toggleTreasureChests = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowTreasureChests", GameOptions.Instance.mapUIOpt.toggleTreasureChests); // 0 - disable, 1 - enable
    }

    public void SavePlayerIncomeToggleOptions()
    {
        // Always show city names toggle options 0 - disable, 1 - enable
        // verify if toggle is currently selected
        if (MapOptions.transform.Find("TogglePlayerIncome").GetComponent<TextToggle>().selected)
        {
            // set option
            GameOptions.Instance.mapUIOpt.togglePlayerIncome = 1;
        }
        else
        {
            // set option
            GameOptions.Instance.mapUIOpt.togglePlayerIncome = 0;
        }
        // save option
        PlayerPrefs.SetInt("MapUIShowPlayerIncome", GameOptions.Instance.mapUIOpt.togglePlayerIncome); // 0 - disable, 1 - enable
    }
    #endregion Save Options

    #region Load Options
    void LoadCitiesNamesToggleOptions()
    {
        // Get map UI options
        GameOptions.Instance.mapUIOpt.toggleCitiesNames = PlayerPrefs.GetInt("MapUIShowCityNames", 0); // default 0 - disable "always show city names" toggle
                                                                                                       // Get City names toggle
        TextToggle textToggle = MapOptions.transform.Find("ToggleCitiesNames").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.Instance.mapUIOpt.toggleCitiesNames == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            MapManager.Instance.SetCitiesNamesVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            MapManager.Instance.SetCitiesNamesVisible(true);
        }
    }

    void LoadHeroesNamesToggleOptions()
    {
        // Get map UI options
        GameOptions.Instance.mapUIOpt.toggleHeroesNames = PlayerPrefs.GetInt("MapUIShowHeroesNames", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = MapOptions.transform.Find("ToggleHeroesNames").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.Instance.mapUIOpt.toggleHeroesNames == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            MapManager.Instance.SetHeroesNamesVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            MapManager.Instance.SetHeroesNamesVisible(true);
        }
    }

    void LoadManaSourcesToggleOptions()
    {
        // Get map UI options
        GameOptions.Instance.mapUIOpt.toggleManaSources = PlayerPrefs.GetInt("MapUIShowManaSources", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = MapOptions.transform.Find("ToggleManaSources").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.Instance.mapUIOpt.toggleManaSources == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            MapManager.Instance.SetManaSourcesVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            MapManager.Instance.SetManaSourcesVisible(true);
        }
    }

    void LoadTreasureChestsToggleOptions()
    {
        // Get map UI options
        GameOptions.Instance.mapUIOpt.toggleTreasureChests = PlayerPrefs.GetInt("MapUIShowTreasureChests", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = MapOptions.transform.Find("ToggleTreasureChests").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.Instance.mapUIOpt.toggleTreasureChests == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide cities names
            MapManager.Instance.SetTreasureChestsVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // always show city names
            MapManager.Instance.SetTreasureChestsVisible(true);
        }
    }

    void LoadPlayerIncomeToggleOptions()
    {
        // Get map UI options
        GameOptions.Instance.mapUIOpt.togglePlayerIncome = PlayerPrefs.GetInt("MapUIShowPlayerIncome", 0); // default 0 - disable "always show heroes names" toggle
        // Get toggle
        TextToggle textToggle = MapOptions.transform.Find("TogglePlayerIncome").GetComponent<TextToggle>();
        // verify if it was enabled before
        if (GameOptions.Instance.mapUIOpt.togglePlayerIncome == 0)
        {
            // disable toggle
            textToggle.selected = false;
            textToggle.SetNormalStatus();
            // hide player income
            SetPlayerIncomeVisible(false);
        }
        else
        {
            // enable toggle
            textToggle.selected = true;
            textToggle.SetPressedStatus();
            // show player income
            SetPlayerIncomeVisible(true);
        }
    }
    #endregion Load Options

    // called via Unity Editor
    public void SetPlayerIncomeVisible(bool doShow)
    {
        Debug.Log("Show player income: " + doShow.ToString());
        playerIncomeInfo.SetActive(doShow);
        // Save menu options
        SavePlayerIncomeToggleOptions();
    }

    public IEnumerator EnterCityEditMode(MapCity mapCity)
    {
        //Debug.Log("EnterCityEditMode");
        mapCity.DimmLabel();
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(mapCity.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // map manager change to browse mode back
        // . - this is done by OnDisable() automatically in MapManager
        //MapManager mapManager = transform.parent.GetComponent<MapManager>();
        //mapManager.SetMode(MapManager.Mode.Browse);
        // Deactivate map manager with map
        MapManager.Instance.gameObject.SetActive(false);
        // Deactivate this map menu
        gameObject.SetActive(keepEnabledAfterStart);
        // Note: everything below related to mapManager or mapScreen will not be processed, because map manager is disabled
        // Activate city view = go to city edit mode
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).SetEditPartyScreenActive(mapCity.LCity);
    }

    public IEnumerator EnterHeroEditMode(MapHero mapHero)
    {
        Debug.Log("Enter hero edit mode.");
        mapHero.DimmLabel();
        // Trigger on mapobject exit to Hide label(s - + hide hero's lable, if it is in city)
        // verify if MapObject's labe is still active and mouse over it
        if (mapHero.GetComponent<MapObject>().Label.GetComponent<Text>().raycastTarget && mapHero.GetComponent<MapObject>().Label.IsMouseOver)
        {
            // disable it
            mapHero.GetComponent<MapObject>().OnPointerExit(null);
        }
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(mapHero.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate map manager with map
        MapManager.Instance.gameObject.SetActive(false);
        // Deactivate this map menu
        gameObject.SetActive(false);
        // Note: everything below related to mapManager or mapScreen will not be processed, because map manager is disabled
        // Activate hero edit menu
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).SetEditPartyScreenActive(mapHero.LHeroParty);
    }

    public void TurnMainMenu()
    {
        // enable main menu
        MainMenuManager.Instance.gameObject.SetActive(true);
        // Enter animation mode on map to prevent Update() from acting
        MapManager.Instance.SetMode(MapManager.Mode.Animation);
        //// disable world map
        //MapManager.Instance.gameObject.SetActive(false);
        //// disable map menu
        //gameObject.SetActive(false);
        // disable current gold info
        playerIncomeInfo.SetActive(false);
    }

    public void SetCitiesNamesVisible(bool doActivate)
    {
        MapManager.Instance.SetCitiesNamesVisible(doActivate);
    }

    public void SetHeroesNamesVisible(bool doActivate)
    {
        MapManager.Instance.SetHeroesNamesVisible(doActivate);
    }

    public void SetManaSourcesVisible(bool doActivate)
    {
        MapManager.Instance.SetManaSourcesVisible(doActivate);
    }

    public void SetTreasureChestsVisible(bool doActivate)
    {
        MapManager.Instance.SetTreasureChestsVisible(doActivate);
    }

    public void SetTileHighlighterVisible(bool doActivate)
    {
        MapManager.Instance.TileHighlighter.gameObject.SetActive(doActivate);
    }

    public MapFocusPanel MapFocusPanel
    {
        get
        {
            return mapFocusPanel;
        }
    }

    public MapOptions MapOptions
    {
        get
        {
            return mapOptions;
        }
    }

    void OnColorPickerSave()
    {
        // ask mapmanager to change player color
        MapManager.Instance.SetPlayerColor(colorPicker.GetSelectedColor());
    }

    void OnColorPickerCancel()
    {
        // nothing to do here
    }

    public void ShowPlayerColorPicker()
    {
        colorPicker.SetActive(ConfigManager.Instance.PlayersObjectsOnMapColor, new UnityEngine.Events.UnityAction(OnColorPickerSave), new UnityEngine.Events.UnityAction(OnColorPickerCancel));
    }
}
