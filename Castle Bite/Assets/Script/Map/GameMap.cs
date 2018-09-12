using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public List<InventoryItemData> itemsOnMap; // used only during game save and load
    public List<PositionOnMap> itemsPositionOnMap; // used only during game save and load, linked to itemsOnMap
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

    public void SetItemsOnMapData()
    {
        // init lists
        MapData.itemsOnMap = new List<InventoryItemData>();
        MapData.itemsPositionOnMap = new List<PositionOnMap>();
        // Loop through transforms 1 level below map (=belongs to the map)
        foreach (Transform childTransform in transform.root.Find("MapScreen/Map"))
        {
            // get map item (chest)
            MapItem mapItem = childTransform.GetComponent<MapItem>();
            // verify if it is not null
            if (mapItem != null)
            {
                // loop through all items linked to this map item (chest)
                foreach (InventoryItem inventoryItem in mapItem.LInventoryItems)
                {
                    // set item data
                    MapData.itemsOnMap.Add(inventoryItem.InventoryItemData);
                    // set item position 
                    MapData.itemsPositionOnMap.Add(
                        new PositionOnMap
                        {
                            offsetMinX = mapItem.GetComponent<RectTransform>().offsetMin.x,
                            offsetMinY = mapItem.GetComponent<RectTransform>().offsetMin.y,
                            offsetMaxX = mapItem.GetComponent<RectTransform>().offsetMax.x,
                            offsetMaxY = mapItem.GetComponent<RectTransform>().offsetMax.y
                        }
                        );
                }
            }
        }
    }
}
