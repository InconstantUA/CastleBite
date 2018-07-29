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

    void SelectSave(GameObject save)
    {
        // save selected save
        selectedSave = save;
        // verify if what mode we operate Save or Load game
        if (mode == Mode.Save)
        {
            // change save name for new save
            transform.Find("NewSave/InputField").GetComponent<InputField>().text = save.GetComponent<Save>().saveName;
            // update save details
            // ..
        }
        else if (mode == Mode.Load)
        {
            // update save details
            // ..
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
            // clean up save details
            // ..
        }
        else if (mode == Mode.Load)
        {
            // clean up save details
            // ..
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
