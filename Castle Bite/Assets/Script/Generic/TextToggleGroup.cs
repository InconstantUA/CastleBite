using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TextToggleGroupToggleTurnOnEvent : UnityEvent<TextToggle>
{
}

public class TextToggleGroup : MonoBehaviour {
    TextToggle selectedToggle;
    [SerializeField]
    bool allowSwitchOff;
    public TextToggleGroupToggleTurnOnEvent OnToggleTurnOn;
    public UnityEvent OnToggleTurnOff;


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
        // trigger regiestered functions
        OnToggleTurnOff.Invoke();
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
        // trigger regiestered functions
        OnToggleTurnOn.Invoke(toggle);
    }

    public TextToggle GetSelectedToggle()
    {
        return selectedToggle;
    }
}
