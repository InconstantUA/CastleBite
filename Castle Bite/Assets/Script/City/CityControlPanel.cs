using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityControlPanel : MonoBehaviour {
    Toggle[] allTogglesInGroup;

    // Use this for initialization
    void Start () {
        // get all toggles in Toggle group
        allTogglesInGroup = transform.parent.GetComponentsInChildren<Toggle>();
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void DimmAllOtherMenusExceptToggled(Toggle callingToggle)
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // make sure that we do not deem ourselves and toggled (selected) unit
            if ((!tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
            {
                tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
            }
        }
        Debug.Log("DimmAllOtherMenusExceptToggled");
    }

    public void DeselectAllOtherTogglesInGroup(Toggle callingToggle)
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            // do not dimm currently selected objects
            // make sure that we do not deem ourselves
            if ((tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
            {
                tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
            }
        }
        Debug.Log("DeselectAllOtherTogglesInGroup");
    }
}
