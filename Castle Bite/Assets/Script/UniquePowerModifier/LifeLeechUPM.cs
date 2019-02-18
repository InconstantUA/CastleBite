using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/LifeLeechUPM")]
public class LifeLeechUPM : UniquePowerModifier
{

    //public override void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    //{
    //    throw new NotImplementedException();
    //}
    //public override void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    //{
    //    throw new NotImplementedException();
    //}

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is BattleContext)
        {
            // .. this can be skipped because this verification (should be) done before upm is applied
            // verify if destination slot has a unit UI
            if (BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>() != null)
            {
                // context match
                return true;
            }
        }
        // by default context doesn't match
        return false;
    }

    public override void Apply(System.Object context)
    {
        // verify if context doesn't match requirements of this UPM
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this UPM
            // skip all actions
            return;
        }
        if (context is BattleContext)
        {
            // get active unit
            PartyUnit activePartyUnit = BattleContext.ActivePartyUnitUI.LPartyUnit;
            // get target unit
            PartyUnit targetPartyUnit = BattleContext.TargetedUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
            UniquePowerModifierConfig uniquePowerModifierConfig = activePartyUnit.UnitAbilityConfig.UniquePowerModifierConfigsSortedByExecutionOrder[BattleContext.ActivatedUPMConfigIndex];
            Debug.LogWarning("Applying " + uniquePowerModifierConfig.DisplayName + " from " + activePartyUnit.UnitName + " to " + targetPartyUnit.UnitName + ", origin is " + BattleContext.UniquePowerModifierID.modifierOrigin);
            // validate if it is really instant UPM (max duration) is 0
            // .. idea: do it in editor with warning highlight
            if (uniquePowerModifierConfig.UpmDurationMax != 0)
            {
                Debug.LogWarning("UpmDurationMax should be 0");
            }
            // instantly trigger UPM, but apply it to src unit as heal
            // Get UPM effective power
            int upmEffectiPower = uniquePowerModifierConfig.GetUpmEffectivePower(activePartyUnit);
            // init damage dealt variable
            int damageDealt = upmEffectiPower; // current power is negative if it is damage dealing ability
            // verify if damage dealt is not higher than current unit health
            if (Math.Abs(damageDealt) > targetPartyUnit.UnitHealthCurr)
            {
                // reset damage dealt to the current unit health
                damageDealt = targetPartyUnit.UnitHealthCurr;
            }
            // Heal active unit to amout of damage dealt
            Debug.Log("Heal " + activePartyUnit.UnitName + " for " + Math.Abs(damageDealt) + " health");
            activePartyUnit.UnitHealthCurr += Math.Abs(damageDealt);
            // Damage target unit
            targetPartyUnit.UnitHealthCurr += upmEffectiPower;
        }
    }

    // not used
    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        throw new NotImplementedException();
    }

}
