using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public InventoryItemData[] itemsOnMap; // used only during game save and load
    public PositionOnMap[] itemsPositionOnMap; // used only during game save and load, linked to itemsOnMap
}

public class GameMap : MonoBehaviour {
    [SerializeField]
    MapData mapData;

    public MapData MapData
    {
        get
        {
            return mapData;
        }

        set
        {
            mapData = value;
        }
    }
}
