using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveGame : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Save()
    {
        // Open file
        string fileName = "save.dat";
        string fullFilePath = Application.persistentDataPath + "/" + fileName;
        Debug.Log("Save to [" + fullFilePath + "] file");
        FileStream fileStream = File.Open(fullFilePath, FileMode.Open);

        // Fill in game data
        GameData gameData = new GameData();
        gameData.playerObj = transform.root.Find("PlayerObj").GetComponent<PlayerObj>();

        // Create binary formater
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        // Put data to a file
        binaryFormatter.Serialize(fileStream, gameData);

        // Close file
        fileStream.Close();
    }

    public void Load()
    {

    }

}

[Serializable]
class GameData
{
    // Map (Scene)
    // Players
    public PlayerObj playerObj;
    // Cities
    // Parties with units
}