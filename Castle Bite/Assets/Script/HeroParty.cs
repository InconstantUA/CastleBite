using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroParty : MonoBehaviour {
    [SerializeField]
    Faction faction;

    public enum PartyMode { Party, Garnizon };
    [SerializeField]
    PartyMode partyMode;

    public enum PartyPlace { Map, City };
    [SerializeField]
    PartyPlace partyPlace;

    [SerializeField]
    MapHero linkedPartyOnMap;

    public bool CanEscapeFromBattle { get; set; }

    public void SetLinkedPartyOnMap(MapHero value)
    {
        linkedPartyOnMap = value;
    }

    public MapHero GetLinkedPartyOnMap()
    {
        return linkedPartyOnMap;
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public void SetFaction(Faction value)
    {
        faction = value;
    }

    public PartyMode GetMode()
    {
        return partyMode;
    }

    public void SetMode(PartyMode value)
    {
        partyMode = value;
    }

    public PartyPlace GetPlace()
    {
        return partyPlace;
    }

    public void SetPlace(PartyPlace value)
    {
        partyPlace = value;
    }


    // Use this for initialization
 //   void Start () {

	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
