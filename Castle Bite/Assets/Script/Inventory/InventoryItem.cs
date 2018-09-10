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
    public List<UniquePowerModifier> uniquePowerModifiers;
    public List<UnitStatModifier> unitStatModifiers;
    public HeroEquipmentSlot heroEquipmentSlot = HeroEquipmentSlot.None;
    public int maxUsagesCount;
    public int leftUsagesCount;
    // item location is determined by the parent object ID and it is saved and loaded together with parent object data, that is why no need to save it here
    // possible locations: equipped on the hero, in party inventory, lying on the map
}

public class InventoryItem : MonoBehaviour {
    [SerializeField]
    InventoryItemData inventoryItemData;

    public string GetUsagesInfo()
    {
        // verify if max usages count is not unlimited
        if (MaxUsagesCount >= 0)
        {
            // set update usages info
            return " <size=12>(" + LeftUsagesCount + "/" + MaxUsagesCount + ")</size>";
        }
        else
        {
            // set unlimited usages info
            return " <size=12>(-)</size>";
        }
    }

    public bool HasActiveModifiers()
    {
        return (HasActiveStatModifiers() || HasActiveUniquePowerModifiers());
    }

    public bool HasActiveStatModifiers()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm is applied actively
            if (usm.modifierApplied == ModifierApplied.Active)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasActiveUniquePowerModifiers()
    {
        // loop though all unique power modifiers
        foreach (UniquePowerModifier upm in UniquePowerModifiers)
        {
            // verify if usm is applied actively
            if (upm.modifierApplied == ModifierApplied.Active)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasActiveFriendlyUnitStatModifiers()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm is applied actively to a friendly unit
            if ((usm.modifierApplied == ModifierApplied.Active) && (usm.modifierScope == ModifierScope.FriendlyUnit))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasActiveFriendlyPartyStatModifiers()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm is applied actively to a firendly party
            if ((usm.modifierApplied == ModifierApplied.Active) && (usm.modifierScope == ModifierScope.FriendlyParty))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasPassiveUPMs()
    {
        // loop though all unique power modifier
        foreach (UniquePowerModifier upm in UniquePowerModifiers)
        {
            // verify if upm does not have instant duration
            if (upm.modifierApplied == ModifierApplied.Passive)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasPassiveUSMs()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm does not have instant duration
            if (usm.modifierApplied == ModifierApplied.Passive)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasPassiveModifiers()
    {
        return (HasPassiveUPMs() || HasPassiveUSMs());
    }

    public void RemoveExpiredUPMs()
    {
        List<int> upmIDsTobeRemoved = new List<int>();
        // loop though all unique power modifier
        for (int i = 0; i < UniquePowerModifiers.Count; i++)
        {
            // verify duration of unique power modifiers
            if (UniquePowerModifiers[i].upmDuration == 0)
            {
                upmIDsTobeRemoved.Add(i);
            }
        }
        // destroy one-time instant UPMs
        foreach (int upmIDTobeRemoved in upmIDsTobeRemoved)
        {
            UniquePowerModifiers.RemoveAt(upmIDTobeRemoved);
        }
    }

    public void RemoveExpiredUSMs()
    {
        List<int> usmIDsTobeRemoved = new List<int>();
        // loop though all unit stat modifiers
        for (int i = 0; i < UnitStatModifiers.Count; i++)
        {
            // verify duration of unit stat modifiers
            if (UnitStatModifiers[i].duration == 0)
            {
                usmIDsTobeRemoved.Add(i);
            }
        }
        // destroy one-time instant USMs
        foreach (int usmIDTobeRemoved in usmIDsTobeRemoved)
        {
            UnitStatModifiers.RemoveAt(usmIDTobeRemoved);
        }
    }

    public void RemoveExpiredModifiers()
    {
        RemoveExpiredUPMs();
        RemoveExpiredUSMs();
    }

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
