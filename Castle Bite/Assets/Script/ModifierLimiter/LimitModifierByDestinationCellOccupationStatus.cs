﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By Destination Cell Occupation Status")]
public class LimitModifierByDestinationCellOccupationStatus : ModifierLimiter
{
    public bool shouldBeOccupied;

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
        if (dstContext is PartyPanelCell)
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
            // context is not in scope of this limiter - don't limit
            return false;
        }
        // set context to party unit
        PartyPanelCell dstPartyPanelCell = (PartyPanelCell)dstContext;
        // verify if destination party panel cell occupation status matches required
        if (dstPartyPanelCell.IsOccupied() == shouldBeOccupied)
        {
            // Required occupation status matches with current occupations status - dont' limit modifier
            return false;
        }
        // Required occupation status doesn't match with current occupations status - discard modifier
        return true;
    }

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if destination unit slot is set
            if (BattleContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
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
            // verify if we need to discard modifier
            return DoDiscardModifierInContextOf(null, BattleContext.DestinationUnitSlot.GetComponentInParent<PartyPanelCell>());
        }
        // don't limit
        return false;
    }


}
