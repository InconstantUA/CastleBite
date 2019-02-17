using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier updater (used by unique power modifier)
[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier By Requiring Active Unit In Battle")]
public class LimitModifierByRequiringActiveUnitInBattle : ModifierLimiter
{
    bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // context match
            return true;
        }
        // verify if context matches equipment context
        if (context is EditPartyScreen)
        {
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
            // it still can be equipped and doesn't affect EquipmentScreenContext
            // don't limit
            return ValidationResult.Pass();
        }
        // verify if it is battle context
        if (context is BattleContext)
        {
            // verify if destination unit slot is set
            if (BattleContext.ActivePartyUnitUI != null)
            {
                // active unit is present - don't limit
                return ValidationResult.Pass();
            }
            else
            {
                // no active unit - discard
                return ValidationResult.Discard(onDiscardMessage);
            }
        }
        // verify if context matches EditPartyScreen context
        if (context is EditPartyScreen)
        {
            // BattleContext is one of requirements of this limitier, so if context doesn't match, then limit
            return ValidationResult.Discard(onDiscardMessage);
        }
        // but it still can be equipped and doesn't affect EquipmentScreenContext
        // default don't limit
        return ValidationResult.Pass();
    }

}
