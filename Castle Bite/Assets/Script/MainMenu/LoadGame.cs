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
    string fileExtension;
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

    public void CreateGamePlayers(PlayerData[] players)
    {
        // Get player object Template
        GameObject gamePlayerTemplate = transform.root.Find("Templates/Obj/GamePlayer").gameObject;
        // Get players root
        Transform gamePlayersRoot = transform.root.Find("GamePlayers");
        // Init new player
        GameObject newGamePlayer;
        // Create players
        foreach (PlayerData player in players)
        {
            // instantiate new player
            newGamePlayer = Instantiate(gamePlayerTemplate, gamePlayersRoot);
            // Set player data
            newGamePlayer.GetComponent<GamePlayer>().PlayerData = player;
        }
    }

    public void RemoveAllPlayers()
    {
        foreach (Transform child in transform.root.Find("GamePlayers"))
        {
            Destroy(child.gameObject);
        }
    }

    void SetPlayers(GameData gameData)
    {
        // Remove old data
        RemoveAllPlayers();
        // Update game with data from save
        CreateGamePlayers(gameData.playersData);
    }

    public void RemoveAllParties()
    {
        foreach(HeroParty heroParty in transform.root.GetComponentsInChildren<HeroParty>(true))
        {
            Destroy(heroParty.gameObject);
        }
    }

    public void CreateParties(PartyData[] partiesData)
    {
        // create party
        // place it in required UI address
        // create party panel
        // create units
        // create party on map
    }

    void SetParties(GameData gameData)
    {
        // remove old data
        RemoveAllParties();
        // Update game with data from save
        CreateParties(gameData.partiesData);
    }

    void SetGameData(GameData gameData)
    {
        // .. Set map
        // Remove old and create new players
        SetPlayers(gameData);
        // .. Set cities
        // .. Set Parties
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
        // Activate map screen
        transform.root.Find("MapScreen").gameObject.SetActive(true);
        // Deactivate this screen
        gameObject.SetActive(false);
        // Activate main menu panel, so it is visible next time main menu is activated
        transform.parent.Find("MainMenuPanel").gameObject.SetActive(true);
        // Deactivate main menu
        transform.root.Find("MainMenu").gameObject.SetActive(false);
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
                    string errMsg = "Error: [" + fullFilePath + "] file not found. Please verify if file is present on disk drive.";
                    transform.root.Find("MiscUI/NotificationPopUp").GetComponent<NotificationPopUp>().DisplayMessage(errMsg);
                }
            }
        }
    }

}
