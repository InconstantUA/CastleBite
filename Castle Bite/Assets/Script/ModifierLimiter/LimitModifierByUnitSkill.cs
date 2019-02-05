using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By x Unit Skill level")]
public class LimitModifierByUnitSkill: ModifierLimiter
{
    public UnitSkillID requiredUnitSkillID;
    public int minRequiredUnitSkillLevel = 1;

    public bool DoesContextMatch(System.Object srcContext, System.Object dstContext)
    {
        return DoesSourceContextMatch(srcContext) && DoesDestinationContextMatch(dstContext);
    }

    public bool DoesSourceContextMatch(System.Object srcContext)
    {
        if (srcContext == null)
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
        if ( (dstContext is ItemSlotDropHandler) || (dstContext is PartyUnit) )
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
        // init party unit variable (skill bearer)
        PartyUnit partyUnit;
        // verify if destination context is of InventorySlotDropHandler type
        if (dstContext is ItemSlotDropHandler)
        {
            // get party unit context from InventorySlotDropHandler
            partyUnit = ((ItemSlotDropHandler)dstContext).GetComponentInParent<HeroEquipment>().LPartyUnit;
        }
        // verify if destination context is of PartyUnit type
        else if (dstContext is PartyUnit)
        {
            // set context to party unit
            partyUnit = (PartyUnit)dstContext;
        }
        // verify if context is not defined
        else
        {
            Debug.LogError("Destination context is not known");
            // limit
            return true;
        }
        // verify if party unit current skill level matches required minimum unit skill level
        if (partyUnit.GetUnitSkillData(requiredUnitSkillID).currentSkillLevel >= minRequiredUnitSkillLevel)
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

    public override string OnLimitMessage
    {
        get
        {
            return "Requires " + requiredUnitSkillID + " level " + minRequiredUnitSkillLevel;
        }
    }
}
