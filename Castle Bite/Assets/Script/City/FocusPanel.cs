using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPanel : MonoBehaviour {
    public GameObject focusedObject;
    [SerializeField]
    TextButton cityUpgradeButton;
    City city;
    //public enum FocusMode { HeroPartyNoFocus, HeroPartyFocus, CityFocus };
    //FocusMode focusMode;
    public enum ChangeType {
        Init,
        HireSingleUnit,
        HireDoubleUnit,
        HirePartyLeader,
        DismissSingleUnit,
        DismissDoubleUnit,
        DismissPartyLeader,
        HeroLeaveCity,
        UnitsPositionChange
    }

    public void OnEnable()
    {
        Debug.Log("Focus Panel Trigger On enable");
        InitFocusPanel();
    }

    public void OnDisable()
    {
        Debug.Log("Focus Panel Trigger On disable");
        // reset focusedObject to null
        focusedObject = null;
        // reset City link to null
        city = null;
        // deactivate all submenus
        foreach(Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(false);
        }
    }

    #region Initialize

    void InitFocusPanel()
    {
        // update information depending on focused object
        // if focused object is city, then populate focus panel with information from this city
        // if focused object is hero or unit, then populate focus panel with information from this hero or unit
        // if focused object is not set, then display just empty information
        if (focusedObject != null)
        {
            // focus is set
            // verify if focused object is city
            if (focusedObject.GetComponent<City>())
            {
                // verify if city is in city edit mode and not in edit hero party mode
                if (focusedObject.GetComponent<City>().GetHeroPartyByMode(PartyMode.Garnizon))
                {
                    SetCityInformation();
                }
            }
            else if (focusedObject.GetComponent<PartyUnitUI>())
            {
                SetLeaderInformation();
            }
            else
            {
                Debug.LogError("Unknown focused object " + focusedObject.name);
            }
        }
        else
        {
            // focus is not set
            // this cannot happen with city garnizon panel
            // because is city focus is always set to the city
            // this can only happen with party leader focus
            // when leader is not hired yet or not present in the city
            // set empty oject information
            // this is only relevan if focus panel is in city, not relevant for battle screen
            // 
            // verify if city screen is active
            if (transform.parent.GetComponentInChildren<EditPartyScreen>(false) != null)
            {
                Debug.Log("City screen is active");
                SetNoPartyInfo();
            }
        }
    }

    void SetCityLevelInfo(City city)
    {
        if (city != null)
        {
            transform.Find("CityFocus").Find("LevelValue").GetComponent<Text>().text = city.CityLevelCurrent.ToString();
            // verify if city has not reached max level or it is not capital city
            if (city.CityLevelCurrent >= ConfigManager.Instance.CityUpgradeConfig.maxCityLevel)
            {
                // add (max) information next to the level
                transform.Find("CityFocus").Find("LevelValue").GetComponent<Text>().text += "<size=12> max</size>";
            }
        }
    }

    void SetCurrentAndMaxUnitsInCityUIValue()
    {
        Debug.Log("Update current and maximum city units capacity");
        // verify if focus panel has city linked
        if (city != null)
        {
            //Debug.Log("City is not null");
            // transform.Find("CityFocus/UnitsValue").GetComponent<Text>().text = city.GetNumberOfPresentUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
            transform.Find("CityFocus/UnitsValue").GetComponent<Text>().text = city.GetHeroPartyByMode(PartyMode.Garnizon).GetLeadershipConsumedByPartyUnits().ToString() + "/" + city.GetUnitsCapacity().ToString();
        }
    }

    void SetUpgradeCityButton(City city)
    {
        // verify if city has not reached max level or it is not capital city
        if (city.CityLevelCurrent >= ConfigManager.Instance.CityUpgradeConfig.maxCityLevel)
        {
            // disable (hide) upgrade city button
            cityUpgradeButton.gameObject.SetActive(false);
        }
        else
        {
            // enable city upgrade city button
            cityUpgradeButton.gameObject.SetActive(true);
        }
    }

    void SetCityInformation()
    {
        // Activate required menu
        transform.Find("FocusedName").gameObject.SetActive(true);
        transform.Find("FocusedDescription").gameObject.SetActive(true);
        transform.Find("CityFocus").gameObject.SetActive(true);
        // city = transform.parent.GetComponent<City>();
        city = focusedObject.GetComponent<City>();
        transform.Find("FocusedName").GetComponent<Text>().text = city.CityName;
        transform.Find("FocusedDescription").GetComponent<Text>().text = city.CityDescription;
        SetCityLevelInfo(city);
        transform.Find("CityFocus").Find("DefenseValue").GetComponent<Text>().text = city.GetCityDefense().ToString() + "%";
        transform.Find("CityFocus").Find("HealPerDayValue").GetComponent<Text>().text = city.GetHealPerDay().ToString() + "%";
        transform.Find("CityFocus").Find("GoldPerDayValue").GetComponent<Text>().text = city.GoldIncomePerDay.ToString();
        transform.Find("CityFocus").Find("ManaPerDayValue").GetComponent<Text>().text = city.ManaIncomePerDay.ToString();
        SetCurrentAndMaxUnitsInCityUIValue();
        SetUpgradeCityButton(city);
    }

    void SetLeaderInformation()
    {
        // Activate required menu
        transform.Find("FocusedName").gameObject.SetActive(true);
        transform.Find("FocusedDescription").gameObject.SetActive(true);
        transform.Find("PartyFocus").gameObject.SetActive(true);
        // get party leader
        PartyUnit partyLeader = focusedObject.GetComponent<PartyUnitUI>().LPartyUnit;
        // populate with info from hero
        transform.Find("FocusedName").GetComponent<Text>().text = partyLeader.GivenName;
        transform.Find("FocusedDescription").GetComponent<Text>().text = partyLeader.UnitName;
        transform.Find("PartyFocus").Find("LevelValue").GetComponent<Text>().text = partyLeader.UnitLevel.ToString();
        transform.Find("PartyFocus").Find("LeadershipValue").GetComponent<Text>().text = partyLeader.GetEffectiveLeadership().ToString();
    }

    void SetNoPartyInfo()
    {
        // Activate required menu
        transform.Find("NoPartyInfo").gameObject.SetActive(true);
    }

    #endregion

    #region On change

    void OnHireSingleUnit()
    {
        // update number of units
        SetCurrentAndMaxUnitsInCityUIValue();
    }

    void OnHireDoubleUnit()
    {
        // update number of units
        SetCurrentAndMaxUnitsInCityUIValue();
    }

    void OnDismissSingleUnit()
    {
        // update number of units
        SetCurrentAndMaxUnitsInCityUIValue();
        //transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = (city.GetNumberOfPresentUnits()-1).ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnDimissDoubleUnit()
    {
        // update number of units
        SetCurrentAndMaxUnitsInCityUIValue();
        //transform.Find("CityFocus").Find("UnitsValue").GetComponent<Text>().text = (city.GetNumberOfPresentUnits()-2).ToString() + "/" + city.GetUnitsCapacity().ToString();
    }

    void OnUnitsPositionChange()
    {
        Debug.Log("OnUnitsPositionChange");
        SetCurrentAndMaxUnitsInCityUIValue();
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
                if (transform.root.GetComponentInChildren<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon, false))
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
            case ChangeType.UnitsPositionChange:
                OnUnitsPositionChange();
                break;
            default:
                Debug.LogError("Unknown condition");
                break;
        }
    }

    #endregion


    void OnCityUpgradeYesConfirmation()
    {
        Debug.Log("Yes");
        // get city
        City city = focusedObject.GetComponent<City>();
        // verify if it is not null
        if (city != null)
        {
            // upgrade city level
            city.CityLevelCurrent += 1;
            // verify if city has custom starting gold income
            if (city.CityData.goldIncomePerDay != -1)
            {
                // upgrade custom income by the difference between levels
                city.CityData.goldIncomePerDay += (ConfigManager.Instance.CityUpgradeConfig.cityGoldIncomePerCityLevel[city.CityLevelCurrent] - ConfigManager.Instance.CityUpgradeConfig.cityGoldIncomePerCityLevel[city.CityLevelCurrent - 1]);
            }
            // verify if city has custom starting mana income
            if (city.CityData.manaIncomePerDay != -1)
            {
                // upgrade custom income by the difference between levels
                city.CityData.manaIncomePerDay += (ConfigManager.Instance.CityUpgradeConfig.cityManaIncomePerCityLevel[city.CityLevelCurrent] - ConfigManager.Instance.CityUpgradeConfig.cityManaIncomePerCityLevel[city.CityLevelCurrent - 1]);
            }
            // reinit city info panel
            SetCityInformation();
        }
        else
        {
            Debug.LogWarning("City is null");
        }
    }

    void OnCityUpgradeNoConfirmation()
    {
        Debug.Log("No");
        // nothing to do here
    }

    public void UpgradeCity()
    {
        // get city
        City city = focusedObject.GetComponent<City>();
        // verify if it is not null
        if (city != null)
        {
            // init city upgrade menu
            Debug.Log("Ugrading city");
            // verify if city has not reached max level (normally button should be disabled, but just in case
            // verify if city has not reached max level or it is not capital city
            if (city.CityLevelCurrent >= ConfigManager.Instance.CityUpgradeConfig.maxCityLevel)
            {
                // Display notification that city has reached maximum level;
                NotificationPopUp.Instance().DisplayMessage("City has reached maximum level. No further upgrades are apossible.");
            }
            else
            {
                // get upgrade cost
                int cityUpgradeCost = ConfigManager.Instance.CityUpgradeConfig.cityUpgradeCostPerCityLevel[city.CityLevelCurrent + 1];
                // verify if player has enough money
                if (TurnsManager.Instance.GetActivePlayer().TotalGold >= cityUpgradeCost)
                {
                    // player has enough money
                    // set confirmation message
                    string confirmationMessage = "Do you want to spend " + cityUpgradeCost.ToString() + " gold to upgrade city to the next level?";
                    // display confirmation pop up asking player whether he wants to upgrade city
                    ConfirmationPopUp.Instance().Choice(confirmationMessage, new UnityEngine.Events.UnityAction(OnCityUpgradeYesConfirmation), new UnityEngine.Events.UnityAction(OnCityUpgradeNoConfirmation));
                }
                else
                {
                    // player doesn't have enough money
                    // Display notification that player doesn't have enough money
                    NotificationPopUp.Instance().DisplayMessage("You don't have enough money for city upgrade. Required " + cityUpgradeCost.ToString() + " gold.");
                }
            }
        }
        else
        {
            Debug.LogWarning("City is null");
        }
    }
}
