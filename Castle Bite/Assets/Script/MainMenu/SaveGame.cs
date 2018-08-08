using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
class GameData : System.Object
{
    // Map (Scene)
    // ..
    // Players
    public PlayerData[] playersData;
    // Cities
    // ..
    // Parties with units
    public PartyData[] partiesData;
}

public class SaveGame : MonoBehaviour {
    [SerializeField]
    string fileExtension;
    string fullFilePath;

    IEnumerator SetListOfSaves()
    {
        // number of saves to load at one time
        int numberOfSavesToLoadAtOneTime = 10;
        // get list of .save files in directory
        FileInfo[] files = new DirectoryInfo(Application.persistentDataPath).GetFiles("*" + fileExtension);
        // sort them by modify date older -> jonger
        Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
        {
            return f2.LastWriteTime.CompareTo(f1.LastWriteTime);
        });
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
        yield return null;
    }

    void OnEnable()
    {
        // update list of saves
        StartCoroutine(SetListOfSaves());
        // set save details
        transform.Find("Saves").GetComponent<SavesMenu>().SetSaveDetails();
    }

    void OnDisable()
    {
        // return save button to normal state
        transform.Find("SaveBtn").GetComponent<TextButton>().SetNormalStatus();
        // Clean up current list of saves
        foreach (Save save in transform.Find("Saves/SavesList/Grid").GetComponentsInChildren<Save>())
        {
            Destroy(save.gameObject);
        }
    }

    void PrepareHeroPartiesForSave(HeroParty[] heroParties)
    {
        foreach (HeroParty heroParty in heroParties)
        {
            // get party UI address
            string partyUIAddress = heroParty.GetPartyUIAddress();
            // verify if this is tempalte
            if (partyUIAddress.Contains("Templates"))
            {
                // skip it
                Debug.Log("Skipping " + heroParty.name + " because party UI address [" + partyUIAddress + "] match Templates");
            }
            else
            {
                // set party map address
                heroParty.PartyData.partyMapPosition = heroParty.GetPartyMapPosition();
                // set party UI address
                heroParty.PartyData.partyUIAddress = partyUIAddress;
                Debug.Log(heroParty.name + " party UI address is " + heroParty.PartyData.partyUIAddress);
                // set party panel data
                heroParty.PartyData.partyPanelData = heroParty.transform.GetComponentInChildren<PartyPanel>(true).PartyPanelData;
                // get all units in party
                PartyUnit[] partyUnits = heroParty.transform.GetComponentInChildren<PartyPanel>(true).GetComponentsInChildren<PartyUnit>(true);
                // init party units data
                heroParty.PartyData.partyPanelData.partyUnitsData = new PartyUnitData[partyUnits.Length];
                // foreach party unit
                for (int i = 0; i < partyUnits.Length; i++)
                {
                    // set party unit cell address
                    // structure: PartyPanel-4[Top/Middle/Bottom]-3[Front/Back/Wide]-2UnitSlot-1UnitCanvas-partyUnit
                    partyUnits[i].PartyUnitData.unitCellAddress = partyUnits[i].GetUnitCellUIAddress();
                    // Debug.Log(partyUnits[i].PartyUnitData.unitName + " unit cell address is " + partyUnits[i].PartyUnitData.unitCellAddress);
                    // set party units data
                    heroParty.PartyData.partyPanelData.partyUnitsData[i] = partyUnits[i].PartyUnitData;
                }
            }
        }
    }

    GameData GetGameData()
    {
        Debug.Log("Get game data");
        // Get game players
        GamePlayer[] players = transform.root.Find("GamePlayers").GetComponentsInChildren<GamePlayer>();
        // Get all parties
        HeroParty[] heroParties = transform.root.GetComponentsInChildren<HeroParty>(true);
        // init game data
        GameData gameData = new GameData
        {
            playersData = new PlayerData[players.Length],
            partiesData = new PartyData[heroParties.Length]
        };
        // Get and write to gameData players data
        for (int i = 0; i < players.Length; i++)
        {
            gameData.playersData[i] = players[i].PlayerData;
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

    void SaveGameData(FileStream file)
    {
        // Create binary formater
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        // Put data to a file
        binaryFormatter.Serialize(file, GetGameData());
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

    public void Save()
    {
        // Open file
        //  get file name from input field
        string fileName = transform.Find("Saves/NewSave/InputField").GetComponent<InputField>().text;
        // verify if file name is set
        if (fileName == "")
        {
            // file name is not set
            // show error message
            string errMsg = "Error: the name of save is not set. Please type new save name or select existing save to overwrite it.";
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
