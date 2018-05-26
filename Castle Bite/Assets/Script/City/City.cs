﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {
    public string cityName;
    public string cityDescription;
    public int cityLevel;
    public enum CityType { Normal, Capital };
    public CityType cityType;
    int maxCityLevel = 5;

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public int GetDefence()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + bonus;
    }

    public int GetHealPerDay()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + 5 + bonus;
    }

    public int GetUnitsCapacity()
    {
        return cityLevel;
    }

    public bool HasCityReachedMaximumLevel()
    {
        bool result = false;
        if  (cityLevel == maxCityLevel)
        {
            // city has reached its maximum level
            result = true;
        }
        return result;
    }
}
