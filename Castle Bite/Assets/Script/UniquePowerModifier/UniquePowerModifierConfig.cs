using UnityEngine;

public enum UniquePowerModifierType
{
    Buff,
    Debuff,
    Summon
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
    [SerializeField]
    private UniquePowerModifierType uniquePowerModifierType;
    // private int upmPower;
    // private int upmPowerIncrementOnLevelUp;
    // private int upmDuration;
    // private PowerSource upmSource;
    //public ModifierOrigin upmOrigin; // ?TODO: move to data
    [SerializeField]
    private ModifierAppliedHow modifierAppliedHow;  // active (example: heal over time buff) or passive (example: defense buff)
    public int upmDurationLeft; // TODO: move to data
    // public int skillPowerMultiplier = 1;
    // private UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
    //[SerializeField]
    //private UniquePowerModifierUpdaterConfig
    [SerializeField]
    private ModifierConfigUpdater[] modifierConfigUpdaters; // updates UnitStatModifierConfig if defined
    [SerializeField]
    private ModifierLimiter[] modifierLimiters; // blocks this upm if limiter condifitions are not met
    [SerializeField]
    private ModifierAdviser[] modifierAdvisers; // highlight differently when it is not advised to use UPM in specific situations (example: heal full health unit)
    [SerializeField]
    private UniquePowerModifierUIConfig uniquePowerModifierUIConfig;

    public void Apply(PartyUnit srcPartyUnit, PartyUnit dstPartyUnit, UniquePowerModifierID uniquePowerModifierID)
    {
        UniquePowerModifier.Apply(srcPartyUnit, dstPartyUnit, this, uniquePowerModifierID);
    }

    public void Apply(InventoryItem inventoryItem, PartyUnit dstPartyUnit, UniquePowerModifierID uniquePowerModifierID)
    {
        UniquePowerModifier.Apply(inventoryItem, dstPartyUnit, this, uniquePowerModifierID);
    }

    public void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        UniquePowerModifier.Trigger(dstPartyUnit, uniquePowerModifierData);
    }

    public bool MatchScope(PartyUnit srcPartyUnitUPMOwner, PartyUnit dstPartyUnit)
    {
        // Match
        switch (ModifierScope)
        {
            case ModifierScope.Self:
                // verify if source and destination party units are the same
                if (srcPartyUnitUPMOwner.GetInstanceID() == dstPartyUnit.GetInstanceID())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ModifierScope.SingleUnit:
            case ModifierScope.EntireParty:
            case ModifierScope.AllPlayerUnits:
                Debug.LogError("Finish this function logic implementation");
                return true;
            default:
                Debug.LogError("Unknown modifier scope: " + ModifierScope.ToString());
                return false;
        }
    }

    public bool MatchRelationships(PartyUnit srcPartyUnitUPMOwner, PartyUnit dstPartyUnit)
    {
        // get relationships
        Relationships.State relationships = Relationships.Instance.GetRelationships(srcPartyUnitUPMOwner.GetUnitParty().Faction, dstPartyUnit.GetUnitParty().Faction);
        // loop through all required relationships
        foreach(Relationships.State relation in RequiredRelationships)
        {
            // verify if relationships match
            if (relation == relationships)
            {
                return true;
            }
        }
        // if none of required relations match return false
        return false;
    }

    public bool AreRequirementsMetInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // loop through all limiters
        foreach (ModifierLimiter modifierLimiter in modifierLimiters)
        {
            // verify is modifier power is limited
            if (modifierLimiter.DoDiscardModifierInContextOf(srcContext, dstContext))
            {
                // at least one requirement is not met
                return false;
            }
        }
        // if none of limiter is limiting, then return true
        return true;
    }

    public bool IsItAdvisedToActInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // loop through all advisers
        foreach (ModifierAdviser modifierAdviser in modifierAdvisers)
        {
            // verify is modifier power is limited
            if (modifierAdviser.DoAdviseAgainstUPMUsageInContextOf(srcContext, dstContext))
            {
                // at least one not advised condition is met
                // advise against acting in current context
                return false;
            }
        }
        // if none of advisers conditions are met, advise to act in current context
        return true;
    }

    public string GetLimiterMessageInContextOf(System.Object srcContext, System.Object dstContext)
    {
        string message = "";
        // loop through all limiters
        foreach (ModifierLimiter modifierLimiter in modifierLimiters)
        {
            // verify is modifier power is limited
            if (modifierLimiter.DoDiscardModifierInContextOf(srcContext, dstContext))
            {
                Debug.Log(".. Verify and adjust message size. Idea: show full message with pop-up on mouse over.");
                // at least one requirement is not met
                return modifierLimiter.OnLimitMessage;
            }
        }
        // return resulting message
        return message;
    }

    UnitStatModifierConfig GetUpdatedUnitStatModifierConfig(System.Object context)
    {
        // init updated usmc with the copy of default usmc (note: avoid modification of default usmc)
        UnitStatModifierConfig updatedUSMC = Instantiate(unitStatModifierConfig);
        // update usmc
        foreach (ModifierConfigUpdater modifierConfigUpdater in modifierConfigUpdaters)
        {
            updatedUSMC = modifierConfigUpdater.GetUpdatedUSMC(updatedUSMC, context);
        }
        // return result
        return updatedUSMC;
    }

    //public int GetUpmCurrentPower(int unitStatsUpgradeCount)
    //{
    //    // calculate current UPM power based on stats upgrade count
    //    return UpmBasePower + UpmPowerIncrementOnLevelUp * unitStatsUpgradeCount;
    //}

    public int GetUpmEffectivePower(System.Object context)
    {
        // return power after all applied updates
        return GetUpdatedUnitStatModifierConfig(context).modifierPower;
    }

    public int GetUpmPowerDifference(System.Object context)
    {
        // return difference between updated usmc power and default usmc power
        return GetUpmEffectivePower(context) - unitStatModifierConfig.modifierPower;
    }

    #region UnitStatModifierConfig properties

    public UnitSkillID AssociatedUnitSkillID
    {
        get
        {
            return unitStatModifierConfig.associatedUnitSkillID;
        }
    }

    public float AssociatedUnitSkillPowerMultiplier
    {
        get
        {
            return unitStatModifierConfig.associatedUnitSkillLevelMultiplier;
        }
    }

    public ModifierCalculatedHow ModifierCalculatedHow
    {
        get
        {
            return unitStatModifierConfig.modifierCalculatedHow;
        }
    }

    public ModifierScope ModifierScope
    {
        get
        {
            return unitStatModifierConfig.modifierScope;
        }
    }

    public UnitStatID ModifiedUnitStatID
    {
        get
        {
            return unitStatModifierConfig.unitStat;
        }
    }

    public Relationships.State[] RequiredRelationships
    {
        get
        {
            return unitStatModifierConfig.requiredRelationships;
        }
    }

    public int UpmBasePower
    {
        get
        {
            return unitStatModifierConfig.modifierPower;
        }
    }

    //public int UpmPowerIncrementOnLevelUp
    //{
    //    get
    //    {
    //        return unitStatModifierConfig.powerIncrementOnStatsUpgrade;
    //    }
    //}

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

    #endregion UnitStatModifierConfig properties

    #region Properties

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

    public UniquePowerModifierType UniquePowerModifierType
    {
        get
        {
            return uniquePowerModifierType;
        }
    }

    public ModifierAppliedHow ModifierAppliedHow
    {
        get
        {
            return modifierAppliedHow;
        }
    }

    public ModifierConfigUpdater[] ModifierConfigUpdaters
    {
        get
        {
            return modifierConfigUpdaters;
        }
    }

    #endregion Properties

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

