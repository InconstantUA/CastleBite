using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityControlPanel : MonoBehaviour {
    public void DimmAllOtherMenusExceptToggled(Toggle callingToggle)
    {
        // get all toggles in Toggle group
        // this might change after hero dismiss, because Hero's equipment button is also member of toggle group
        Toggle[] allTogglesInGroup = transform.parent.GetComponentsInChildren<Toggle>();
        // 
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // make sure that we do not deem ourselves and toggled (selected) unit
            if ((!tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
            {
                tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
            }
        }
        // Debug.Log("DimmAllOtherMenusExceptToggled");
    }

    public void DeselectAllOtherTogglesInGroup(Toggle callingToggle)
    {
        // get all toggles in Toggle group
        // this might change after hero dismiss, because Hero's equipment button is also member of toggle group
        Toggle[] allTogglesInGroup = transform.parent.GetComponentsInChildren<Toggle>();
        // 
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // make sure that we do not deem ourselves
            if ((tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
            {
                tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
            }
        }
        // Debug.Log("DeselectAllOtherTogglesInGroup");
    }
}
