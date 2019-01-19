using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Display multi-select popup for Flags enum correctly.
/// </summary>
public class EnumFlagAttribute : PropertyAttribute
{
}

[Serializable]
[Flags]
public enum HeroEquipmentSlots
{
    None        = 0,
    Shard       = 1 << 1,
    Head        = 1 << 2,
    Neck        = 1 << 3,
    RightHand   = 1 << 4,
    LeftHand    = 1 << 5,
    Chest       = 1 << 6,
    BeltSlot1   = 1 << 7,
    BeltSlot2   = 1 << 8,
    Boots       = 1 << 9,
    // commonly used variants
    BeltSlots   = BeltSlot1 | BeltSlot2
}

//[Serializable]
//public enum HeroEquipmentSlot
//{
//    Shard,
//    Head,
//    Neck,
//    RightHand,
//    LeftHand,
//    Chest,
//    BeltSlot1,
//    BeltSlot2,
//    Boots,
//    None
//}

[Serializable]
public enum InventoryItemID
{
    None                        =   0,
    // 1 Consumable BeltSlot1/2
    //  100-199 Health
    ScrollOfResurrection        =   1100,
    SmallHealingPoition         =   1101,
    MedumHealingPoition         =   1102,
    BigHealingPotion            =   1103,
    SuperHealingPotion          =   1104,
    //  200-299 Defense
    SmallFortificationPotion    =   1201,
    //  300-399 Power
    SmallStrengthPotion         =   1301,
    //  400-499 Initiative
    //  500-599 Move Points
    FlaskOfWellWater            =   1401,
    // 2 Equipment
    //  100-199 Shard
    ShardOfFristStrike          =   2101,
    ShardOfProtection           =   2102,
    //  200-299 Head
    EagleEyeHelmet              =   2201,
    GeneralHelmet               =   2202,
    MaskOfMadness               =   2203,
    //  300-399 Neck
    CloakOfAgility              =   2301,
    AmuletOfFireResistance      =   2302,
    //  400-499 Left/Right hand
    //      Protective  1-9
    AegisOfImmortal             =   2401,
    //      Attacking   20-29
    SwordOfDespair              =   2420,
    //      Orbs        30-39
    VampireOrb                  =   2430,
    //  500-599 Chest
    BoneArmor                   =   2501,
    //  600-699 Boots
    BootsOfSpeed                =   2601
}

[Serializable]
public class InventoryItemData : System.Object
{
    // public InventoryItemID inventoryItemType;
    public InventoryItemID inventoryItemID;
    // public InventoryItemConfig inventoryItemConfig;
    // public string itemName; // config
    // public int itemValue; // config
    // public HeroEquipmentSlot[] compatibleEquipmentSlots; // config
    private List<UnitStatModifierData> unitStatModifiersData; // this is created when item is being used to calculate duration left, if item is consumable
    public List<UnitStatModifier> unitStatModifiers; // config
    public List<UniquePowerModifier> uniquePowerModifiers; // config
    public List<UnitStatusModifier> unitStatusModifiers; // should behave based on stats // there shouldn't be more than 1 status modifier, because it does not make sense, because only one status can be active at a time
    public HeroEquipmentSlots currentHeroEquipmentSlot = HeroEquipmentSlots.None; // serializable data of equipped/unequipped item
    // public int maxUsagesCount; // config
    public int leftUsagesCount; // serializable
    // public bool itemIsStackable = false; // config // item effects can be re-applied using item of the same type
    // item location is determined by the parent object ID and it is saved and loaded together with parent object data, that is why no need to save it here
    // possible locations: equipped on the hero, in party inventory, lying on the map
}

public class InventoryItem : MonoBehaviour {
    [SerializeField]
    InventoryItemData inventoryItemData;
    InventoryItemConfig inventoryItemConfig; // initialized on first access

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
        return (HasActiveStatModifiers() || HasActiveUniquePowerModifiers() || HasActiveStatusModifiers());
    }

    public bool HasActiveStatModifiers()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm is applied actively
            if (usm.modifierAppliedHow == ModifierAppliedHow.Active)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasActiveStatusModifiers()
    {
        // all status modifiers are active by default
        // verify if there is at least one unit status modifier
        if (UnitStatusModifiers.Count >= 1)
        {
            return true;
        }
        return false;
    }

    public bool HasActiveUniquePowerModifiers()
    {
        // loop though all unique power modifiers
        foreach (UniquePowerModifier upm in UniquePowerModifiers)
        {
            // verify if usm is applied actively
            if (upm.modifierApplied == ModifierAppliedHow.Active)
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
            if (upm.modifierApplied == ModifierAppliedHow.Passive)
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
            if (usm.modifierAppliedHow == ModifierAppliedHow.Passive)
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

    public bool HasUSMModifiersAppliedToMaxStatValues()
    {
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm does not have instant duration
            if (usm.modifierAppliedTo == ModifierAppliedTo.MaxStat )
            {
                return true;
            }
        }
        return false;
    }

    public bool HasModifiersAppliedToMaxStatValues()
    {
        return HasUSMModifiersAppliedToMaxStatValues();
    }

    public void RemoveExpiredUPMs()
    {
        for (int i = UniquePowerModifiers.Count - 1; i >= 0; i--)
        {
            // verify if usm duration is 0 (instant upm) or duration left is 0
            if ((UniquePowerModifiers[i].UpmDurationMax == 0) || (UniquePowerModifiers[i].upmDurationLeft == 0))
            {
                UniquePowerModifiers.RemoveAt(i);
            }
        }
    }

    public void RemoveExpiredUSMs()
    {
        for (int i = UnitStatModifiers.Count - 1; i >= 0; i--)
        {
            // verify if usm duration is 0 (instant usm) or duration left is 0
            if ((UnitStatModifiers[i].duration == 0) || (UnitStatModifiers[i].durationLeft == 0))
            {
                // verify if USM is applied to current stat
                if (UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.CurrentStat)
                {
                    // just remove modifier
                    UnitStatModifiers.RemoveAt(i);
                }
                else if(UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.MaxStat)
                {
                    // no need to remove modifier because it is applied passively and will not be calculated automatically on USM remove
                    //// remove modifier effect on unit's max stat value
                    //transform.parent.GetComponent<PartyUnit>().RemoveUSMEffectOnMaxStatValue(UnitStatModifiers[i]);
                    // remove modifier
                    UnitStatModifiers.RemoveAt(i);
                }
                else
                {
                    Debug.LogError("Do not know how modifier is applied");
                }
            }
        }
    }

    public void RemoveExpiredModifiers()
    {
        RemoveExpiredUPMs();
        RemoveExpiredUSMs();
        // status modifiers do not have duration attribute, they just turn some status On
    }

    public void DecrementModifiersDuration()
    {
        // loop though all unit stat modifiers
        foreach (UnitStatModifier usm in UnitStatModifiers)
        {
            // verify if usm has non-permanent duration
            if (usm.duration >= 1)
            {
                // decrement duration left
                usm.durationLeft -= 1;
            }
        }
        // loop though all unique power modifier
        foreach (UniquePowerModifier upm in UniquePowerModifiers)
        {
            // verify if usm has non-permanent duration
            if (upm.UpmDurationMax >= 0)
            {
                // decrement duration left
                upm.upmDurationLeft -= 1;
            }
        }
    }

    public void SelfDestroyIfExpired()
    {
        // verify if item has passive modifiers
        if (HasPassiveModifiers())
        {
            Debug.Log("Item has passive modifiers.");
            // do not do anything to the item
            // instant modifiers are already applied
            // non-instant modifiers are also applied by placing item into the unit's inventory
        }
        else if (HasModifiersAppliedToMaxStatValues())
        {
            Debug.Log("Item has modifiers applied to max stat values.");
            // do not do anything to the item
            // those modifiers with their effects should be removed after duration has expired
        }
        else
        {
            // only modifiers which are applied to current stat values are left here
            // verify if item run out of usages
            if (LeftUsagesCount == 0)
            {
                // inventory item only does not have passive modifiers
                // item should be destroyed, because it has nothing applied afterwards
                Debug.Log("Destroy item");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Item still has usages left");
                // item still has usages left
                // it will drop back to its original slot
            }
        }
    }

    public void ExecutePreTurnActions()
    {
        DecrementModifiersDuration();
        RemoveExpiredModifiers();
        SelfDestroyIfExpired();
    }

    #region Properties

    public InventoryItemConfig InventoryItemConfig
    {
        get
        {
            // verify if config is not null
            if (inventoryItemConfig == null)
            {
                // get item config from configs manager by item type
                inventoryItemConfig = Array.Find(ConfigManager.Instance.InventoryItemConfigs, e => e.inventoryItemID == InventoryItemID);
            }
            return inventoryItemConfig;
        }
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

    public InventoryItemID InventoryItemID
    {
        get
        {
            return inventoryItemData.inventoryItemID;
        }

        set
        {
            inventoryItemData.inventoryItemID = value;
        }
    }

    public string ItemName
    {
        get
        {
            //return inventoryItemData.itemName;
            return InventoryItemConfig.itemDisplayName;
        }

        //set
        //{
        //    inventoryItemData.itemName = value;
        //}
    }

    public int ItemCost
    {
        get
        {
            //return inventoryItemData.itemValue;
            return InventoryItemConfig.itemCost;
        }

        //set
        //{
        //    inventoryItemData.itemValue = value;
        //}
    }

    public HeroEquipmentSlots CompatibleEquipmentSlots
    {
        get
        {
            return InventoryItemConfig.compatibleEquipmentSlots;
        }
    }
    //public HeroEquipmentSlot[] CompatibleEquipmentSlots
    //{
    //    get
    //    {
    //        return inventoryItemData.compatibleEquipmentSlots;
    //    }

    //    set
    //    {
    //        inventoryItemData.compatibleEquipmentSlots = value;
    //    }
    //}

    public int MaxUsagesCount
    {
        get
        {
            //return inventoryItemData.maxUsagesCount;
            return InventoryItemConfig.maxUsagesCount;
        }

        //set
        //{
        //    inventoryItemData.maxUsagesCount = value;
        //}
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

    //public HeroEquipmentSlot CurrentHeroEquipmentSlot
    //{
    //    get
    //    {
    //        return inventoryItemData.heroEquipmentSlot;
    //    }

    //    set
    //    {
    //        inventoryItemData.heroEquipmentSlot = value;
    //    }
    //}
    public HeroEquipmentSlots CurrentHeroEquipmentSlot
    {
        get
        {
            return inventoryItemData.currentHeroEquipmentSlot;
        }

        set
        {
            inventoryItemData.currentHeroEquipmentSlot = value;
        }
    }

    public bool ItemIsStackable
    {
        get
        {
            //return inventoryItemData.itemIsStackable;
            return InventoryItemConfig.itemIsStackable;
        }

        //set
        //{
        //    inventoryItemData.itemIsStackable = value;
        //}
    }

    public List<UnitStatusModifier> UnitStatusModifiers
    {
        get
        {
            return inventoryItemData.unitStatusModifiers;
        }

        set
        {
            inventoryItemData.unitStatusModifiers = value;
        }
    }
    #endregion Properties

}
