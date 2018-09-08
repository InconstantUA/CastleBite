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
    Boots,
    None
}

[Serializable]
public class InventoryItemData : System.Object
{
    public string itemName;
    public int itemValue;
    public HeroEquipmentSlot[] compatibleEquipmentSlots;
    public int maxUsagesCount;
    public List<UniquePowerModifier> uniquePowerModifiers;
    public List<UnitStatModifier> unitStatModifiers;
    public int leftUsagesCount;
    public HeroEquipmentSlot heroEquipmentSlot = HeroEquipmentSlot.None;
    // item location is determined by the parent object ID and it is saved and loaded together with parent object data, that is why no need to save it here
    // possible locations: equipped on the hero, in party inventory, lying on the map
}

public class InventoryItem : MonoBehaviour {
    [SerializeField]
    InventoryItemData inventoryItemData;

    public InventoryItemData InventoryItemData
    {
        get
        {
            return inventoryItemData;
        }

        set
        {
            inventoryItemData = value;
        }
    }

    public string ItemName
    {
        get
        {
            return inventoryItemData.itemName;
        }

        set
        {
            inventoryItemData.itemName = value;
        }
    }

    public int ItemValue
    {
        get
        {
            return inventoryItemData.itemValue;
        }

        set
        {
            inventoryItemData.itemValue = value;
        }
    }

    public HeroEquipmentSlot[] CompatibleEquipmentSlots
    {
        get
        {
            return inventoryItemData.compatibleEquipmentSlots;
        }

        set
        {
            inventoryItemData.compatibleEquipmentSlots = value;
        }
    }

    public int MaxUsagesCount
    {
        get
        {
            return inventoryItemData.maxUsagesCount;
        }

        set
        {
            inventoryItemData.maxUsagesCount = value;
        }
    }

    public List<UniquePowerModifier> UniquePowerModifiers
    {
        get
        {
            return inventoryItemData.uniquePowerModifiers;
        }

        set
        {
            inventoryItemData.uniquePowerModifiers = value;
        }
    }

    public List<UnitStatModifier> UnitStatModifiers
    {
        get
        {
            return inventoryItemData.unitStatModifiers;
        }

        set
        {
            inventoryItemData.unitStatModifiers = value;
        }
    }

    public int LeftUsagesCount
    {
        get
        {
            return inventoryItemData.leftUsagesCount;
        }

        set
        {
            inventoryItemData.leftUsagesCount = value;
        }
    }

    public HeroEquipmentSlot HeroEquipmentSlot
    {
        get
        {
            return inventoryItemData.heroEquipmentSlot;
        }

        set
        {
            inventoryItemData.heroEquipmentSlot = value;
        }
    }

}
