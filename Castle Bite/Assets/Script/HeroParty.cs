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
    public int x;
    public int y;
}

[Serializable]
public class PartyData : System.Object
{
    public Faction faction;
    public PartyMode partyMode;
    public PartyMapPosition partyMapPosition;
    public string partyUIAddress;
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

    public PartyMapPosition GetPartyMapPosition()
    {
        // initialize map position with default values
        PartyMapPosition partyMapPosition = new PartyMapPosition
        {
            x = 0,
            y = 0
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
                if (linkedPartyOnMap == null)
                {
                    Debug.LogError("Linked party on map is null");
                    // return default position
                    return partyMapPosition;
                }
                else
                {
                    // get position
                    Vector2Int position = mapManager.GetTileByPosition(linkedPartyOnMap.transform.position);
                    // return position in PartyMapPosition format, which can be serialized
                    return new PartyMapPosition
                    {
                        x = position.x,
                        y = position.y
                    };
                }
            }
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

}
