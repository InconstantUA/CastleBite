using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour {
    [SerializeField]
    InventoryItem[] lInventoryItems;

    public InventoryItem[] LInventoryItems
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
