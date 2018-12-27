using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaveMenu : MonoBehaviour
{
    [SerializeField]
    Text symbol;
    [SerializeField]
    InputField numberOfLatestSavesToKeepInputField;
    [SerializeField]
    Text numberOfLatestSavesToKeepPlaceHolder;

    void OnEnable()
    {
        // Verify if auto-save is enabled in player options
        if (GameOptions.Instance.gameOpt.DoAutoSave >= 1)
        {
            // enable auto-save
            SetActive(true);
        }
        else if (GameOptions.Instance.gameOpt.DoAutoSave <= 0)
        {
            // disable auto-save
            SetActive(false);
        }
    }

    public void SetActive(bool doActivate)
    {
        if (doActivate)
        {
            // activate number of latest saves to keep input field, if it is not active already
            if (!numberOfLatestSavesToKeepInputField.gameObject.activeSelf)
            {
                numberOfLatestSavesToKeepInputField.gameObject.SetActive(true);
            }
            // activate autosave in game options, if it is not active already
            if (GameOptions.Instance.gameOpt.DoAutoSave != 1)
            {
                GameOptions.Instance.gameOpt.DoAutoSave = 1;
            }
            // verify if auto-save option number of last saves to keep is not invalid
            if (GameOptions.Instance.gameOpt.LastAutoSavesToKeep <= 0)
            {
                // set it at least to 1
                GameOptions.Instance.gameOpt.LastAutoSavesToKeep = 1;
            }
            // configure auto-save options based on game options (this normally should not be done if SetActive is called from by the InputField itself, but this doesn't harm, because we do validation)
            if (numberOfLatestSavesToKeepInputField.text != GameOptions.Instance.gameOpt.LastAutoSavesToKeep.ToString())
            {
                numberOfLatestSavesToKeepInputField.text = GameOptions.Instance.gameOpt.LastAutoSavesToKeep.ToString();
            }
            // set symbol visible if it is not active yet
            if (!symbol.gameObject.activeSelf)
            {
                symbol.gameObject.SetActive(true);
            }
            // verify if toggle is not On already (this can be during game start)
            if (!GetComponent<TextToggle>().selected)
            {
                // set toggle On
                GetComponent<TextToggle>().TurnOn();
            }
        }
        else
        {
            // deactivate number of latest saves to keep input field
            numberOfLatestSavesToKeepInputField.gameObject.SetActive(false);
            // dactivate autosave in options, but keep number of saves to keep the same, because it may be required for user to keep it with this number
            GameOptions.Instance.gameOpt.DoAutoSave = 0;
            // hide symbol
            symbol.gameObject.SetActive(false);
            // verify if toggle is not Off already (this can be during game start)
            if (GetComponent<TextToggle>().selected)
            {
                // set toggle Off
                GetComponent<TextToggle>().TurnOff();
            }
        }
    }

    public void ChangeNumberOfLatestSavesToKeep()
    {
        int numberOfLatestSavesToKeep = 1;
        // try to parse text from input field and convert it to integer
        if (Int32.TryParse(numberOfLatestSavesToKeepInputField.text, out numberOfLatestSavesToKeep))
        {
            // parsing attempt was successful
            // get number of last saves to keep
            numberOfLatestSavesToKeep = Int32.Parse(numberOfLatestSavesToKeepInputField.text);
            // save value to the Options
            GameOptions.Instance.gameOpt.LastAutoSavesToKeep = numberOfLatestSavesToKeep;
            // verify if value is equal or greater than 1
            if (numberOfLatestSavesToKeep >= 1)
            {
                // enable auto-save
                SetActive(true);
            }
            else if (numberOfLatestSavesToKeep <= 0)
            {
                // disable auto-save
                SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Input field String to Int Parsing failed");
            // assume that user wants to keep it On, because input field was On and reset it to 1
            GameOptions.Instance.gameOpt.DoAutoSave = 1;
        }
    }
}
