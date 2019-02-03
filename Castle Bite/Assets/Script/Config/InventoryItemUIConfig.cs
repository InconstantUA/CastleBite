using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Inventory/Item/UIConfig")]
public class InventoryItemUIConfig : ScriptableObject
{
    public Color itemIsApplicableForUnitSlotColor;
    public Color itemIsNotApplicableForUnitSlotColor;
    public Color itemCanBeEquippedColor;
    public Color itemIsNotCompatibleWithEquipmentSlotColor;
    public Color itemPrerequsitesAreNotMetForThisUnitColor;
}
