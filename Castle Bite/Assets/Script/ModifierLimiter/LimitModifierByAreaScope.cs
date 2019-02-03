using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Applicable Only To x units")]
public class LimitModifierByAreaScope : ModifierLimiter
{
    public ModifierScope modifierScope;

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        switch (modifierScope)
        {
            case ModifierScope.Self:
                // verify if source context and destination context are of PartyUnit type (also verifies if it is not null)
                if ((srcContext is PartyUnit) && (dstContext is PartyUnit))
                {
                    // verify if source and destination party units are the same
                    if ( ((PartyUnit)srcContext).GetInstanceID() == ((PartyUnit)dstContext).GetInstanceID())
                    {
                        // don't limit
                        return false;
                    }
                }
                // limit by default
                return true;
            case ModifierScope.SingleUnit:
                // verify if destination context is of PartyUnit type (also verifies if it is not null)
                if (dstContext is PartyUnit)
                {
                    // don't limit
                    return false;
                }
                // limit by default
                return true;
            case ModifierScope.EntireParty:
                // verify if destination context is of Party or PartyUnit type (also verifies if it is not null)
                if ( (dstContext is HeroParty) || (dstContext is PartyUnit) )
                {
                    // don't limit
                    return false;
                }
                // limit by default
                return true;
            case ModifierScope.AllPlayerUnits:
                // use case: global spells, player abilities
                // verify if destination context is of Player type (also verifies if it is not null)
                if ( (dstContext is GamePlayer) || (dstContext is HeroParty) || (dstContext is PartyUnit) )
                {
                    // don't limit
                    return false;
                }
                // limit by default
                return true;
            default:
                Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
                // limit by default
                return true;
        }
    }

}
