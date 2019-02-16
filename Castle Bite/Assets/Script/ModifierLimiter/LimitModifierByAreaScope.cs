﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Applicable Only To x units")]
public class LimitModifierByAreaScope : ModifierLimiter
{
    public ModifierScope modifierScope;

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        if (srcContext is PartyUnit)
        {
            // match
            return true;
        }
        if (srcContext is UnitSlotDropHandler)
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
        if ((dstContext is GamePlayer) || (dstContext is HeroParty) || (dstContext is PartyUnit))
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
        //if (dstContext is InventorySlotDropHandler)
        //{
        //    // ignore this limiter (don't discard)
        //    return false;
        //}
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
                // verify if source context is UnitSlotDropHandler and destination context is of PartyUnit type
                if (srcContext is UnitSlotDropHandler && dstContext is PartyUnit)
                {
                    // get unit UI in slot drop handler
                    PartyUnitUI partyUnitUI = ((UnitSlotDropHandler)srcContext).GetComponentInChildren<PartyUnitUI>();
                    // verify if it is not null
                    if (partyUnitUI != null)
                    {
                        // make sure that UPM is applied only to destination unit if modifier scope is single unit
                        // verify if this unit belongs to this unit slot (normally this is the slot to which item has been dropped) (IDs of units are the same)
                        if (partyUnitUI.LPartyUnit == (PartyUnit)dstContext)
                        {
                            // don't limit
                            return false;
                        }
                        else
                        {
                            // limit
                            return true;
                        }
                    }
                    else
                    {
                        // limit
                        return true;
                    }
                }
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

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if active unit and destination units are set
            if (BattleContext.ActivePartyUnitUI != null && BattleContext.DestinationUnitSlot != null)
            // context match
            return true;
        }
        // by default context doesn't match
        return false;
    }

    public override bool DoDiscardModifierInContextOf(System.Object context)
    {
        // verify if context doesn't match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this limiter
            // don't limit
            return false;
        }
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify at which phase we are:
            //  - new unit has just been activated (target unit is not set)
            //  - applying ability from active unit to destination slot (target unit is set)
            // verify if target unit have been set and that modifier scope is single unit and that destination slot has unit
            if (BattleContext.TargetedUnitSlot != null && modifierScope == ModifierScope.SingleUnit)
            {
                // verify if tareted unit is the same as destination slot
                if (BattleContext.TargetedUnitSlot.gameObject.GetInstanceID() == BattleContext.DestinationUnitSlot.gameObject.GetInstanceID())
                {
                    // don't limit
                    return false;
                }
                else
                {
                    // limit
                    return true;
                }
            }
            else
            {
                // use default verify if we need to discard modifier
                return DoDiscardModifierInContextOf(BattleContext.ActivePartyUnitUI, BattleContext.DestinationUnitSlot);
            }

        }
        // don't limit
        return false;
    }

}