﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityControlPanel : MonoBehaviour {
    Toggle[] allTogglesInGroup;
    Toggle heroEquipmentToggle;

    // Use this for initialization
    void Start () {
        // get all toggles in Toggle group
        allTogglesInGroup = transform.parent.GetComponentsInChildren<Toggle>();
    }

    //// Update is called once per frame
    //void Update () {

    //}
    public void SetHeroEquipmentToggle(Toggle heroEqTgl)
    {
        heroEquipmentToggle = heroEqTgl;
    }

    void VerifyAndDimmToggle(Toggle callingToggle, Toggle tmpTgl)
    {
        // do not dimm currently selected objects
        // make sure that we do not deem ourselves and toggled (selected) unit
        if ((!tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
        {
            tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
        }
    }

    public void DimmAllOtherMenusExceptToggled(Toggle callingToggle)
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            VerifyAndDimmToggle(callingToggle, tmpTgl);
        }
        // if hero is in the city (heroEquipmentToggle not null), then verify his toggle too
        if(heroEquipmentToggle)
        {
            VerifyAndDimmToggle(callingToggle, heroEquipmentToggle);
        }
        Debug.Log("DimmAllOtherMenusExceptToggled");
    }

    void VerifyAndDeselectToggle(Toggle callingToggle, Toggle tmpTgl)
    {
        // do not dimm currently selected objects
        // make sure that we do not deem ourselves
        if ((tmpTgl.isOn) && (callingToggle.name != tmpTgl.name))
        {
            tmpTgl.GetComponentInChildren<Text>().color = tmpTgl.colors.normalColor;
        }
    }

    public void DeselectAllOtherTogglesInGroup(Toggle callingToggle)
    {
        foreach (Toggle tmpTgl in allTogglesInGroup)
        {
            VerifyAndDeselectToggle(callingToggle, tmpTgl);
        }
        if (heroEquipmentToggle)
        {
            VerifyAndDeselectToggle(callingToggle, heroEquipmentToggle);
        }
        Debug.Log("DeselectAllOtherTogglesInGroup");
    }
}