using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenu : MonoBehaviour {

    GameObject selectedSave;

    void OnDisable()
    {
        DeselectSave();
    }

    void SelectSave(GameObject save)
    {
        // save selected save
        selectedSave = save;
        // change save name for new save
        transform.Find("NewSave/InputField").GetComponent<InputField>().text = save.GetComponent<Save>().saveName;
    }

    public void DeselectSave()
    {
        // clean up selectedSave
        selectedSave = null;
        // clean save name for new save
        transform.Find("NewSave/InputField").GetComponent<InputField>().text = "";
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
