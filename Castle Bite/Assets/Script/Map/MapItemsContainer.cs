using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemsContainer : MonoBehaviour {
    [SerializeField]
    List<InventoryItem> lInventoryItems;

    public List<InventoryItem> LInventoryItems
    {
        get
        {
            return lInventoryItems;
        }

        set
        {
            lInventoryItems = value;
        }
    }
}
