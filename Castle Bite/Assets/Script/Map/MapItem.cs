using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour {
    [SerializeField]
    InventoryItem lInventoryItem;

    public InventoryItem LInventoryItem
    {
        get
        {
            return lInventoryItem;
        }

        set
        {
            lInventoryItem = value;
        }
    }
}
