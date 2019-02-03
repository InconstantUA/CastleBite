using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By Unit Status")]
public class LimitModifierByUnitStatus : ModifierLimiter
{
    public UnitStatus[] requiredAnyOfUnitStatus;

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
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
