using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier If Unit Health Is Full")]
public class LimitModifierIfUnitHealthIsFull : ModifierLimiter
{
    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if destination context is of InventorySlotDropHandler type
        if (dstContext is InventorySlotDropHandler)
        {
            // ignore this limiter (don't discard)
            return false;
        }
        // get destination context as PartyUnit
        if (dstContext is PartyUnit)
        {
            PartyUnit dstPartyUnit = (PartyUnit)dstContext;
            // verify if destination unit health is already max
            if (dstPartyUnit.UnitHealthCurr >= dstPartyUnit.GetUnitEffectiveMaxHealth())
            {
                // limit
                return true;
            }
        }
        // don't limit
        return false;
    }
}
