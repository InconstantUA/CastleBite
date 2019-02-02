using System.Collections;
using System.Collections.Generic;
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
    public List<UnitStatModifierConfig> unitStatModifierConfigs; // TODO: replace with uniquePowerModifierConfigs
    public ModifierAppliedHow modifierAppliedHow;
    public int maxUsagesCount;
    public bool itemIsStackable = false; // item effects can be combined (added one to each other) by using item of the same type
    public List<UniquePowerModifierConfig> uniquePowerModifierConfigs;
    public InventoryItemUIConfig inventoryItemUIConfig;
}
