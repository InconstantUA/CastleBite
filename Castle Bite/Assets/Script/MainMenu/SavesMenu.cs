using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenu : MonoBehaviour {
    public enum Mode { Save, Load };
    [SerializeField]
    Mode mode;
    GameObject selectedSave;

    void OnDisable()
    {
        DeselectSave();
    }

    public void SetSaveDetails(GameObject save = null)
    {
        // init save data
        SaveData saveData;
        // get save data
        if (save != null)
        {
            saveData = save.GetComponent<Save>().SaveData;
        }
        else
        {
            // Get game players
            GamePlayer[] players = transform.root.Find("GamePlayers").GetComponentsInChildren<GamePlayer>();
            // set save data from currently running game
            saveData = new SaveData
            {
                saveName = "",
                date = DateTime.Now,
                turnNumber = 0,
                playersData = new PlayerData[players.Length]
            };
            // Get and save players data
            for (int i = 0; i < players.Length; i++)
            {
                saveData.playersData[i] = players[i].PlayerData;
            }
        }
        // update UI
        // set save details  transform
        Transform saveDetails = transform.parent.Find("SaveDetails");
        // activate info
        saveDetails.Find("Info").gameObject.SetActive(true);
        // set turn nuber
        saveDetails.Find("Info/Turn/Value").GetComponent<Text>().text = saveData.turnNumber.ToString();
        // set date string
        saveDetails.Find("Info/Date/Value").GetComponent<Text>().text = saveData.date.ToLocalTime().ToShortDateString() + " " + saveData.date.ToLocalTime().ToShortTimeString();
        // set player information
        // get players info root UI
        Transform playersInfoRoot = saveDetails.Find("Info/Players/List");
        // remove old information
        foreach(Transform child in playersInfoRoot)
        {
            Destroy(child.gameObject);
        }
        // get player info template
        GameObject playerInfoTemplateUI = transform.root.Find("Templates/UI/Menu/PlayerInfoTemplate").gameObject;
        foreach (PlayerData playerData in saveData.playersData)
        {
            // clone template
            GameObject newPlayerInfo = Instantiate(playerInfoTemplateUI, playersInfoRoot);
            // activate it
            newPlayerInfo.SetActive(true);
            // set player name
            newPlayerInfo.transform.Find("Name").GetComponent<Text>().text = playerData.givenName;
            newPlayerInfo.transform.Find("Faction").GetComponent<Text>().text = playerData.faction.ToString();
        }
    }

    void SelectSave(GameObject save)
    {
        // save selected save
        selectedSave = save;
        // verify if what mode we operate Save or Load game
        if (mode == Mode.Save)
        {
            // change save name for new save
            transform.Find("NewSave/InputField").GetComponent<InputField>().text = save.GetComponent<Save>().SaveName;
            // update save details
            SetSaveDetails(save);
        }
        else if (mode == Mode.Load)
        {
            // update save details
            SetSaveDetails(save);
        }
    }

    public void DeselectSave()
    {
        // clean up selectedSave
        selectedSave = null;
        // verify if what mode we operate Save or Load game
        if (mode == Mode.Save)
        {
            // clean save name for new save
            transform.Find("NewSave/InputField").GetComponent<InputField>().text = "";
            // set current save (which is about to be saved) details
            SetSaveDetails();
        }
        else if (mode == Mode.Load)
        {
            // clean up save details by deactivating info UI
            transform.parent.Find("SaveDetails/Info").gameObject.SetActive(false);
        }
    }

    public void SetSelectedSave(GameObject value)
    {
        // verify if we already have any save selected
        if (selectedSave != null)
        {
            // deleselect prevoius save
            DeselectSave();
        }
        // select new save
        SelectSave(value);
    }
}
