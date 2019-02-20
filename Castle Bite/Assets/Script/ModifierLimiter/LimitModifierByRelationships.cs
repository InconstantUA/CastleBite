using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Required Relationships")]
public class LimitModifierByRelationships : ModifierLimiter
{
    public Relationships.State[] requiredAnyOfRelationships; // friendly, enemy, neutral, etc.

    Faction GetFaction(System.Object context)
    {
        // verify if context is not null
        if (context == null)
        {
            return Faction.Unknown;
        }
        // verify if context is of PartyUnit type
        if (context is PartyUnit)
        {
            // get party
            HeroParty heroParty = ((PartyUnit)context).GetUnitParty();
            // verify if hero Party is not null
            if (heroParty != null)
            {
                // return party faction
                return heroParty.Faction;
            }
            else
            {
                Debug.LogWarning(((PartyUnit)context).GetUnitDisplayName() + " unit has no Party");
            }
        }
        // verify if context is of HeroParty type
        if (context is HeroParty)
        {
            // return party faction
            return ((HeroParty)context).Faction;
        }
        // verify if context is of City type
        if (context is City)
        {
            // return city faction
            return ((City)context).CityCurrentFaction;
        }
        // verify if context is of Player type
        if (context is GamePlayer)
        {
            // return player faction
            return ((GamePlayer)context).Faction;
        }
        // by default return unknown faction
        Debug.LogWarning("Unknown Faction");
        return Faction.Unknown;
    }

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        if ((srcContext is GamePlayer) || (srcContext is City) || (srcContext is HeroParty) || (srcContext is PartyUnit))
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
        if ((dstContext is GamePlayer) || (dstContext is City) || (dstContext is HeroParty) || (dstContext is PartyUnit))
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
        //// verify if destination context is of HeroEquipmentSlots type
        //if (dstContext is InventorySlotDropHandler)
        //{
        //    // ignore this limiter (don't discard)
        //    return false;
        //}
        // get relationships
        Relationships.State relationships = Relationships.Instance.GetRelationships( GetFaction(srcContext), GetFaction(dstContext) );
        // loop through all required relationships
        foreach (Relationships.State relation in requiredAnyOfRelationships)
        {
            // verify if relationships match
            if (relation == relationships)
            {
                // don't limit
                return ValidationResult.Pass();
            }
        }
        // if none of required relations match then limit modifier
        return ValidationResult.Discard(onDiscardMessage);
    }

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // verify if active and destination units are set
            if (BattleContext.ActivePartyUnitUI != null && BattleContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
        }
        // verify if context matches edit party screen context
        if (context is EditPartyScreenContext)
        {
            // verify if destination unit is set
            // we assume that when edit party screen is active, then all units have the same faction
            if (EditPartyScreenContext.DestinationUnitSlot != null)
            {
                // context match
                return true;
            }
        }
        // verify if context matches edit party screen context
        if (context is ItemPropagationContext)
        {
            // Get propagation context
            ItemPropagationContext propagationContext = (ItemPropagationContext)context;
            // verify if source item and destination unit is set
            if (propagationContext.SourceItem != null && propagationContext.DestinationPartyUnit != null)
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
            // get source hero party
            HeroParty sourceParty = BattleContext.ActivePartyUnitUI.GetComponentInParent<HeroPartyUI>().LHeroParty;
            // get destination hero party
            HeroParty destinationParty = BattleContext.DestinationUnitSlot.GetComponentInParent<HeroPartyUI>().LHeroParty;
            // verify if we need to discard modifier
            return DoDiscardModifierInContextOf(sourceParty, destinationParty);
        }
        // verify if context matches battle context
        if (context is EditPartyScreenContext)
        {
            // loop through all required relationships
            foreach (Relationships.State relation in requiredAnyOfRelationships)
            {
                // verify if relationships match same faction
                // we assume that when edit party screen is active, then all units have the same faction
                if (relation == Relationships.State.SameFaction)
                {
                    // don't limit
                    return ValidationResult.Pass();
                }
            }
            // if none of required relations match then limit modifier
            return ValidationResult.Discard(onDiscardMessage);
        }
        // verify if context matches item propagation context
        if (context is ItemPropagationContext)
        {
            // Get propagation context
            ItemPropagationContext propagationContext = (ItemPropagationContext)context;
            // get item owner
            PartyUnit itemOwnerPartyUnit = propagationContext.SourceItem.GetComponentInParent<PartyUnit>();
            // verify if item owner is not null = item is being equipped
            if (itemOwnerPartyUnit != null)
            {
                // get item owner party
                HeroParty itemOwnerParty = itemOwnerPartyUnit.GetComponentInParent<HeroParty>();
                // get destination unit party
                HeroParty destinationUnitParty = propagationContext.DestinationPartyUnit.GetComponentInParent<HeroParty>();
                // verify if both parties are not null
                if (itemOwnerParty != null && destinationUnitParty != null)
                {
                    // get relationships between item ower and destination (validated) unit
                    Relationships.State relations = Relationships.Instance.GetRelationships(itemOwnerParty.Faction, destinationUnitParty.Faction);
                    // loop through all required relationships
                    foreach (Relationships.State relation in requiredAnyOfRelationships)
                    {
                        // verify if relationships match required
                        if (relation == relations)
                        {
                            // don't limit
                            return ValidationResult.Pass();
                        }
                    }
                }
            }
            // default: don't allow propagation, limit
            return ValidationResult.Discard(onDiscardMessage);
        }
        // default: don't limit
        return ValidationResult.Pass();
    }

}
