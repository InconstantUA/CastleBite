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
    Transform gamePlayersRoot;
    [SerializeField]
    MenuButton menuButton;
    [SerializeField]
    string fileExtension;
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
    string fullFilePath;
    TextToggle selectedToggle;

    IEnumerator SetListOfSaves()
    {
        int numberOfSavesToLoadAtOneTime = 10;
        // update list of saves
        // get list of .save files in directory
        FileInfo[] files = new DirectoryInfo(Application.persistentDataPath).GetFiles("*" + fileExtension);
        // sort them by modify date older -> jonger
        Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
        {
            return f2.LastWriteTime.CompareTo(f1.LastWriteTime);
        });
        // verify if all object are loaded
        if (transform.Find("Saves"))
            if (transform.Find("Saves/SavesList"))
                if (transform.Find("Saves/SavesList/SaveTemplate"))
                {
                    // Get save UI template
                    GameObject saveUITemplate = transform.Find("Saves/SavesList/SaveTemplate").gameObject;
                    // Get parent for new saves UI
                    Transform savesParentTr = transform.Find("Saves/SavesList/Grid");
                    // create entry in UI for each *.save file, if it does not exist
                    GameObject newSave;
                    for (int i = 0; i < files.Length; i++)
                    //foreach (FileInfo file in files)
                    {
                        // create save UI
                        newSave = Instantiate(saveUITemplate, savesParentTr);
                        // read and set save data
                        newSave.GetComponent<Save>().SetSaveData(files[i]);
                        // rename game object 
                        newSave.name = newSave.GetComponent<Save>().SaveName;
                        // enable save
                        newSave.gameObject.SetActive(true);
                        // verify if it is time to wait
                        if (i % numberOfSavesToLoadAtOneTime == 0)
                        {
                            // skip to next frame
                            yield return null;
                            //yield return new WaitForSeconds(2);
                        }
                    }
                    // preselect first save in the list on the next frame
                    StartCoroutine(SelectLastestSave());
                }
        yield return null;
    }

    void OnEnable()
    {
        // update list of saves on a next frame
        StartCoroutine(SetListOfSaves());
    }

    void OnDisable()
    {
        // check if load button is not destroyed yet
        if (transform.Find("LoadBtn"))
        {
            // return load button to normal state
            transform.Find("LoadBtn").GetComponent<TextButton>().SetNormalStatus();
        }
        // check if objects are not destroyed yet
        if (transform.Find("Saves"))
            if (transform.Find("Saves/SavesList"))
                if (transform.Find("Saves/SavesList/Grid"))
                {
                    // Clean up current list of saves
                    foreach (Save save in transform.Find("Saves/SavesList/Grid").GetComponentsInChildren<Save>())
                    {
                        Destroy(save.gameObject);
                    }
                }
    }

    public void CreateGamePlayers(PlayerData[] playersData)
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Create players
        foreach (PlayerData playerData in playersData)
        {
            ObjectsManager.Instance.CreatePlayer(playerData);
        }
    }

    public void RemoveAllPlayers()
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // remove players
        foreach (GamePlayer gamePlayer in gamePlayersRoot.GetComponentsInChildren<GamePlayer>())
        {
            ObjectsManager.Instance.RemovePlayer(gamePlayer);
        }
    }

    public void RemoveAllCities()
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Remove cities
        foreach (City city in transform.root.Find("Map/Cities").GetComponentsInChildren<City>(true))
        {
            ObjectsManager.Instance.RemoveCity(city);
        }
        Debug.LogWarning("All cities removed");
        //Debug.Break();
    }

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

    public void RemoveAllParties()
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Remove all parties
        foreach (HeroParty heroParty in transform.root.GetComponentsInChildren<HeroParty>(true))
        {
            ObjectsManager.Instance.RemoveHeroParty(heroParty);
        }
        Debug.LogWarning("All parties removed");
        //Debug.Break();
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

    public void RemoveAllInventoryItems()
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Remove all items on map, other items will be removed automatically with respective parties or units
        ObjectsManager.Instance.RemoveAllInventoryItemsOnMap();
    }

    void SetTurnsManager(GameData gameData)
    {
        // Get turns manager
        // TurnsManager turnsManager = transform.root.Find("Managers").GetComponent<TurnsManager>();
        // Set data
        TurnsManager.Instance.TurnsData = gameData.turnsData;
        // Update turns number in UI
        TurnsManager.Instance.UpdateTurnNumberText();
    }

    IEnumerator CleanGameBeforeLoad()
    {
        // Block mouse input
        InputBlocker.Instance.SetActive(true);
        // Note order is imporant, because some parties are children of cities
        RemoveAllInventoryItems();
        RemoveAllParties();
        RemoveAllCities();
        RemoveAllPlayers();
        // wait for guaranteed next updated, in case of performance issues
        yield return new WaitForFixedUpdate();
        // Wait for all animations to finish
        yield return new WaitForSeconds(0.25f);
    }

    void CreateInventoryItemsOnMap(MapData mapData)
    {
        // Get objects manager
        // ObjectsManager objectsManager = transform.root.GetComponentInChildren<ObjectsManager>();
        // Init map items container
        MapItemsContainer mapItemsContainer = null;
        // Init position on map variable
        PositionOnMap previousItemPositionOnMap = new PositionOnMap();
        // Get game map transform
        GameMap gameMap = transform.root.GetComponentInChildren<GameMap>();
        // Create parties
        for (int i = 0; i < mapData.itemsOnMap.Count; i++)
        {
            // verify if do not have already container on map for this item
            if ((mapItemsContainer == null)
                // verify if item is not using same container by comparing current position with previous
                // all items which has identical position on map are placed into the same container
                || (previousItemPositionOnMap != mapData.itemsPositionOnMap[i]))
            {
                // create item container
                mapItemsContainer = ObjectsManager.Instance.CreateInventoryItemContainerOnMap(mapData.itemsPositionOnMap[i]);
                // save previous position
                previousItemPositionOnMap = mapData.itemsPositionOnMap[i];
            }
            // create item
            InventoryItem inventoryItem = ObjectsManager.Instance.CreateInventoryItem(mapData.itemsOnMap[i], gameMap.transform);
            // link item to container
            mapItemsContainer.LInventoryItems.Add(inventoryItem);
        }
    }

    IEnumerator CreateGameObjects(GameData gameData)
    {
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
        // Activate map screen
        MapManager.Instance.gameObject.SetActive(true);
        MapMenuManager.Instance.gameObject.SetActive(true);
        // UnFreeze map (during Animation internal updates are not done)
        MapManager.Instance.SetMode(MapManager.Mode.Browse);
        // Activate main menu panel, so it is visible next time main menu is activated
        transform.parent.Find("MainMenuPanel").gameObject.SetActive(true);
        // Deactivate main menu
        transform.root.Find("MainMenu").gameObject.SetActive(false);
        // trigger main menu changes related to running game state
        menuButton.OnGameStartMenuChanges();
        // Deactivate this screen
        gameObject.SetActive(false);
        // Wait a bit
        yield return new WaitForSeconds(3);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate Loading screen
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<LoadingScreen>().SetActive(false);
    }

    void SetGameData(GameData gameData)
    {
        // Activate Loading screen
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<LoadingScreen>(true).SetActive(true);
        // we use coroutine to make sure that all objects are removed before new objects are created and to show some animation
        // .. Set map
        // Activate map screen - it is needed to create objects
        MapManager.Instance.gameObject.SetActive(true);
        // Freeze map (during Animation internal updates are not done)
        MapManager.Instance.SetMode(MapManager.Mode.Animation);
        // remove old data
        ChapterManager.Instance.CoroutineQueue.Run(CleanGameBeforeLoad());
        // create new objects from saved data
        ChapterManager.Instance.CoroutineQueue.Run(CreateGameObjects(gameData));
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
        // Set game data
        SetGameData(gameData);
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

    IEnumerator SelectLastestSave()
    {
        Debug.Log("Select latest save");
        yield return new WaitForSeconds(0.01f);
        foreach (TextToggle toggle in transform.Find("Saves/SavesList/Grid").GetComponentsInChildren<TextToggle>())
        {
            Debug.Log("Latest save is " + toggle.name);
            // select first save and exit loop
            toggle.ActOnLeftMouseClick();
            break;
        }
    }

    void DeleteSaveUI(TextToggle selectedToggle)
    {
        // remove current toggle
        Destroy(selectedToggle.gameObject);
        // select other save on next frame if there is any present
        StartCoroutine(SelectLastestSave());
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
        selectedToggle = transform.Find("Saves").GetComponent<TextToggleGroup>().GetSelectedToggle();
        // verify if there is any toggle selected now
        if (selectedToggle == null)
        {
            // no any save available
            // show error message
            string errMsg = "Error: no any save.";
            transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
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
                transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
            }
            else
            {
                //  construct full file name
                fullFilePath = Application.persistentDataPath + "/" + fileName + fileExtension;
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
                    transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
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
        TextToggle selectedToggle = transform.Find("Saves").GetComponent<TextToggleGroup>().GetSelectedToggle();
        // verify if there is any toggle selected now
        if (selectedToggle == null)
        {
            // no any save available
            // show error message
            string errMsg = "Error: no any save.";
            transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
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
                transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
            }
            else
            {
                //  construct full file name
                fullFilePath = Application.persistentDataPath + "/" + fileName + fileExtension;
                //Debug.Log("File name is " + fullFilePath + "");
                // verify if file exists
                if (File.Exists(fullFilePath))
                {
                    // file exists
                    // verify if game is already running
                    // check if there are players objects
                    if (gamePlayersRoot.GetComponentsInChildren<GamePlayer>().Length > 0)
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
                    transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
                }
            }
        }
    }

}
