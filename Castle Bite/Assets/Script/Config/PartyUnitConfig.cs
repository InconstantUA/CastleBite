using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyUnitConfig", menuName = "Config/PartyUnit")]
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
    public int unitBaseExperienceReward; // experience reward on 1st level
    public int unitExperienceRewardIncrementOnLevelUp;
    // Defensive attributes
    //-public int unitHealthMax;
    public int unitBaseHealthMax; // max health on 1st level
    //+() public int GetUnitHealthMax() - get health based on lvl
    public int unitHealthMaxIncrementOnLevelUp;
    public int unitHealthRegenPercent;
    //-public int unitDefense;
    //+add physical resistance instead
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
}
