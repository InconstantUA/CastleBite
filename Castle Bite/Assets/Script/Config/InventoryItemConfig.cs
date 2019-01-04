using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Inventory/Item")]
public class InventoryItemConfig : ScriptableObject
{
    public InventoryItemType inventoryItemType;
    public string itemDisplayName;
    public int itemCost;
    [EnumFlag]
    public HeroEquipmentSlots compatibleEquipmentSlots;
    public List<UnitStatModifierConfig> unitStatModifierConfigs;
    public ModifierAppliedHow modifierAppliedHow;
    public int maxUsagesCount;
    public bool itemIsStackable = false; // item effects can be combined (added one to each other) by using item of the same type
}
