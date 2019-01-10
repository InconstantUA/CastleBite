using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveGame : MonoBehaviour {
    //[SerializeField]
    //string fileExtension;
    string fullFilePath;
    [SerializeField]
    GameObject saveButton;
    [SerializeField]
    GameObject backButton;
    [SerializeField]
    SavesMenu savesMenu;
    [SerializeField]
    InputField saveNameInputField;

    void SetButtonsActive(bool doActivate)
    {
        saveButton.SetActive(doActivate);
        backButton.SetActive(doActivate);
    }

    void OnEnable()
    {
        // update list of saves
        StartCoroutine(savesMenu.SetListOfSaves());
        // set save details
        transform.Find("Saves").GetComponent<SavesMenu>().SetSaveDetails();
        // enable buttons
        SetButtonsActive(true);
    }

    void OnDisable()
    {
        //// return save button to normal state
        //transform.Find("SaveBtn").GetComponent<TextButton>().SetNormalStatus();
        //// Clean up current list of saves
        //foreach (Save save in transform.Find("Saves/SavesList/Grid").GetComponentsInChildren<Save>())
        //{
        //    Destroy(save.gameObject);
        //}
        // disable buttons
        SetButtonsActive(false);
    }

    void PrepareCitiesForSave(City[] cities)
    {
        foreach (City city in cities)
        {
            // save city ID (it is used during restore to map hero parties to cities, where they were located)
            //city.CityID = city.gameObject.GetInstanceID();
            // save city position
            //city.CityData.cityMapPosition = city.GetCityMapPosition();
            city.CityData.cityMapCoordinates = MapManager.Instance.GetCoordinatesByWorldPosition(city.LMapCity.transform.position);
        }
    }

    void PrepareHeroPartiesForSave(HeroParty[] heroParties)
    {
        foreach (HeroParty heroParty in heroParties)
        {
            // set party ID
            heroParty.PartyData.partyID = heroParty.gameObject.GetInstanceID();
            // verify if party is linked to the city
            if (heroParty.transform.parent.GetComponent<City>() != null)
            {
                // party is linked to a city
                // set linked city ID
                heroParty.PartyData.linkedCityID = heroParty.transform.parent.GetComponent<City>().CityID;
                // set UI address to null, because it is not needed any more, if party is linked to the city
                heroParty.PartyData.partyUIAddress = null;
            }
            else
            {
                // party is not linked to a city
                // reset linked city ID
                heroParty.PartyData.linkedCityID = CityID.None;
                // get party UI address
                string partyUIAddress = heroParty.GetPartyUIAddress();
                // set party UI address
                heroParty.PartyData.partyUIAddress = partyUIAddress;
                Debug.Log(heroParty.name + " party UI address is " + heroParty.PartyData.partyUIAddress);
            }
            // set party map address
            //heroParty.PartyData.partyMapPosition = heroParty.GetPartyMapPosition();
            heroParty.PartyData.partyMapCoordinates = heroParty.GetPartyMapCoordinates();
            // init party inventory data
            heroParty.PartyData.partyInventory = new List<InventoryItemData>();
            // loop through all items in the party one level below (items on deeper levels belong to units)
            foreach (Transform childTransform in heroParty.transform)
            {
                // verify if it is item
                if (childTransform.GetComponent<InventoryItem>() != null)
                {
                    // add item data to the list
                    heroParty.PartyData.partyInventory.Add(childTransform.GetComponent<InventoryItem>().InventoryItemData);
                }
            }
            // get all units in party
            PartyUnit[] partyUnits = heroParty.GetComponentsInChildren<PartyUnit>(true);
            // init party units data
            heroParty.PartyData.partyUnitsData = new PartyUnitData[partyUnits.Length];
            // foreach party unit
            for (int i = 0; i < partyUnits.Length; i++)
            {
                // init unit's inventory
                partyUnits[i].PartyUnitData.unitIventory = new List<InventoryItemData>();
                // loop though all items owned by unit
                foreach (InventoryItem inventoryItem in partyUnits[i].GetComponentsInChildren<InventoryItem>())
                {
                    // add item's data to the list
                    partyUnits[i].PartyUnitData.unitIventory.Add(inventoryItem.InventoryItemData);
                }
                // set party units data
                heroParty.PartyData.partyUnitsData[i] = partyUnits[i].PartyUnitData;
            }
        }
    }

    public GameData GetGameData(bool isAutoSave = false)
    {
        Debug.Log("Get game data");
        // Get game players
        GamePlayer[] players = ObjectsManager.Instance.GetGamePlayers();
        // Get game map
        GameMap gameMap = ObjectsManager.Instance.GetComponentInChildren<GameMap>();
        // Get cities
        City[] cities = gameMap.transform.Find("Cities").GetComponentsInChildren<City>();
        // Get hero parties
        HeroParty[] heroParties = gameMap.GetComponentsInChildren<HeroParty>();
        // init game data
        GameData gameData = new GameData
        {
            saveData = new SaveData(),
            chapterData = new ChapterData(),
            turnsData = new TurnsData(),
            playersData = new PlayerData[players.Length],
            mapData = new MapData(),
            citiesData = new CityData[cities.Length],
            partiesData = new PartyData[heroParties.Length]
        };
        // Set save Data (set whether this is automatically generated save file)
        gameData.saveData.isAutosave = isAutoSave;
        // Set chapter data
        gameData.chapterData = ChapterManager.Instance.ActiveChapter.ChapterData;
        // Set turns manager data
        gameData.turnsData = TurnsManager.Instance.TurnsData;
        // Prepare map data
        gameMap.SetItemsOnMapData();
        // Get and write Map data
        gameData.mapData = gameMap.MapData;
        // Get and write to gameData players data
        for (int i = 0; i < players.Length; i++)
        {
            // copy player data
            gameData.playersData[i] = players[i].PlayerData;
        }
        // Prepare city data from the save
        PrepareCitiesForSave(cities);
        // Get and write to gameData cities data
        for (int i = 0; i < cities.Length; i++)
        {
            gameData.citiesData[i] = cities[i].CityData;
        }
        // Set parties data - there are some values which are initialized only during save
        PrepareHeroPartiesForSave(heroParties);
        // Get and write to gameData parties data;
        for (int i = 0; i < heroParties.Length; i++)
        {
            gameData.partiesData[i] = heroParties[i].PartyData;
        }
        // return result
        return gameData;
    }

    void SaveGameData(FileStream file, bool isAutoSave = false)
    {
        // Create binary formater
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        // Put data to a file
        binaryFormatter.Serialize(file, GetGameData(isAutoSave));
        // Close file
        file.Close();
        // Activate main menu panel
        transform.parent.Find("MainMenuPanel").gameObject.SetActive(true);
        // Deactivate this screen
        gameObject.SetActive(false);
    }

    void OnOverwriteSaveYesConfirmation()
    {
        Debug.Log("Yes");
        // Overwrite save
        // open file stream for write
        FileStream file = File.OpenWrite(fullFilePath);
        // write to a file and close it
        SaveGameData(file);
    }

    void OnOverwriteSaveNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    void CleanupOldSaves()
    {
        // get list of saves files sorted from youngest[0] to the oldest[size-1]
        FileInfo[] files = ConfigManager.Instance.GameSaveConfig.GetSavesFilesSortedYoungerToOlder();
        // init save to use its GetSaveInfo() function;
        SaveInfo saveInfo;
        // init saves counter
        int savesCount = 0;
        // remove all files, which are out of auto-save limit
        // loop through all files
        for (int i = 0; i < files.Length; i++)
        {
            // get save info
            saveInfo = ConfigManager.Instance.GameSaveConfig.GetSaveInfo(files[i]);
            // verify if save is not corrupted and it is auto-save
            if (!saveInfo.isCorrupted) {
                Debug.Log("Reading isAutosave parameter from " + saveInfo.saveName + " save.");
                // cannot read game data if save is corrupted, that is why we verify it only after we know that it is not corrupted
                if (saveInfo.gameData.saveData.isAutosave)
                {
                    // increment saves counter
                    savesCount += 1;
                    // verify savesCount is more than the limit
                    if (savesCount > GameOptions.Instance.gameOpt.LastAutoSavesToKeep)
                    {
                        // remove this save file
                        Debug.Log("Removing old " + saveInfo.saveName + " save.");
                        files[i].Delete();
                    }
                }
            }
        }
    }

    public void AutoSave()
    {
        // get full file name
        fullFilePath = ConfigManager.Instance.GameSaveConfig.GetAutoSaveFullFileName();
        Debug.Log("File name is " + fullFilePath + "");
        // create file
        FileStream file = File.Create(fullFilePath);
        // set is autosave flag (just for code readability)
        bool isAutosave = true;
        // write to a file and close it
        SaveGameData(file, isAutosave);
        // cleanup old saves
        CleanupOldSaves();
    }

    public void Save()
    {
        // Open file
        //  get file name from input field
        string fileName = saveNameInputField.text;
        // verify if file name is set
        if (fileName == "")
        {
            // file name is not set
            // show error message
            string errMsg = "Error: the name of save is not set. Please type new save name or select existing save to overwrite it.";
            NotificationPopUp.Instance().DisplayMessage(errMsg);
        }
        else
        {
            //  construct full file name
            fullFilePath = ConfigManager.Instance.GameSaveConfig.GetSaveFullNameByFileName(fileName);
            Debug.Log("File name is " + fullFilePath + "");
            // verify if file exists
            if (File.Exists(fullFilePath))
            {
                // file exists
                // Ask user whether he wants to overwrite previous save
                ConfirmationPopUp confirmationPopUp = ConfirmationPopUp.Instance();
                // set actions
                UnityAction YesAction = new UnityAction(OnOverwriteSaveYesConfirmation);
                UnityAction NoAction = new UnityAction(OnOverwriteSaveNoConfirmation);
                // set message
                string confirmationMessage = "Do you want to overwrite existing save with new data?";
                // send actions to Confirmation popup, so he knows how to react on no and yes btn presses
                confirmationPopUp.Choice(confirmationMessage, YesAction, NoAction);
            }
            else
            {
                // create file
                FileStream file = File.Create(fullFilePath);
                // write to a file and close it
                SaveGameData(file);
            }
        }
    }
}
