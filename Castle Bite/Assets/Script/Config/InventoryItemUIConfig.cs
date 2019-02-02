using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Inventory/Item/UIConfig")]
public class InventoryItemUIConfig : ScriptableObject
{
    public Color itemIsApplicableColor;
    public Color itemIsNotApplicableColor;
}
