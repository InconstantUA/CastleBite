using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSaveConfig", menuName = "Config/Game/SaveConfig")]
public class GameSaveConfig : ScriptableObject
{
    public string saveFileExtension = ".save";
    public string datetimeFormat = "yyyyMMddHHmmss";
    public string autoSaveFileName = "Autosave_<datetimeFormat>";
    public int doAutoSave = 1; // 0 disabled, 1 enabled
    public int lastAutoSavesToKeep = 10;
    // number of saves to load at one time to the saves list
    public int numberOfSavesToLoadAtOneTime = 10;

    public string GetSaveFullNameByFileName(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName + saveFileExtension;
    }

    public string GetAutoSaveFullFileName()
    {
        // get date and time with required format
        string datetimeString = DateTime.Now.ToString(datetimeFormat);
        // get file name with date
        string fileName = autoSaveFileName.Replace("<datetimeFormat>", datetimeString);
        // return full file name
        return GetSaveFullNameByFileName(fileName);
    }

    FileInfo[] GetSavesFiles()
    {
        return new DirectoryInfo(Application.persistentDataPath).GetFiles("*" + saveFileExtension);
    }

    public FileInfo[] GetSavesFilesSortedYoungerToOlder()
    {
        // get list of .save files in directory
        FileInfo[] files = GetSavesFiles();
        // sort them by modify date jonger[0] -> older[size-1]
        Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
        {
            return f2.LastWriteTime.CompareTo(f1.LastWriteTime);
        });
        // return result
        return files;
    }

    //public FileInfo[] GetSavesFilesSortedYoungerToOlder()
    //{
    //    // get list of .save files in directory
    //    FileInfo[] files = GetSavesFiles();
    //    // sort them by modify date older -> jonger
    //    Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
    //    {
    //        return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
    //    });
    //    // return result
    //    return files;
    //}

    // placed this function here 
    public SaveInfo GetSaveInfo(FileInfo file)
    {
        SaveInfo saveInfo = new SaveInfo();
        // check if file exist
        if (file.Exists)
        {
            // read data from a file
            // set save name in Save object to the name of the file without extension
            saveInfo.saveName = file.Name.Replace(saveFileExtension, "");
            // set date and time
            saveInfo.date = file.LastWriteTime;
            // Create binary formater
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // open file stream for read
            FileStream fileStream = File.OpenRead(file.FullName);
            // Get and set game data
            try
            {
                saveInfo.gameData = (GameData)binaryFormatter.Deserialize(fileStream);
                // set save is not corrupted flag
                saveInfo.isCorrupted = false;
            }
            catch
            {
                Debug.LogWarning("Failed to read save data from " + file.Name + " save file.");
                // set save is corrupted flag
                saveInfo.isCorrupted = true;
                // nullify game data
                saveInfo.gameData = null;
            }
            // close file
            fileStream.Close();
        }
        else
        {
            Debug.LogError("Attemt to process file which does not exist");
        }
        // return result
        return saveInfo;
    }

}
