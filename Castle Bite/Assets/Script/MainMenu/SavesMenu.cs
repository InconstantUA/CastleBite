using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenu : MonoBehaviour {
    public enum Mode { Save, Load };
    [SerializeField]
    Mode mode;
    [SerializeField]
    Save saveUITemplate;
    [SerializeField]
    Transform savesListTransform;
    [SerializeField]
    TextButton loadButton;

    //GameObject selectedSave;

    void OnEnable()
    {
        // get toggle group
        TextToggleGroup toggleGroup = GetComponent<TextToggleGroup>();
        // register function when Save toggle is deselected
        toggleGroup.OnToggleTurnOff.AddListener(DeselectSave);
        // register function when Save toggle is selected
        toggleGroup.OnToggleTurnOn.AddListener(SetSelectedSave);
    }

    void OnDisable()
    {
        // remove all listeners
        // get toggle group
        TextToggleGroup toggleGroup = GetComponent<TextToggleGroup>();
        // register function when Save toggle is deselected
        toggleGroup.OnToggleTurnOff.RemoveAllListeners();
        // register function when Save toggle is selected
        toggleGroup.OnToggleTurnOn.RemoveAllListeners();
        // Clean up current list of saves
        foreach (Save save in savesListTransform.GetComponentsInChildren<Save>())
        {
            Destroy(save.gameObject);
        }
    }

    public void SetSaveDetails(Save save = null)
    {
        // init save data
        SaveInfo saveInfo;
        // get save data
        if (save != null)
        {
            saveInfo = save.SaveInfo;
        }
        else
        {
            // Get game players information from running game
            //GamePlayer[] players = ObjectsManager.Instance.GetGamePlayers();
            // set save data from currently running game
            saveInfo = new SaveInfo
            {
                saveName = "",
                date = DateTime.Now,
                gameData = GetComponentInParent<SaveGame>().GetGameData(),
                isCorrupted = false
            };
        }
        // update UI
        // set save details  transform
        Transform saveDetails = transform.parent.Find("SaveDetails");
        // verify if save is not corrupted
        if (!saveInfo.isCorrupted)
        {
            // activate info
            saveDetails.Find("Info").gameObject.SetActive(true);
            // set chapter name
            saveDetails.Find("Info/Chapter/Value").GetComponent<Text>().text = saveInfo.gameData.chapterData.chapterDisplayName;
            // set turn nuber
            saveDetails.Find("Info/Turn/Value").GetComponent<Text>().text = saveInfo.gameData.turnsData.turnNumber.ToString();
            // set date string
            saveDetails.Find("Info/Date/Value").GetComponent<Text>().text = saveInfo.date.ToLocalTime().ToShortDateString() + " " + saveInfo.date.ToLocalTime().ToShortTimeString();
            // set player information
            // get players info root UI
            Transform playersInfoRoot = saveDetails.Find("Info/Players/List");
            // remove old information
            foreach (Transform child in playersInfoRoot)
            {
                Destroy(child.gameObject);
            }
            // get player info template
            GameObject playerInfoTemplateUI = transform.root.Find("Templates/UI/Menu/PlayerInfoTemplate").gameObject;
            foreach (PlayerData playerData in saveInfo.gameData.playersData)
            {
                // clone template
                GameObject newPlayerInfo = Instantiate(playerInfoTemplateUI, playersInfoRoot);
                // activate it
                newPlayerInfo.SetActive(true);
                // set player name
                newPlayerInfo.transform.Find("Name").GetComponent<Text>().text = playerData.givenName;
                newPlayerInfo.transform.Find("Faction").GetComponent<Text>().text = playerData.faction.ToString();
            }
            // verify if mode is load
            if (mode == Mode.Load)
            {
                // verify if load button is disabled
                if (!loadButton.interactable)
                {
                    // enable load button
                    loadButton.SetInteractable(true);
                }
            }
        }
        else
        {
            // verify if mode is load
            if (mode == Mode.Load)
            {
                // verify if load button is enabled
                if (loadButton.interactable)
                {
                    // disable load button
                    loadButton.SetInteractable(false);
                }
            }
        }
    }

    void SelectSave(Save save)
    {
        // save selected save
        //selectedSave = saveToggle;
        // verify if what mode we operate Save or Load game
        if (mode == Mode.Save)
        {
            // change save name for new save
            transform.Find("NewSave/InputField").GetComponent<InputField>().text = save.SaveName;
            // update save details
            SetSaveDetails(save);
        }
        else if (mode == Mode.Load)
        {
            // update save details
            SetSaveDetails(save);
        }
    }

    void DeselectSave()
    {
        // clean up selectedSave
        //selectedSave = null;
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

    void SetSelectedSave(TextToggle saveToggle)
    {
        //// verify if we already have any save selected
        //if (selectedSave != null)
        //{
        //    // deleselect prevoius save
        //    DeselectSave();
        //}
        // select new save
        SelectSave(saveToggle.GetComponent<Save>());
    }

    public IEnumerator SelectLastestSave()
    {
        Debug.Log("Select latest save");
        yield return new WaitForSeconds(0.01f);
        foreach (Save save in savesListTransform.GetComponentsInChildren<Save>())
        {
            // verify if save is not corrupted
            if (!save.SaveInfo.isCorrupted)
            {
                Debug.Log("Latest save is " + save.name);
                // select first save and exit loop
                save.GetComponent<TextToggle>().ActOnLeftMouseClick();
                break;
            }
        }
    }

    public IEnumerator SetListOfSaves()
    {
        // get list of .save files in directory
        FileInfo[] files = ConfigManager.Instance.GameSaveConfig.GetSavesFilesSortedYoungerToOlder();
        // init new save variable to reuse it later in a for loop
        Save newSave;
        // create entry in UI for each *.save file, if it does not exist
        for (int i = 0; i < files.Length; i++)
        {
            // create new save
            newSave = Instantiate(saveUITemplate.gameObject, savesListTransform).GetComponent<Save>();
            // try to read and set save data
            newSave.SaveInfo = ConfigManager.Instance.GameSaveConfig.GetSaveInfo(files[i]);
            // enable save
            newSave.gameObject.SetActive(true);
            // set toggle group to this menu toggle group
            newSave.GetComponent<TextToggle>().toggleGroup = GetComponent<TextToggleGroup>();
            // verify if it is time to wait
            if (i % ConfigManager.Instance.GameSaveConfig.numberOfSavesToLoadAtOneTime == 0)
            {
                // skip to next frame
                yield return null;
            }
        }
        // verify if it is in Load mode
        if (mode == Mode.Load)
        {
            StartCoroutine(SelectLastestSave());
        }
        yield return null;
    }
}
