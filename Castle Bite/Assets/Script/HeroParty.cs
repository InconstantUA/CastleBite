using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartyMode
{
    Party,
    Garnizon
}

[Serializable]
public class PartyData : System.Object
{
    public Faction faction;
    public PartyMode partyMode;
    public PartyPanelData partyPanelData; // initialized and used only during game save and load
}

public class HeroParty : MonoBehaviour {
    [SerializeField] // required for parties creation and configuration via Unity Editor
    PartyData partyData;
    //[SerializeField]
    //Faction faction;
    //[SerializeField]
    //PartyMode partyMode;
    [SerializeField]
    MapHero linkedPartyOnMap;
    //public enum PartyPlace { Map, City };
    //[SerializeField]
    //PartyPlace partyPlace;
    // For battle
    public bool CanEscapeFromBattle { get; set; }
    public Transform PreBattleParentTr { get; set; }

    public PartyData PartyData
    {
        get
        {
            return partyData;
        }

        set
        {
            partyData = value;
        }
    }

    public void SetLinkedPartyOnMap(MapHero value)
    {
        linkedPartyOnMap = value;
    }

    public MapHero GetLinkedPartyOnMap()
    {
        return linkedPartyOnMap;
    }

    public Faction Faction
    {
        get
        {
            return partyData.faction;
        }

        set
        {
            partyData.faction = value;
        }
    }

    public PartyMode PartyMode
    {
        get
        {
            return partyData.partyMode;
        }

        set
        {
            partyData.partyMode = value;
        }
    }

    //public Faction GetFaction()
    //{
    //    return faction;
    //}

    //public void SetFaction(Faction value)
    //{
    //    faction = value;
    //}

    //public PartyMode GetMode()
    //{
    //    return partyMode;
    //}

    //public void SetMode(PartyMode value)
    //{
    //    partyMode = value;
    //}

    //public PartyPlace GetPlace()
    //{
    //    return partyPlace;
    //}

    //public void SetPlace(PartyPlace value)
    //{
    //    partyPlace = value;
    //}


    // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
