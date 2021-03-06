﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Applicable Only To x units")]
public class LimitModifierByAreaScope : ModifierLimiter
{
    [SerializeField]
    private ModifierScopeID modifierScopeID;
    [System.NonSerialized]
    private ModifierScope modifierScope;

    public ModifierScope ModifierScope
    {
        get
        {
            // verify if modifier scope has not been set
            if (modifierScope == null)
            {
                // set modifier scope
                modifierScope = new ModifierScope(modifierScopeID);
            }
            // return modifier scope
            return modifierScope;
        }
    }

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

    ValidationResult DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // verify if source or destination context do not match requirements of this limiter
        if (!DoesContextMatch(srcContext, dstContext))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        //if (dstContext is InventorySlotDropHandler)
        //{
        //    // ignore this limiter (don't discard)
        //    return false;
        //}
        switch (modifierScopeID)
        {
            case ModifierScopeID.Self:
                // verify if source context and destination context are of PartyUnit type (also verifies if it is not null)
                if ((srcContext is PartyUnit) && (dstContext is PartyUnit))
                {
                    // verify if source and destination party units are the same
                    if ( ((PartyUnit)srcContext).GetInstanceID() == ((PartyUnit)dstContext).GetInstanceID())
                    {
                        // don't limit
                        return ValidationResult.Pass();
                    }
                }
                // limit by default
                return ValidationResult.Discard(onDiscardMessage);
            case ModifierScopeID.SingleUnit:
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
                            return ValidationResult.Pass();
                        }
                        else
                        {
                            // limit
                            return ValidationResult.Discard(onDiscardMessage);
                        }
                    }
                    else
                    {
                        // limit
                        return ValidationResult.Discard(onDiscardMessage);
                    }
                }
                // verify if destination context is of PartyUnit type (also verifies if it is not null)
                if (dstContext is PartyUnit)
                {
                    // don't limit
                    return ValidationResult.Pass();
                }
                // limit by default
                return ValidationResult.Discard(onDiscardMessage);
            case ModifierScopeID.EntireParty:
                // verify if destination context is of Party or PartyUnit type (also verifies if it is not null)
                if ( (dstContext is HeroParty) || (dstContext is PartyUnit) )
                {
                    // don't limit
                    return ValidationResult.Pass();
                }
                // limit by default
                return ValidationResult.Discard(onDiscardMessage);
            case ModifierScopeID.AllPlayerUnits:
                // use case: global spells, player abilities
                // verify if destination context is of Player type (also verifies if it is not null)
                if ( (dstContext is GamePlayer) || (dstContext is HeroParty) || (dstContext is PartyUnit) )
                {
                    // don't limit
                    return ValidationResult.Pass();
                }
                // limit by default
                return ValidationResult.Discard(onDiscardMessage);
            default:
                Debug.LogError("Unknown modifier scope: " + modifierScopeID.ToString());
                // limit by default
                return ValidationResult.Discard("Unknown modifier scope");
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
        // verify if context matches edit party screen context
        if (context is EditPartyScreenContext)
        {
            // destination unit is set
            if (EditPartyScreenContext.DestinationUnitSlot != null)
                // context match
                return true;
        }
        // verify if context matches party unit propagation context
        if (context is PartyUnitPropagationContext)
        {
            // Get propagation context
            PartyUnitPropagationContext propagationContext = (PartyUnitPropagationContext)context;
            // verify if source party unit and destination unit is set
            if (propagationContext.SourcePartyUnit != null && propagationContext.DestinationPartyUnit != null)
                // context match
                return true;
        }
        // verify if context matches city propagation context
        if (context is CityPropagationContext)
        {
            // Get propagation context
            CityPropagationContext propagationContext = (CityPropagationContext)context;
            // verify if source city and destination unit is set
            if (propagationContext.SourceCity != null && propagationContext.DestinationPartyUnit != null)
                // context match
                return true;
        }
        // by default context doesn't match
        return false;
    }

    public override ValidationResult DoDiscardModifierInContextOf(System.Object context)
    {
        // verify if context doesn't match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        // verify if context matches battle context
        if (context is BattleContext)
        {
            switch (modifierScopeID)
            {
                case ModifierScopeID.Self:
                    // verify if destination (validated) unit slot is the same as active unit slot
                    if (BattleContext.DestinationUnitSlot.gameObject.GetInstanceID() == BattleContext.ActivePartyUnitUI.GetComponentInParent<UnitSlot>().gameObject.GetInstanceID())
                    {
                        // don't limit
                        return ValidationResult.Pass();
                    }
                    // limit by default
                    return ValidationResult.Discard(onDiscardMessage);
                case ModifierScopeID.SingleUnit:
                    // verify at which phase we are:
                    //  - new unit has just been activated (target unit is not set)
                    //  - applying ability from active unit to destination slot (target unit is set)
                    // verify if target unit have been set and that modifier scope is single unit and that destination slot has unit
                    if (BattleContext.TargetedUnitSlot != null)
                    {
                        // verify if tareted unit is the same as destination slot
                        if (BattleContext.TargetedUnitSlot.gameObject.GetInstanceID() == BattleContext.DestinationUnitSlot.gameObject.GetInstanceID())
                        {
                            // don't limit
                            return ValidationResult.Pass();
                        }
                        else
                        {
                            // limit
                            return ValidationResult.Discard(onDiscardMessage);
                        }
                    }
                    else
                    {
                        // don't limit, because target unit slot has not been set yet
                        return ValidationResult.Pass();
                    }
                case ModifierScopeID.EntireParty:
                    // don't limit
                    return ValidationResult.Pass();
                case ModifierScopeID.AllPlayerUnits:
                    // don't limit
                    return ValidationResult.Pass();
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScopeID.ToString());
                    // limit by default
                    return ValidationResult.Discard("Unknown modifier scope");
            }
        }
        // verify if context matches edit party screen context
        if (context is EditPartyScreenContext)
        {
            switch (modifierScopeID)
            {
                case ModifierScopeID.Self:
                    // this scope is not applicable in EditPartyScreen context
                    // don't limit
                    return ValidationResult.Pass();
                case ModifierScopeID.SingleUnit:
                    // verify at which phase we are:
                    //  - new unit has just been activated (target unit is not set)
                    //  - applying ability from active unit to destination slot (target unit is set)
                    // verify if target unit have been set and that modifier scope is single unit and that destination slot has unit
                    if (EditPartyScreenContext.TargetedUnitSlot != null)
                    {
                        // verify if tareted unit is the same as destination slot
                        if (EditPartyScreenContext.TargetedUnitSlot.gameObject.GetInstanceID() == EditPartyScreenContext.DestinationUnitSlot.gameObject.GetInstanceID())
                        {
                            // don't limit
                            return ValidationResult.Pass();
                        }
                        else
                        {
                            // limit
                            return ValidationResult.Discard(onDiscardMessage);
                        }
                    }
                    else
                    {
                        // don't limit, because target unit slot has not been set yet
                        return ValidationResult.Pass();
                    }
                case ModifierScopeID.EntireParty:
                    // don't limit
                    return ValidationResult.Pass();
                case ModifierScopeID.AllPlayerUnits:
                    // don't limit
                    return ValidationResult.Pass();
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScopeID.ToString());
                    // limit by default
                    return ValidationResult.Discard("Unknown modifier scope");
            }
        }
        // verify if context matches party unit propagation context
        if (context is PartyUnitPropagationContext)
        {
            // Get propagation context
            PartyUnitPropagationContext propagationContext = (PartyUnitPropagationContext)context;
            switch (modifierScopeID)
            {
                case ModifierScopeID.Self:
                    // verify if item owner is the same unit as destination (validated) unit
                    if (propagationContext.SourcePartyUnit.GetInstanceID() == propagationContext.DestinationPartyUnit.GetInstanceID())
                    {
                        // don't limit (allow propagation)
                        return ValidationResult.Pass();
                    }
                    // default: don't allow propagation, limit
                    return ValidationResult.Discard(onDiscardMessage);
                case ModifierScopeID.SingleUnit:
                    // this scope is not applicable for propagation use case
                    Debug.LogWarning("Single Unit scope is not applicable for propagation use case. Don't propagate");
                    // default: don't allow propagation, limit
                    return ValidationResult.Discard(onDiscardMessage);
                case ModifierScopeID.EntireParty:
                    // don't limit (allow propagation)
                    return ValidationResult.Pass();
                case ModifierScopeID.AllPlayerUnits:
                    // don't limit (allow propagation)
                    return ValidationResult.Pass();
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScopeID.ToString());
                    // limit by default
                    return ValidationResult.Discard("Unknown modifier scope");
            }
        }
        // verify if context matches city propagation context
        if (context is CityPropagationContext)
        {
            // Get propagation context
            //CityPropagationContext propagationContext = (CityPropagationContext)context;
            switch (modifierScopeID)
            {
                case ModifierScopeID.Self:
                    // this scope is not applicable for propagation use case
                    Debug.LogWarning("Self scope is not applicable for propagation use case. Don't propagate");
                    // default: don't allow propagation, limit
                    return ValidationResult.Discard(onDiscardMessage);
                case ModifierScopeID.SingleUnit:
                    // this scope is not applicable for propagation use case
                    Debug.LogWarning("Single Unit scope is not applicable for propagation use case. Don't propagate");
                    // default: don't allow propagation, limit
                    return ValidationResult.Discard(onDiscardMessage);
                case ModifierScopeID.EntireParty:
                    // don't limit (allow propagation)
                    return ValidationResult.Pass();
                case ModifierScopeID.AllPlayerUnits:
                    // don't limit (allow propagation)
                    return ValidationResult.Pass();
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScopeID.ToString());
                    // limit by default
                    return ValidationResult.Discard("Unknown modifier scope");
            }
        }
        // don't limit
        return ValidationResult.Pass();
    }

}
