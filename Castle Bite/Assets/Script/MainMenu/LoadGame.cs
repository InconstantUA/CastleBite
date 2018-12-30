using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LoadGame : MonoBehaviour
{
    [SerializeField]
    MenuButton menuButton;
    //[SerializeField]
    //string fileExtension;
    //[SerializeField]
    //GameObject gamePlayerTemplate;
    //[SerializeField]
    //GameObject heroPartyTemplate;
    //[SerializeField]
    //GameObject cityGarnizonTemplate;
    //[SerializeField]
    //GameObject unitCanvasTemplate;
    //[SerializeField]
    //GameObject heroPartyOnMapTemplate;
    [SerializeField]
    GameObject loadButton;
    [SerializeField]
    GameObject deleteButton;
    [SerializeField]
    GameObject backButton;
    [SerializeField]
    SavesMenu savesMenu;
    [SerializeField]
    LoadingScreen loadingScreen;
    [SerializeField]
    LoadGameConfig loadGameConfig;

    string fullFilePath;
    TextToggle selectedToggle;

    //IEnumerator SetListOfSaves()
    //{
    //    // update list of saves
    //    // get list of .save files in directory
    //    FileInfo[] files = ConfigManager.Instance.GameSaveConfig.GetSavesFilesSortedOlderToYounger();
    //    // verify if all object are loaded
    //    if (transform.Find("Saves"))
    //        if (transform.Find("Saves/SavesList"))
    //            if (transform.Find("Saves/SavesList/SaveTemplate"))
    //            {
    //                // Get save UI template
    //                GameObject saveUITemplate = transform.Find("Saves/SavesList/SaveTemplate").gameObject;
    //                // Get parent for new saves UI
    //                Transform savesParentTr = transform.Find("Saves/SavesList/Grid");
    //                // create entry in UI for each *.save file, if it does not exist
    //                GameObject newSave;
    //                for (int i = 0; i < files.Length; i++)
    //                //foreach (FileInfo file in files)
    //                {
    //                    // create save UI
    //                    newSave = Instantiate(saveUITemplate, savesParentTr);
    //                    // read and set save data
    //                    newSave.GetComponent<Save>().SetSaveInfo(files[i]);
    //                    // enable save
    //                    newSave.gameObject.SetActive(true);
    //                    // verify if it is time to wait
    //                    if (i % numberOfSavesToLoadAtOneTime == 0)
    //                    {
    //                        // skip to next frame
    //                        yield return null;
    //                        //yield return new WaitForSeconds(2);
    //                    }
    //                }
    //                // preselect first save in the list on the next frame
    //                StartCoroutine(SelectLastestSave());
    //            }
    //    yield return null;
    //}

    void SetButtonsActive(bool doActivate)
    {
        loadButton.SetActive(doActivate);
        deleteButton.SetActive(doActivate);
        backButton.SetActive(doActivate);
    }

    void OnEnable()
    {
        // update list of saves on a next frame
        StartCoroutine(savesMenu.SetListOfSaves());
        // enable buttons
        SetButtonsActive(true);
    }

    void OnDisable()
    {
        //// check if load button is not destroyed yet
        //if (transform.Find("LoadBtn"))
        //{
        //    // return load button to normal state
        //    transform.Find("LoadBtn").GetComponent<TextButton>().SetNormalStatus();
        //}
        //// check if objects are not destroyed yet
        //if (transform.Find("Saves"))
        //    if (transform.Find("Saves/SavesList"))
        //        if (transform.Find("Saves/SavesList/Grid"))
        //        {
        //            // Clean up current list of saves
        //            foreach (Save save in transform.Find("Saves/SavesList/Grid").GetComponentsInChildren<Save>())
        //            {
        //                Destroy(save.gameObject);
        //            }
        //        }
        // disable buttons
        SetButtonsActive(false);
    }

    public void CreateGamePlayers(PlayerData[] playersData)
    {
        Debug.Log("Create Game Players");
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Create players
        foreach (PlayerData playerData in playersData)
        {
            ObjectsManager.Instance.CreatePlayer(playerData);
        }
    }

    //public void RemoveAllPlayers()
    //{
    //    // Get objects manager
    //    // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
    //    // remove players
    //    //foreach (GamePlayer gamePlayer in ObjectsManager.Instance.GetGamePlayers())
    //    //{
    //    //    ObjectsManager.Instance.RemovePlayer(gamePlayer);
    //    //}
    //    //Debug.Log("All Players Removed");
    //    ObjectsManager.Instance.RemoveAllCities();
    //}

    //public void RemoveAllCities()
    //{
    //    // Get objects manager
    //    // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
    //    // Remove cities
    //    //foreach (City city in ObjectsManager.Instance.transform.Find("Map/Cities").GetComponentsInChildren<City>(true))
    //    //{
    //    //    ObjectsManager.Instance.RemoveCity(city);
    //    //}
    //    //Debug.Log("All cities removed");
    //    //Debug.Break();
    //    ObjectsManager.Instance.RemoveAllCities();
    //}

    //public void RemoveAllParties()
    //{
    //    // Get objects manager
    //    // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
    //    // Remove all parties
    //    //foreach (HeroParty heroParty in transform.root.GetComponentsInChildren<HeroParty>(true))
    //    //{
    //    //    ObjectsManager.Instance.RemoveHeroParty(heroParty);
    //    //}
    //    //Debug.LogWarning("All parties removed");
    //    //Debug.Break();
    //    ObjectsManager.Instance.RemoveAllParties();
    //}

    //public void RemoveAllInventoryItems()
    //{
    //    // Get objects manager
    //    // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
    //    // Remove all items on map, other items will be removed automatically with respective parties or units
    //    ObjectsManager.Instance.RemoveAllInventoryItemsOnMap();
    //}

    public void CreateCities(CityData[] citiesData)
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Create cities
        foreach (CityData cityData in citiesData)
        {
            ObjectsManager.Instance.CreateCity(cityData);
        }
    }

    public void CreateParties(PartyData[] partiesData)
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Create parties
        foreach (PartyData partyData in partiesData)
        {
            ObjectsManager.Instance.CreateHeroParty(partyData);
        }
    }

    void SetTurnsManager(GameData gameData)
    {
        // Get turns manager
        // TurnsManager turnsManager = transform.root.Find("Managers").GetComponent<TurnsManager>();
        // Set data
        TurnsManager.Instance.TurnsData = gameData.turnsData;
        // Update turns number in UI
        TurnsManager.Instance.UpdateTurnNumberText();
        //// update active player name
        //UIRoot.Instance.GetComponentInChildren<MapMenuManager>(true).UpdateActivePlayerNameOnMapUI();
    }

    public IEnumerator CleanNewWorldBeforeLoad()
    {
        // Activate Map Manager - it is needed to manipulate with map objects
        ChapterManager.Instance.ActiveChapter.GetComponentInChildren<MapManager>(true).gameObject.SetActive(true);
        // Freeze map (during Animation internal updates are not done)
        MapManager.Instance.SetMode(MapManager.Mode.Animation);
        // Note order is imporant, because some parties are children of cities
        ObjectsManager.Instance.RemoveAllInventoryItemsOnMap(); // all other items will be removed because they are hierarchically inside of the party or city or hero
        ObjectsManager.Instance.RemoveAllParties();
        ObjectsManager.Instance.RemoveAllCities();
        ObjectsManager.Instance.RemoveAllPlayers();
        // wait for guaranteed next update, in case of performance issues
        yield return new WaitForFixedUpdate();
        // Wait for all animations to finish
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Game has been cleaned");
    }

    void CreateInventoryItemsOnMap(MapData mapData)
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Init map items container
        MapItemsContainer mapItemsContainer = null;
        // Init position on map variable
        // PositionOnMap previousItemPositionOnMap = new PositionOnMap();
        // init map coordinates variable
        MapCoordinates previousItemMapCoordinates = new MapCoordinates();
        // Get game map transform
        GameMap gameMap = ObjectsManager.Instance.GetComponentInChildren<GameMap>();
        // Create parties
        for (int i = 0; i < mapData.itemsOnMap.Count; i++)
        {
            // verify if do not have already container on map for this item
            if ((mapItemsContainer == null)
                // verify if item is not using same container by comparing current position with previous
                // all items which has identical position on map are placed into the same container
                // || (previousItemPositionOnMap != mapData.itemsPositionOnMap[i]))
                || (previousItemMapCoordinates.x != mapData.itemsMapCoordinates[i].x)
                || (previousItemMapCoordinates.y != mapData.itemsMapCoordinates[i].y))
            {
                // create item container
                //mapItemsContainer = ObjectsManager.Instance.CreateInventoryItemContainerOnMap(mapData.itemsPositionOnMap[i]);
                mapItemsContainer = ObjectsManager.Instance.CreateInventoryItemContainerOnMap(mapData.itemsMapCoordinates[i]);
                // save previous position
                //previousItemPositionOnMap = mapData.itemsPositionOnMap[i];
                previousItemMapCoordinates = mapData.itemsMapCoordinates[i];
            }
            // create item
            InventoryItem inventoryItem = ObjectsManager.Instance.CreateInventoryItem(mapData.itemsOnMap[i], gameMap.transform);
            // link item to container
            mapItemsContainer.LInventoryItems.Add(inventoryItem);
        }
    }

    IEnumerator CreateGameObjects(GameData gameData)
    {
        Debug.Log("Create Game Objects");
        // Note order is important, if some party was child of a city, then city should be created first
        // Update game with data from save
        CreateGamePlayers(gameData.playersData);
        // Set Turns manager data
        SetTurnsManager(gameData);
        // Create items on map
        CreateInventoryItemsOnMap(gameData.mapData);
        // Update game with data from save
        CreateCities(gameData.citiesData);
        // Update game with data from save
        CreateParties(gameData.partiesData);
        // wait for guaranteed next updated, in case of performance issues
        yield return new WaitForFixedUpdate();
        // Wait for all animations to finish
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator ActivateScreens()
    {
        yield return null;
        Debug.Log("Activate Screens");
        // Activate map screen
        // MapManager should be already active
        // MapManager.Instance.gameObject.SetActive(true);
        UIRoot.Instance.GetComponentInChildren<MapMenuManager>(true).gameObject.SetActive(true);
        // UnFreeze map (during Animation internal updates are not done)
        MapManager.Instance.SetMode(MapManager.Mode.Browse);
        // Activate main menu panel, so it is visible next time main menu is activated
        transform.parent.Find("MainMenuPanel").gameObject.SetActive(true);
        // Deactivate main menu
        transform.root.Find("MainMenu").gameObject.SetActive(false);
        // trigger main menu changes related to running game state
        menuButton.OnGameStartMenuChanges();
        // execute pre-turn actions for MapMenu
        MapMenuManager.Instance.ExecutePreTurnActions(TurnsManager.Instance.GetActivePlayer());
        // execute pre-turn actions for MapManager
        MapManager.Instance.ExecutePreTurnActions();
        // Deactivate this screen
        gameObject.SetActive(false);
        // Bring loading screen to front
        loadingScreen.transform.SetAsLastSibling();
        // Wait a bit
        yield return new WaitForSeconds(loadGameConfig.loadingScreenExplicitDelaySeconds);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate Loading screen
        loadingScreen.SetActive(false);
    }

    //IEnumerator FreezeMap()
    //{
    //    // Freeze map (during Animation internal updates are not done)
    //    MapManager.Instance.SetMode(MapManager.Mode.Animation);
    //    yield return null;
    //}

    IEnumerator LoadChapterWorldFromTemplate(GameData gameData)
    {
        ChapterManager.Instance.LoadChapter(gameData.chapterData.chapterName);
        yield return null;
    }

    IEnumerator RemoveCurrentWorld()
    {
        // skip first frame
        yield return null;
        World.Instance.RemoveCurrentChapter();
        yield return null;
    }

    void LoadGameData()
    {
        // open file stream for read
        Debug.Log("Loading game data from " + fullFilePath + " file");
        FileStream file = File.OpenRead(fullFilePath);
        // Create binary formater
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        // Deserialize game data
        GameData gameData = (GameData)binaryFormatter.Deserialize(file);
        // Close file
        file.Close();
        // Activate Loading screen
        loadingScreen.SetActive(true, loadGameConfig.loadingGameTextString);
        // Block mouse input
        InputBlocker.Instance.SetActive(true);
        // We use coroutine to make sure that all objects are removed before new objects are created and to show some animation
        // remove current world
        ChapterManager.Instance.CoroutineQueue.Run(RemoveCurrentWorld());
        // load chapter world from template
        ChapterManager.Instance.CoroutineQueue.Run(LoadChapterWorldFromTemplate(gameData));
        // remove old data
        ChapterManager.Instance.CoroutineQueue.Run(CleanNewWorldBeforeLoad());
        // create new objects from saved data
        ChapterManager.Instance.CoroutineQueue.Run(CreateGameObjects(gameData));
        // activate screens
        ChapterManager.Instance.CoroutineQueue.Run(ActivateScreens());
    }

    void OnLoadSaveYesConfirmation()
    {
        Debug.Log("Yes");
        // Load save
        // Load game data from file
        LoadGameData();
    }

    void OnLoadSaveNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    void DeleteSaveUI(TextToggle selectedToggle)
    {
        // remove current toggle
        Destroy(selectedToggle.gameObject);
        // select other save on next frame if there is any present
        StartCoroutine(savesMenu.SelectLastestSave());
    }

    void OnDeleteSaveYesConfirmation()
    {
        Debug.Log("Yes");
        // Delete file
        File.Delete(fullFilePath);
        // Delete UI entry
        DeleteSaveUI(selectedToggle);
    }

    void OnDeleteSaveNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    public void DeleteSave()
    {
        Debug.Log("Delete save");
        // querry toggle group for currently selected toggle
        selectedToggle = savesMenu.GetComponent<TextToggleGroup>().GetSelectedToggle();
        // verify if there is any toggle selected now
        if (selectedToggle == null)
        {
            // no any save available
            // show error message
            string errMsg = "Error: no any save.";
            NotificationPopUp.Instance().DisplayMessage(errMsg);
        }
        else
        {
            // toggle is selected
            //  get file name from selected save
            string fileName = selectedToggle.GetComponent<Save>().SaveName;
            // verify if file name is set
            if (fileName == "")
            {
                // file name is empty
                string errMsg = "Error: file name is empty.";
                NotificationPopUp.Instance().DisplayMessage(errMsg);
            }
            else
            {
                //  construct full file name
                fullFilePath = ConfigManager.Instance.GameSaveConfig.GetSaveFullNameByFileName(fileName); // Application.persistentDataPath + "/" + fileName + fileExtension;
                Debug.Log("File name is " + fullFilePath + "");
                // verify if file exists
                if (File.Exists(fullFilePath))
                {
                    // file exists
                    // Ask user whether he wants to delete save
                    ConfirmationPopUp confirmationPopUp = ConfirmationPopUp.Instance();
                    // set actions
                    UnityAction YesAction = new UnityAction(OnDeleteSaveYesConfirmation);
                    UnityAction NoAction = new UnityAction(OnDeleteSaveNoConfirmation);
                    // set message
                    string confirmationMessage = "Do you want to delete '" + selectedToggle.name + "' save?";
                    // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
                    confirmationPopUp.Choice(confirmationMessage, YesAction, NoAction);
                }
                else
                {
                    // display error message
                    string errMsg = "Error: [" + fullFilePath + "] file not found. Please try to exit 'Load' menu and open it again.";
                    NotificationPopUp.Instance().DisplayMessage(errMsg);
                    // remove UI element
                    Destroy(selectedToggle.gameObject);
                }
            }
        }
    }

    public void Load()
    {
        Debug.Log("Load save");
        // querry toggle group for currently selected toggle
        TextToggle selectedToggle = savesMenu.GetComponent<TextToggleGroup>().GetSelectedToggle();
        // verify if there is any toggle selected now
        if (selectedToggle == null)
        {
            // no any save available
            // show error message
            string errMsg = "Error: no any save.";
            NotificationPopUp.Instance().DisplayMessage(errMsg);
        }
        else
        {
            // toggle is selected
            //  get file name from selected save
            string fileName = selectedToggle.GetComponent<Save>().SaveName;
            // verify if file name is set
            if (fileName == "")
            {
                // file name is empty
                string errMsg = "Error: file name is empty.";
                NotificationPopUp.Instance().DisplayMessage(errMsg);
            }
            else
            {
                //  construct full file name
                fullFilePath = ConfigManager.Instance.GameSaveConfig.GetSaveFullNameByFileName(fileName); // Application.persistentDataPath + "/" + fileName + fileExtension;
                //Debug.Log("File name is " + fullFilePath + "");
                // verify if file exists
                if (File.Exists(fullFilePath))
                {
                    // file exists
                    // verify if game is already running
                    // check if there is a Chapter in World
                    if (World.Instance.GetComponentInChildren<Chapter>(true) != null)
                    {
                        // game is already running
                        // Ask user whether he wants to load new game
                        ConfirmationPopUp confirmationPopUp = ConfirmationPopUp.Instance();
                        // set actions
                        UnityAction YesAction = new UnityAction(OnLoadSaveYesConfirmation);
                        UnityAction NoAction = new UnityAction(OnLoadSaveNoConfirmation);
                        // set message
                        string confirmationMessage = "Do you want to terminate current game and load saved game? Not saved progress will be lost.";
                        // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
                        confirmationPopUp.Choice(confirmationMessage, YesAction, NoAction);
                    }
                    else
                    {
                        // game is not running yet
                        // Load game data
                        LoadGameData();
                    }
                }
                else
                {
                    string errMsg = "Error: [" + fullFilePath + "] file not found. Please verify if file is present on disk drive.";
                    NotificationPopUp.Instance().DisplayMessage(errMsg);
                }
            }
        }
    }

}
