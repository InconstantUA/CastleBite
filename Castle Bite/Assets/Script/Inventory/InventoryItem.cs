using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum HeroEquipmentSlot
{
    Shard,
    Head,
    Neck,
    RightHand,
    LeftHand,
    Chest,
    BeltSlot1,
    BeltSlot2,
    Boots
}

[Serializable]
public class InventoryItemData : System.Object
{
    public string itemName;
    public int itemValue;
    public HeroEquipmentSlot[] compatibleEquipmentSlots;
    public bool isConsumableItem;
    public UniquePowerModifier[] uniquePowerModifiers;
    public UnitStatModifier[] unitStatModifiers;
    // item location is determined by the parent object ID and it is saved and loaded together with parent object data, that is why no need to save it here
    // possible locations: equipped on the hero, in party inventory, lying on the map
}

public class InventoryItem : MonoBehaviour {
    [SerializeField]
    InventoryItemData inventoryItemData;


}
