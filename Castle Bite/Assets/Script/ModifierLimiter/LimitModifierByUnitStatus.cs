using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By Unit Status")]
public class LimitModifierByUnitStatus : ModifierLimiter
{
    public UnitStatus[] requiredAnyOfUnitStatus;

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        // ignore source context
        return true;
    }

    public bool DoesDestinationContextMatch(System.Object dstContext)
    {
        if (dstContext is PartyUnit)
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
        //// verify if destination context is of InventorySlotDropHandler type
        //if (dstContext is InventorySlotDropHandler)
        //{
        //    // ignore this limiter (don't discard)
        //    return false;
        //}
        // set context to party unit
        PartyUnit dstPartyUnit = (PartyUnit)dstContext;
        // loop through all required statuses
        foreach (UnitStatus matchStatus in requiredAnyOfUnitStatus)
        {
            if (dstPartyUnit.UnitStatus == matchStatus)
            {
                // don't limit
                return false;
            }
        }
        // No any of required statuses match - discard modifier
        return true;
    }
}
