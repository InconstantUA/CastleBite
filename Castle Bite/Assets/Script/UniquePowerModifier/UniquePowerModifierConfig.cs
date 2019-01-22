using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UniquePowerModifierType
{
    Buff,
    Debuff
}

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
    //public UnitBuff upmAppliedBuff;
    //public UnitDebuff upmAppliedDebuff;
    public UniquePowerModifierType uniquePowerModifierType;
    // private int upmPower;
    // private int upmPowerIncrementOnLevelUp;
    // private int upmDuration;
    // private PowerSource upmSource;
    //public ModifierOrigin upmOrigin; // ?TODO: move to data
    public ModifierAppliedHow modifierAppliedHow;  // active (example: heal over time buff) or passive (example: defense buff)
    public int upmDurationLeft; // TODO: move to data
    // public int skillPowerMultiplier = 1;
    // private UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
    [SerializeField]
    private UniquePowerModifierUIConfig uniquePowerModifierUIConfig;

    public void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierID uniquePowerModifierID)
    {
        UniquePowerModifier.Apply(srcPartyUnit, dstPartyUnit, this, uniquePowerModifierID);
    }

    public void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        UniquePowerModifier.Trigger(dstPartyUnit, uniquePowerModifierData);
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

    public UniquePowerModifierUIConfig UniquePowerModifierUIConfig
    {
        get
        {
            return uniquePowerModifierUIConfig;
        }
    }

    public UniquePowerModifier UniquePowerModifier
    {
        get
        {
            return uniquePowerModifier;
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

