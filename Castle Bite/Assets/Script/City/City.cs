﻿using System;
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
public class CityData
{
    public int cityID = -1;
    //public int linkedPartyID = -1;
    //public int linkedGarnizonID = -1;
    public Faction cityFaction;
    public CityType cityType;
    public string cityName;
    public string cityDescription;
    public int cityLevelCurrent = 1;
    public int cityLevelMax = 5;
    public UnitType[] hireablePartyLeaders;
    public UnitType[] hireableCommonUnits;
    public PositionOnMap cityMapPosition;   // used only during load and save
}

public class City : MonoBehaviour {
    [SerializeField]
    CityData cityData;
    [SerializeField]
    MapCity lMapCity;

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

    public int GetNumberOfPresentUnits()
    {
        int value = 0;
        // loop through all units in city garnizon
        foreach (PartyUnit partyUnit in GetHeroPartyByMode(PartyMode.Garnizon).GetComponentsInChildren<PartyUnit>())
        {
            // verify unit size
            if (partyUnit.UnitSize == UnitSize.Double)
            {
                value += 2;
            }
            else
            {
                value += 1;
            }
        }
        return value;
    }

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

    public PositionOnMap GetCityMapPosition()
    {
        // initialize map position with default values
        PositionOnMap cityMapPosition = new PositionOnMap
        {
            offsetMinX = 0,
            offsetMinY = 0,
            offsetMaxX = 0,
            offsetMaxY = 0
        };
        // get map manager
        MapManager mapManager = transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
        // verify if map manager is present
        if (mapManager == null)
        {
            Debug.LogError("cannot find map manager");
            // return default position
            return cityMapPosition;
        }
        else
        {
            // verify if linked party on map is defined
            if (lMapCity == null)
            {
                Debug.LogError("Linked party on map is null");
                // return default position
                return cityMapPosition;
            }
            else
            {
                // return position in PartyMapPosition format, which can be serialized
                //Debug.Log(" offsetMin.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.x.ToString());
                //Debug.Log(" offsetMin.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.y.ToString());
                //Debug.Log(" offsetMax.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.x.ToString());
                //Debug.Log(" offsetMax.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.y.ToString());
                return new PositionOnMap
                {
                    offsetMinX = lMapCity.GetComponent<RectTransform>().offsetMin.x,
                    offsetMinY = lMapCity.GetComponent<RectTransform>().offsetMin.y,
                    offsetMaxX = lMapCity.GetComponent<RectTransform>().offsetMax.x,
                    offsetMaxY = lMapCity.GetComponent<RectTransform>().offsetMax.y
                };
            }
        }
    }

    #region City Properties
    public Faction CityFaction
    {
        get
        {
            return cityData.cityFaction;
        }

        set
        {
            if (cityData.cityFaction != value)
            {
                Faction oldValue = cityData.cityFaction;
                cityData.cityFaction = value;
                // trigger event
                transform.root.GetComponentInChildren<EventsAdmin>().IHasChanged(this, oldValue);
            }
        }
    }

    public CityType CityType
    {
        get
        {
            return cityData.cityType;
        }

        set
        {
            cityData.cityType = value;
        }
    }

    public string CityName
    {
        get
        {
            return cityData.cityName;
        }

        set
        {
            cityData.cityName = value;
        }
    }

    public string CityDescription
    {
        get
        {
            return cityData.cityDescription;
        }

        set
        {
            cityData.cityDescription = value;
        }
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
        }
    }

    public int CityLevelMax
    {
        get
        {
            return cityData.cityLevelMax;
        }

        set
        {
            cityData.cityLevelMax = value;
        }
    }

    public UnitType[] HireablePartyLeaders
    {
        get
        {
            return cityData.hireablePartyLeaders;
        }

        set
        {
            cityData.hireablePartyLeaders = value;
        }
    }

    public UnitType[] HireableCommonUnits
    {
        get
        {
            return cityData.hireableCommonUnits;
        }

        set
        {
            cityData.hireableCommonUnits = value;
        }
    }

    public int CityID
    {
        get
        {
            return cityData.cityID;
        }

        set
        {
            cityData.cityID = value;
        }
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
    #endregion City Properties

}
