using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Config")]
public class UniquePowerModifierConfig : ScriptableObject
{
    [SerializeField]
    private string displayName;
    [SerializeField]
    private string description;
    [SerializeField]
    private UniquePowerModifier uniquePowerModifier;
    [SerializeField]
    private UnitStatModifierConfig unitStatModifierConfig;
    // define possible origins (who is the source of unique power modifier)
    // private ModifierScope modifierScope;
    public UnitBuff upmAppliedBuff;
    public UnitDebuff upmAppliedDebuff;
    // private int upmPower;
    // private int upmPowerIncrementOnLevelUp;
    // private int upmDuration;
    // private PowerSource upmSource;
    public ModifierOrigin upmOrigin; // data
    public ModifierAppliedHow modifierApplied;  // active/passive
    public int upmDurationLeft; // data
    // public int skillPowerMultiplier = 1;
    // private UnitStatus[] canBeAppliedToTheUnitsWithStatuses;

    public void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierID uniquePowerModifierID)
    {
        uniquePowerModifier.Apply(srcPartyUnit, dstPartyUnit, this, uniquePowerModifierID);
    }

    public ModifierScope ModifierScope
    {
        get
        {
            return unitStatModifierConfig.modifierScope;
        }
    }

    public int UpmBasePower
    {
        get
        {
            return unitStatModifierConfig.modifierPower;
        }
    }

    public int GetUpmCurrentPower(int unitStatsUpgradeCount)
    {
        // calculate current UPM power based on stats upgrade count
        return UpmBasePower + UpmPowerIncrementOnLevelUp * unitStatsUpgradeCount;
    }

    public int UpmPowerIncrementOnLevelUp
    {
        get
        {
            return unitStatModifierConfig.powerIncrementOnStatsUpgrade;
        }
    }

    public int UpmDurationMax
    {
        get
        {
            return unitStatModifierConfig.duration;
        }
    }

    public PowerSource UpmSource
    {
        get
        {
            return unitStatModifierConfig.powerSource;
        }
    }

    public UnitStatus[] CanBeAppliedToTheUnitsWithStatuses
    {
        get
        {
            return unitStatModifierConfig.canBeAppliedToTheUnitsWithStatuses;
        }
    }

    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    //public string GetDisplayName()
    //{
    //    switch (upmAppliedDebuff)
    //    {
    //        case UnitDebuff.Burned:
    //            return "Burn";
    //        case UnitDebuff.Chilled:
    //            return "Chill";
    //        case UnitDebuff.Paralyzed:
    //            return "Paralyze";
    //        case UnitDebuff.Poisoned:
    //            return "Poison";
    //        default:
    //            Debug.LogError("Unknown debuf");
    //            return "Error";
    //    }
    //}
}

