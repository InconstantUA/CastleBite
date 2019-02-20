using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum UnitType
{
    None,
    Archangel, // Dominion Capital guard
    Knight, Ranger, Archmage, Seraphim, Thief, Warrior, Mage, Priest, Colossus, Archer, // Dominion
    OrcWarrior, Goblin, Ogre, Cyclop, Troll, // Orcs
    Chevalier, Gladiator, Templar, Lancer, Warlord, Paladin, Champion, // Warrior upgrades
    GiantToad, OrcShaman, // More Orcs
    Reaper, Vampire, Warlock, Wraith, Shadow, // Undead
    Hades, // Undead Capital guard
    Skeleton, Nighthaunt, Necromancer, Ghost, Abomination, // Undead common units
    Megara, // Storyline hero
    Elder, // Dominion, mass heal
    Ancient, // Dominion, resurect and mass heal
    Unknown
};

[Serializable]
public enum UnitSize
{
    Single,
    Double
};


[Serializable]
public enum UnitAbilityID
{
    ThrowRock,          // Greenskin Cyclop
    StompWithFoot,      // Greenskin Ogre
    CutWithAxe,         // Greenskin Orc warrior
    CutWithDagger,      // Greenskin Goblin
    ThrowSpear,         // Greenskin Troll
    HolyWord,           // Dominion Lahabiel capital guard
    BlowWithGreatSword, // Dominion Knight leader
    ShootWithCompoudBow,// Dominion Ranger leader
    StabWithDagger,     // Dominion Thief leader
    Resurect,           // Dominion Seraphim leader
    CastLightningStorm, // Dominion Archmage leader
    SlashWithSword,     // Dominion Swordsman
    ShootWithBow,       // Dominion Archer
    HealingWord,        // Dominion Priest
    HealingSong,        // Dominion Elder
    BlowWithMaul,       // Dominion Colossus
    CastChainLightning, // Dominion Mage
    EarthShatteringLeap,// Greenskin Bombul captial guard
    Malediction,        // Greenskin Orc shaman
    LastCall,           // Undead Hades capital guard
    DrainLife,          // Undead Vampire
    SacrificingEcho,    // Dominion Ancient
    DefensiveStance,    // Default defense stance ability
    None
};

[Serializable, Flags]
public enum UnitAbilityTypes
{
    Healing     = 1 << 1,
    Damaging    = 1 << 2,
    Resurecting = 1 << 3
}

[Serializable]
public enum PowerSource : int
{
    Physical,   // Example: attack with metal weapons
    Water,
    Fire,
    Earth,
    Wind,
    Life,       // Heal
    Death,      // Poison
    Pure,       // Cannot be resisted
    Mind,       // Paralyze
    None        // last element
}

[Serializable]
public class Resistance
{
    public PowerSource source;
    public int percent;
}

[Serializable]
public enum UnitAbilityRange
{
    Mele,
    Ranged
}

//[Serializable]
//public enum UnitPowerScope
//{
//    OneUnit,
//    EntireParty
//}

[Serializable]
public enum UnitStatus
{
    Active, // not Dead and not Escaped = can fight
    Waiting,
    Escaping,
    Escaped,
    Dead
}

//[Serializable, Flags]
//public enum UnitStatuses
//{
//    Active      = 1 << 1, // not Dead and not Escaped = can fight
//    Waiting     = 1 << 2,
//    Escaping    = 1 << 3,
//    Escaped     = 1 << 4,
//    Dead        = 1 << 5
//}

//[Serializable]
//public enum UnitDebuff : int
//{
//    None,
//    Poisoned,
//    Burned,
//    Chilled,
//    Paralyzed,
//    ArrSize
//}

//[Serializable]
//public enum UnitBuff : int
//{
//    None,
//    DefenseStance,
//    ArrSize // for dynamic resizing of UnitBuffs array
//}

[Serializable]
public enum UnitSkillID
{
    Leadership,
    Offense,
    Defense,
    Pathfinding,
    Scouting,
    Healing,
    DeathResistance,
    FireResistance,
    WaterResistance,
    MindResistance,
    ShardAura,
    LifelessContinuation,
    Initiative,
    None
}

[Serializable]
public class UnitSkillData
{
    public UnitSkillID unitSkill;
    ////[NonSerialized]
    //public string mDisplayName;
    //[Serializable]
    //public class SkillLevel
    //{
    //    public int mCurrent;
    //    [NonSerialized]
    //    public int mMax;
    //    public SkillLevel(int current, int max)
    //    {
    //        mCurrent = current;
    //        mMax = max;
    //    }
    //}
    //public SkillLevel mLevel;
    public int currentSkillLevel;
    ////[NonSerialized]
    //public int requiredHeroLevel;
    ////[NonSerialized]
    //public int levelUpEveryXHeroLevels; // skill can be learned only after this number of levels has passed
    ////[NonSerialized]
    //public string description;
    public UnitSkillData(UnitSkillID unitSkillID, int currentSkillLevel)
    {
        this.unitSkill = unitSkillID;
        this.currentSkillLevel = currentSkillLevel;
    }
    //public UnitSkillData(UnitSkill name, string displayName, int currentLevel, int maxLevel, int requiredHeroLevel, int levelUpIncrementStep, string description)
    //{
    //    mName = name;
    //    mDisplayName = displayName;
    //    mLevel = new SkillLevel(currentLevel, maxLevel);
    //    mRequiredHeroLevel = requiredHeroLevel;
    //    mLevelUpIncrementStep = levelUpIncrementStep;
    //    mDescription = description;
    //}
    public bool EqualTo(UnitSkillData unitSkillData)
    {

        if (
            // verify skill name
            (unitSkillData.unitSkill == this.unitSkill) &&
            // verify skill current level
            (unitSkillData.currentSkillLevel == this.currentSkillLevel) // &&
            //// verify skill max level
            //(unitSkill.mLevel.mMax == mLevel.mMax) &&
            //// verify skill required hero level level
            //(unitSkill.mRequiredHeroLevel == mRequiredHeroLevel) &&
            //// verify skill level up increment step level
            //(unitSkill.mLevelUpIncrementStep == mLevelUpIncrementStep)
        )
        {
            return true;
        }
        return false;
    }
}

[Serializable]
public enum ModifierOrigin
{
    Ability,
    Item,
    City,
    Skill
}

// note modifier scope shoud be defined from lower to higher scope, because it impacts further logics
[Serializable]
public enum ModifierScopeID
{
    None            = 0,
    Self            = 1,
    SingleUnit      = 4,
    EntireParty     = 8,
    AllPlayerUnits  = 16,
    Unlimited       = 1024
}

[Serializable]
public class UniquePowerModifierID : System.Object
{
    public InventoryItemID inventoryItemID = InventoryItemID.None;
    public UnitAbilityID unitAbilityID = UnitAbilityID.None;
    public CityID cityID = CityID.None;
    public UnitSkillID unitSkillID;
    public int uniquePowerModifierConfigIndex = 0;
    public ModifierOrigin modifierOrigin;
    public int destinationGameObjectID; // this is not required for serialization, but required to uniquely identify UniquePowerModifierID among other similar UniquePowerModifiers which are applied by the same source

    public static bool operator ==(UniquePowerModifierID upmID1, UniquePowerModifierID upmID2)
    {

        return Comparison(upmID1, upmID2);
    }

    public static bool operator !=(UniquePowerModifierID upmID1, UniquePowerModifierID upmID2)
    {

        return Comparison(upmID1, upmID2);

    }

    public override bool Equals(object obj)
    {

        if (!(obj is UniquePowerModifierID)) return false;

        return this == (UniquePowerModifierID)obj;

    }

    public static bool Comparison(UniquePowerModifierID upmID1, UniquePowerModifierID upmID2)
    {
        // Check if both IDs are null
        if (System.Object.ReferenceEquals(null, upmID1) && System.Object.ReferenceEquals(null, upmID2))
        {
            return true;
        }
        // Check if one of the IDs is null
        if (System.Object.ReferenceEquals(null, upmID1) || System.Object.ReferenceEquals(null, upmID2))
        {
            return false;
        }
        // compare inventory item id
        if ((upmID1.inventoryItemID == upmID2.inventoryItemID)
            // compare ability id
            && (upmID1.unitAbilityID == upmID2.unitAbilityID)
            // compare index
            && (upmID1.uniquePowerModifierConfigIndex == upmID2.uniquePowerModifierConfigIndex)
            // compare origin
            && (upmID1.modifierOrigin == upmID2.modifierOrigin)
            // compare destination game object id
            && (upmID1.destinationGameObjectID == upmID2.destinationGameObjectID))
        {
            // all match
            return true;
        }
        else
        {
            // some not match
            return false;
        }
    }

    // (C) http://www.loganfranken.com/blog/692/overriding-equals-in-c-part-2/
    public override int GetHashCode()
    {
        unchecked
        {
            // Choose large primes to avoid hashing collisions
            const int HashingBase = (int)2166136261;
            const int HashingMultiplier = 16777619;

            int hash = HashingBase;
            hash = (hash * HashingMultiplier) ^ (!System.Object.ReferenceEquals(null, inventoryItemID) ? inventoryItemID.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!System.Object.ReferenceEquals(null, unitAbilityID) ? unitAbilityID.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!System.Object.ReferenceEquals(null, uniquePowerModifierConfigIndex) ? uniquePowerModifierConfigIndex.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!System.Object.ReferenceEquals(null, modifierOrigin) ? modifierOrigin.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!System.Object.ReferenceEquals(null, destinationGameObjectID) ? modifierOrigin.GetHashCode() : 0);
            return hash;
        }
    }

    public override string ToString()
    {
        string myString = "";
        if (inventoryItemID != InventoryItemID.None)
        {
            myString += inventoryItemID.ToString();
        }
        else if (unitAbilityID != UnitAbilityID.None)
        {
            myString += unitAbilityID.ToString();
        }
        return "UPM origin is " + myString + "(" + modifierOrigin.ToString() + ") and index is " + uniquePowerModifierConfigIndex.ToString();
    }
}

[Serializable]
public class UniquePowerModifierData : System.Object
{
    // values should be public to be serialized by Instantiate for Backup (clone) party unit to work without writing your own clone function
    public UniquePowerModifierID uniquePowerModifierID;
    public int durationLeft; // it is reset when USM is applied
    public int currentPower;

    public UniquePowerModifierID UniquePowerModifierID
    {
        get
        {
            return uniquePowerModifierID;
        }

        set
        {
            uniquePowerModifierID = value;
        }
    }

    public int DurationLeft
    {
        get
        {
            return durationLeft;
        }

        set
        {
            durationLeft = value;
        }
    }

    public int CurrentPower
    {
        get
        {
            return currentPower;
        }

        set
        {
            currentPower = value;
        }
    }

    public UniquePowerModifierConfig GetUniquePowerModifierConfig()
    {
        if (UniquePowerModifierID.inventoryItemID != InventoryItemID.None)
        {
            // get and return config from config manager by item ID and UPM index in that item
            return ConfigManager.Instance[UniquePowerModifierID.inventoryItemID].uniquePowerModifierConfigs[UniquePowerModifierID.uniquePowerModifierConfigIndex];
        }
        else if (UniquePowerModifierID.unitAbilityID != UnitAbilityID.None)
        {
            // get and return config from config manager by ability ID and UPM index in that ability
            // return ConfigManager.Instance[UniquePowerModifierID.unitAbilityID].postActionUniquePowerModifierConfigs[UniquePowerModifierID.uniquePowerModifierConfigIndex];
            return ConfigManager.Instance[UniquePowerModifierID.unitAbilityID].uniquePowerModifierConfigs[UniquePowerModifierID.uniquePowerModifierConfigIndex];
        }
        else if (UniquePowerModifierID.cityID != CityID.None)
        {
            // get and return config from config manager by city ID and UPM index in that ability
            return ConfigManager.Instance[UniquePowerModifierID.cityID].UniquePowerModifierConfigs[UniquePowerModifierID.uniquePowerModifierConfigIndex];
        }
        else if (UniquePowerModifierID.unitSkillID != UnitSkillID.None)
        {
            // get and return config from config manager by city ID and UPM index in that ability
            return ConfigManager.Instance[UniquePowerModifierID.unitSkillID].UniquePowerModifierConfigs[UniquePowerModifierID.uniquePowerModifierConfigIndex];
        }
        else
        {
            // this should not happen
            Debug.LogError("Unhandled exception");
            return null;
        }
    }

    public string GetOriginDisplayName()
    {
        if (UniquePowerModifierID.inventoryItemID != InventoryItemID.None)
        {
            // get and return config from config manager by item ID and UPM index in that item
            return ConfigManager.Instance[UniquePowerModifierID.inventoryItemID].itemDisplayName;
        }
        else if (UniquePowerModifierID.unitAbilityID != UnitAbilityID.None)
        {
            // get and return config from config manager by ability ID and UPM index in that ability
            return ConfigManager.Instance[UniquePowerModifierID.unitAbilityID].abilityDisplayName;
        }
        else if (UniquePowerModifierID.cityID != CityID.None)
        {
            // get and return config from config manager by ability ID and UPM index in that ability
            return ConfigManager.Instance[UniquePowerModifierID.cityID].cityName;
        }
        else if (UniquePowerModifierID.unitSkillID != UnitSkillID.None)
        {
            // get and return config from config manager by ability ID and UPM index in that ability
            return ConfigManager.Instance[UniquePowerModifierID.unitSkillID].skillDisplayName;
        }
        else
        {
            // this should not happen
            Debug.LogError("Unhandled exception");
            return null;
        }
    }
}

[Serializable]
public enum UnitStatID
{
    Leadership,
    Health,
    Defense,
    Power,
    Initiative,
    MovePoints,
    ScoutingRange,
    HitChance,
    DeathResistance,
    FireResistance,
    WaterResistance,
    MindResistance
}

[Serializable]
public enum ModifierCalculatedHow
{
    Additively,
    PercentToBase,
    PercentToAll
}

[Serializable]
public enum ModifierAppliedTo
{
    CurrentStat,
    MaxStat
}

[Serializable]
public enum TriggerCondition
{
    NonePassive,    // Give continuous bonus, example: Equipment: armor, boots, weapon with passive modifiers
    NoneImmediate,  // Apply its power immeadiately upon usage: Healing Potions
    AtTurnStart     // Debuffs and Buffs with DoTs (Damage over time), which are triggered at turn start
    //Active, // Potions, scrolls, weapon with active modifiers (paralise)
    //Passive // Equipment: armor, boots, weapon with passive modifiers
}

[Serializable]
public enum ModifierDurationType
{
    Permananent = 1,
    RemoveAtBattleEnd = 2,
    NumberOfBattleTurns = 4,
    NumberOfGameTurns = 8
}

//[Serializable]
//public enum UnitStatModifierID
//{
//    // Defense-base stats 10-19
//    DefenseAdd5                     =  100005,
//    DefenseAdd15                    =  100015,
//    DefenseAdd30                    =  100030,
//    DefenseAdd50                    =  100050,
//    DefenseSubtract5                =  110005,
//    DefenseSubtract15               =  110015,
//    DefenseSubtract30               =  110030,
//    // Power-base stats 20-29
//    PowerAdd5Percent                =  200005,
//    PowerAdd15Percent               =  200015,
//    PowerAdd30Percent               =  200030,
//    PowerAdd50Percent               =  200050,
//    PowerSubtract5Percent           =  210005,
//    PowerSubtract15Percent          =  210015,
//    PowerSubtract30Percent          =  210030,
//    // Initiative-based stats 30-39
//    Inititative5Percent             =  300005,
//    Inititative15Percent            =  300015,
//    Inititative30Percent            =  300030,
//    Inititative50Percent            =  300050,
//    // MovePoints-based stats 40-49
//    MovePointsCurr50Percent         =  400050,
//    MovePointsCurr100Percent        =  400100,
//    MovePointsMax50Percent          =  410050,
//    MovePointsMax70Percent          =  410070,
//    MovePointsMax100Percent         =  410100,
//    // ScoutingRange-based stats 50-59
//    Scouting1                       =  500001,
//    Scouting3                       =  500003,
//    Scouting5                       =  500005,
//    Scouting7                       =  500007,
//    // Leadership-based stats 60-69
//    Leadership1                     =  600001,
//    Leadership3                     =  600003,
//    Leadership5                     =  600005,
//    // Resistance-based stats 100-199
//    WaterResistance50               = 1000050,
//    WaterResistance100              = 1000100,
//    FireResistance50                = 1100050,
//    FireResistance100               = 1100100,
//    EarthResistance50               = 1200050,
//    EarthResistance100              = 1200100,
//    WindResistance50                = 1300050,
//    WindResistance100               = 1300100,
//    DeathResistance50               = 1400050,
//    DeathResistance100              = 1400100,
//    PureResistance50                = 1500050,
//    PureResistance100               = 1500100,
//    MindResistance50                = 1600050,
//    MindResistance100               = 1600100,
//    // Healh-base stats 200-400
//    Resurect1                       = 2000001,
//    HealthCurrAdd                   = 2100000,
//    HealthCurrAdd50                 = 50    + HealthCurrAdd,
//    HealthCurrAdd100                = 100   + HealthCurrAdd,
//    HealthCurrAdd200                = 200   + HealthCurrAdd,
//    HealthCurrAdd400                = 400   + HealthCurrAdd,
//    HealthMaxAddPercent             = 2200000,
//    HealthMaxAdd5Percent            = 5     + HealthMaxAddPercent,
//    HealthMaxAdd15Percent           = 15    + HealthMaxAddPercent,
//    HealthMaxAdd30Percent           = 30    + HealthMaxAddPercent,
//    HealthMaxAdd50Percent           = 50    + HealthMaxAddPercent,
//    HealthCurrSubtract              = 2300000,
//  //SourceNone                      =   00000,
//    SourcePhysical                  =   10000,
//    SourceWater                     =   20000,
//    SourceFire                      =   30000,
//    SourceEarth                     =   40000,
//    SourceWind                      =   50000,
//    SourceDeath                     =   60000,
//    SourcePure                      =   70000,
//    SourceMind                      =   80000,
//    HealthCurrSubtract50Physcal     = 50    + HealthCurrSubtract    + SourcePhysical,
//  //DurationInstant                 =    0000,
//    Duration1Round                  =    1000,
//    Duration2Rounds                 =    2000,
//    HealthCurrSubtract20Over2Rounds = 20    + HealthCurrSubtract    + SourceDeath,
//    Last                            = 9999999
//}

[Serializable]
public class UnitStatModifier : System.Object
{
    public UnitStatID unitStat;
    public ModifierAppliedTo modifierAppliedTo;
    public ModifierScopeID modifierScope;
    public int modifierPower;
    public int skillPowerMultiplier = 1; // move to data
    public ModifierCalculatedHow modifierCalculatedHow;
    //public TriggerCondition modifierAppliedHow;
    public int duration;
    public int durationLeft; // to be moved to data
    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
}

//[Serializable]
//public class UnitStatusModifier : System.Object
//{
//    public ModifierScope modifierScope;
//    public UnitStatus modifierSetStatus;
//    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
//}

[Serializable]
public class UnitStatData : System.Object
{
    public UnitStatID unitStatID;
    public int currentValue;
}



[Serializable]
public class PartyUnitData : System.Object
{
    // Misc attributes
    // public string unitName;
    public UnitType unitType;
    // Leader attributes
    public bool isLeader;
    // public int unitLeadership;
    public string givenName;
    // Level and experience
    public int unitLevel;
    public int unitExperience;
    // public int unitExperienceRequiredToReachNewLevel;
    // public int unitExperienceReward;
    // public int unitExperienceRewardIncrementOnLevelUp;
    // Defensive attributes
    public int unitHealthCurr;
    // public int unitHealthMax;
    // public int unitHealthMaxIncrementOnLevelUp;
    // public int unitHealthRegenPercent;
    // public int unitDefense;
    // public Resistance[] unitResistances;
    // Offensive attributes
    // public UnitAbility unitAbility;
    // public int unitPower;
    // public int unitPowerIncrementOnLevelUp; // OnStatsUpgrade
    // public UnitPowerSource unitPowerSource;
    // public UnitPowerDistance unitPowerDistance;
    // public UnitPowerScope unitPowerScope;
    // public int unitInitiative = 10;
    // Unique power modifiers
    // public List<UniquePowerModifier> uniquePowerModifiers;
    // Misc Description
    // public string unitRole;
    // public string unitBriefDescription;
    // public string unitFullDescription;
    // Misc hire and edit unit attributes
    // public int unitCost;
    // public UnitSize unitSize;
    // public bool isInterpartyMovable;
    // public bool isDismissable;
    // Unit statuses and [de]buffs
    public UnitStatus unitStatus;
    //public UnitDebuff[] unitDebuffs;
    //public UnitBuff[] unitBuffs;
    // For Upgrade
    public int unitUpgradePoints;
    public int unitStatPoints;
    // public bool classIsUpgradable;
    public int unitClassPoints;
    // public bool canLearnSkills;
    public int unitSkillPoints;
    // For Stats upgrade
    // Stats-upgrade Dependant attributes:
    //  Max Health
    //  Primary Ability Power
    //  Skill-based Unique Power Modifiers
    public int statsUpgradesCount;
    // For class upgrade
    //public UnitType requiresUnitType;
    //public UnitType[] unlocksUnitTypes;
    //public int upgradeCost;
    // Misc attributes for map
    public int movePointsCurrent;
    // public int movePointsMax;
    // public int scoutingRange;
    // Skills
    // Note: if this array initialization changes, then remimport all Units prefabs and other prefabs which use units prefabs:
    //  (Project Assets -> Prefabs -> Units)
    //  (Project Assets -> Prefabs -> Chapters)
    //  Make sure that there is no units who are not inheriting from prefab in the Chapter
    public UnitSkillData[] unitSkillsData = new UnitSkillData[]
    {
        new UnitSkillData(
            UnitSkillID.Leadership,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Initiative,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Offense,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Defense,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Pathfinding,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Scouting,
            0
        ),
        new UnitSkillData(
            UnitSkillID.Healing,
            0
        ),
        new UnitSkillData(
            UnitSkillID.WaterResistance,
            0
        ),
        new UnitSkillData(
            UnitSkillID.FireResistance,
            0
        ),
        new UnitSkillData(
            UnitSkillID.DeathResistance,
            0
        ),
        new UnitSkillData(
            UnitSkillID.MindResistance,
            0
        ),
        new UnitSkillData(
            UnitSkillID.ShardAura,
            0
        ),
        new UnitSkillData(
            UnitSkillID.LifelessContinuation,
            0
        ),
        new UnitSkillData(
            UnitSkillID.None,
            0
            )
    };
    // Unit stats data
    public UnitStatData[] unitStatsData = new UnitStatData[]
    {
        new UnitStatData{ unitStatID = UnitStatID.Leadership },
        new UnitStatData{ unitStatID = UnitStatID.Initiative },
        new UnitStatData{ unitStatID = UnitStatID.Defense },
        new UnitStatData{ unitStatID = UnitStatID.MovePoints },
        new UnitStatData{ unitStatID = UnitStatID.ScoutingRange },
        new UnitStatData{ unitStatID = UnitStatID.DeathResistance },
        new UnitStatData{ unitStatID = UnitStatID.FireResistance },
        new UnitStatData{ unitStatID = UnitStatID.MindResistance },
        new UnitStatData{ unitStatID = UnitStatID.WaterResistance }
    };
    // UI attributes
    public PartyPanel.Row unitPPRow;  // used during game save and load and when party UI is displayed
    public PartyPanel.Cell unitPPCell;  // used during game save and load and when party UI is displayed
    // Unit Equipment
    public List<InventoryItemData> unitIventory; // information saved and loaded during game save and load, during game running phase all data can be retrieved from the child items of the party leader unit
    // All active stat modifiers inherited from used items or applied abilities (this is required for game save / restore)
    public List<UniquePowerModifierData> appliedUniquePowerModifiersData;
    // update this on fields changes
    // this is a function to restore original values on Cancel(ation) of PartyUnit Upgrade
    public void CancelUpgrade(PartyUnitData backupPartyUnitData)
    {
        // copy every property, which is not the reference-type (not the class)
        unitType = backupPartyUnitData.unitType;
        isLeader = backupPartyUnitData.isLeader;
        givenName = backupPartyUnitData.givenName;
        unitLevel = backupPartyUnitData.unitLevel;
        unitExperience = backupPartyUnitData.unitExperience;
        unitHealthCurr = backupPartyUnitData.unitHealthCurr;
        unitStatus = backupPartyUnitData.unitStatus;
        unitUpgradePoints = backupPartyUnitData.unitUpgradePoints;
        unitStatPoints = backupPartyUnitData.unitStatPoints;
        unitClassPoints = backupPartyUnitData.unitClassPoints;
        unitSkillPoints = backupPartyUnitData.unitSkillPoints;
        statsUpgradesCount = backupPartyUnitData.statsUpgradesCount;
        movePointsCurrent = backupPartyUnitData.movePointsCurrent;
        // we assume that array size and members position doesn't change
        for (int i = 0; i < unitSkillsData.Length; i++)
        {
            unitSkillsData[i].currentSkillLevel = backupPartyUnitData.unitSkillsData[i].currentSkillLevel;
        }
        // we assume that array size and members position doesn't change
        for (int i = 0; i < unitStatsData.Length; i++)
        {
            unitStatsData[i].currentValue = backupPartyUnitData.unitStatsData[i].currentValue;
        }
        unitPPRow = backupPartyUnitData.unitPPRow;
        unitPPCell = backupPartyUnitData.unitPPCell;
        //List<InventoryItemData> unitIventory; // type: reference and this should not change on upgrade
        //List<UniquePowerModifierData> uniquePowerModifiersData // type: reference and this should not change on upgrade
    }
}

// For events admin
public class HealthCurrent
{

}

public class HealthMax
{

}

public class PartyUnit : MonoBehaviour {
    // Data which will be saved later
    [SerializeField]
    PartyUnitData partyUnitData;
    //// event for UI when UPM has been removed
    //[SerializeField]
    //GameEvent uniquePowerModifierHasBeenRemovedEvent;


    PartyUnitConfig partyUnitConfigCache; // init on Awake
    UnitStatusConfig unitStatusConfigCache;
    UnitAbilityConfig unitAbilityConfigCache; // init on first Get

    // Misc Battle attributes
    private bool hasMoved = false;

    //void InitUnitBuffs()
    //{
    //    UnitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
    //    for (int i = 0; i < (int)UnitBuff.ArrSize; i++)
    //    {
    //        UnitBuffs[i] = UnitBuff.None;
    //    }
    //}

    //void InitUnitDebuffs()
    //{
    //    UnitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
    //    for (int i = 0; i < (int)UnitDebuff.ArrSize; i++)
    //    {
    //        UnitDebuffs[i] = UnitDebuff.None;
    //    }
    //}

    //private void Awake()
    //{
    //    InitUnitBuffs();
    //    InitUnitDebuffs();
    //    //Debug.Log("Awake " + UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
    //}

    //private void Start()
    //{
    //    //unitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
    //    //UnitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
    //    //Debug.Log("Start " + UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
    //}

    //public Transform GetUnitCell()
    //{
    //    // structure: 3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
    //    return transform.parent.parent.parent;
    //}

    //public Text GetUnitStatusText()
    //{
    //    return GetUnitCell().Find("UnitStatus").GetComponent<Text>();
    //}

    //public Text GetUnitInfoPanelText()
    //{
    //    return GetUnitCell().Find("InfoPanel").GetComponent<Text>();
    //}

    //public Transform GetUnitBuffsPanel()
    //{
    //    return GetUnitCell().Find("UnitStatus/Buffs");
    //}

    //public Transform GetUnitDebuffsPanel()
    //{
    //    return GetUnitCell().Find("UnitStatus/Debuffs");
    //}

    public string GetUnitDisplayName()
    {
        string unitName;
        // verify is unit has given name
        if (GivenName != UnitName)
        {
            // start with Hero's given name information
            unitName = GivenName.ToString() + "\r\n<size=12>" + UnitName.ToString() + "</size>";
        }
        else
        {
            unitName = UnitName.ToString();
        }
        return unitName;
    }

    public HeroParty GetUnitParty()
    {
        // structure: 1HeroParty-PartyUnit
        return GetComponentInParent<HeroParty>();
        //// structure: 6HeroParty/CityGarnizon-5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
        //// verify if unit is member of party
        //if (transform.parent)
        //    if (transform.parent.parent)
        //        if (transform.parent.parent.parent)
        //            if (transform.parent.parent.parent.parent)
        //                if (transform.parent.parent.parent.parent.parent.parent)
        //                    return transform.parent.parent.parent.parent.parent.parent.GetComponent<HeroParty>();
    }

    //public PartyPanel GetUnitPartyPanel()
    //{
    //    // structure: 5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
    //    // verify if unit is member of party
    //    if (transform.parent.parent.parent)
    //    {
    //        if (transform.parent.parent.parent.parent)
    //        {
    //            return transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
    //        }
    //    }
    //    return null;
    //}

    //public City GetUnitCity()
    //{
    //    // structure: 7city-6HeroParty/CityGarnizon-5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
    //    // verify if unit is member of party and city
    //    if (transform.parent)
    //        if (transform.parent.parent)
    //            if (transform.parent.parent.parent)
    //                if (transform.parent.parent.parent)
    //                    if (transform.parent.parent.parent.parent)
    //                        if (transform.parent.parent.parent.parent.parent.parent)
    //                            if (transform.parent.parent.parent.parent.parent.parent.parent)
    //                                return transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<City>();
    //    return null;
    //}

    public int GetCityDefenseBonus()
    {
        // get city
        // structure: 2City-1HeroParty-PartyUnit
        City city = transform.parent.parent.GetComponent<City>();
        if (city)
        {
            // verify if city is friendly
            HeroParty party = GetUnitParty();
            if (city.CityCurrentFaction == party.Faction)
            {
                return city.GetCityDefense();
            }
        }
        return 0;
    }

    Dictionary<UnitSkillID, UnitSkillData> unitSkills = new Dictionary<UnitSkillID, UnitSkillData>(); // to prevent searches in array

    public UnitSkillData GetUnitSkillData(UnitSkillID unitSkillID)
    {
        Debug.Log("Unit skill id is " + unitSkillID);
        // verify if unitSkills dictionary required key has not been set yet
        if (!unitSkills.ContainsKey(unitSkillID))
        {
            // set the key and value from UnitSkillsData
            unitSkills[unitSkillID] = Array.Find(UnitSkillsData, element => element.unitSkill == unitSkillID);
        }
        // return reference to unit skill data
        return unitSkills[unitSkillID];
    }

    public int GetSkillDefenseBonus()
    {
        return GetSameStatUPMsBonusesValue(GetSkillsSelfBonusesForStat(UnitStatID.Defense));
        //// get skill from partyUnit
        //UnitSkillData skill = GetUnitSkillData(UnitSkillID.Defense); // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Defense);
        //// get bonus based on fact that 1 skill level = 10 defense
        //return skill.currentSkillLevel * 10;
    }

    public float GetDefensiveStanceBonus()
    {
        //// Applied Multiplicatively
        //// verify if buffs array is not null
        //if (UnitBuffs != null)
        //{
        //    //Debug.Log(UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
        //    // verify if buffs array was initialized properly
        //    if (UnitBuffs.Length != (int)UnitBuff.ArrSize)
        //    {
        //        // initialize buffs array
        //        InitUnitBuffs();
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Unit buffs array is null");
        //}
        //// verify if unit has defense stance buff
        //if (UnitBuff.DefenseStance == UnitBuffs[(int)UnitBuff.DefenseStance])
        //{
        //    // reduce damage by half
        //    return 0.5f;
        //}
        // verify if defensive stance ability Unique Power Modifier is applied
        // search for DefensiveStance in applied UPMs data
        UniquePowerModifierData defensiveStanceUPMData = AppliedUniquePowerModifiersData.Find(e => e.UniquePowerModifierID.unitAbilityID == UnitAbilityID.DefensiveStance);
        // verify if UPM for DefensiveStance ability is found
        if (defensiveStanceUPMData != null)
        {
            // get bonus
            // .. todo fix to calculate the value based on unit stat ModifierCalculatedHow
            return (float)defensiveStanceUPMData.CurrentPower / 100f;
        }
        // default: no impact on defense
        return 0f; 
    }

    public int GetTotalAdditiveDefense()
    {
        // init with 0
        int totalDefense = 0;
        // Get base defense
        int baseDefense = UnitBaseDefense;
        // apply base defense
        totalDefense += baseDefense;
        // Verify if unit is in a friendly city and get city defense modifier
        int cityDefenseModifier = GetCityDefenseBonus();
        // apply city defense modifier
        totalDefense += cityDefenseModifier;
        // Get items modifier
        int itemsDefenseModifier = GetItemsDefenseBonus();
        // apply items defense modifier
        totalDefense += itemsDefenseModifier;
        // Get skills modifier
        int skillsDefenseModifier = GetSkillDefenseBonus();
        // apply skills defense modifier
        totalDefense += skillsDefenseModifier;
        // return result
        return totalDefense;
    }

    public int GetItemsDefenseBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.Defense, UnitBaseDefense);
    }

    //public UnitSkillData this[UnitSkillID unitSkillID]
    //{
    //    get
    //    {
    //        return Array.Find(UnitSkillsData, e => e.unitSkillID == unitSkillID);
    //    }
    //}

    public int GetEffectiveBonusValue(int value, UnitSkillID unitSkillID = UnitSkillID.None, float unitSkillMultiplier = 1)
    {
        // get skill bonus for defined stat
        return Mathf.CeilToInt(value * unitSkillMultiplier * (1 + GetUnitSkillData(unitSkillID).currentSkillLevel));
    }

    public List<UniquePowerModifierData> AddBonusesPropagatedbyCity(List<UniquePowerModifierData> allBonuses)
    {
        // try to get city in a parent
        City parentCity = GetComponentInParent<City>();
        // verify if unit is in city (parent city != null)
        if (parentCity != null)
        {
            allBonuses.AddRange(parentCity.GetPropagatedBonuses(this));
        }
        //else
        //{
        //    Debug.Log("========== no parent city");
        //}
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> GetSkillsSelfBonusesForStat(UnitStatID unitStatID)
    {
        // init list of UPMs data
        List<UniquePowerModifierData> upmsData = new List<UniquePowerModifierData>();
        // loop through all unit skills
        foreach (UnitSkillData unitSkillData in PartyUnitData.unitSkillsData)
        {
            // verify if skill has been learned
            if (unitSkillData.currentSkillLevel > 0)
            {
                // get skill config
                UnitSkillConfig unitSkillConfig = ConfigManager.Instance[unitSkillData.unitSkill];
                // loop through all UPMs in skill config
                for (int i = 0; i < unitSkillConfig.UniquePowerModifierConfigs.Count; i++)
                {
                    // verify if stat matches
                    if ((unitSkillConfig.UniquePowerModifierConfigs[i].ModifiedUnitStatID == unitStatID)
                        // verify if UPM is passive
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].TriggerCondition == TriggerCondition.NonePassive)
                        // verify if UPM relationships requirements are met
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].MatchRelationships(this, this))
                        // verify if dst party unit hass mass scope
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].ModifierScope.ModifierScopeID >= ModifierScopeID.Self))
                    {
                        // create and add UPM data to the list
                        upmsData.Add(new UniquePowerModifierData
                        {
                            uniquePowerModifierID = new UniquePowerModifierID
                            {
                                unitSkillID = unitSkillConfig.unitSkillID,
                                uniquePowerModifierConfigIndex = i,
                                modifierOrigin = ModifierOrigin.Skill,
                                destinationGameObjectID = gameObject.GetInstanceID()
                            },
                            durationLeft = unitSkillConfig.UniquePowerModifierConfigs[i].UpmDurationMax,
                            currentPower = Mathf.RoundToInt(unitSkillConfig.UniquePowerModifierConfigs[i].UpmBasePower * unitSkillData.currentSkillLevel * unitSkillConfig.UniquePowerModifierConfigs[i].AssociatedUnitSkillPowerMultiplier)
                        });
                    }
                }
            }
        }
        // return the list with UPMs data
        return upmsData;
    }

    public List<UniquePowerModifierData> GetSkillsPropagatedBonuses(PartyUnit dstPartyUnit)
    {
        // init list of UPMs data
        List<UniquePowerModifierData> upmsData = new List<UniquePowerModifierData>();
        // loop through all unit skills
        foreach (UnitSkillData unitSkillData in PartyUnitData.unitSkillsData)
        {
            // verify if skill has been learned
            if (unitSkillData.currentSkillLevel > 0)
            {
                // get skill config
                UnitSkillConfig unitSkillConfig = ConfigManager.Instance[unitSkillData.unitSkill];
                // loop through all UPMs in skill config
                for (int i = 0; i < unitSkillConfig.UniquePowerModifierConfigs.Count; i++)
                {
                    // verify if UPM is passive
                    if ((unitSkillConfig.UniquePowerModifierConfigs[i].TriggerCondition == TriggerCondition.NonePassive)
                        // verify if UPM relationships requirements are met
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].MatchRelationships(this, dstPartyUnit))
                        // verify if dst party unit hass mass scope
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].ModifierScope.ModifierScopeID >= ModifierScopeID.EntireParty))
                    {
                        // create and add UPM data to the list
                        upmsData.Add(new UniquePowerModifierData
                        {
                            uniquePowerModifierID = new UniquePowerModifierID
                            {
                                unitSkillID = unitSkillConfig.unitSkillID,
                                uniquePowerModifierConfigIndex = i,
                                modifierOrigin = ModifierOrigin.Skill,
                                destinationGameObjectID = dstPartyUnit.gameObject.GetInstanceID()
                            },
                            durationLeft = unitSkillConfig.UniquePowerModifierConfigs[i].UpmDurationMax,
                            currentPower = Mathf.RoundToInt(unitSkillConfig.UniquePowerModifierConfigs[i].UpmBasePower * unitSkillData.currentSkillLevel * unitSkillConfig.UniquePowerModifierConfigs[i].AssociatedUnitSkillPowerMultiplier)
                        });
                    }
                }
            }
        }
        // return the list with UPMs data
        return upmsData;
    }

    public List<UniquePowerModifierData> GetItemsPropagatedBonuses(PartyUnit dstPartyUnit)
    {
        // init list of UPMs data
        List<UniquePowerModifierData> upmsData = new List<UniquePowerModifierData>();
        // init current power variable
        //int currentPower;
        // loop through all equipped items
        foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        {
            // set ItemPropagationContext
            ItemPropagationContext itemPropagationContext = new ItemPropagationContext(inventoryItem, dstPartyUnit);
            // verify if item is not in belt
            //if (((HeroEquipmentSlots.BeltSlots & inventoryItem.CurrentHeroEquipmentSlot) != inventoryItem.CurrentHeroEquipmentSlot)
            //    // verify if item is not consumed (slot is none)
            //    && (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None))
            //{
            // loop through the list of all UPMs in the item
            for (int i = 0; i < inventoryItem.UniquePowerModifierConfigs.Count; i++)
            {
                //// verify if UPM is passive
                //if ((inventoryItem.UniquePowerModifierConfigs[i].TriggerCondition == TriggerCondition.NonePassive)
                //    // verify if UPM relationships requirements are met
                //    && (inventoryItem.UniquePowerModifierConfigs[i].MatchRelationships(this, dstPartyUnit))
                //    // verify if dst party unit hass mass scope
                //    && (inventoryItem.UniquePowerModifierConfigs[i].ModifierScope.ModifierScopeID >= ModifierScopeID.EntireParty))
                // verify if UPM can be propagated
                if (inventoryItem.UniquePowerModifierConfigs[i].AreRequirementsMetInContextOf(itemPropagationContext).doDiscardModifier == false)
                {
                    // verify if UPM is associated with a skill
                    //if (UniquePowerModifierConfigs[i].AssociatedUnitSkillID != UnitSkillID.None)
                    //{
                    //    currentPower = UniquePowerModifierConfigs[i].UpmBasePower;
                    //}
                    //else
                    //{
                    //    // get current power multiplied by this party unit skill level * multiplied by skill multiplier defined in config
                    //    currentPower = Mathf.RoundToInt(UniquePowerModifierConfigs[i].UpmBasePower * GetUnitSkillData(inventoryItem.UniquePowerModifierConfigs[i].AssociatedUnitSkillID).currentSkillLevel * inventoryItem.UniquePowerModifierConfigs[i].AssociatedUnitSkillPowerMultiplier);
                    //}

                    // create and add UPM data to the list
                    upmsData.Add(new UniquePowerModifierData
                    {
                        uniquePowerModifierID = new UniquePowerModifierID
                        {
                            inventoryItemID = inventoryItem.InventoryItemID,
                            uniquePowerModifierConfigIndex = i,
                            modifierOrigin = ModifierOrigin.Item,
                            destinationGameObjectID = dstPartyUnit.gameObject.GetInstanceID()
                        },
                        durationLeft = inventoryItem.UniquePowerModifierConfigs[i].UpmDurationMax,
                        //currentPower = currentPower
                        currentPower = inventoryItem.UniquePowerModifierConfigs[i].GetUpmEffectivePower(this)
                    });
                    Debug.LogWarning("Propagate [" + inventoryItem.InventoryItemID + "] Item UPM [" + inventoryItem.UniquePowerModifierConfigs[i].DisplayName + "] power [" + inventoryItem.UniquePowerModifierConfigs[i].GetUpmEffectivePower(this) + "] to [" + dstPartyUnit.UnitName + "] unit");
                }
            }
            //}
        }
        // return the list with UPMs data
        return upmsData;
    }

    public List<UniquePowerModifierData> GetAbilityPropagatedBonuses(PartyUnit dstPartyUnit)
    {
        // init list of UPMs data
        List<UniquePowerModifierData> upmsData = new List<UniquePowerModifierData>();
        // init current power variable
        int currentPower;
        // loop through the list of all UPMs in unit ability
        for (int i = 0; i < UniquePowerModifierConfigs.Count; i++)
        {
            // verify if UPM is passive
            if ((UniquePowerModifierConfigs[i].TriggerCondition == TriggerCondition.NonePassive)
                // verify if UPM relationships requirements are met
                && (UniquePowerModifierConfigs[i].MatchRelationships(this, dstPartyUnit))
                // verify if dst party unit hass mass scope
                && (UniquePowerModifierConfigs[i].ModifierScope.ModifierScopeID >= ModifierScopeID.EntireParty))
            {
                // verify if UPM is associated with a skill
                if (UniquePowerModifierConfigs[i].AssociatedUnitSkillID != UnitSkillID.None)
                {
                    currentPower = UniquePowerModifierConfigs[i].UpmBasePower;
                }
                else
                {
                    // get current power multiplied by this party unit skill level * multiplied by skill multiplier defined in config
                    currentPower = Mathf.RoundToInt(UniquePowerModifierConfigs[i].UpmBasePower * GetUnitSkillData(UniquePowerModifierConfigs[i].AssociatedUnitSkillID).currentSkillLevel * UniquePowerModifierConfigs[i].AssociatedUnitSkillPowerMultiplier);
                }
                // create and add UPM data to the list
                upmsData.Add(new UniquePowerModifierData
                {
                    uniquePowerModifierID = new UniquePowerModifierID
                    {
                        unitAbilityID = UnitAbilityID,
                        uniquePowerModifierConfigIndex = i,
                        modifierOrigin = ModifierOrigin.Ability,
                        destinationGameObjectID = dstPartyUnit.gameObject.GetInstanceID()
                    },
                    durationLeft = UniquePowerModifierConfigs[i].UpmDurationMax,
                    currentPower = currentPower
                });
            }
        }
        // return the list with UPMs data
        return upmsData;
    }

    public List<UniquePowerModifierData> GetPropagatedBonuses(PartyUnit dstPartyUnit)
    {
        // init list of UPMs data
        List<UniquePowerModifierData> upmsData = new List<UniquePowerModifierData>();
        // Find all UPMs, which are propagated by this party unit (normally this is party leader)
        // Those can be UPMs from:
        //  - unit ability
        //  - item
        //  - unit skill
        // add ability bonuses
        upmsData.AddRange(GetAbilityPropagatedBonuses(dstPartyUnit));
        // add items bonuses
        upmsData.AddRange(GetItemsPropagatedBonuses(dstPartyUnit));
        // add skills bonuses
        upmsData.AddRange(GetSkillsPropagatedBonuses(dstPartyUnit));
        // return the list with UPMs data
        return upmsData;
    }

    public List<UniquePowerModifierData> AddBonusesPropagatedbyPartyLeader(List<UniquePowerModifierData> allBonuses)
    {
        // try to get party
        HeroParty heroParty = GetComponentInParent<HeroParty>();
        // verify if we have hero party
        if (heroParty != null)
        {
            // try to get party leader
            PartyUnit partyLeader = heroParty.GetPartyLeader();
            // verify if unit is in city (parent city != null)
            if (partyLeader != null)
            {
                allBonuses.AddRange(partyLeader.GetPropagatedBonuses(this));
            }
        }
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> AddBonusesFromEquippedItems(List<UniquePowerModifierData> allBonuses)
    {
        // loop through all equipped items
        foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        {
            // verify if item is not in belt
            if ( ((HeroEquipmentSlots.BeltSlots & inventoryItem.CurrentHeroEquipmentSlot) != inventoryItem.CurrentHeroEquipmentSlot)
                // verify if item is not consumed (slot is none)
                && (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlots.None) )
            {
                allBonuses.AddRange(inventoryItem.GetPropagatedBonuses(this));
            }
        }
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> AddBonusesFromConsumedItems(List<UniquePowerModifierData> allBonuses)
    {
        // loop through all equipped items
        foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        {
            // verify if item is has been consumed (slot is none)
            if (inventoryItem.CurrentHeroEquipmentSlot == HeroEquipmentSlots.None)
            {
                allBonuses.AddRange(inventoryItem.GetPropagatedBonuses(this));
            }
        }
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> AddBonusesFromLearnedSkills(List<UniquePowerModifierData> allBonuses)
    {
        // loop through all unit skills
        foreach (UnitSkillData unitSkillData in PartyUnitData.unitSkillsData)
        {
            // verify if skill has been learned
            if (unitSkillData.currentSkillLevel > 0)
            {
                // get skill config
                UnitSkillConfig unitSkillConfig = ConfigManager.Instance[unitSkillData.unitSkill];
                // loop through all UPMs in skill config
                for (int i = 0; i < unitSkillConfig.UniquePowerModifierConfigs.Count; i++)
                {
                    // verify if UPM is passive
                    if ((unitSkillConfig.UniquePowerModifierConfigs[i].TriggerCondition == TriggerCondition.NonePassive)
                        // verify if UPM relationships requirements are met
                        && (unitSkillConfig.UniquePowerModifierConfigs[i].MatchRelationships(this, this)))
                    {
                        // create and add UPM data to the list
                        allBonuses.Add(new UniquePowerModifierData
                        {
                            uniquePowerModifierID = new UniquePowerModifierID
                            {
                                unitSkillID = unitSkillConfig.unitSkillID,
                                uniquePowerModifierConfigIndex = i,
                                modifierOrigin = ModifierOrigin.Skill,
                                destinationGameObjectID = gameObject.GetInstanceID()
                            },
                            durationLeft = unitSkillConfig.UniquePowerModifierConfigs[i].UpmDurationMax,
                            currentPower = Mathf.RoundToInt(unitSkillConfig.UniquePowerModifierConfigs[i].UpmBasePower * unitSkillData.currentSkillLevel * unitSkillConfig.UniquePowerModifierConfigs[i].AssociatedUnitSkillPowerMultiplier)
                        });
                    }
                }
            }
        }
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> AddBonusesFromPlayerUniqueAbility(List<UniquePowerModifierData> allBonuses)
    {
        Debug.Log("todo");
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> AddBonusesFromAppliedAbilities(List<UniquePowerModifierData> allBonuses)
    {
        foreach (UniquePowerModifierData uniquePowerModifierData in AppliedUniquePowerModifiersData)
        {
            // verify if UPM is passive
            if (uniquePowerModifierData.GetUniquePowerModifierConfig().TriggerCondition == TriggerCondition.NonePassive)
            {
                // add data to the list of all bonuses
                allBonuses.Add(uniquePowerModifierData);
            }
        }
        // return updated list of all bonuses
        return allBonuses;
    }

    public List<UniquePowerModifierData> GetAllBonuses()
    {
        // init list
        List<UniquePowerModifierData> allBonuses = new List<UniquePowerModifierData>();
        // add bonuses propagated by city
        allBonuses = AddBonusesPropagatedbyCity(allBonuses);
        // add bonuses propagated by party leader
        allBonuses = AddBonusesPropagatedbyPartyLeader(allBonuses);
        // get bonuses from equipped items (normally it is applicable only to party-leader)
        allBonuses = AddBonusesFromEquippedItems(allBonuses);
        //get gained bonuses from consumed items
        allBonuses = AddBonusesFromConsumedItems(allBonuses);
        // get bonuses from learned skills (normally it is applicable only to party-leader) 
        allBonuses = AddBonusesFromLearnedSkills(allBonuses);
        // get gained bonuses from player unique ability
        allBonuses = AddBonusesFromPlayerUniqueAbility(allBonuses);
        //get gained bonuses from applied abilities
        allBonuses = AddBonusesFromAppliedAbilities(allBonuses);
        // Debug log all bonuses
        foreach (UniquePowerModifierData uniquePowerModifierData in allBonuses)
        {
            Debug.Log("Bonus: " + uniquePowerModifierData.GetOriginDisplayName() + " power: " + uniquePowerModifierData.CurrentPower);
        }
        // return result
        return allBonuses;
    }

    Dictionary<UnitStatID, UnitStatData> statsDataCache = new Dictionary<UnitStatID, UnitStatData>(); // to prevent searching through array each time
    public UnitStatData GetStatData(UnitStatID unitStatID)
    {
        // verify if statsData is not set
        if (!statsDataCache.ContainsKey(unitStatID))
        {
            // int key
            statsDataCache[unitStatID] = Array.Find(PartyUnitData.unitStatsData, e => e.unitStatID == unitStatID);
        }
        // return
        return statsDataCache[unitStatID];
    }

    Dictionary<UnitStatID, UnitStatConfig> statConfigsCache = new Dictionary<UnitStatID, UnitStatConfig>(); // to prevent searching through array each time
    public UnitStatConfig GetStatConfig(UnitStatID unitStatID)
    {
        // verify if statsData is not set
        if (!statConfigsCache.ContainsKey(unitStatID))
        {
            // int key
            statConfigsCache[unitStatID] = Array.Find(PartyUnitConfig.unitStatConfigs, e => e.unitStatID == unitStatID);
        }
        // return
        return statConfigsCache[unitStatID];
    }

    public int GetBaseStatValue(UnitStatID unitStatID)
    {
        return GetStatConfig(unitStatID).baseValue;
    }

    public float GetStatModifierPowerValueByCalculationType(UnitStatID unitStatID, List<UniquePowerModifierData> allBonuses, ModifierCalculatedHow modifierCalculatedHow)
    {
        // int result
        float result;
        // verify if this is multiplier to all
        if (modifierCalculatedHow == ModifierCalculatedHow.PercentToAll)
        {
            result = 1f; // because it will be multiplied
        }
        else
        {
            result = 0f;
        }
        // predefine variable used in a loop
        UniquePowerModifierConfig uniquePowerModifierConfig;
        // loop through all bonuses
        foreach (UniquePowerModifierData uniquePowerModifierData in allBonuses)
        {
            // get upm config
            uniquePowerModifierConfig = uniquePowerModifierData.GetUniquePowerModifierConfig();
            // verify if unique power modifier is applied to required stat
            if ( (uniquePowerModifierConfig.ModifiedUnitStatID == unitStatID)
                // verify if upm is of required calculation type
                && (uniquePowerModifierConfig.ModifierCalculatedHow == modifierCalculatedHow) )
            {
                // verify if this is multiplier to all
                if (modifierCalculatedHow == ModifierCalculatedHow.PercentToAll)
                {
                    // multiply matching UPM power to result
                    result *= (100f - (float)uniquePowerModifierData.CurrentPower) / 100f;
                }
                else
                {
                    // add matching UPM power to result
                    result += uniquePowerModifierData.CurrentPower;
                }
            }
        }
        // return result
        return result;
    }

    public int GetEffectiveStatValue(UnitStatID unitStatID)
    {
        // get all passive bonuses
        List<UniquePowerModifierData> allBonuses = GetAllBonuses();
        // get base stat value
        float baseStat = GetBaseStatValue(unitStatID);
        // get total percent to base stat modifiers power
        float percentToBase = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.PercentToBase);
        // get total additive to base stat modifiers power
        float additiveToBase = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.Additively);
        // get total percent to all stat modifiers power
        float toAllMultiplier = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.PercentToAll);
        // return result based on the formula below
        return Mathf.RoundToInt(((baseStat * (100f + percentToBase) / 100f) + additiveToBase) * toAllMultiplier);
    }

    public int GetSameStatUPMsBonusesValue(List<UniquePowerModifierData> upmBonuses)
    {
        // verify if list is not empty
        if (upmBonuses.Count >= 1)
        {
            // init statID from the first upm in a list (assume that all UPMs are for the same stat)
            UnitStatID unitStatID = upmBonuses[0].GetUniquePowerModifierConfig().ModifiedUnitStatID;
            // get all passive bonuses
            List<UniquePowerModifierData> allBonuses = GetAllBonuses();
            // get base stat value
            float baseStat = GetBaseStatValue(unitStatID);
            // get total percent to base stat modifiers power
            float percentToBase = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.PercentToBase);
            // get total additive to base stat modifiers power
            float additiveToBase = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.Additively);
            // get total percent to all stat modifiers power
            float toAllMultiplier = GetStatModifierPowerValueByCalculationType(unitStatID, allBonuses, ModifierCalculatedHow.PercentToAll);
            // get total percent to base stat modifiers power
            float upmPercentToBase = GetStatModifierPowerValueByCalculationType(unitStatID, upmBonuses, ModifierCalculatedHow.PercentToBase);
            // get total additive to base stat modifiers power
            float upmAdditiveToBase = GetStatModifierPowerValueByCalculationType(unitStatID, upmBonuses, ModifierCalculatedHow.Additively);
            // get total percent to all stat modifiers power
            float upmToAllMultiplier = GetStatModifierPowerValueByCalculationType(unitStatID, upmBonuses, ModifierCalculatedHow.PercentToAll);
            // Get effective bonus value
            float effectiveBonusValue = Mathf.RoundToInt(((baseStat * (100f + percentToBase) / 100f) + additiveToBase) * toAllMultiplier);
            // Get effective bonus value without UPM bonuses
            float effectiveBonusValueWithoutUPMBonuses = ((baseStat * (100f + percentToBase - upmPercentToBase) / 100f) + additiveToBase - upmAdditiveToBase) * toAllMultiplier / upmToAllMultiplier;
            // return difference between effecitive bonuse with and without UPM
            return Mathf.RoundToInt(effectiveBonusValue - effectiveBonusValueWithoutUPMBonuses);
        }
        else
        {
            return 0;
        }
    }

    public int GetUPMBonusValue(UniquePowerModifierData uniquePowerModifierData)
    {
        // create upmBonuses list (just to be able to call the same functions)
        List<UniquePowerModifierData> upmBonuses = new List<UniquePowerModifierData> { uniquePowerModifierData };
        return GetSameStatUPMsBonusesValue(upmBonuses);
    }

    public int GetEffectiveDefense()
    {
        //// ADDITIVE
        //int totalAdditiveDefense = GetTotalAdditiveDefense();
        //// MULTIPLICATIVE
        //// Get additional defense mondifiers:
        //// Apply status modifier;
        //int restDamagePercent = 100 - totalAdditiveDefense;
        //int totalDefenseWithModifiers = totalAdditiveDefense + (int)Math.Round(restDamagePercent * GetDefensiveStanceBonus());
        //// Return result
        //return totalDefenseWithModifiers;
        return GetEffectiveStatValue(UnitStatID.Defense);
    }

    //public void RemoveAllBuffs()
    //{
    //    //Debug.Log("RemoveAllBuffs");
    //    // in unit properties
    //    for (int i = 0; i < UnitBuffs.Length; i++)
    //    {
    //        UnitBuffs[i] = UnitBuff.None;
    //    }
    //    // in UI
    //    UnitBuffIndicator[] allBuffs = GetUnitCell().Find("UnitStatus/Buffs").GetComponentsInChildren<UnitBuffIndicator>();
    //    foreach (UnitBuffIndicator buff in allBuffs)
    //    {
    //        Destroy(buff.gameObject);
    //    }
    //}

    //public void RemoveAllDebuffs()
    //{
    //    // in unit properties
    //    for (int i = 0; i < UnitDebuffs.Length; i++)
    //    {
    //        UnitDebuffs[i] = UnitDebuff.None;
    //    }
    //    // in UI
    //    UnitDebuffIndicator[] allDebuffs = GetUnitCell().Find("UnitStatus/Debuffs").GetComponentsInChildren<UnitDebuffIndicator>();
    //    foreach (UnitDebuffIndicator buff in allDebuffs)
    //    {
    //        Destroy(buff.gameObject);
    //    }
    //}

    //public void RemoveAllBuffsAndDebuffs()
    //{
    //    RemoveAllBuffs();
    //    RemoveAllDebuffs();
    //}


    public int GetAbilityDamageDealt(PartyUnit activeBattleUnit)
    {
        int damageDealt = 0;
        int srcUnitDamage = activeBattleUnit.GetUnitEffectivePower();
        int dstUnitDefense = GetEffectiveDefense();
        // calculate damage dealt
        damageDealt = (int)Math.Round((((float)srcUnitDamage * (100f - (float)dstUnitDefense)) / 100f));
        return damageDealt;
    }

    public int GetDebuffDamageDealt(UniquePowerModifierConfig appliedUniquePowerModifier)
    {
        Debug.LogWarning("Fix it: calculate current uniquepowermodifier power based on the active (source) unit statsupgradescount");
        // init damage dealt
        int damageDealt = 0;
        // get resistance multiplier
        float resistanceMultiplier = (100 - GetUnitEffectiveResistance(appliedUniquePowerModifier.UpmSource)) / 100;
        // verify if resistance multiplier is not less than 0
        if (resistanceMultiplier < 0)
        {
            // reset it to 0
            resistanceMultiplier = 0;
        }
        // get damage dealt based on the current upm power and destination unit restistance to the upm power source;
        damageDealt = Mathf.RoundToInt((float)appliedUniquePowerModifier.UpmBasePower * resistanceMultiplier);
        // return result
        return damageDealt;
    }

    //// Note: animation should be identical to the function with the same name in PartyPanel
    //public void FadeUnitCellInfo(float alpha)
    //{
    //    Text infoPanel = GetUnitCell().Find("InfoPanel").GetComponent<Text>();
    //    Color c = infoPanel.color;
    //    c.a = alpha;
    //    infoPanel.color = c;
    //}

    //public void DeactivateExpiredBuffs()
    //{
    //    // Deactivate expired buffs in UI
    //    // PartyUnit unit = GetComponent<PartyUnit>();
    //    UnitBuffIndicator[] buffsUI = GetUnitBuffsPanel().GetComponentsInChildren<UnitBuffIndicator>();
    //    foreach (UnitBuffIndicator buffUI in buffsUI)
    //    {
    //        // First decrement buff current duration
    //        buffUI.DecrementCurrentDuration();
    //        // Verify if it has timed out;
    //        if (buffUI.GetCurrentDuration() == 0)
    //        {
    //            // buff has timed out
    //            // deactivate it (it will be destroyed at the end of animation)
    //            buffUI.SetActiveAdvance(false);
    //            // deactivate it in unit properties too
    //            UnitBuffs[(int)buffUI.GetUnitBuff()] = UnitBuff.None;
    //        }
    //    }
    //}

    //public void TriggerAppliedDebuffs()
    //{
    //    //Debug.Log("TriggerAppliedDebuffs");
    //    UnitDebuffIndicator[] debuffsIndicators = GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
    //    //UnitDebuffsUI unitDebuffsUI = unit.GetUnitDebuffsPanel().GetComponent<UnitDebuffsUI>();
    //    foreach (UnitDebuffIndicator debuffIndicator in debuffsIndicators)
    //    {
    //        Debug.Log(name);
    //        // as long as we cannot initiate all debuffs at the same time
    //        // we add debuffs to the queue and they will be triggered one after another
    //        // CoroutineQueue queue = unitDebuffsUI.GetQueue();
    //        CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).GetQueue();
    //        //if (queue == null)
    //        //{
    //        //    Debug.LogError("No queue");
    //        //}
    //        //if (debuffIndicator == null)
    //        //{
    //        //    Debug.LogError("No debuffIndicator");
    //        //}
    //        IEnumerator coroutine = debuffIndicator.TriggerDebuff(GetComponent<PartyUnit>());
    //        //if (coroutine == null)
    //        //{
    //        //    Debug.LogError("No coroutine");
    //        //}
    //        queue.Run(coroutine);
    //        // Trigger debuff against player
    //        // Decrement buff current duration
    //        debuffIndicator.DecrementCurrentDuration();
    //    }
    //}

    //public void HighlightActiveUnitInBattle(bool doHighlight)
    //{
    //    // Highlight
    //    Color highlightColor;
    //    if (doHighlight)
    //    {
    //        //Debug.Log(" HighlightActiveUnitInBattle");
    //        highlightColor = Color.white;
    //    }
    //    else
    //    {
    //        //Debug.Log(" Remove highlight from active unit in battle");
    //        highlightColor = Color.grey;
    //    }
    //    // highlight unit canvas with required color
    //    Text canvasText = GetUnitCell().Find("Br").GetComponent<Text>();
    //    canvasText.color = highlightColor;
    //}

    //public string GetUnitStatusString()
    //{
    //    switch (unitStatus)
    //    {
    //        case UnitStatus.Active:
    //            return "";
    //        case UnitStatus.Dead:
    //            return "Dead";
    //        case UnitStatus.Escaped:
    //            return "Escaped";
    //        case UnitStatus.Waiting:
    //            return "Waiting";
    //        default:
    //            Debug.LogError("Unknown unit status");
    //            return null;
    //    }
    //}

    //public Color GetUnitStatusColor()
    //{
    //    switch (unitStatus)
    //    {
    //        case UnitStatus.Active:
    //            return Color.gray;
    //        case UnitStatus.Dead:
    //            return new Color32(64, 64, 64, 255); // darkest grey
    //        case UnitStatus.Escaped:
    //            return new Color32(96, 96, 96, 255); // darker grey
    //        case UnitStatus.Waiting:
    //            return new Color32(128, 128, 128, 255); // grey
    //        default:
    //            Debug.LogError("Unknown unit status");
    //            return Color.red;
    //    }
    //}

    //public IEnumerator EscapeBattle()
    //{
    //    Debug.LogWarning("EscapeBattle " + name);
    //    // set escaped status
    //    SetUnitStatus(UnitStatus.Escaped);
    //    // Play animation
    //    yield return new WaitForSeconds(1f);
    //}

    //public void SetUnitStatus(UnitStatus value)
    //{
    //    Debug.Log("Set unit " + UnitName + " status " + value.ToString());
    //    UnitStatus = value;
    //    // get new UI color according ot unit status
    //    Color32 newUIColor;
    //    // set dead in status
    //    string statusString;
    //    switch (value)
    //    {
    //        case UnitStatus.Active:
    //            newUIColor = new Color32(160, 160, 160, 255);
    //            statusString = "";
    //            break;
    //        case UnitStatus.Waiting:
    //            newUIColor = new Color32(96, 96, 96, 96);
    //            statusString = "Waiting";
    //            break;
    //        case UnitStatus.Escaping:
    //            newUIColor = new Color32(96, 96, 96, 255);
    //            statusString = "Escaping";
    //            break;
    //        case UnitStatus.Escaped:
    //            newUIColor = new Color32(64, 64, 64, 255);
    //            statusString = "Escaped";
    //            // clear unit buffs and debuffs
    //            RemoveAllBuffsAndDebuffs();
    //            break;
    //        case UnitStatus.Dead:
    //            newUIColor = new Color32(64, 64, 64, 255);
    //            statusString = "Dead";
    //            // clear unit buffs and debuffs
    //            RemoveAllBuffsAndDebuffs();
    //            break;
    //        default:
    //            Debug.LogError("Unknown status " + value.ToString());
    //            newUIColor = Color.red;
    //            statusString = "Error";
    //            break;
    //    }
    //    GetUnitCurrentHealthText().color = newUIColor;
    //    GetUnitMaxHealthText().color = newUIColor;
    //    GetUnitCanvasText().color = newUIColor;
    //    GetUnitStatusText().color = newUIColor;
    //    GetUnitStatusText().text = statusString;
    //}

    public int GetOffenceSkillPowerBonus(UnitSkillData skill = null)
    {
        // verify if custom skill parameter was passed
        if (skill == null)
        {
            // get local party unit skill
            skill = Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.Offense);
        }
        return (int)Math.Round(UnitAbilityEffectivePower * skill.currentSkillLevel * 0.15f);
    }

    public int GetItemsPowerBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.Power, UnitAbilityEffectivePower);
    }

    public int GetUnitEffectivePower()
    {
        // get unit power plus skill bonus
        return UnitAbilityEffectivePower + GetOffenceSkillPowerBonus() + GetItemsPowerBonus();
    }

    List<UnitStatModifier> GetItemUnitStatModifierByStat(UnitStatID unitStat)
    {
        // init list of UnitStatModifier from items, there can be more than one
        List<UnitStatModifier> usmsList = new List<UnitStatModifier>();
        // loop through all items for this unit
        foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        {
            // verify if item is not in belt
            //if (   (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlot.BeltSlot1)
            //    && (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlot.BeltSlot2))
            if ((HeroEquipmentSlots.BeltSlots & inventoryItem.CurrentHeroEquipmentSlot) != inventoryItem.CurrentHeroEquipmentSlot)
            {
                // verify if item gives required stat bonus
                // loop through item stat modifiers
                foreach (UnitStatModifier usm in inventoryItem.UnitStatModifiers)
                {
                    // verify if usm applies to the required stat type and that stat is not instant (duration != 0)
                    if ((usm.unitStat == unitStat) && (usm.duration != 0))
                    {
                        //// .. verify if there is no already same item and that its unit stat modifier is not stackable, maybe this check is not needed here, because it is done before item is consumed
                        //Debug.Log("Verify non-stackable USMs from the same item");
                        // verify if this is shard slot item
                        if ((inventoryItem.CurrentHeroEquipmentSlot & HeroEquipmentSlots.Shard) == HeroEquipmentSlots.Shard)
                        {
                            // apply skill modifier to USMs power
                            usm.skillPowerMultiplier = GetUnitSkillData(UnitSkillID.ShardAura).currentSkillLevel; // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.ShardAura).currentSkillLevel;
                        }
                        // add modifier to the list
                        usmsList.Add(usm);
                    }
                }
            }
        }
        // get leader items, which has global scope
        // verify if this is not leader unit, to not apply the same item 2 times
        // we assume that only leader can have items with global scope
        if (!IsLeader)
        {
            // verify if this unit already part of a hero party, because it may be from hire unit menu
            if (GetComponentInParent<HeroParty>() != null)
            {
                // Get party leader unit
                PartyUnit partyLeader = GetComponentInParent<HeroParty>().GetPartyLeader();
                // verify if party has leader, this is not the case for non-capital city garnizons
                if (partyLeader != null)
                {
                    // loop through all items owned by party leader
                    foreach (InventoryItem inventoryItem in partyLeader.GetComponentsInChildren<InventoryItem>())
                    {
                        // verify if item is not for belt
                        //if ((inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlot.BeltSlot1)
                        //    && (inventoryItem.CurrentHeroEquipmentSlot != HeroEquipmentSlot.BeltSlot2))
                        if ((HeroEquipmentSlots.BeltSlots & inventoryItem.CurrentHeroEquipmentSlot) != inventoryItem.CurrentHeroEquipmentSlot)
                        {
                            // verify if item gives stat bonus
                            // loop through item stat modifiers
                            foreach (UnitStatModifier usm in inventoryItem.UnitStatModifiers)
                            {
                                // verify if usm applies to the stat and that it has mass scope
                                if ((usm.unitStat == unitStat) && (usm.modifierScope >= ModifierScopeID.EntireParty))
                                {
                                    //// .. verify if there is no already same item and that its unit stat modifier is not stackable, maybe this check is not needed here, because it is done before item is consumed
                                    //Debug.Log("Verify non-stackable USMs from the same item with entire party scope");
                                    // verify if this is shard slot item
                                    //if (inventoryItem.CurrentHeroEquipmentSlot == HeroEquipmentSlot.Shard)
                                    if ((inventoryItem.CurrentHeroEquipmentSlot & HeroEquipmentSlots.Shard) == HeroEquipmentSlots.Shard)
                                    {
                                        // apply skill modifier to USMs power
                                        usm.skillPowerMultiplier = GetUnitSkillData(UnitSkillID.ShardAura).currentSkillLevel; // Array.Find(partyLeader.UnitSkillsData, element => element.unitSkillID == UnitSkillID.ShardAura).currentSkillLevel;
                                    }
                                    // add modifier to the list
                                    usmsList.Add(usm);
                                }
                            }
                        }
                    }
                }
            }
        }
        return usmsList;
    }

    int GetGenericStatItemBonus(UnitStatID unitStat, int statBaseValue)
    {
        // init bonus value
        int statTotalBonus = 0;
        // get list of uni unit stat modifiers from unit's items
        List<UnitStatModifier> usmsList = GetItemUnitStatModifierByStat(unitStat);
        // apply usms
        foreach (UnitStatModifier usm in usmsList)
        {
            // get new stat total bonus
            statTotalBonus += GetUSMStatBonus(usm, statBaseValue);
        }
        // return result
        return statTotalBonus;
    }

    public int GetLeadershipItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.Leadership, UnitLeadership);
    }

    public int GetEffectiveLeadership()
    {
        // get current skill leadership bonus = skill level
        // UnitSkillData skill = Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Leadership);
        int skillBonus = GetUnitSkillData(UnitSkillID.Leadership).currentSkillLevel; // .currentSkillLevel;
        // get items bonus
        int itemsBonus = GetLeadershipItemsBonus();
        // return result
        return UnitLeadership + skillBonus + itemsBonus;
    }

    public float GetPathfindingSkillMultiplier()
    {
        // get current skill level
        int currentSkillLevel = GetUnitSkillData(UnitSkillID.Pathfinding).currentSkillLevel; // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Pathfinding).currentSkillLevel;
        // get and return bonus multiplier
        return (float)currentSkillLevel * 0.5f;
    }

    public int GetAdditiveMovePoints()
    {
        // get all move points which stack additively
        // get move points without bonuses
        int totalMovePoints = MovePointsMax;
        // get other move points
        // ..
        return totalMovePoints;
    }

    public int GetPathfindingSkillBonus()
    {
        // get return additional move points
        return (int)Math.Round(GetPathfindingSkillMultiplier() * GetAdditiveMovePoints());
    }

    public int GetMovePointsItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.MovePoints, MovePointsMax);
    }

    public int GetScoutingSkillBonus()
    {
        // get and return current skill level (= scouting bonus)
        return GetUnitSkillData(UnitSkillID.Scouting).currentSkillLevel; // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Scouting).currentSkillLevel;
    }

    public int GetLeadershipSkillBonus()
    {
        // get and return current skill level (= scouting bonus)
        return GetUnitSkillData(UnitSkillID.Leadership).currentSkillLevel; // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Leadership).currentSkillLevel;
    }

    public int GetEffectiveMaxMovePoints()
    {
        // return result
        return GetAdditiveMovePoints() + GetPathfindingSkillBonus() + GetMovePointsItemsBonus();
    }

    public int GetScoutingPointsItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.ScoutingRange, ScoutingRange);
    }

    public int GetItemsHealthBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.Health, UnitHealthMax);
    }

    public int GetEffectiveScoutingRange()
    {
        // get and return sum of default scouting range plus skill bonus
        return ScoutingRange + GetScoutingSkillBonus() + GetScoutingPointsItemsBonus();
    }

    public int GetInitiativeSkillBonus()
    {
        // get skill level multiplied by 15
        return GetUnitSkillData(UnitSkillID.Initiative).currentSkillLevel * 15; ; // Array.Find(UnitSkillsData, element => element.unitSkillID == UnitSkillID.Initiative).currentSkillLevel * 15;
    }

    public int GetInitiativeItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStatID.Initiative, UnitBaseInitiative);
    }

    public int GetEffectiveInitiative()
    {
        // get and return sum of default scouting range plus skill bonus
        return UnitBaseInitiative + GetInitiativeSkillBonus() + GetInitiativeItemsBonus();
    }

    public int GetUnitHealthRegenPerDay()
    {
        return (int)Math.Floor(((float)UnitHealthMax * (float)UnitHealthRegenPercent) / 100f);
    }

    public float GetUnitHealSkillHealthRegenModifier()
    {
        // get and return skill current level multiplied by 0.15
        return (float)Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.Healing).currentSkillLevel * 0.15f;
    }

    public int GetUnitHealSkillHealthRegenBonusPerDay()
    {
        // get and return skill current level multiplied by 0.15
        return (int)Math.Floor(GetUnitHealSkillHealthRegenModifier() * (float)UnitHealthMax);
    }

    public int GetUnitHealItemsHealthRegenBonusPerDay()
    {
        // get and return skill current level multiplied by 0.15
        return (int)Math.Floor(((float)GetItemsHealthBonus() * (float)UnitHealthRegenPercent) / 100f);
    }

    public float GetUnitEffectiveHealthRegen()
    {
        return ((float)UnitHealthRegenPercent / 100f + GetUnitHealSkillHealthRegenModifier());
    }

    public int GetUnitEffectiveHealthRegenPerDay()
    {
        return (int)Math.Floor((float)GetUnitEffectiveMaxHealth() * GetUnitEffectiveHealthRegen());
    }

    public int GetUnitResistanceSkillBonus(PowerSource source)
    {
        switch (source)
        {
            case PowerSource.Death:
                return Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.DeathResistance).currentSkillLevel * 50;
            case PowerSource.Fire:
                return Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.FireResistance).currentSkillLevel * 50;
            case PowerSource.Mind:
                return Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.MindResistance).currentSkillLevel * 50;
            case PowerSource.Water:
                return Array.Find(UnitSkillsData, element => element.unitSkill == UnitSkillID.WaterResistance).currentSkillLevel * 50;
            case PowerSource.Wind:
            case PowerSource.Pure:
            case PowerSource.Earth:
            case PowerSource.Life:
            case PowerSource.Physical:
                return 0;
            default:
                Debug.LogError("Unknown source " + source.ToString());
                return 0;
        }
    }

    public int GetUnitResistanceItemsBonusBySource(PowerSource source)
    {
        switch (source)
        {
            case PowerSource.Death:
                return GetGenericStatItemBonus(UnitStatID.DeathResistance, GetUnitBaseResistance(source));
            case PowerSource.Fire:
                return GetGenericStatItemBonus(UnitStatID.FireResistance, GetUnitBaseResistance(source));
            case PowerSource.Mind:
                return GetGenericStatItemBonus(UnitStatID.MindResistance, GetUnitBaseResistance(source));
            case PowerSource.Water:
                return GetGenericStatItemBonus(UnitStatID.WaterResistance, GetUnitBaseResistance(source));
            case PowerSource.Wind:
            case PowerSource.Pure:
            case PowerSource.Earth:
            case PowerSource.Life:
            case PowerSource.Physical:
                Debug.Log("Not implemented resistance source: " + source);
                return 0;
            default:
                Debug.LogError("Unknown source " + source.ToString());
                return 0;
        }
    }

    public int GetUnitBaseResistance(PowerSource source)
    {
        // Get Resistance from config
        Resistance resistance = Array.Find(UnitBaseResistances, e => e.source == source);
        // Verify if unit has this resistance = value is not null
        if (resistance != null)
        {
            return resistance.percent;
        }
        else
        {
            // no resistance = 0
            return 0;
        }
    }

    public int GetUnitEffectiveResistance(PowerSource source)
    {
        // get and return effective resistance
        return GetUnitBaseResistance(source) + GetUnitResistanceSkillBonus(source) + GetUnitResistanceItemsBonusBySource(source);
    }

    bool ApplyActiveUPM(UniquePowerModifierConfig uniquePowerModifier, bool doPreview = false)
    {
        Debug.LogWarning("Apply unique power modifier with instant duration to the enemy");
        return false;
    }

    public int GetUSMStatBonus(UnitStatModifier unitStatModifier, int baseStatMaxValue)
    {
        switch (unitStatModifier.modifierCalculatedHow)
        {
            case ModifierCalculatedHow.Additively:
                // increase current value by amout unit stat modifier by power
                return unitStatModifier.skillPowerMultiplier * unitStatModifier.modifierPower;
            case ModifierCalculatedHow.PercentToBase:
                // increase current value by amout of base value multiplied by power
                return unitStatModifier.skillPowerMultiplier * baseStatMaxValue * unitStatModifier.modifierPower / 100; // note use float and ceil to int like below
            case ModifierCalculatedHow.PercentToAll:
                // increase current value by amout of percent from base value
                return Mathf.CeilToInt( ((float)unitStatModifier.skillPowerMultiplier * (float)baseStatMaxValue * (float)unitStatModifier.modifierPower) / 100f );
            default:
                Debug.LogError("Do not know how to apply modifier " + unitStatModifier.modifierCalculatedHow.ToString());
                return 0;
        }
    }

    int ApplyInstantUSMPower(UnitStatModifier unitStatModifier, int baseStatCurrentValue, int baseStatMaxValue, bool allowMaxOverflow = true)
    {
        Debug.Log("Apply unit state modifier power " + unitStatModifier.modifierCalculatedHow.ToString());
        // get usm bonus
        int usmBonus = GetUSMStatBonus(unitStatModifier, baseStatMaxValue);
        // get new stat current value
        baseStatCurrentValue += usmBonus;
        // verify if current value is not higher than max value
        if ((baseStatCurrentValue > baseStatMaxValue) && !allowMaxOverflow)
        {
            // reset current value to max
            baseStatCurrentValue = baseStatMaxValue;
        }
        // return new value of current stat
        return baseStatCurrentValue;
    }

    bool MatchStatuses(UnitStatus[] matchStatuses)
    {
        foreach(UnitStatus matchStatus in matchStatuses)
        {
            if (UnitStatus == matchStatus)
            {
                return true;
            }
        }
        return false;
    }

    //bool ApplyUSMToCurrentStatValue(UnitStatModifier unitStatModifier, bool doPreview = false)
    //{
    //    Debug.LogWarning("Apply unit stat modifier");
    //    switch (unitStatModifier.unitStat)
    //    {
    //        case UnitStatID.Leadership:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.Health:
    //            // verify if unit health is not max already and if unit is not dead
    //            if (UnitHealthCurr != GetUnitEffectiveMaxHealth())
    //            {
    //                // verify if it is not preview
    //                if (!doPreview)
    //                {
    //                    // apply usm power to this unit
    //                    UnitHealthCurr = ApplyInstantUSMPower(unitStatModifier, UnitHealthCurr, GetUnitEffectiveMaxHealth(), false);
    //                }
    //                // item is applicable to this unit
    //                return true;
    //            }
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.Defense:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.Power:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.Initiative:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.MovePoints:
    //            // verify if unit is a party leader 
    //            if (IsLeader)
    //            {
    //                // verify if current move points are not max already
    //                if (MovePointsCurrent != GetEffectiveMaxMovePoints())
    //                {
    //                    // verify if it is not preview
    //                    if (!doPreview)
    //                    {
    //                        // apply usm power to this unit
    //                        MovePointsCurrent = ApplyInstantUSMPower(unitStatModifier, MovePointsCurrent, GetEffectiveMaxMovePoints(), false);
    //                    }
    //                    // item is applicable to this unit
    //                    return true;
    //                }
    //            }
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.ScoutingRange:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.DeathResistance:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.FireResistance:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.WaterResistance:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        case UnitStatID.MindResistance:
    //            // not applicable for instant unit stat modifier, because it is not consumable unit stat
    //            // by default item is not applicable
    //            return false;
    //        default:
    //            Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
    //            // by default item is not applicable
    //            return false;
    //    }
    //}

    //bool ApplyUSMToMaxStatValue(UnitStatModifier unitStatModifier, bool doPreview = false)
    //{
    //    Debug.LogWarning("Apply unit stat modifier");
    //    // init resistance var
    //    Resistance resistance;
    //    // 
    //    switch (unitStatModifier.unitStat)
    //    {
    //        case UnitStat.Leadership:
    //            // verify if unit is a party leader 
    //            if (IsLeader)
    //            {
    //                if (!doPreview)
    //                    UnitLeadership += GetUSMStatBonus(unitStatModifier, UnitLeadership);
    //                // return usm can be applied as result;
    //                return true;
    //            }
    //            // this is not applicable to non-leaders
    //            return false;
    //        case UnitStat.Health:
    //            if (!doPreview)
    //                UnitHealthMax += GetUSMStatBonus(unitStatModifier, UnitHealthMax);
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.Defense:
    //            if (!doPreview)
    //                UnitDefense += GetUSMStatBonus(unitStatModifier, UnitDefense);
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.Power:
    //            if (!doPreview)
    //                UnitPower += GetUSMStatBonus(unitStatModifier, UnitPower);
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.Initiative:
    //            if (!doPreview)
    //                UnitInitiative += GetUSMStatBonus(unitStatModifier, UnitInitiative);
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.MovePoints:
    //            // verify if unit is a party leader 
    //            if (IsLeader)
    //            {
    //                if (!doPreview)
    //                    MovePointsMax += GetUSMStatBonus(unitStatModifier, MovePointsMax);
    //                // return usm can be applied as result;
    //                return true;
    //            }
    //            // this is not applicable to non-leaders
    //            return false;
    //        case UnitStat.ScoutingRange:
    //            // verify if unit is a party leader 
    //            if (IsLeader)
    //            {
    //                if (!doPreview)
    //                    ScoutingRange += GetUSMStatBonus(unitStatModifier, ScoutingRange);
    //                // return usm can be applied as result;
    //                return true;
    //            }
    //            // this is not applicable to non-leaders
    //            return false;
    //        case UnitStat.DeathResistance:
    //            if (!doPreview)
    //            {
    //                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Death);
    //                resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            }
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.FireResistance:
    //            if (!doPreview)
    //            {
    //                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Fire);
    //                resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            }
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.WaterResistance:
    //            if (!doPreview)
    //            {
    //                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Water);
    //                resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            }
    //            // return usm can be applied as result;
    //            return true;
    //        case UnitStat.MindResistance:
    //            if (!doPreview)
    //            {
    //                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Mind);
    //                resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            }
    //            // return usm can be applied as result;
    //            return true;
    //        default:
    //            Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
    //            // by default item is not applicable
    //            return false;
    //    }
    //}

    bool VerifyIfUSMCanBeAppliedToMaxStatValue(UnitStatModifier unitStatModifier)
    {
        Debug.Log("Verify if unit stat modifier can be applied to max stat");
        switch (unitStatModifier.unitStat)
        {
            case UnitStatID.Leadership:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStatID.Health:
                // return usm can be applied as result;
                return true;
            case UnitStatID.Defense:
                // return usm can be applied as result;
                return true;
            case UnitStatID.Power:
                // return usm can be applied as result;
                return true;
            case UnitStatID.Initiative:
                // return usm can be applied as result;
                return true;
            case UnitStatID.MovePoints:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStatID.ScoutingRange:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStatID.DeathResistance:
                // return usm can be applied as result;
                return true;
            case UnitStatID.FireResistance:
                // return usm can be applied as result;
                return true;
            case UnitStatID.WaterResistance:
                // return usm can be applied as result;
                return true;
            case UnitStatID.MindResistance:
                // return usm can be applied as result;
                return true;
            default:
                Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
                // by default item is not applicable
                return false;
        }
    }

    InventoryItem GetUnitItemByName(string itemName)
    {
        // loop through all items in this unit
        foreach (InventoryItem currentUnitItem in GetComponentsInChildren<InventoryItem>())
        {
            // normally item name is also used as unique item identifier, that is why it is enough to compare 2 item names
            // compare if item which is about to be consumed is the same which unit already has
            if (currentUnitItem.ItemName == itemName)
            {
                Debug.LogWarning("Found " + itemName + " item in unit items");
                return currentUnitItem;
            }
        }
        Debug.LogWarning("Failed to find " + itemName + " item");
        return null;
    }

    //public void RemoveUSMEffectOnMaxStatValue(UnitStatModifier unitStatModifier)
    //{
    //    Debug.Log("Remove USM's effect on max stat value");
    //    // init resistance var
    //    Resistance resistance;
    //    // 
    //    switch (unitStatModifier.unitStat)
    //    {
    //        case UnitStat.Leadership:
    //            UnitLeadership -= GetUSMStatBonus(unitStatModifier, UnitLeadership);
    //            break;
    //        case UnitStat.Health:
    //            UnitHealthMax -= GetUSMStatBonus(unitStatModifier, UnitHealthMax);
    //            break;
    //        case UnitStat.Defense:
    //            UnitDefense -= GetUSMStatBonus(unitStatModifier, UnitDefense);
    //            break;
    //        case UnitStat.Power:
    //            UnitPower -= GetUSMStatBonus(unitStatModifier, UnitPower);
    //            break;
    //        case UnitStat.Initiative:
    //            UnitInitiative -= GetUSMStatBonus(unitStatModifier, UnitInitiative);
    //            break;
    //        case UnitStat.MovePoints:
    //            MovePointsMax -= GetUSMStatBonus(unitStatModifier, MovePointsMax);
    //            break;
    //        case UnitStat.ScoutingRange:
    //            ScoutingRange -= GetUSMStatBonus(unitStatModifier, ScoutingRange);
    //            break;
    //        case UnitStat.DeathResistance:
    //            resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Death);
    //            resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            break;
    //        case UnitStat.FireResistance:
    //            resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Fire);
    //            resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            break;
    //        case UnitStat.WaterResistance:
    //            resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Water);
    //            resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            break;
    //        case UnitStat.MindResistance:
    //            resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Mind);
    //            resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
    //            break;
    //        default:
    //            Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
    //            // by default item is not applicable
    //            break;
    //    }
    //}

    bool MatchItemScope(ModifierScopeID modifierScope)
    {
        switch (modifierScope)
        {
            case ModifierScopeID.Self:
            case ModifierScopeID.SingleUnit:
            case ModifierScopeID.EntireParty:
            case ModifierScopeID.AllPlayerUnits:
                return true;
            default:
                Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
                return false;
        }
        // Note: this should be remade:
        //// verify if we are in edit party screen
        //if (UIRoot.Instance.transform.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        //{
        //    switch (modifierScope)
        //    {
        //        case ModifierScope.Self:
        //        case ModifierScope.FriendlyUnit:
        //        case ModifierScope.FriendlyParty:
        //            return true;
        //        case ModifierScope.EnemyUnit:
        //        case ModifierScope.EnemyParty:
        //            return false;
        //        default:
        //            Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
        //            return false;
        //    }
        //}
        //// verify if we are in battle screen
        //else if (UIRoot.Instance.transform.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        //{
        //    // get party leader who has this item equipped
        //    PartyUnit itemWearerLeaderUnit = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.transform.parent.GetComponent<PartyUnit>();
        //    switch (modifierScope)
        //    {
        //        case ModifierScope.Self:
        //            // verify if itemWearerLeaderUnit matches current unit
        //            if (itemWearerLeaderUnit.gameObject.GetInstanceID() == gameObject.GetInstanceID())
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        case ModifierScope.FriendlyUnit:
        //        case ModifierScope.FriendlyParty:
        //            // verify if faction matches
        //            if (itemWearerLeaderUnit.GetComponentInParent<HeroParty>().Faction == GetComponentInParent<HeroParty>().Faction)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        case ModifierScope.EnemyUnit:
        //        case ModifierScope.EnemyParty:
        //            // verify if faction does not match
        //            if (itemWearerLeaderUnit.GetComponentInParent<HeroParty>().Faction != GetComponentInParent<HeroParty>().Faction)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        default:
        //            Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
        //            return false;
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Unknown screen active");
        //    // by default return false
        //    return false;
        //}
    }

    // doPreview - use item without actually using it, just verify if item can be used by this unit
    //public bool UseItem(InventoryItem inventoryItem, bool doPreview = false)
    //{
    //    // consumable items can have different duration, for example
    //    // healing poition - instant usm (0)
    //    // apply scroll with harmful spell on the enemy - instant upm (0)
    //    // potion of agility - duration 1 day (>=1), increases unit's inititative by x
    //    // potion of Titan might - duration is permanent (<0), increases unit's strength by 10%
    //    // voodoo doll - instant upm, with 3 usages
    //    // resurection stone - instant usm, with 3 usages
    //    // to be more generic we assume that somebody may create item with different UPMs and USMs with differnet durations
    //    // init list of UPMs and USMs ids which are one-time and should be removed from the list of UPMs and USMs after being used
    //    //List<int> upmIDsTobeRemoved = new List<int>();
    //    //List<int> usmIDsTobeRemoved = new List<int>();
    //    // init is applicable by default with false, if at least one modifier is applicable it should reset this flag to true
    //    bool isApplicable = false;
    //    // verify if the same item is not already applying its UPMs and USMs, because bonuses from the same items may be not stackable
    //    // normally this check is not needed for the entire-party scope items, which are equipped on the party leader, 
    //    // that is why we do not do this additional check against partly-leader-equipped items
    //    if ((GetUnitItemByName(inventoryItem.ItemName) != null)
    //        // and that item is not stackable
    //        && (!inventoryItem.ItemIsStackable)
    //        // and that item is not in equipment slot
    //        && (inventoryItem.CurrentHeroEquipmentSlot == HeroEquipmentSlots.None))
    //    {
    //        // unit already has this item
    //        Debug.LogWarning("Unit already has " + inventoryItem.name + " item or its bonuses applied. The same item cannot be applied or consumed again.");
    //        // same item cannot be applied once again
    //        isApplicable = false;
    //    }
    //    else
    //    {
    //        // unit does not have this item yet or item is stackable, do additional checks
    //        // consume unique power modifiers
    //        for (int i = 0; i < inventoryItem.UniquePowerModifierConfigs.Count; i++)
    //        {
    //            // verify if this is active modifier
    //            if ((inventoryItem.UniquePowerModifierConfigs[i].ModifierAppliedHow == TriggerCondition.Active)
    //            // verify if UPM required statuses match current unit status
    //            && (MatchStatuses(inventoryItem.UniquePowerModifierConfigs[i].CanBeAppliedToTheUnitsWithStatuses))
    //            // verify if UPM scope matches
    //            && (MatchItemScope(inventoryItem.UniquePowerModifierConfigs[i].ModifierScope)))
    //            {
    //                // upm has instant one-time effect

    //                // or temporary effect defined by the Duration in number of game or battle turns
    //                // it is destroyed before start of the next turn if duration reaches
    //                // it is applied automatically during calculations (for example damage or defence calculations)

    //                // upm is normally applied only during the battle and to the enemy
    //                // if we are in this situation, then it means that enemy has applied item with upm on this hero

    //                // verify whether it is applied to base, max or current stat values
    //                // apply upm
    //                bool modifierCanBeApplied = ApplyActiveUPM(inventoryItem.UniquePowerModifierConfigs[i], doPreview);
    //                // if at least one upm is applicable, then set applicable flag to true
    //                if (modifierCanBeApplied)
    //                    isApplicable = true;
    //                //    upmIDsTobeRemoved.Add(i);
    //            }
    //            else
    //            {
    //                // this is passive modifier
    //                // it is applied automatically during calculations (for example damage or defence calculations)
    //            }
    //        }
    //        for (int i = 0; i < inventoryItem.UnitStatModifiers.Count; i++)
    //        {
    //            // verify if this is active USM
    //            if ((inventoryItem.UnitStatModifiers[i].modifierAppliedHow == TriggerCondition.Active)
    //            // verify if USM required statuses match current unit status
    //            && (MatchStatuses(inventoryItem.UnitStatModifiers[i].canBeAppliedToTheUnitsWithStatuses))
    //            // verify if USM scope matches
    //            && (MatchItemScope(inventoryItem.UnitStatModifiers[i].modifierScope)))
    //            {
    //                // verify if usm applies to current stats
    //                if (inventoryItem.UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.CurrentStat)
    //                {
    //                    // apply usm
    //                    bool modifierCanBeApplied = ApplyUSMToCurrentStatValue(inventoryItem.UnitStatModifiers[i], doPreview);
    //                    // if at least one usm is applicable, then set applicable flag to true
    //                    if (modifierCanBeApplied)
    //                        isApplicable = true;
    //                }
    //                else if (inventoryItem.UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.MaxStat)
    //                {
    //                    // apply usm, without actually applying it, because it will be applied passively
    //                    bool modifierCanBeApplied = VerifyIfUSMCanBeAppliedToMaxStatValue(inventoryItem.UnitStatModifiers[i]);
    //                    // if at least one usm is applicable, then set applicable flag to true
    //                    if (modifierCanBeApplied)
    //                        isApplicable = true;
    //                }
    //                else
    //                {
    //                    Debug.LogError("Do not know to which stat (current or max) unit stat modifier is applied to");
    //                }
    //                // usm has instant one-time effect
    //                // instant modifiers can applied only to the current stat value (example current health)
    //                // but they cannot be applied to the base or max value (example to max health)

    //                // or temporary effect defined by the Duration in number of game or battle turns
    //                // it is destroyed before start of the next turn if duration reaches
    //                // it is applied automatically during calculations (for example damage or defence calculations)

    //                // verify whether it is applied to base, max or current stat values
    //                // verify if it is applicable and that item is not usable any more
    //                //if (isApplicable && (inventoryItem.LeftUsagesCount <= 1))
    //                //    usmIDsTobeRemoved.Add(i);
    //            }
    //            else
    //            {
    //                // usm applied passively from being placed in equipment slot
    //                // or it does not apply
    //                // it is applied automatically during calculations (for example damage or defence calculations)
    //            }
    //        }
    //        //// verify if there are unit status modifiers
    //        //if (inventoryItem.UnitStatusModifiers.Count >= 1)
    //        //{
    //        //    // verify if unit status matches
    //        //    if ((MatchStatuses(inventoryItem.UnitStatusModifiers[0].canBeAppliedToTheUnitsWithStatuses))
    //        //    // verify if scope matches
    //        //    && (MatchItemScope(inventoryItem.UnitStatusModifiers[0].modifierScope)))
    //        //    {
    //        //        // set is applicable flag
    //        //        isApplicable = true;
    //        //        // verify if this is not preview
    //        //        if (!doPreview)
    //        //        {
    //        //            // Apply unit Status
    //        //            UnitStatus = inventoryItem.UnitStatusModifiers[0].modifierSetStatus;
    //        //        }
    //        //    }
    //        //}
    //    }
    //    // verify if at leats one UPM or USM has been applied
    //    if (isApplicable)
    //    {
    //        // verify if this is not preview
    //        if (!doPreview)
    //        {
    //            //// destroy one-time instant USMs and UPMs
    //            //foreach (int upmIDTobeRemoved in upmIDsTobeRemoved)
    //            //{
    //            //    inventoryItem.UniquePowerModifiers.RemoveAt(upmIDTobeRemoved);
    //            //}
    //            //foreach (int usmIDTobeRemoved in usmIDsTobeRemoved)
    //            //{
    //            //    inventoryItem.UnitStatModifiers.RemoveAt(usmIDTobeRemoved);
    //            //}
    //            // decrement usages left counter
    //            inventoryItem.LeftUsagesCount -= 1;
    //        }
    //        // return item is consumable
    //        return true;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Item is not applicable to this unit");
    //        // this item could not be applied to that unit
    //        // item will return back automatically
    //        // return item is not consumable
    //        return false;
    //    }
    //}

    public int GetUnitEffectiveMaxHealth()
    {
        return UnitHealthMax + GetItemsHealthBonus();
    }

    public void ApplyDailyHealthRegen()
    {
        // verify if unit is not dead
        if (UnitStatus != UnitStatus.Dead)
        {
            // verify if health is not max already
            if (UnitHealthCurr != GetUnitEffectiveMaxHealth())
            {
                // apply daily health regen
                // Note: this should be done before taking control
                UnitHealthCurr += GetUnitEffectiveHealthRegenPerDay();
                // verify if health is not higher than max
                if (UnitHealthCurr > GetUnitEffectiveMaxHealth())
                {
                    // reset current health to max
                    UnitHealthCurr = GetUnitEffectiveMaxHealth();
                }
            }
        }
    }

    public void ExecutePreTurnActions()
    {
        // apply daily heal to all party members
        ApplyDailyHealthRegen();
        // .. decrement daily buffs
        // loop through all items and verify if it has expired
        //foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        //{
        //    inventoryItem.ExecutePreTurnActions();
        //}
    }

    #region Attributes accessors
    public UnitStatusConfig UnitStatusConfig
    {
        get
        {
            if (unitStatusConfigCache == null)
            {
                // get config from Configs Manager
                unitStatusConfigCache = Array.Find(ConfigManager.Instance.UnitStatusConfigs, e => e.unitStatus == UnitStatus);
            }
            return unitStatusConfigCache;
        }

        set
        {
            // normally used to reset status to null;
            unitStatusConfigCache = value;
        }
    }

    public string UnitName
    {
        get
        {
            // return partyUnitData.unitName;
            // search for display name in config
            return PartyUnitConfig.unitDisplayName;
        }
    }

    public UnitType UnitType
    {
        get
        {
            return partyUnitData.unitType;
        }

        set
        {
            partyUnitData.unitType = value;
        }
    }

    public bool IsLeader
    {
        get
        {
            return partyUnitData.isLeader;
        }

        set
        {
            partyUnitData.isLeader = value;
        }
    }

    public int UnitLeadership
    {
        get
        {
            //return partyUnitData.unitLeadership;
            // search for base leadership in config
            return PartyUnitConfig.unitLeadership;
        }

        //set
        //{
        //    partyUnitData.unitLeadership = value;
        //}
    }

    public string GivenName
    {
        get
        {
            return partyUnitData.givenName;
        }

        set
        {
            partyUnitData.givenName = value;
        }
    }

    public int UnitLevel
    {
        get
        {
            return partyUnitData.unitLevel;
        }

        set
        {
            partyUnitData.unitLevel = value;
        }
    }

    public int UnitExperience
    {
        get
        {
            return partyUnitData.unitExperience;
        }

        set
        {
            partyUnitData.unitExperience = value;
        }
    }

    public int UnitExperienceRequiredToReachNextLevel
    {
        get
        {
            //return partyUnitData.unitExperienceRequiredToReachNewLevel;
            // Experience required to reach next level depends on the current unit level
            // Call for a function which calculates it
            return PartyUnitConfig.GetUnitExperienceRequiredToReachNextLevel(UnitLevel);
        }

        //set
        //{
        //    partyUnitData.unitExperienceRequiredToReachNewLevel = value;
        //}
    }

    public int UnitExperienceReward
    {
        get
        {
            //return partyUnitData.unitExperienceReward;
            // Experience reward depends on the current unit level
            // Call for a function which calculates it
            return PartyUnitConfig.GetUnitExperienceReward(UnitLevel);
        }

        //set
        //{
        //    partyUnitData.unitExperienceReward = value;
        //}
    }

    //public int UnitExperienceRewardIncrementOnLevelUp
    //{
    //    get
    //    {
    //        return partyUnitData.unitExperienceRewardIncrementOnLevelUp;
    //    }

    //    set
    //    {
    //        partyUnitData.unitExperienceRewardIncrementOnLevelUp = value;
    //    }
    //}

    public void ResetCurrentHealth()
    {
        // we do it in a separate function to not trigger all logic when unit health is being set
        partyUnitData.unitHealthCurr = GetUnitEffectiveMaxHealth();
    }

    public int UnitHealthCurr
    {
        get
        {
            return partyUnitData.unitHealthCurr;
        }

        set
        {
            // save previous health value
            int previousHealth = partyUnitData.unitHealthCurr;
            // set current health
            partyUnitData.unitHealthCurr = value;
            // verify if current health is not higher than the maximum
            if (partyUnitData.unitHealthCurr > GetUnitEffectiveMaxHealth())
            {
                // reset it to max
                partyUnitData.unitHealthCurr = GetUnitEffectiveMaxHealth();
            }
            // verify if current health is lower then 0
            if (partyUnitData.unitHealthCurr < 0)
            {
                // reset it to 0
                partyUnitData.unitHealthCurr = 0;
            }
            // verify if unit is dead
            if (0 == partyUnitData.unitHealthCurr)
            {
                // set unit is dead attribute
                UnitStatus = UnitStatus.Dead;
            }
            // verify if unit has been resurected
            if (0 == previousHealth)
            {
                // remove unit is dead attribute
                UnitStatus = UnitStatus.Active;
            }
            // get the difference
            int difference = partyUnitData.unitHealthCurr - previousHealth;
            // rise an event
            // UnitEvents.unitHealthCurr.HasChanged.Raise(gameObject);
            UnitEvents.unitHealthCurr.HasChanged.Raise(gameObject, difference);
            //// trigger event
            //EventsAdmin.Instance.IHasChanged(this, new HealthCurrent());
        }
    }

    public int UnitHealthMax
    {
        get
        {
            //return partyUnitData.unitHealthMax;
            // Max Health depends on the current stats upgrades count
            // Call for a function which calculates it
            return PartyUnitConfig.GetUnitMaxHealth(StatsUpgradesCount);
        }

        //set
        //{
        //    partyUnitData.unitHealthMax = value;
        //}
    }

    public int UnitHealthMaxIncrementOnStatsUpgrade
    {
        get
        {
            //return partyUnitData.unitHealthMaxIncrementOnLevelUp;
            // Get data from config
            return PartyUnitConfig.unitHealthMaxIncrementOnStatsUpgrade;
        }

        //set
        //{
        //    partyUnitData.unitHealthMaxIncrementOnLevelUp = value;
        //}
    }

    public int UnitHealthRegenPercent
    {
        get
        {
            //return partyUnitData.unitHealthRegenPercent;
            // Get data from config
            return PartyUnitConfig.unitHealthRegenPercent;
        }

        //set
        //{
        //    partyUnitData.unitHealthRegenPercent = value;
        //}
    }

    public int UnitBaseDefense
    {
        get
        {
            //return partyUnitData.unitDefense;
            // Defense != Physical resistance
            // Defense is all around protection agains all sources
            // Get it from config
            return PartyUnitConfig.unitBaseDefense;
        }

        //set
        //{
        //    partyUnitData.unitDefense = value;
        //}
    }

    public Resistance[] UnitBaseResistances
    {
        get
        {
            // Get Resistances from config
            return PartyUnitConfig.unitResistances;
        }

        //set
        //{
        //    partyUnitData.unitResistances = value;
        //}
    }

    public UnitAbilityConfig UnitAbilityConfig
    {
        get
        {
            // verify if unit ability config is not defined yet
            if (unitAbilityConfigCache == null)
            {
                // set it from config based on ability defined in party unit config
                unitAbilityConfigCache = ConfigManager.Instance[UnitAbilityID];
            }
            // return ability config
            return unitAbilityConfigCache;
        }
    }

    public UnitAbilityID UnitAbilityID
    {
        get
        {
            // return partyUnitData.unitAbility;
            // get ability from party unit config
            return PartyUnitConfig.unitAbilityID;
        }

        //set
        //{
        //    partyUnitData.unitAbility = value;
        //}
    }

    //public int UnitAbilityBasePower
    //{
    //    get
    //    {
    //        // return partyUnitData.unitPower;
    //        // get ability power from unit stat modifier in ability config
    //        // todo: take into account all modifiers, not only the first one
    //        return UnitAbilityConfig.unitStatModifierConfig.modifierPower;
    //    }
    //}

    //public int UnitPowerIncrementOnStatsUpgrade
    //{
    //    get
    //    {
    //        // return partyUnitData.unitPowerIncrementOnLevelUp;
    //        // get unit power increment on stats upgrade from config
    //        // todo: take into account all modifiers, not only the first one
    //        //return UnitAbilityConfig.unitStatModifierConfig.powerIncrementOnStatsUpgrade;
    //        return UnitAbilityConfig.unitStatModifierConfig.powerIncrementOnStatsUpgrade;
    //    }

    //    //set
    //    //{
    //    //    partyUnitData.unitPowerIncrementOnLevelUp = value;
    //    //}
    //}

    //public UniquePowerModifierConfig PrimaryUniquePowerModifier
    //{
    //    get
    //    {
    //        // get UPM which has primary attribute set (should be only one)
    //        return UniquePowerModifierConfigs.Find(e => e.IsPrimary == true);
    //    }
    //}

    public int UnitAbilityEffectivePower
    {
        get
        {
            // return partyUnitData.unitPower;
            //// calculate ability power based on unit base power
            //// return UnitAbilityBasePower +
            //return UnitAbilityConfig.unitStatModifierConfig.modifierPower +
            //// add stats upgrade count multiplied by power increment on stats upgrade
            //UnitPowerIncrementOnStatsUpgrade* StatsUpgradesCount;
            //return UnitAbilityConfig.primaryUniquePowerModifierConfig.GetUpmEffectivePower(this);
            return UnitAbilityConfig.PrimaryUniquePowerModifierConfig.GetUpmEffectivePower(this);
        }

        //set
        //{
        //    partyUnitData.unitPower = value;
        //}
    }

    public PowerSource UnitAbilityPowerSource
    {
        get
        {
            // return partyUnitData.unitPowerSource;
            // Get it from unit ability config
            // todo: take into account all modifiers, not only the first one
            //return UnitAbilityConfig.unitStatModifierConfig.powerSource;
            //return UnitAbilityConfig.primaryUniquePowerModifierConfig.UpmSource;
            return UnitAbilityConfig.PrimaryUniquePowerModifierConfig.UpmSource;
        }

        //set
        //{
        //    partyUnitData.unitPowerSource = value;
        //}
    }

    public UnitAbilityRange UnitAbilityRange
    {
        get
        {
            // return partyUnitData.unitPowerDistance;
            // Get it from unit ability config
            return UnitAbilityConfig.unitAbilityRange;
        }

        //set
        //{
        //    partyUnitData.unitPowerDistance = value;
        //}
    }

    public ModifierScope UnitAbilityPowerScope
    {
        get
        {
            // return partyUnitData.unitPowerScope;
            // Get it from unit ability config
            //return UnitAbilityConfig.unitStatModifierConfig.modifierScope;
            //return UnitAbilityConfig.primaryUniquePowerModifierConfig.ModifierScope;
            return UnitAbilityConfig.PrimaryUniquePowerModifierConfig.ModifierScope;
        }

        //set
        //{
        //    partyUnitData.unitPowerScope = value;
        //}
    }

    public int UnitBaseInitiative
    {
        get
        {
            //return partyUnitData.unitInitiative;
            // Get it from config
            return PartyUnitConfig.unitInitiative;
        }

        //set
        //{
        //    partyUnitData.unitInitiative = value;
        //}
    }

    public string UnitRole
    {
        get
        {
            //return partyUnitData.unitRole;
            // Get it from config
            return PartyUnitConfig.unitRole;
        }

        //set
        //{
        //    partyUnitData.unitRole = value;
        //}
    }

    public string UnitBriefDescription
    {
        get
        {
            //return partyUnitData.unitBriefDescription;
            // Get it from config
            return PartyUnitConfig.unitBriefDescription;
        }

        //set
        //{
        //    partyUnitData.unitBriefDescription = value;
        //}
    }

    public string UnitFullDescription
    {
        get
        {
            // return partyUnitData.unitFullDescription;
            // Get it from config
            return PartyUnitConfig.unitFullDescription;
        }

        //set
        //{
        //    partyUnitData.unitFullDescription = value;
        //}
    }

    public int UnitCost
    {
        get
        {
            //return partyUnitData.unitCost;
            // Get it from config
            return PartyUnitConfig.unitCost;
        }

        //set
        //{
        //    partyUnitData.unitCost = value;
        //}
    }

    public UnitSize UnitSize
    {
        get
        {
            //return partyUnitData.unitSize;
            // Get it from config
            return PartyUnitConfig.unitSize;
        }

        //set
        //{
        //    partyUnitData.unitSize = value;
        //}
    }

    public bool IsInterpartyMovable
    {
        get
        {
            //return partyUnitData.isInterpartyMovable;
            // Get it from config
            return PartyUnitConfig.isInterpartyMovable;
        }

        //set
        //{
        //    partyUnitData.isInterpartyMovable = value;
        //}
    }

    public bool IsDismissable
    {
        get
        {
            //return partyUnitData.isDismissable;
            // Get it from config
            return PartyUnitConfig.isDismissable;
        }

        //set
        //{
        //    partyUnitData.isDismissable = value;
        //}
    }

    UnitEventsConfig unitEvents;
    public UnitEventsConfig UnitEvents
    {
        get
        {
            if (unitEvents == null)
            {
                // get events from config
                unitEvents = ConfigManager.Instance.UnitEventsConfig;
            }
            return unitEvents;
        }
    }

    public void RemoveAppliedUnitUniquePowerModifiers()
    {
        // Loop through all UPMs on this party unit in backwards order (so we can remove items in a loop)
        for (int i = AppliedUniquePowerModifiersData.Count - 1; i >= 0; i--)
        {
            // trigger UPM removed event
            //UnitEvents.uniquePowerModifierHasBeenRemovedEvent.Raise(UniquePowerModifiersData[i]);
            AppliedUniquePowerModifiersData[i].GetUniquePowerModifierConfig().UniquePowerModifier.Events.DataHasBeenRemovedEvent.Raise(AppliedUniquePowerModifiersData[i]);
            // remove it from the list
            AppliedUniquePowerModifiersData.RemoveAt(i);
        }
    }

    public UnitStatus UnitStatus
    {
        get
        {
            return partyUnitData.unitStatus;
        }

        set
        {
            partyUnitData.unitStatus = value;
            // Reset previous unit status config
            UnitStatusConfig = null;
            // verify if new status is Dead or Escaped
            if (UnitStatus == UnitStatus.Dead || UnitStatus == UnitStatus.Escaped)
            {
                // remove all applied unit's buffs and debuffs
                RemoveAppliedUnitUniquePowerModifiers();
            }
            // set party unit which is rising event
            // UnitEvents.unitStatus.HasChanged.partyUnit = this;
            // rise an event
            UnitEvents.unitStatus.HasChanged.Raise(gameObject);
            // EventsManager.TriggerEvent(UnitEvents.unitStatus.HasChanged, gameObject);
        }
    }

    //public UnitDebuff[] UnitDebuffs
    //{
    //    get
    //    {
    //        return partyUnitData.unitDebuffs;
    //    }

    //    set
    //    {
    //        partyUnitData.unitDebuffs = value;
    //    }
    //}

    //public UnitBuff[] UnitBuffs
    //{
    //    get
    //    {
    //        return partyUnitData.unitBuffs;
    //    }

    //    set
    //    {
    //        partyUnitData.unitBuffs = value;
    //    }
    //}

    public int UnitUpgradePoints
    {
        get
        {
            return partyUnitData.unitUpgradePoints;
        }

        set
        {
            partyUnitData.unitUpgradePoints = value;
        }
    }

    public int UnitStatPoints
    {
        get
        {
            return partyUnitData.unitStatPoints;
        }

        set
        {
            partyUnitData.unitStatPoints = value;
        }
    }

    public bool ClassIsUpgradable
    {
        get
        {
            //return partyUnitData.classIsUpgradable;
            // Get it from config
            return PartyUnitConfig.classIsUpgradable;
        }

        //set
        //{
        //    partyUnitData.classIsUpgradable = value;
        //}
    }

    public int UnitClassPoints
    {
        get
        {
            return partyUnitData.unitClassPoints;
        }

        set
        {
            partyUnitData.unitClassPoints = value;
        }
    }

    public bool CanLearnSkills
    {
        get
        {
            // return partyUnitData.canLearnSkills;
            // Get it from config
            return PartyUnitConfig.canLearnSkills;
        }

        //set
        //{
        //    partyUnitData.canLearnSkills = value;
        //}
    }

    public int UnitSkillPoints
    {
        get
        {
            return partyUnitData.unitSkillPoints;
        }

        set
        {
            partyUnitData.unitSkillPoints = value;
        }
    }

    public UnitType RequiresUnitType
    {
        get
        {
            //return partyUnitData.requiresUnitType;
            // Get it from config
            return PartyUnitConfig.requiresUnitType;
        }

        //set
        //{
        //    partyUnitData.requiresUnitType = value;
        //}
    }

    public UnitType[] UnlocksUnitTypes
    {
        get
        {
            //return partyUnitData.unlocksUnitTypes;
            // Get it from config
            return PartyUnitConfig.unlocksUnitTypes;
        }

        //set
        //{
        //    partyUnitData.unlocksUnitTypes = value;
        //}
    }

    public int UpgradeCost
    {
        get
        {
            //return partyUnitData.upgradeCost;
            // Get it from config
            return PartyUnitConfig.upgradeCost;
        }

        //set
        //{
        //    partyUnitData.upgradeCost = value;
        //}
    }

    public int StatsUpgradesCount
    {
        get
        {
            return partyUnitData.statsUpgradesCount;
        }

        set
        {
            partyUnitData.statsUpgradesCount = value;
            // trigger events for all stats-dependant attributes
            // EventsAdmin.Instance.IHasChanged(this, new HealthMax());
            UnitEvents.unitHealthMax.HasChanged.Raise(gameObject);
        }
    }

    public void ResetCurrentMovePointsNumber()
    {
        partyUnitData.movePointsCurrent = MovePointsMax;
    }

    public int MovePointsCurrent
    {
        get
        {
            return partyUnitData.movePointsCurrent;
        }

        set
        {
            partyUnitData.movePointsCurrent = value;
        }
    }

    public int MovePointsMax
    {
        get
        {
            //return partyUnitData.movePointsMax;
            // Get it from config
            return PartyUnitConfig.movePointsMax;
        }

        //set
        //{
        //    partyUnitData.movePointsMax = value;
        //}
    }

    public int ScoutingRange
    {
        get
        {
            //return partyUnitData.scoutingRange;
            // Get it from config
            return PartyUnitConfig.scoutingRange;
        }

        //set
        //{
        //    partyUnitData.scoutingRange = value;
        //}
    }

    public UnitSkillData[] UnitSkillsData
    {
        get
        {
            return partyUnitData.unitSkillsData;
        }

        set
        {
            partyUnitData.unitSkillsData = value;
        }
    }

    public List<UniquePowerModifierConfig> UniquePowerModifierConfigs
    {
        get
        {
            //return partyUnitData.uniquePowerModifiers;
            // return UnitAbilityConfig.postActionUniquePowerModifierConfigs;
            return UnitAbilityConfig.uniquePowerModifierConfigs;
        }

        //set
        //{
        //    partyUnitData.uniquePowerModifiers = value;
        //}
    }

    public PartyPanel.Row UnitPPRow
    {
        get
        {
            return partyUnitData.unitPPRow;
        }

        set
        {
            partyUnitData.unitPPRow = value;
        }
    }

    public PartyPanel.Cell UnitPPCell
    {
        get
        {
            return partyUnitData.unitPPCell;
        }

        set
        {
            partyUnitData.unitPPCell = value;
        }
    }

    #endregion Attributes accessors

    #region Battle attributes
    public bool HasMoved
    {
        get
        {
            return hasMoved;
        }

        set
        {
            hasMoved = value;
        }
    }
    #endregion Battle attributes

    public PartyUnitData PartyUnitData
    {
        get
        {
            return partyUnitData;
        }

        set
        {
            partyUnitData = value;
        }
    }

    public PartyUnitConfig PartyUnitConfig
    {
        get
        {
            if (partyUnitConfigCache == null)
            {
                // link config
                partyUnitConfigCache = Array.Find(ConfigManager.Instance.PartyUnitConfigs, e => e.unitType == UnitType);
            }
            return partyUnitConfigCache;
        }
    }

    //public void AddUPMData(UniquePowerModifierData uniquePowerModifierData)
    //{
    //    partyUnitData.uniquePowerModifiersData.Add(uniquePowerModifierData);
    //    uniquePowerModifierDataAdd.Raise(this, uniquePowerModifierData);
    //}

    //public void RemoveUPMData(UniquePowerModifierData uniquePowerModifierData)
    //{
    //    partyUnitData.uniquePowerModifiersData.Remove(uniquePowerModifierData);
    //    uniquePowerModifierDataRemove.Raise(this, uniquePowerModifierData);
    //}

    //// (c) https://stackoverflow.com/questions/3052991/forbid-public-add-and-delete-for-a-listt
    //public IEnumerable<UniquePowerModifierData> UniquePowerModifiersData
    //{
    //    get
    //    {
    //        // dont' return reference to a full list to prevent uncontrolled additions and removals
    //        // because we want to rise an event on this for UI
    //        // return partyUnitData.uniquePowerModifiersData;
    //        foreach (var uniquePowerModifierData in partyUnitData.uniquePowerModifiersData) yield return uniquePowerModifierData;
    //    }
    //}

    public List<UniquePowerModifierData> AppliedUniquePowerModifiersData
    {
        get
        {
            return partyUnitData.appliedUniquePowerModifiersData;
        }
    }
}
