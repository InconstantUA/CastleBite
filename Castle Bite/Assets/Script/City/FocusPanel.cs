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
    public enum ChangeType { Init, HireSingleUnit, HireDoubleUnit, HirePartyLeader, DismissSingleUnit, DismissDoubleUnit, DismissPartyLeader }


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
                SetCityInformation();
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
            SetNoPartyInfo();
        }
    }

    void SetCityInformation()
    {
        // city = transform.parent.GetComponent<City>();
        city = focusedObject.GetComponent<City>();
        transform.Find("FocusedName").GetComponent<Text>().text = city.cityName;
        transform.Find("FocusedDescription").GetComponent<Text>().text = city.cityDescription;
        transform.Find("CityFocus").Find("LevelValue").GetComponent<Text>().text = city.cityLevel.ToString();
        transform.Find("CityFocus").Find("DefenceValue").GetComponent<Text>().text = city.GetDefence().ToString();
        transform.Find("CityFocus").Find("HealPerDayValue").GetComponent<Text>().text = city.GetHealPerDay().ToString();
        partyPanel = focusedObject.transform.Find("CityGarnizon").Find("PartyPanel").GetComponent<PartyPanel>();
        // partyPanel = transform.parent.Find("CityGarnizon").Find("PartyPanel").GetComponent<PartyPanel>();
        // PartyPanel partyPanel = GameObject.FindObjectOfType(typeof(PartyPanel)) as PartyPanel;
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = partyPanel.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void SetLeaderInformation()
    {
        //  first deactivate NoPartyInfo and activate FocusedName, FocusedDescription and PartyFocus
        transform.Find("NoPartyInfo").gameObject.SetActive(false);
        transform.Find("FocusedName").gameObject.SetActive(true);
        transform.Find("FocusedDescription").gameObject.SetActive(true);
        transform.Find("PartyFocus").gameObject.SetActive(true);
        // get party leader
        partyLeader = focusedObject.GetComponent<PartyUnit>();
        // populate with info from hero
        transform.Find("FocusedName").GetComponent<Text>().text = partyLeader.GetGivenName();
        transform.Find("FocusedDescription").GetComponent<Text>().text = partyLeader.GetUnitName();
        transform.Find("PartyFocus").Find("LevelValue").GetComponent<Text>().text = partyLeader.GetLevel().ToString();
        transform.Find("PartyFocus").Find("LeadershipValue").GetComponent<Text>().text = partyLeader.GetLeadership().ToString();
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
        // looks like unit is not destroyed immediately and still present in the cell
        // decrease number of units by one here manually
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = (partyPanel.GetNumberOfPresentUnits()-1).ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnDimissDoubleUnit()
    {
        // looks like unit is not destroyed immediately and still present in the cell
        // decrease number of units by one here manually
        transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = (partyPanel.GetNumberOfPresentUnits() - 2).ToString() + "/" + city.GetUnitsCapacity().ToString();
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
                SetNoPartyInfo();
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
