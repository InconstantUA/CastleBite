using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By x Unit Skill level")]
public class LimitModifierByUnitSkill: ModifierLimiter
{
    public UnitSkillID requiredUnitSkillID;
    public int minRequiredUnitSkillLevel = 1;

    public override bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // set context to party unit
        PartyUnit partyUnit = (PartyUnit)dstContext;
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
