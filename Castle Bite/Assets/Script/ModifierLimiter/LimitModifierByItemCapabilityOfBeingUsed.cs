using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By Item Capability Of Being Used")]
public class LimitModifierByItemCapabilityOfBeingUsed : ModifierLimiter
{
    // this should not be modified (actually requirements are hard-coded)
    [ReadOnly]
    public bool shouldBeUsable = true; // example: consumable items: potions, or items which can summon, or items which can apply active/passive buff/debuff (vampire orb, flame essence).

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        if (srcContext is InventoryItem)
        {
            // match
            return true;
        }
        Debug.Log("Source context doesn't match");
        // doesn't match
        return false;
    }

    public bool DoesDestinationContextMatch(System.Object dstContext)
    {
        if (dstContext is PartyPanelCell)
        {
            // match
            return true;
        }
        else if (dstContext is PartyUnit)
        {
            // match
            return true;
        }
        Debug.Log("Destination context doesn't match");
        // doesn't match
        return false;
    }

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(srcContext, dstContext))
        {
            // context is not in scope of this limiter
            // don't limit
            return false;
        }
        // set context to Inventory Item
        InventoryItem inventoryItem = (InventoryItem)srcContext;
        // verify if inventory item is usable
        if (inventoryItem.IsUsable == shouldBeUsable)
        {
            // Usable - dont' limit modifier
            return false;
        }
        // Not usable - discard modifier
        return true;
    }

}
