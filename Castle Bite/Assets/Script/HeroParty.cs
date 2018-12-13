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
    public int linkedCityID = -1;
    public Faction faction;
    public PartyMode partyMode;
    public PositionOnMap partyMapPosition;
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

    public PositionOnMap GetPartyMapPosition()
    {
        // initialize map position with default values
        PositionOnMap partyMapPosition = new PositionOnMap
        {
            offsetMinX = 0,
            offsetMinY = 0,
            offsetMaxX = 0,
            offsetMaxY = 0
        };
        // get map manager
        // MapManager mapManager = transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
        // verify if map manager is present
        if (MapManager.Instance == null)
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
                    return new PositionOnMap
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

    // todo: fix duplicate function in PartyPanel
    public PartyUnit GetActiveUnitWithHighestInitiative(BattleScreen.TurnPhase turnPhase)
    {
        PartyUnit unitWithHighestInitiative = null;
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            // verify if 
            //  - unit has moved or not
            //  - unit is alive
            //  - unit has not escaped from the battle
            if (!partyUnit.HasMoved)
            {
                // during main phase check for Active units
                bool doProceed = false;
                if ((BattleScreen.TurnPhase.Main == turnPhase)
                    && ((partyUnit.UnitStatus == UnitStatus.Active)
                       || (partyUnit.UnitStatus == UnitStatus.Escaping)))
                {
                    doProceed = true;
                }
                // during post wait phase check for units which are in Waiting status
                if ((BattleScreen.TurnPhase.PostWait == turnPhase) && (partyUnit.UnitStatus == UnitStatus.Waiting))
                {
                    doProceed = true;
                }
                if (doProceed)
                {
                    // compare initiative with other unit, if it was found
                    if (unitWithHighestInitiative != null)
                    {
                        if (partyUnit.UnitInitiative > unitWithHighestInitiative.UnitInitiative)
                        {
                            // found unit with highest initiative, update unitWithHighestInitiative variable
                            unitWithHighestInitiative = partyUnit;
                        }
                    }
                    else
                    {
                        // no other unit found yet, assume that this unit has the highest initiative
                        unitWithHighestInitiative = partyUnit;
                    }
                }
            }
        }
        return unitWithHighestInitiative;
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
        while (parentTransform.parent != transform.root)
        {
            parentTransform = parentTransform.parent;
            address = parentTransform.name + "/" + address;
        }
        return address;
    }

    public int GetNumberOfPresentUnits()
    {
        int unitsNumber = 0;
        foreach (PartyUnit partyUnit in GetComponentsInChildren<PartyUnit>())
        {
            // if this is double unit, then count is as +2
            if (partyUnit.UnitSize == UnitSize.Double)
            {
                // double unit
                unitsNumber += 2;
            }
            else
            {
                // single unit
                unitsNumber += 1;
            }
        }
        return unitsNumber;
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
