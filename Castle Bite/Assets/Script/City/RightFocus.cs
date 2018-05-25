using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightFocus : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        City city = transform.parent.GetComponent<City>();
        transform.Find("FocusedName").GetComponent<Text>().text = city.cityName;
        transform.Find("FocusedDescription").GetComponent<Text>().text = city.cityDescription;
        transform.Find("BriefInfo").Find("LevelValue").GetComponent<Text>().text = city.cityLevel.ToString();
        transform.Find("BriefInfo").Find("DefenceValue").GetComponent<Text>().text = city.GetDefence().ToString();
        transform.Find("BriefInfo").Find("HealPerDayValue").GetComponent<Text>().text = city.GetHealPerDay().ToString();
        PartyPanel garnizonPanelf = transform.parent.Find("CityGarnizon").Find("PartyPanel").GetComponent<PartyPanel>();
        // PartyPanel garnizonPanelf = GameObject.FindObjectOfType(typeof(PartyPanel)) as PartyPanel;
        transform.Find("BriefInfo").Find("UnitsValue").GetComponent<Text>().text = garnizonPanelf.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    //// Update is called once per frame
    //void Update () {
    //}
}
