using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Inventory/Item/Config")]
public class InventoryItemConfig : ScriptableObject
{
    // public InventoryItemID inventoryItemType;
    public InventoryItemID inventoryItemID;
    public string itemDisplayName;
    public int itemCost;
    [EnumFlag]
    public HeroEquipmentSlots compatibleEquipmentSlots;
    // public List<UnitStatModifierConfig> unitStatModifierConfigs; // TODO: replace with uniquePowerModifierConfigs
    // public ModifierAppliedHow modifierAppliedHow;
    public bool isUsable;
    public int maxUsagesCount;
    public bool itemIsStackable = false; // item effects can be combined (added one to each other) by using item of the same type
    public List<UniquePowerModifierConfig> uniquePowerModifierConfigs;
    public InventoryItemUIConfig inventoryItemUIConfig;

    [NonSerialized]
    private UniquePowerModifierConfig primaryUniquePowerModifierConfig;
    [NonSerialized]
    private List<UniquePowerModifierConfig> uniquePowerModifierConfigsSortedByExecutionOrder;

    public UniquePowerModifierConfig PrimaryUniquePowerModifierConfig
    {
        get
        {
            // verify if primary upm is not set yet
            if (primaryUniquePowerModifierConfig == null)
            {
                // get UPM which has primary attribute set (should be only one)
                primaryUniquePowerModifierConfig = uniquePowerModifierConfigs.Find(e => e.IsPrimary == true);
            }
            return primaryUniquePowerModifierConfig;
        }
    }

    public List<UniquePowerModifierConfig> UniquePowerModifierConfigsSortedByExecutionOrder
    {
        get
        {
            // verify if uniquePowerModifierConfigsSortedByExecutionOrder is not set yet
            if (uniquePowerModifierConfigsSortedByExecutionOrder == null)
            {
                // set it
                uniquePowerModifierConfigsSortedByExecutionOrder = uniquePowerModifierConfigs.OrderBy(o => o.ExecutionOrder).ToList();
            }
            // return uniquePowerModifierConfigs Sorted By Execution Order
            return uniquePowerModifierConfigsSortedByExecutionOrder;
        }
    }
}
