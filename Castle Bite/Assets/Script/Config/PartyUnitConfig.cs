using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PartyUnitConfig", menuName = "Config/Unit/General")]
public class PartyUnitConfig : ScriptableObject
{
    // Misc attributes
    public UnitType unitType;
    public string unitDisplayName;
    // Leader attributes
    public int unitLeadership;
    // Level and experience
    public int unitBaseExperienceRequiredToReachNewLevel;
    public int unitExperienceRequiredToReachNewLevelIncrementOnLevelUp;
    //() public int GetUnitExperienceRequiredToReachNextLevel(int currentUnitLevel);
    public int unitBaseExperienceReward; // experience reward on 1st level
    public int unitExperienceRewardIncrementOnLevelUp;
    //() public int GetUnitExperienceReward(int currentUnitLevel);
    // Defensive attributes
    // All-around defense, applied before any other resistances are applied.
    public int unitBaseDefense;
    //-public int unitHealthMax;
    public int unitBaseHealthMax; // max health on 1st level
    //() public int GetUnitMaxHealth(StatsUpgradesCount) - get health based on number of stats upgrades
    [FormerlySerializedAs("unitHealthMaxIncrementOnStatsUpgrade")]
    // public int unitHealthMaxIncrementOnLevelUp;
    // [FormerlySerializedAs("unitHealthMaxIncrementOnLevelUp")]
    public int unitHealthMaxIncrementOnStatsUpgrade;
    // [FormerlySerializedAs("unitHealthMaxIncrementOnStatsUpgrade")]
    // public int unitHealthMaxIncrementOnStatsUpgradeNew;
    public int unitHealthRegenPercent;
    public Resistance[] unitResistances;
    // Offensive attributes
    public UnitAbility unitAbility;
    //-public int unitPower;
    //-public int unitPowerIncrementOnLevelUp;
    //-public UnitPowerSource unitPowerSource;
    //-public UnitPowerDistance unitPowerDistance;
    //-public UnitPowerScope unitPowerScope;
    public int unitInitiative = 10;
    // Unique power modifiers
    //-public List<UniquePowerModifier> uniquePowerModifiers;
    // Misc Description
    public string unitRole;
    public string unitBriefDescription;
    public string unitFullDescription;
    // Misc hire and edit unit attributes
    public int unitCost;
    public UnitSize unitSize;
    public bool isInterpartyMovable;
    public bool isDismissable;
    // Unit statuses and [de]buffs
    
    // For party leader upgrades
    public bool canLearnSkills;
    // For common units class upgrade
    public bool classIsUpgradable;
    public UnitType requiresUnitType;
    public UnitType[] unlocksUnitTypes;
    public int upgradeCost;
    // Misc attributes for map
    public int movePointsMax;
    public int movePointsIncrementOnLevelUp;
    public int scoutingRange;
    // Skills

    // UI attributes

    // Unit Equipment

    // Functions
    public int GetUnitExperienceRequiredToReachNextLevel(int currentUnitLevel)
    {
        return unitBaseExperienceRequiredToReachNewLevel + unitExperienceRequiredToReachNewLevelIncrementOnLevelUp * currentUnitLevel;
    }

    public int GetUnitExperienceReward(int currentUnitLevel)
    {
        return unitBaseExperienceReward + unitExperienceRewardIncrementOnLevelUp * currentUnitLevel;
    }

    public int GetUnitMaxHealth(int statsUpgradesCount)
    {
        return unitBaseHealthMax + unitHealthMaxIncrementOnStatsUpgrade * statsUpgradesCount;
    }
}
