using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPanel : MonoBehaviour {
    public GameObject focusedObject;
    City city;
    PartyUnit partyLeader;
    PartyPanel partyPanel;
    public enum FocusMode { HeroPartyNoFocus, HeroPartyFocus, CityFocus };
    FocusMode focusMode;
    public enum ChangeType { Init, HireSingleUnit, HireDoubleUnit, HirePartyLeader, DismissSingleUnit, DismissDoubleUnit, DismissPartyLeader, HeroLeaveCity }

    // Use this for initialization
    void Start()
    {
        InitFocusPanel();
    }

    #region Initialize

    void InitFocusPanel()
    {
        // update information depending on focused object
        // if focused object is city, then populate focus panel with information from this city
        // if focused object is hero or unit, then populate focus panel with information from this hero or unit
        // if focused object is not set, then display just empty information
        if (focusedObject)
        {
            // focus is set
            if (focusedObject.GetComponent<City>())
            {
                // verify if city is in city edit mode and not in edit hero party mode
                if (focusedObject.GetComponent<City>().transform.Find("CityGarnizon"))
                {
                    SetCityInformation();
                }
            }
            else if (focusedObject.GetComponent<PartyUnit>())
            {
                SetLeaderInformation();
            }
        }
        else
        {
            // focus not set
            // this cannot happen with city garnizon panel
            // because is city focus is always set to the city
            // this can only happen with party leader focus
            // when leader is not hired yet or not present in the city
            // set empty oject information
            // this is only relevan if focus panel is in city, not relevant for battle screen
            if (transform.parent.GetComponent<City>())
            {
                // verify if we are in city edit mode and not in hero edit mode
                if (transform.parent.GetComponent<City>().transform.Find("CityGarnizon"))
                {
                    SetNoPartyInfo();
                }
            }
        }
    }

    void SetCityInformation()
    {
        // city = transform.parent.GetComponent<City>();
        city = focusedObject.GetComponent<City>();
        transform.Find("FocusedName").GetComponent<Text>().text = city.GetCityName();
        transform.Find("FocusedDescription").GetComponent<Text>().text = city.GetCityDescription();
        transform.Find("CityFocus").Find("LevelValue").GetComponent<Text>().text = city.GetCityLevel().ToString();
        transform.Find("CityFocus").Find("DefenseValue").GetComponent<Text>().text = city.GetCityDefense().ToString();
        transform.Find("CityFocus").Find("HealPerDayValue").GetComponent<Text>().text = city.GetHealPerDay().ToString();
        partyPanel = focusedObject.transform.Find("CityGarnizon").Find("PartyPanel").GetComponent<PartyPanel>();
        // partyPanel = transform.parent.Find("CityGarnizon").Find("PartyPanel").GetComponent<PartyPanel>();
        // PartyPanel partyPanel = GameObject.FindObjectOfType(typeof(PartyPanel)) as PartyPanel;
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void SetLeaderInformation()
    {
        //  first deactivate NoPartyInfo and activate FocusedName, FocusedDescription and PartyFocus
        // this is relevant only for focus panel in city
        if (transform.parent.GetComponent<City>())
        {
            transform.Find("NoPartyInfo").gameObject.SetActive(false);
            transform.Find("FocusedName").gameObject.SetActive(true);
            transform.Find("FocusedDescription").gameObject.SetActive(true);
            transform.Find("PartyFocus").gameObject.SetActive(true);
        }
        // get party leader
        partyLeader = focusedObject.GetComponent<PartyUnit>();
        // populate with info from hero
        transform.Find("FocusedName").GetComponent<Text>().text = partyLeader.GivenName;
        transform.Find("FocusedDescription").GetComponent<Text>().text = partyLeader.UnitName;
        transform.Find("PartyFocus").Find("LevelValue").GetComponent<Text>().text = partyLeader.UnitLevel.ToString();
        transform.Find("PartyFocus").Find("LeadershipValue").GetComponent<Text>().text = partyLeader.GetEffectiveLeadership().ToString();
    }

    void SetNoPartyInfo()
    {
        //  first deactivate NoPartyInfo and activate FocusedName, FocusedDescription and PartyFocus
        transform.Find("NoPartyInfo").gameObject.SetActive(true);
        transform.Find("FocusedName").gameObject.SetActive(false);
        transform.Find("PartyFocus").gameObject.SetActive(false);
    }

    #endregion

    #region On change

    void OnHireSingleUnit()
    {
        // update number of units
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnHireDoubleUnit()
    {
        // update number of units
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnDismissSingleUnit()
    {
        // update number of units
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnDimissDoubleUnit()
    {
        if (partyPanel)
        {
            partyPanel.GetNumberOfPresentUnits().ToString();
            city.GetUnitsCapacity().ToString();
            // update number of units
            transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
        }
    }

    public void OnChange(ChangeType changeType)
    {
        switch (changeType)
        {
            case ChangeType.Init:
                InitFocusPanel();
                break;
            case ChangeType.HirePartyLeader:
                SetLeaderInformation();
                break;
            case ChangeType.HireSingleUnit:
                OnHireSingleUnit();
                break;
            case ChangeType.HireDoubleUnit:
                OnHireDoubleUnit();
                break;
            case ChangeType.DismissPartyLeader:
            case ChangeType.HeroLeaveCity:
                // verify if we are in city or edit hero mode
                if (transform.parent.GetComponent<City>().transform.Find("CityGarnizon"))
                {
                    SetNoPartyInfo();
                }
                break;
            case ChangeType.DismissSingleUnit:
                OnDismissSingleUnit();
                break;
            case ChangeType.DismissDoubleUnit:
                OnDimissDoubleUnit();
                break;
            default:
                Debug.LogError("Unknown condition");
                break;
        }
    }

    #endregion

    //// Update is called once per frame
    //void Update () {
    //}
}
