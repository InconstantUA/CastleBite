using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/BaseInstantHealingOrDamagingUPM")]
public class BaseInstantHealingOrDamagingUPM : UniquePowerModifier
{

    public override void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        Debug.Log("Applying " + uniquePowerModifierConfig.DisplayName + " from " + srcPartyUnit.UnitName + " to " + dstPartyUnit.UnitName + ", origin is " + uniquePowerModifierID.modifierOrigin);
        // init upm data variable (this is required for Trigger)
        UniquePowerModifierData upmData = new UniquePowerModifierData
        {
            // set UPM ID
            UniquePowerModifierID = uniquePowerModifierID,
            // rest upm duration left to max duration
            DurationLeft = uniquePowerModifierConfig.UpmDurationMax,
            // set upm current power based on source unit stats upgrades count
            //CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
            CurrentPower = uniquePowerModifierConfig.GetUpmEffectivePower(srcPartyUnit)
        };
        // validate if it is really instant UPM (max duration) is 0
        // .. idea: do it in editor with warning highlight
        if (uniquePowerModifierConfig.UpmDurationMax != 0)
        {
            Debug.LogWarning("UpmDurationMax should be 0");
        }
        // instantly trigger UPM
        Trigger(dstPartyUnit, upmData);
    }

    public override void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierConfig uniquePowerModifierConfig, UniquePowerModifierID uniquePowerModifierID)
    {
        Debug.Log("Applying " + uniquePowerModifierConfig.DisplayName + " from " + inventoryItem.ItemName + " to " + dstPartyUnit.UnitName + ", origin is " + uniquePowerModifierID.modifierOrigin);
        // init upm data variable (this is required for Trigger)
        UniquePowerModifierData upmData = new UniquePowerModifierData
        {
            // set UPM ID
            UniquePowerModifierID = uniquePowerModifierID,
            // rest upm duration left to max duration
            DurationLeft = uniquePowerModifierConfig.UpmDurationMax,
            // set upm current power based on source unit stats upgrades count
            //CurrentPower = uniquePowerModifierConfig.GetUpmCurrentPower(srcPartyUnit.StatsUpgradesCount)
            CurrentPower = uniquePowerModifierConfig.GetUpmEffectivePower(inventoryItem)
        };
        // validate if it is really instant UPM (max duration) is 0
        // .. idea: do it in editor with warning highlight
        if (uniquePowerModifierConfig.UpmDurationMax != 0)
        {
            Debug.LogWarning("UpmDurationMax should be 0");
        }
        // instantly trigger UPM
        Trigger(dstPartyUnit, upmData);
    }

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
            PartyUnit srcPartyUnit = BattleContext.ActivePartyUnitUI.LPartyUnit;
            PartyUnit dstPartyUnit = BattleContext.DestinationUnitSlot.GetComponentInChildren<PartyUnitUI>().LPartyUnit;
            UniquePowerModifierConfig uniquePowerModifierConfig = srcPartyUnit.UnitAbilityConfig.uniquePowerModifierConfigs[BattleContext.ActivatedUPMConfigIndex];
            UniquePowerModifierID uniquePowerModifierID = BattleContext.UniquePowerModifierID;
            Apply(srcPartyUnit, dstPartyUnit, uniquePowerModifierConfig, uniquePowerModifierID);
        }
    }

    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        Debug.Log("Trigger " + uniquePowerModifierData.GetOriginDisplayName() + " UPM");
        // Apply upm current power to destination unit
        dstPartyUnit.UnitHealthCurr += uniquePowerModifierData.CurrentPower; // current power is negative if it is damage dealing ability
    }

}
