using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum CityType {
    Normal,
    Capital
};

[Serializable]
public enum CityID
{
    None,
    Stormberg,
    Zrurrugh,
    SintJoris,
    Nartley
}

[Serializable]
public class CityData
{
    public CityID cityID;
    //public int linkedPartyID = -1;
    //public int linkedGarnizonID = -1;
    public Faction cityCurrentFaction;
    // public CityType cityType;
    // public string cityName;
    // public string cityDescription;
    public int cityLevelCurrent = 1;
    // public int cityLevelMax = 5;
    // public UnitType[] hireablePartyLeaders;
    // public UnitType[] hireableCommonUnits;
    // public PositionOnMap cityMapPosition;   // used only during load and save
    public MapCoordinates cityMapCoordinates;   // used only during load and save
    // public int isStarting; // defines whether this city is a starting city. It is used to place the first highered hero
    // Normally this is taken from CityUpgradeConfig, but you can apply custom bonus or penalty
    public int goldIncomePerDayModifier = 0;
    // Normally this is taken from CityUpgradeConfig, but you can apply custom bonus or penalty
    public int manaIncomePerDayModifier = 0;
}

public class City : MonoBehaviour {
    //[SerializeField]
    //MapManager mapManager;
    [SerializeField]
    CityData cityData;
    [SerializeField]
    MapCity lMapCity;
    [SerializeField]
    GameEvent cityLevelUpEvent;

    CityConfig cityConfig;
    FactionConfig factionConfig;

    // to be moved to other class
    // City view state is required to effectively change between different states
    // and do not forget to enable or disable something
    // we need to correct transition to normal state after leaving city view
    // to view it normally again after we enter it
    // and do not have heal/resurect/dismiss states active after we enter city next time
    //
    // For the same purpose we have City inventory state
    // If we leave city, then we should deactivate inventory view
    //
    // The same for city occupation state
    // If hero leaves city, the we should return city state o NoHeroIn
    // and activate hire hero button


    public int GetCityDefense()
    {
        int bonus = 0;
        if (CityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (CityLevelCurrent * 5) + bonus;
    }

    public int GetHealPerDay()
    {
        int bonus = 0;
        if (CityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (CityLevelCurrent * 5) + 5 + bonus;
    }

    public int GetUnitsCapacity()
    {
        return CityLevelCurrent;
    }

    //public int GetNumberOfPresentUnits()
    //{
    //    int value = 0;
    //    // loop through all units in city garnizon
    //    foreach (PartyUnit partyUnit in GetHeroPartyByMode(PartyMode.Garnizon).GetComponentsInChildren<PartyUnit>())
    //    {
    //        // verify unit size
    //        if (partyUnit.UnitSize == UnitSize.Double)
    //        {
    //            value += 2;
    //        }
    //        else
    //        {
    //            value += 1;
    //        }
    //    }
    //    return value;
    //}

    public HeroParty GetHeroPartyByMode(PartyMode partyMode)
    {
        // Loop through hero parties untill we find the party in party not garnizon mode
        foreach (HeroParty heroParty in transform.GetComponentsInChildren<HeroParty>())
        {
            // compare if hero party is in required mode
            if (heroParty.PartyMode == partyMode)
            {
                return heroParty;
            }
        }
        return null;
    }

    //public PositionOnMap GetCityMapPosition()
    //{
    //    // initialize map position with default values
    //    PositionOnMap cityMapPosition = new PositionOnMap
    //    {
    //        offsetMinX = 0,
    //        offsetMinY = 0,
    //        offsetMaxX = 0,
    //        offsetMaxY = 0
    //    };
    //    // get map manager
    //    //MapManager mapManager = transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
    //    // verify if map manager is present
    //    if (MapManager.Instance == null)
    //    {
    //        Debug.LogError("cannot find map manager");
    //        // return default position
    //        return cityMapPosition;
    //    }
    //    else
    //    {
    //        // verify if linked city on map is defined
    //        if (lMapCity == null)
    //        {
    //            Debug.LogError("Linked city on map is null");
    //            // return default position
    //            return cityMapPosition;
    //        }
    //        else
    //        {
    //            return new PositionOnMap
    //            {
    //                offsetMinX = lMapCity.GetComponent<RectTransform>().offsetMin.x,
    //                offsetMinY = lMapCity.GetComponent<RectTransform>().offsetMin.y,
    //                offsetMaxX = lMapCity.GetComponent<RectTransform>().offsetMax.x,
    //                offsetMaxY = lMapCity.GetComponent<RectTransform>().offsetMax.y
    //            };
    //        }
    //    }
    //}

    #region City Properties
    public Faction CityCurrentFaction
    {
        get
        {
            return cityData.cityCurrentFaction;
        }

        set
        {
            // verify if city faction is not the same
            if (cityData.cityCurrentFaction != value)
            {
                // reset faction config, so it is updated on next Get request
                factionConfig = null;
                // save old city faction value
                Faction oldValue = cityData.cityCurrentFaction;
                // set new city faction
                cityData.cityCurrentFaction = value;
                // change city on map color to the new color
                lMapCity.SetColor(ObjectsManager.Instance.GetPlayerByFaction(value).PlayerColor);
                // trigger event
                EventsAdmin.Instance.IHasChanged(this, oldValue);
            }
        }
    }

    public Faction CityStartingFaction
    {
        get
        {
            return CityConfig.cityStartingFaction;
        }
    }

    public CityType CityType
    {
        get
        {
            // return cityData.cityType;
            // get data from config
            return CityConfig.cityType;
        }

        //set
        //{
        //    cityData.cityType = value;
        //}
    }

    public string CityName
    {
        get
        {
            // return cityData.cityName;
            // get data from config
            return CityConfig.cityName;
        }

        //set
        //{
        //    cityData.cityName = value;
        //}
    }

    public string CityDescription
    {
        get
        {
            // return cityData.cityDescription;
            // get data from config
            return CityConfig.cityDescription;
        }

        //set
        //{
        //    cityData.cityDescription = value;
        //}
    }

    public int CityLevelCurrent
    {
        get
        {
            return cityData.cityLevelCurrent;
        }

        set
        {
            cityData.cityLevelCurrent = value;
            // trigger city level up event
            cityLevelUpEvent.Raise();
        }
    }

    public int CityLevelMax
    {
        get
        {
            // return cityData.cityLevelMax;
            // get data from config
            return CityConfig.cityLevelMax;
        }

        //set
        //{
        //    cityData.cityLevelMax = value;
        //}
    }

    public FactionConfig FactionConfig
    {
        get
        {
            // verify if faction config has not been set yet
            if (factionConfig == null)
            {
                // get config from manager
                factionConfig = Array.Find(ConfigManager.Instance.FactionConfigs, c => c.faction == CityCurrentFaction);
            }
            // return faction config;
            return factionConfig;
        }
    }

    public UnitType[] HireablePartyLeaders
    {
        get
        {
            // return cityData.hireablePartyLeaders;
            return FactionConfig.hireablePartyLeaders;
        }

        //set
        //{
        //    cityData.hireablePartyLeaders = value;
        //}
    }

    public UnitType[] HireableCommonUnits
    {
        get
        {
            // return cityData.hireableCommonUnits;
            return FactionConfig.hireableCommonUnits;
        }

        //set
        //{
        //    cityData.hireableCommonUnits = value;
        //}
    }

    public CityID CityID
    {
        get
        {
            return cityData.cityID;
        }

        //set
        //{
        //    cityData.cityID = value;
        //}
    }

    public int IsStarting
    {
        get
        {
            // return cityData.isStarting;
            // get data from config
            return CityConfig.isStarting;
        }

        //set
        //{
        //    cityData.isStarting = value;
        //}
    }

    public int GoldIncomePerDay
    {
        get
        {
            // get data from config based on the city level
            return ConfigManager.Instance.CityUpgradeConfig.cityGoldIncomePerCityLevel[CityLevelCurrent] + cityData.goldIncomePerDayModifier;
        }

        //set
        //{
        //    cityData.goldIncomePerDay = value;
        //}
    }

    public int ManaIncomePerDay
    {
        get
        {
            // get data from config based on the city level
            return ConfigManager.Instance.CityUpgradeConfig.cityManaIncomePerCityLevel[CityLevelCurrent] + cityData.manaIncomePerDayModifier;
        }

        //set
        //{
        //    cityData.manaIncomePerDay = value;
        //}
    }

    //public int LinkedPartyID
    //{
    //    get
    //    {
    //        return cityData.linkedPartyID;
    //    }

    //    set
    //    {
    //        cityData.linkedPartyID = value;
    //    }
    //}

    //public int LinkedGarnizonID
    //{
    //    get
    //    {
    //        return cityData.linkedGarnizonID;
    //    }

    //    set
    //    {
    //        cityData.linkedGarnizonID = value;
    //    }
    //}

    public MapCity LMapCity
    {
        get
        {
            return lMapCity;
        }

        set
        {
            lMapCity = value;
        }
    }

    public CityData CityData
    {
        get
        {
            return cityData;
        }

        set
        {
            cityData = value;
        }
    }

    public CityConfig CityConfig
    {
        get
        {
            // verify if cityConfig is not set yet
            if (cityConfig == null)
            {
                // get config from manager
                // cityConfig = ConfigManager.Instance[CityID];
                cityConfig = Array.Find(ConfigManager.Instance.CityConfigs, e => e.cityID == CityID);
            }
            // return city config
            return cityConfig;
        }
    }
    #endregion City Properties

}
