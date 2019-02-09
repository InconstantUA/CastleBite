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
        throw new System.NotImplementedException();
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

    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        Debug.Log("Trigger " + uniquePowerModifierData.GetOriginDisplayName() + " UPM");
        // Apply upm current power to destination unit
        dstPartyUnit.UnitHealthCurr += uniquePowerModifierData.CurrentPower; // current power is negative if it is damage dealing ability
    }

}
