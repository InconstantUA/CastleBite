using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveGame : MonoBehaviour {
    [SerializeField]
    string fileExtension;
    string fullFilePath;

    void OnEnable()
    {
        // update list of saves
        // get list of .save files in directory
        FileInfo[] files = new DirectoryInfo(Application.persistentDataPath).GetFiles("*" + fileExtension);
        // sort them by modify date older -> jonger
        Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
        {
            return f2.LastWriteTime.CompareTo(f1.LastWriteTime);
        });
        // Get save UI template
        GameObject saveUITemplate = transform.Find("Saves/SavesList/Grid/SaveTemplate").gameObject;
        // Get parent for new saves UI
        Transform savesParentTr = transform.Find("Saves/SavesList/Grid");
        // create entry in UI for each *.save file, if it does not exist
        GameObject newSave;
        foreach (FileInfo file in files)
        {
            // create save UI
            newSave = Instantiate(saveUITemplate, savesParentTr);
            // set save name in Save object to the name of the file without extension
            newSave.GetComponent<Save>().saveName = file.Name.Replace(fileExtension, "");
            // rename game object 
            newSave.name = newSave.GetComponent<Save>().saveName;
            // enable save
            newSave.gameObject.SetActive(true);
        }
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

    void SaveGameData(FileStream file)
    {
        // Set game data
        GameData gameData = new GameData
        {
            playerData = transform.root.Find("PlayerObj").GetComponent<PlayerObj>().PlayerData
        };
        // Create binary formater
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        // Put data to a file
        binaryFormatter.Serialize(file, gameData);
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

[Serializable]
class GameData : System.Object
{
    // Map (Scene)
    // Players
    public PlayerData playerData;
    // Cities
    // Parties with units
}