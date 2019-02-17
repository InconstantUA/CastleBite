using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapContext : Singleton<MapContext>
{
    // inventory item which has been used
    public static InventoryItem ItemBeingUsed { get; set; }

}
