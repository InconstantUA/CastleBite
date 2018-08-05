using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SaveData : System.Object
{
    // Minium save data, which is displayed in the load- and save game UI
    public string saveName;
    public DateTime date;
    // values below actually are read from other datas
    public int turnNumber;
    public PlayerData[] playersData;
}

public class Save : MonoBehaviour {
    [SerializeField]
    string fileExtension;
    SaveData saveData;

    public string SaveName
    {
        get
        {
            return saveData.saveName;
        }
    }

    public SaveData SaveData
    {
        get
        {
            return saveData;
        }
    }

    public void SetSaveData(FileInfo file)
    {
        // init save data
        if (saveData == null)
        {
            saveData = new SaveData();
        }
        // check if file exist
        if (file.Exists)
        {
            // read data from a file
            // set save name in Save object to the name of the file without extension
            saveData.saveName = file.Name.Replace(fileExtension, "");
            // set date and time
            saveData.date = file.LastWriteTime;
            // Create binary formater
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // open file stream for read
            FileStream fileStream = File.OpenRead(file.FullName);
            // Get game data
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            // set turn number
            // .. read turn number from save file
            saveData.turnNumber = 0;
            // init list of players
            saveData.playersData = gameData.playersData;
        }
        else
        {
            Debug.LogError("Attemt to process file which does not exist");
        }

    }

    void OnEnable()
    {
        // set Toggle's text
        // pad right will calcualte number of spaces required to keep constant string length so brakets are always located at the edges
        GetComponent<Text>().text = "[ " + saveData.saveName.PadRight(28) + "]";
    }
}
