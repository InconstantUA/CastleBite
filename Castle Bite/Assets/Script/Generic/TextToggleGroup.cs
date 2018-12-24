﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToggleGroup : MonoBehaviour {
    TextToggle selectedToggle;
    [SerializeField]
    bool allowSwitchOff;

    public bool AllowSwitchOff
    {
        get
        {
            return allowSwitchOff;
        }
    }

    void OnDisable()
    {
        // deactivate active toggle if it is present
        if (selectedToggle != null)
        {
            // deleselect prevoius toggle
            DeselectToggle();
        }
    }

    void SelectToggle(TextToggle toggle)
    {
        // toggle selected toggle
        selectedToggle = toggle;
    }

    public void DeselectToggle()
    {
        selectedToggle.TurnOff();
        // clean up selectedToggle
        selectedToggle = null;
    }

    public void SetSelectedToggle(TextToggle toggle)
    {
        // verify if we already have any toggle selected
        if (selectedToggle != null)
        {
            // deleselect prevoius toggle
            DeselectToggle();
        }
        // select new toggle
        SelectToggle(toggle);
    }

    public TextToggle GetSelectedToggle()
    {
        return selectedToggle;
    }
}
