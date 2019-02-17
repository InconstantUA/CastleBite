using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier If Unit Health Is Full")]
public class LimitModifierIfUnitHealthIsFull : ModifierLimiter
{
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

    ValidationResult DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(srcContext, dstContext))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        //// verify if destination context is of InventorySlotDropHandler type
        //if (dstContext is InventorySlotDropHandler)
        //{
        //    // ignore this limiter (don't discard)
        //    return false;
        //}
        // get destination context as PartyUnit
        if (dstContext is PartyUnit)
        {
            PartyUnit dstPartyUnit = (PartyUnit)dstContext;
            // verify if destination unit health is already max
            if (dstPartyUnit.UnitHealthCurr >= dstPartyUnit.GetUnitEffectiveMaxHealth())
            {
                // limit
                return ValidationResult.Discard(onDiscardMessage);
            }
        }
        // don't limit
        return ValidationResult.Pass();
    }

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if destination unit is set
            if (BattleContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
        }
        // verify if context matches edit party screen context
        if (context is EditPartyScreenContext)
        {
            // verify if destination unit is set
            if (EditPartyScreenContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
        }
        // by default context doesn't match
        return false;
    }

    public override ValidationResult DoDiscardModifierInContextOf(System.Object context)
    {
        // verify if context doesn't match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // verify if destination unit is set
            if (BattleContext.DestinationUnitSlot != null)
            {
                // context is not in scope of this limiter
                // don't limit
                return ValidationResult.Pass();
            }
        }
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // get party unit UI in destination slot
            PartyUnitUI partyUnitUI = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>();
            // verify if destination slot has unit
            if (partyUnitUI != null)
            {
                // verify if we need to discard this modifier
                //  ignore source context
                return DoDiscardModifierInContextOf(null, partyUnitUI.LPartyUnit);
            }
        }
        // verify if context matches EditPartyScreenContext context
        if (context is EditPartyScreenContext)
        {
            // get party unit UI in destination slot
            PartyUnitUI partyUnitUI = EditPartyScreenContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>();
            // verify if destination slot has unit
            if (partyUnitUI != null)
            {
                // verify if we need to discard this modifier
                //  ignore source context
                return DoDiscardModifierInContextOf(null, partyUnitUI.LPartyUnit);
            }
        }
        // don't limit
        return ValidationResult.Pass();
    }
}
