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
public class PartyData : System.Object
{
    public int partyID = -1;
    public CityID linkedCityID;
    public Faction faction;
    public PartyMode partyMode;
    // public PositionOnMap partyMapPosition;
    public string partyUIAddress = null;
    public bool holdPosition = false;
    public PartyUnitData[] partyUnitsData; // initialized and used only during game save and load
    public List<InventoryItemData> partyInventory;
    public MapCoordinates partyMapCoordinates; // used only during save and load
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

    //public PartyUnit GetHeroPartyLeaderUnit()
    //{
    //    foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
    //    {
    //        // verify if unit is leader
    //        if (partyUnit.IsLeader)
    //        {
    //            return partyUnit;
    //        }
    //    }
    //    return null;
    //}

    //public PositionOnMap GetPartyMapPosition()
    //{
    //    // initialize map position with default values
    //    PositionOnMap partyMapPosition = new PositionOnMap
    //    {
    //        offsetMinX = 0,
    //        offsetMinY = 0,
    //        offsetMaxX = 0,
    //        offsetMaxY = 0
    //    };
    //    // get map manager
    //    // MapManager mapManager = transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
    //    // verify if map manager is present
    //    if (MapManager.Instance == null)
    //    {
    //        Debug.LogError("cannot find map manager");
    //        // return default position
    //        return partyMapPosition;
    //    }
    //    else
    //    {
    //        // verify if this is city garnizon
    //        if (PartyMode.Garnizon == PartyMode)
    //        {
    //            // return default values as those values are not relevan
    //            return partyMapPosition;
    //        }
    //        else
    //        {
    //            // verify if linked party on map is defined
    //            if (lMapHero == null)
    //            {
    //                Debug.LogError("Linked party on map is null");
    //                // return default position
    //                return partyMapPosition;
    //            }
    //            else
    //            {
    //                //// get position
    //                //Vector2Int position = mapManager.GetTileByPosition(linkedPartyOnMap.transform.position);
    //                //// return position in PartyMapPosition format, which can be serialized
    //                //Debug.Log(" offsetMin.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.x.ToString());
    //                //Debug.Log(" offsetMin.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMin.y.ToString());
    //                //Debug.Log(" offsetMax.x " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.x.ToString());
    //                //Debug.Log(" offsetMax.y " + linkedPartyOnMap.GetComponent<RectTransform>().offsetMax.y.ToString());
    //                return new PositionOnMap
    //                {
    //                    offsetMinX = lMapHero.GetComponent<RectTransform>().offsetMin.x,
    //                    offsetMinY = lMapHero.GetComponent<RectTransform>().offsetMin.y,
    //                    offsetMaxX = lMapHero.GetComponent<RectTransform>().offsetMax.x,
    //                    offsetMaxY = lMapHero.GetComponent<RectTransform>().offsetMax.y
    //                };
    //            }
    //        }
    //    }
    //}

    public MapCoordinates GetPartyMapCoordinates()
    {
        // initialize map position with default values
        MapCoordinates partyMapCoordinates = new MapCoordinates
        {
            x = 0,
            y = 0
        };
        // verify if this is city garnizon
        if (PartyMode.Garnizon == PartyMode)
        {
            // return default values as those values are not relevant
            return partyMapCoordinates;
        }
        else
        {
            // verify if linked party on map is defined
            if (lMapHero == null)
            {
                Debug.LogError("Linked party on map is null");
                // return default position
                return partyMapCoordinates;
            }
            else
            {
                return MapManager.Instance.GetCoordinatesByWorldPosition(LMapHero.transform.position);
            }
        }
    }

    public bool HasUnitsWhichCanFight()
    {
        // loop through all units in party
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            // verify if unit can be given turn in battle based on its status
            if (partyUnit.UnitStatusConfig.GetCanBeGivenATurnInBattle())
            {
                return true;
            }
        }
        // not found units which can fight
        return false;
    }

    public void RemoveDeadPartyUnits()
    {
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            if (partyUnit.UnitStatus == UnitStatus.Dead)
            {
                // destroy unit canvas
                Debug.Log("Verify: destroy dead unit " + partyUnit.name);
                Destroy(partyUnit.gameObject);
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
        // while (parentTransform.parent != transform.root)
        while (parentTransform.parent != ObjectsManager.Instance.transform)
        {
            parentTransform = parentTransform.parent;
            address = parentTransform.name + "/" + address;
        }
        return address;
    }

    public int GetLeadershipConsumedByPartyUnits()
    {
        int consumedLeadership = 0;
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            // if this is double unit, then count is as +2
            if (partyUnit.UnitSize == UnitSize.Double)
            {
                // double unit
                consumedLeadership += 2;
            }
            else
            {
                // single unit
                consumedLeadership += 1;
            }
        }
        return consumedLeadership;
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
        //Debug.LogError("No Leader in party.");
        return null;
    }

    public void ExecutePreTurnActions()
    {
        // loop through all party units
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            partyUnit.ExecutePreTurnActions();
        }
    }

    public void ExecutePostTurnActions()
    {
        // verify if this is not city harnizon party
        if (PartyMode != PartyMode.Garnizon)
        {
            Debug.Log("Reset move points to max for " + name + " party");
            // reset move points to max
            // Note: this should be done before giving control to other player, so during his turn he can make impact on the other parties move points
            GetPartyLeader().MovePointsCurrent = GetPartyLeader().GetEffectiveMaxMovePoints();
        }
        // .. decrement daily debuffs
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

    public CityID LinkedCityID
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

    public bool HoldPosition
    {
        get
        {
            return partyData.holdPosition;
        }

        set
        {
            partyData.holdPosition = value;
        }
    }

}
