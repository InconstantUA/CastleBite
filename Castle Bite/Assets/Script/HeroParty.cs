using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PartyMode
{
    Party,
    Garnizon
}

[Serializable]
public struct PartyMapPosition
{
    public float offsetMinX;
    public float offsetMinY;
    public float offsetMaxX;
    public float offsetMaxY;
}

[Serializable]
public class PartyData : System.Object
{
    public int partyID = -1;
    public int linkedCityID = -1;
    public Faction faction;
    public PartyMode partyMode;
    public PartyMapPosition partyMapPosition;
    public string partyUIAddress;
    //public PartyPanelData partyPanelData; // initialized and used only during game save and load
    public PartyUnitData[] partyUnitsData; // initialized and used only during game save and load
}

public class HeroParty : MonoBehaviour {
    [SerializeField] // required for parties creation and configuration via Unity Editor
    PartyData partyData;
    //[SerializeField]
    //Faction faction;
    //[SerializeField]
    //PartyMode partyMode;
    [SerializeField]
    MapHero lMapHero;
    //public enum PartyPlace { Map, City };
    //[SerializeField]
    //PartyPlace partyPlace;
    // For battle
    public bool CanEscapeFromBattle { get; set; }
    public Transform PreBattleParentTr { get; set; }

    public PartyUnit GetHeroPartyLeaderUnit()
    {
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            // verify if unit is leader
            if (partyUnit.IsLeader)
            {
                return partyUnit;
            }
        }
        return null;
    }

    public PartyMapPosition GetPartyMapPosition()
    {
        // initialize map position with default values
        PartyMapPosition partyMapPosition = new PartyMapPosition
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
            return partyMapPosition;
        }
        else
        {
            // verify if this is city garnizon
            if (PartyMode.Garnizon == PartyMode)
            {
                // return default values as those values are not relevan
                return partyMapPosition;
            }
            else
            {
                // verify if linked party on map is defined
                if (lMapHero == null)
                {
                    Debug.LogError("Linked party on map is null");
                    // return default position
                    return partyMapPosition;
                }
                else
                {
                    //// get position
                    //Vector2Int position = mapManager.GetTileByPosition(linkedPartyOnMap.transform.position);
                    //// return position in PartyMapPosition format, which can be serialized
                    //Debug.Log(" offsetMin.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.x.ToString());
                    //Debug.Log(" offsetMin.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.y.ToString());
                    //Debug.Log(" offsetMax.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.x.ToString());
                    //Debug.Log(" offsetMax.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.y.ToString());
                    return new PartyMapPosition
                    {
                        offsetMinX = lMapHero.GetComponent<RectTransform>().offsetMin.x,
                        offsetMinY = lMapHero.GetComponent<RectTransform>().offsetMin.y,
                        offsetMaxX = lMapHero.GetComponent<RectTransform>().offsetMax.x,
                        offsetMaxY = lMapHero.GetComponent<RectTransform>().offsetMax.y
                    };
                }
            }
        }
    }

    public string GetPartyUIAddress(string address = "")
    {
        // init parent transform
        Transform parentTransform = transform.parent;
        // init address with the parent's address
        address = parentTransform.name;
        // get address untill we reach root
        while (parentTransform.parent != transform.root)
        {
            parentTransform = parentTransform.parent;
            address = parentTransform.name + "/" + address;
        }
        return address;
    }

    public PartyUnit GetPartyLeader()
    {
        // find leader unit
        foreach(PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            if (partyUnit.IsLeader)
            {
                return partyUnit;
            }
        }
        Debug.LogError("No Leader in party.");
        return null;
    }

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

    public int PartyID
    {
        get
        {
            return partyData.partyID;
        }

        set
        {
            partyData.partyID = value;
        }
    }

    public int LinkedCityID
    {
        get
        {
            return partyData.linkedCityID;
        }

        set
        {
            partyData.linkedCityID = value;
        }
    }

    public MapHero LMapHero
    {
        get
        {
            return lMapHero;
        }

        set
        {
            lMapHero = value;
        }
    }
}
