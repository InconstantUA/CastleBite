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

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // get relationships
        Relationships.State relationships = Relationships.Instance.GetRelationships( GetFaction(srcContext), GetFaction(dstContext) );
        // loop through all required relationships
        foreach (Relationships.State relation in requiredAnyOfRelationships)
        {
            // verify if relationships match
            if (relation == relationships)
            {
                // don't limit
                return false;
            }
        }
        // if none of required relations match then limit modifier
        return true;
    }

}
