using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/City/Config")]
public class CityConfig : ScriptableObject
{
    // code below doesn't work on duplicate Ctrl+D in editor
    //[UniqueIdentifier]
    //public string cityGUID;
    // code below doesn't give you drop-down menu in Editor
    //public CityIDEnumberable cityID;
    public CityID cityID;
    public Faction cityFaction;
    public CityType cityType;
    public string cityName;
    public string cityDescription;
    public int cityLevelMax = 5;
    public UnitType[] hireablePartyLeaders;
    public UnitType[] hireableCommonUnits;
    public int isStarting; // defines whether this city is a starting city. It is used to place the first highered hero
    public int goldIncomePerDay = -1; // Normally this is taken from CityUpgradeConfig, but can be overwritten here, if not -1
    public int manaIncomePerDay = -1; // Normally this is taken from CityUpgradeConfig, but can be overwritten here, if not -1

}
