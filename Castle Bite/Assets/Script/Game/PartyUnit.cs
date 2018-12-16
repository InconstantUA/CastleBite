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
    Unknown
};

[Serializable]
public enum UnitSize
{
    Single,
    Double
};

[Serializable]
public enum UnitAbility
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
    HealingSong,        // Dominion ?
    BlowWithMaul,       // Dominion Colossus
    CastChainLightning, // Dominion Mage
    EarthShatteringLeap,// Greenskin Bombul captial guard
    Malediction,        // Greenskin Orc shaman
    LastCall,           // Undead Hades capital guard
    None
};

[Serializable]
public enum UnitPowerSource : int
{
    Physical,   // Attack with metal weapons
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
public struct Resistance
{
    public UnitPowerSource source;
    public int percent;
}

[Serializable]
public enum UnitPowerDistance
{
    Mele,
    Ranged
}

[Serializable]
public enum UnitPowerScope
{
    OneUnit,
    EntireParty
}

[Serializable]
public enum UnitStatus
{
    Active, // not Dead and not Escaped = can fight
    Waiting,
    Escaping,
    Escaped,
    Dead
}

[Serializable]
public enum UnitDebuff : int
{
    None,
    Poisoned,
    Burned,
    Chilled,
    Paralyzed,
    ArrSize
}

[Serializable]
public enum UnitBuff : int
{
    None,
    DefenseStance,
    ArrSize // for dynamic resizing of UnitBuffs array
}


[Serializable]
public class UnitSkill
{
    [Serializable]
    public enum SkillName {
        Leadership,
        Offence,
        Defense,
        Pathfinding,
        Scouting,
        Healing,
        DeathResistance,
        FireResistance,
        WaterResistance,
        MindResistance,
        ShardAura,
        LifelessContinuation
    }
    public SkillName mName;
    [NonSerialized]
    public string mDisplayName;
    [Serializable]
    public class SkillLevel
    {
        public int mCurrent;
        [NonSerialized]
        public int mMax;
        public SkillLevel(int current, int max)
        {
            mCurrent = current;
            mMax = max;
        }
    }
    public SkillLevel mLevel;
    [NonSerialized]
    public int mRequiredHeroLevel;
    [NonSerialized]
    public int mLevelUpIncrementStep; // skill can be learned only after this number of levels has passed
    [NonSerialized]
    public string mDescription;
    public UnitSkill(SkillName name, string displayName, int currentLevel, int maxLevel, int requiredHeroLevel, int levelUpIncrementStep, string description)
    {
        mName = name;
        mDisplayName = displayName;
        mLevel = new SkillLevel(currentLevel, maxLevel);
        mRequiredHeroLevel = requiredHeroLevel;
        mLevelUpIncrementStep = levelUpIncrementStep;
        mDescription = description;
    }
    public bool EqualTo(UnitSkill unitSkill)
    {

        if (
            // verify skill name
            (unitSkill.mName == mName) &&
            // verify skill current level
            (unitSkill.mLevel.mCurrent == mLevel.mCurrent) &&
            // verify skill max level
            (unitSkill.mLevel.mMax == mLevel.mMax) &&
            // verify skill required hero level level
            (unitSkill.mRequiredHeroLevel == mRequiredHeroLevel) &&
            // verify skill level up increment step level
            (unitSkill.mLevelUpIncrementStep == mLevelUpIncrementStep)
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
    Talent,
    Artifact
}

[Serializable]
public enum ModifierScope
{
    Self,
    FriendlyUnit,
    FriendlyParty,
    EnemyUnit,
    EnemyParty
}

[Serializable]
public class UniquePowerModifier
{
    // define possible origins (who is the source of unique power modifier)
    public ModifierScope modifierScope;
    public UnitBuff upmAppliedBuff;
    public UnitDebuff upmAppliedDebuff;
    public int upmPower;
    public int upmPowerIncrementOnLevelUp;
    public int upmDuration;
    public int upmChance;
    public int upmChanceIncrementOnLevelUp;
    public UnitPowerSource upmSource;
    public ModifierOrigin upmOrigin;
    public ModifierAppliedHow modifierApplied;
    public int upmDurationLeft;
    public int skillPowerMultiplier = 1;
    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;

    public string GetDisplayName()
    {
        switch (upmAppliedDebuff)
        {
            case UnitDebuff.Burned:
                return "Burn";
            case UnitDebuff.Chilled:
                return "Chill";
            case UnitDebuff.Paralyzed:
                return "Paralyze";
            case UnitDebuff.Poisoned:
                return "Poison";
            default:
                Debug.LogError("Unknown debuf");
                return "Error";
        }
    }
}

[Serializable]
public enum UnitStat
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
    Multiplicatively,
    Percent
}

[Serializable]
public enum ModifierAppliedHow
{
    Active,
    Passive
}

[Serializable]
public enum ModifierAppliedTo
{
    CurrentStat,
    MaxStat
}

[Serializable]
public class UnitStatModifier : System.Object
{
    public UnitStat unitStat;
    public ModifierAppliedTo modifierAppliedTo;
    public ModifierScope modifierScope;
    public int modifierPower;
    public int skillPowerMultiplier = 1;
    public ModifierCalculatedHow modifierCalculatedHow;
    public ModifierAppliedHow modifierAppliedHow;
    public int duration;
    public int durationLeft;
    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
}

[Serializable]
public class UnitStatusModifier : System.Object
{
    public ModifierScope modifierScope;
    public UnitStatus modifierSetStatus;
    public UnitStatus[] canBeAppliedToTheUnitsWithStatuses;
}

[Serializable]
public class PartyUnitData : System.Object
{
    // Misc attributes
    public string unitName;
    public UnitType unitType;
    // Leader attributes
    public bool isLeader;
    public int unitLeadership;
    public string givenName;
    // Level and experience
    public int unitLevel;
    public int unitExperience;
    public int unitExperienceRequiredToReachNewLevel;
    public int unitExperienceReward;
    public int unitExperienceRewardIncrementOnLevelUp;
    // Defensive attributes
    public int unitHealthCurr;
    public int unitHealthMax;
    public int unitHealthMaxIncrementOnLevelUp;
    public int unitHealthRegenPercent;
    public int unitDefense;
    public Resistance[] unitResistances;
    // Offensive attributes
    public UnitAbility unitAbility;
    public int unitPower;
    public int unitPowerIncrementOnLevelUp;
    public UnitPowerSource unitPowerSource;
    public UnitPowerDistance unitPowerDistance;
    public UnitPowerScope unitPowerScope;
    public int unitInitiative = 10;
    // Unique power modifiers
    public List<UniquePowerModifier> uniquePowerModifiers;
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
    public UnitStatus unitStatus;
    public UnitDebuff[] unitDebuffs;
    public UnitBuff[] unitBuffs;
    // For Upgrade
    public int unitUpgradePoints;
    public int unitStatPoints;
    public bool classIsUpgradable;
    public int unitClassPoints;
    public bool canLearnSkills;
    public int unitSkillPoints;
    // For class upgrade
    public UnitType requiresUnitType;
    public UnitType[] unlocksUnitTypes;
    public int upgradeCost;
    public int statsUpgradesCount;
    // Misc attributes for map
    public int movePointsCurrent;
    public int movePointsMax;
    public int scoutingRange;
    // Skills
    public UnitSkill[] unitSkills = new UnitSkill[]
    {
        new UnitSkill(
            UnitSkill.SkillName.Leadership,
            "Leadership",
            0, 3,
            3, 2,
            "Allows hero to have 1 multiplied by skill level additional unit(s) in his party."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.Offence,
            "Offence",
            0, 3,
            2, 2,
            "Increase hero attack power by 15% multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.Defense,
            "Defense",
            0, 3,
            2, 2,
            "Increase hero defense from all sources by adding 10 defense points multiplied by skill level to current hero defense value."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.Pathfinding,
            "Pathfinding",
            0, 3,
            2, 1,
            "Increase hero move points by 50% multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
        ),
        new UnitSkill(
            UnitSkill.SkillName.Scouting,
            "Scouting",
            0, 3,
            2, 1,
            "Increase hero scouting range in the fog of war by 1 tile multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
        ),
        new UnitSkill(
            UnitSkill.SkillName.Healing,
            "Healing",
            0, 2,
            2, 3,
            "Increase hero and its party members daily healing rate by 15% from total health multiplied by skill level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.WaterResistance,
            "Water Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Water-based attacks, for example Chill."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.FireResistance,
            "Fire Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Fire-based attacks, for example Burning."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.DeathResistance,
            "Death Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Death-based attacks, for example Poison."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.MindResistance,
            "Mind Resistance",
            0, 2,
            7, 7,
            "Increase hero change to resist Mind-based attacks, for example paralyze."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 7 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.ShardAura,
            "Shard Aura",
            0, 3,
            8, 5,
            "Allow hero to use shards which emit different aura based on the shard type."
            + "\r\n" + "Earth shard - defence, Sun shard - offence, Lighting shard - initiative."
            + "\r\n" + "Each level increases shard's base aura bonus by 100%."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
        ),
        new UnitSkill(
            UnitSkill.SkillName.LifelessContinuation,
            "Lifeless Continuation",
            0, 1,
            9, 0,
            "Allow hero to continue battle after death."
            + "\r\n" + "After death all debuffs are removed and hero enters spirit form with half of the normal health and physical damage immunity. "
            + "Hero can be resurected by friendly party unit or by using blood stone during the battle. "
            + "If not resurected before the end of the battle hero dies without gainig experience."
            + "\r\n" + "Maximum level: 1."
            //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
        )
    };
    // UI attributes
    public string unitCellAddress;  // used during game save and load and when party UI is displayed
    // Unit Equipment
    public List<InventoryItemData> unitIventory; // information saved and loaded during game save and load, during game running phase all data can be retrieved from the child items of the party leader unit
}

public class PartyUnit : MonoBehaviour {
    // Data which will be saved later
    [SerializeField]
    PartyUnitData partyUnitData;

    // Misc Battle attributes
    private bool hasMoved = false;

    //void Reset()
    //{
    //    Debug.Log("Reset to default values");
    //    partyUnitData.unitSkills = new UnitSkill[]
    //    {
    //    new UnitSkill(
    //        UnitSkill.SkillName.Leadership,
    //        "Leadership",
    //        0, 3,
    //        3, 2,
    //        "Allows hero to have 1 multiplied by skill level additional unit(s) in his party."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.Offence,
    //        "Offence",
    //        0, 3,
    //        2, 2,
    //        "Increase hero attack power by 15% multiplied by skill level."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.Defense,
    //        "Defense",
    //        0, 3,
    //        2, 2,
    //        "Increase hero defense from all sources by adding 10 defense points multiplied by skill level to current hero defense value."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.Pathfinding,
    //        "Pathfinding",
    //        0, 3,
    //        2, 1,
    //        "Increase hero move points by 50% multiplied by skill level."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.Scouting,
    //        "Scouting",
    //        0, 3,
    //        2, 1,
    //        "Increase hero scouting range in the fog of war by 1 tile multiplied by skill level."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.Healing,
    //        "Healing",
    //        0, 2,
    //        2, 3,
    //        "Increase hero and its party members daily healing rate by 15% from total health multiplied by skill level."
    //        + "\r\n" + "Maximum level: 2."
    //        //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.WaterResistance,
    //        "Water Resistance",
    //        0, 2,
    //        5, 5,
    //        "Increase hero change to resist Water-based attacks, for example Chill."
    //        + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
    //        + "\r\n" + "Maximum level: 2."
    //        //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.FireResistance,
    //        "Fire Resistance",
    //        0, 2,
    //        5, 5,
    //        "Increase hero change to resist Fire-based attacks, for example Burning."
    //        + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
    //        + "\r\n" + "Maximum level: 2."
    //        //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.DeathResistance,
    //        "Death Resistance",
    //        0, 2,
    //        5, 5,
    //        "Increase hero change to resist Death-based attacks, for example Poison."
    //        + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
    //        + "\r\n" + "Maximum level: 2."
    //        //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.MindResistance,
    //        "Mind Resistance",
    //        0, 2,
    //        7, 7,
    //        "Increase hero change to resist Mind-based attacks, for example paralyze."
    //        + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
    //        + "\r\n" + "Maximum level: 2."
    //        //+ "\r\n" + "2nd skill level can be learned after 7 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.ShardAura,
    //        "Shard Aura",
    //        0, 3,
    //        8, 5,
    //        "Allow hero to use shards which emit different aura based on the shard type."
    //        + "\r\n" + "Earth shard - defence, Sun shard - offence, Lighting shard - initiative."
    //        + "\r\n" + "Each level increases shards aura bonus by 5."
    //        + "\r\n" + "Maximum level: 3."
    //        //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
    //    ),
    //    new UnitSkill(
    //        UnitSkill.SkillName.LifelessContinuation,
    //        "Lifeless Continuation",
    //        0, 1,
    //        9, 0,
    //        "Allow hero to continue battle after death."
    //        + "\r\n" + "After death all debuffs are removed and hero enters spirit form with half of the normal health and physical damage immunity. "
    //        + "Hero can be resurected by friendly party unit or by using blood stone during the battle. "
    //        + "If not resurected before the end of the battle hero dies without gainig experience."
    //        + "\r\n" + "Maximum level: 1."
    //        //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
    //    )
    //    };
    //}

    void InitUnitBuffs()
    {
        UnitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
        for (int i = 0; i < (int)UnitBuff.ArrSize; i++)
        {
            UnitBuffs[i] = UnitBuff.None;
        }
    }

    void InitUnitDebuffs()
    {
        UnitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
        for (int i = 0; i < (int)UnitDebuff.ArrSize; i++)
        {
            UnitDebuffs[i] = UnitDebuff.None;
        }
    }

    private void Awake()
    {
        InitUnitBuffs();
        InitUnitDebuffs();
        //Debug.Log("Awake " + UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
    }

    private void Start()
    {
        //unitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
        //UnitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
        //Debug.Log("Start " + UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
    }

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
            if (city.CityFaction == party.Faction)
            {
                return city.GetCityDefense();
            }
        }
        return 0;
    }

    public int GetSkillDefenseBonus()
    {
        // get skill from partyUnit
        UnitSkill skill = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Defense);
        // get bonus based on fact that 1 skill level = 10 defense
        return skill.mLevel.mCurrent * 10;
    }

    public float GetStatusDefenseBonus()
    {
        // Applied Multiplicatively
        // verify if buffs array is not null
        if (UnitBuffs != null)
        {
            //Debug.Log(UnitName + " " + GivenName + " " + UnitBuffs.Length.ToString());
            // verify if buffs array was initialized properly
            if (UnitBuffs.Length != (int)UnitBuff.ArrSize)
            {
                // initialize buffs array
                InitUnitBuffs();
            }
        }
        else
        {
            Debug.LogError("Unit buffs array is null");
        }
        // verify if unit has defense stance buff
        if (UnitBuff.DefenseStance == UnitBuffs[(int)UnitBuff.DefenseStance])
        {
            // reduce damage by half
            return 0.5f;
        }
        return 0f; // no impact on defense
    }

    public int GetTotalAdditiveDefense()
    {
        // init with 0
        int totalDefense = 0;
        // Get base defense
        int baseDefense = UnitDefense;
        // apply base defense
        totalDefense += baseDefense;
        // Verify if unit is in a friendly city and get city defense modifier
        int cityDefenseModifier = GetCityDefenseBonus();
        // apply city defense modifier
        totalDefense += cityDefenseModifier;
        // Get items modifier
        // no items yet, skip
        // ..
        int itemsDefenseModifier = 0;
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
        return GetGenericStatItemBonus(UnitStat.Defense, UnitDefense);
    }

    public int GetEffectiveDefense()
    {
        // ADDITIVE
        int totalDefense = GetTotalAdditiveDefense();
        // MULTIPLICATIVE
        // Get additional defense mondifiers:
        // Get status modifiers, example: defense stance buff
        float statusModifier = GetStatusDefenseBonus();
        // Apply status modifier;
        int restDamagePercent = 100 - totalDefense;
        int totalDefenseWithModifiers = totalDefense + (int)Math.Round(restDamagePercent * statusModifier);
        // apply items bonus
        totalDefenseWithModifiers += GetItemsDefenseBonus();
        // Return result
        return totalDefenseWithModifiers;
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
        int srcUnitDamage = activeBattleUnit.UnitPower;
        int dstUnitDefense = GetEffectiveDefense();
        // calculate damage dealt
        damageDealt = (int)Math.Round((((float)srcUnitDamage * (100f - (float)dstUnitDefense)) / 100f));
        return damageDealt;
    }

    public int GetDebuffDamageDealt(UniquePowerModifier appliedUniquePowerModifier)
    {
        return appliedUniquePowerModifier.upmPower;
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

    public int GetOffenceSkillPowerBonus(UnitSkill skill = null)
    {
        // verify if custom skill parameter was passed
        if (skill == null)
        {
            // get local party unit skill
            skill = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Offence);
        }
        return (int)Math.Round(UnitPower * skill.mLevel.mCurrent * 0.15f);
    }

    public int GetItemsPowerBonus()
    {
        return GetGenericStatItemBonus(UnitStat.Power, UnitPower);
    }

    public int GetUnitEffectivePower()
    {
        // get unit power plus skill bonus
        return UnitPower + GetOffenceSkillPowerBonus() + GetItemsPowerBonus();
    }

    List<UnitStatModifier> GetItemUnitStatModifierByStat(UnitStat unitStat)
    {
        // init list of UnitStatModifier from items, there can be more than one
        List<UnitStatModifier> usmsList = new List<UnitStatModifier>();
        // loop through all items for this unit
        foreach (InventoryItem inventoryItem in GetComponentsInChildren<InventoryItem>())
        {
            // verify if item is not in belt
            if (   (inventoryItem.HeroEquipmentSlot != HeroEquipmentSlot.BeltSlot1)
                && (inventoryItem.HeroEquipmentSlot != HeroEquipmentSlot.BeltSlot2))
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
                        if (inventoryItem.HeroEquipmentSlot == HeroEquipmentSlot.Shard)
                        {
                            // apply skill modifier to USMs power
                            usm.skillPowerMultiplier = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.ShardAura).mLevel.mCurrent;
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
                        if ((inventoryItem.HeroEquipmentSlot != HeroEquipmentSlot.BeltSlot1)
                            && (inventoryItem.HeroEquipmentSlot != HeroEquipmentSlot.BeltSlot2))
                        {
                            // verify if item gives stat bonus
                            // loop through item stat modifiers
                            foreach (UnitStatModifier usm in inventoryItem.UnitStatModifiers)
                            {
                                // verify if usm applies to the stat and that it has global scope
                                if ((usm.unitStat == unitStat) && (usm.modifierScope == ModifierScope.FriendlyParty))
                                {
                                    //// .. verify if there is no already same item and that its unit stat modifier is not stackable, maybe this check is not needed here, because it is done before item is consumed
                                    //Debug.Log("Verify non-stackable USMs from the same item with entire party scope");
                                    // verify if this is shard slot item
                                    if (inventoryItem.HeroEquipmentSlot == HeroEquipmentSlot.Shard)
                                    {
                                        // apply skill modifier to USMs power
                                        usm.skillPowerMultiplier = Array.Find(partyLeader.UnitSkills, element => element.mName == UnitSkill.SkillName.ShardAura).mLevel.mCurrent;
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

    int GetGenericStatItemBonus(UnitStat unitStat, int statBaseValue)
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
        return GetGenericStatItemBonus(UnitStat.Leadership, UnitLeadership);
    }

    public int GetEffectiveLeadership()
    {
        // get current skill leadership bonus = skill level
        UnitSkill skill = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Leadership);
        int skillBonus = skill.mLevel.mCurrent;
        // get items bonus
        int itemsBonus = GetLeadershipItemsBonus();
        // return result
        return UnitLeadership + skillBonus + itemsBonus;
    }

    public float GetPathfindingSkillMultiplier()
    {
        // get current skill level
        int currentSkillLevel = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Pathfinding).mLevel.mCurrent;
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
        return GetGenericStatItemBonus(UnitStat.MovePoints, MovePointsMax);
    }

    public int GetScoutingSkillBonus()
    {
        // get and return current skill level (= scouting bonus)
        return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Scouting).mLevel.mCurrent;
    }

    public int GetLeadershipSkillBonus()
    {
        // get and return current skill level (= scouting bonus)
        return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Leadership).mLevel.mCurrent;
    }

    public int GetEffectiveMaxMovePoints()
    {
        // return result
        return GetAdditiveMovePoints() + GetPathfindingSkillBonus() + GetMovePointsItemsBonus();
    }

    public int GetScoutingPointsItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStat.ScoutingRange, ScoutingRange);
    }

    public int GetItemsHealthBonus()
    {
        return GetGenericStatItemBonus(UnitStat.Health, UnitHealthMax);
    }

    public int GetEffectiveScoutingRange()
    {
        // get and return sum of default scouting range plus skill bonus
        return ScoutingRange + GetScoutingSkillBonus() + GetScoutingPointsItemsBonus();
    }

    public int GetInitiativeSkillBonus()
    {
        Debug.Log("Not implemented: GetInitiativeSkillBonus");
        return 0;
    }

    public int GetInitiativeItemsBonus()
    {
        return GetGenericStatItemBonus(UnitStat.Initiative, UnitInitiative);
    }

    public int GetEffectiveInitiative()
    {
        // get and return sum of default scouting range plus skill bonus
        return UnitInitiative + GetInitiativeSkillBonus() + GetInitiativeItemsBonus();
    }

    public int GetUnitHealthRegenPerDay()
    {
        return (int)Math.Floor(((float)UnitHealthMax * (float)UnitHealthRegenPercent) / 100f);
    }

    public float GetUnitHealSkillHealthRegenModifier()
    {
        // get and return skill current level multiplied by 0.15
        return (float)Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Healing).mLevel.mCurrent * 0.15f;
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

    public int GetUnitResistanceSkillBonus(UnitPowerSource source)
    {
        switch (source)
        {
            case UnitPowerSource.Death:
                return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.DeathResistance).mLevel.mCurrent * 50;
            case UnitPowerSource.Fire:
                return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.FireResistance).mLevel.mCurrent * 50;
            case UnitPowerSource.Mind:
                return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.MindResistance).mLevel.mCurrent * 50;
            case UnitPowerSource.Water:
                return Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.WaterResistance).mLevel.mCurrent * 50;
            case UnitPowerSource.Wind:
            case UnitPowerSource.Pure:
            case UnitPowerSource.Earth:
            case UnitPowerSource.Life:
            case UnitPowerSource.Physical:
                return 0;
            default:
                Debug.LogError("Unknown source " + source.ToString());
                return 0;
        }
    }

    public int GetUnitResistanceItemsBonus(UnitPowerSource source)
    {
        switch (source)
        {
            case UnitPowerSource.Death:
                return GetGenericStatItemBonus(UnitStat.DeathResistance, GetUnitBaseResistance(source));
            case UnitPowerSource.Fire:
                return GetGenericStatItemBonus(UnitStat.FireResistance, GetUnitBaseResistance(source));
            case UnitPowerSource.Mind:
                return GetGenericStatItemBonus(UnitStat.MindResistance, GetUnitBaseResistance(source));
            case UnitPowerSource.Water:
                return GetGenericStatItemBonus(UnitStat.WaterResistance, GetUnitBaseResistance(source));
            case UnitPowerSource.Wind:
            case UnitPowerSource.Pure:
            case UnitPowerSource.Earth:
            case UnitPowerSource.Life:
            case UnitPowerSource.Physical:
                return 0;
            default:
                Debug.LogError("Unknown source " + source.ToString());
                return 0;
        }
    }

    public int GetUnitBaseResistance(UnitPowerSource source)
    {
        // verify if unit has this base resistance
        foreach (Resistance resistance in UnitResistances)
        {
            // verify if there is resistance with the same name
            if (resistance.source == source)
            {
                // return resistance percent
                return resistance.percent;
            }
        }
        return 0;
    }

    public int GetUnitEffectiveResistance(UnitPowerSource source)
    {
        // get and return effective resistance
        return GetUnitBaseResistance(source) + GetUnitResistanceSkillBonus(source) + GetUnitResistanceItemsBonus(source);
    }

    bool ApplyActiveUPM(UniquePowerModifier uniquePowerModifier, bool doPreview = false)
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
            case ModifierCalculatedHow.Multiplicatively:
                // increase current value by amout of base value multiplied by power
                return unitStatModifier.skillPowerMultiplier * baseStatMaxValue * unitStatModifier.modifierPower;
            case ModifierCalculatedHow.Percent:
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

    bool ApplyUSMToCurrentStatValue(UnitStatModifier unitStatModifier, bool doPreview = false)
    {
        Debug.LogWarning("Apply unit stat modifier");
        switch (unitStatModifier.unitStat)
        {
            case UnitStat.Leadership:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.Health:
                // verify if unit health is not max already and if unit is not dead
                if (UnitHealthCurr != GetUnitEffectiveMaxHealth())
                {
                    // verify if it is not preview
                    if (!doPreview)
                    {
                        // apply usm power to this unit
                        UnitHealthCurr = ApplyInstantUSMPower(unitStatModifier, UnitHealthCurr, GetUnitEffectiveMaxHealth(), false);
                    }
                    // item is applicable to this unit
                    return true;
                }
                // by default item is not applicable
                return false;
            case UnitStat.Defense:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.Power:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.Initiative:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.MovePoints:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    // verify if current move points are not max already
                    if (MovePointsCurrent != GetEffectiveMaxMovePoints())
                    {
                        // verify if it is not preview
                        if (!doPreview)
                        {
                            // apply usm power to this unit
                            MovePointsCurrent = ApplyInstantUSMPower(unitStatModifier, MovePointsCurrent, GetEffectiveMaxMovePoints(), false);
                        }
                        // item is applicable to this unit
                        return true;
                    }
                }
                // by default item is not applicable
                return false;
            case UnitStat.ScoutingRange:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.DeathResistance:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.FireResistance:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.WaterResistance:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            case UnitStat.MindResistance:
                // not applicable for instant unit stat modifier, because it is not consumable unit stat
                // by default item is not applicable
                return false;
            default:
                Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
                // by default item is not applicable
                return false;
        }
    }

    bool ApplyUSMToMaxStatValue(UnitStatModifier unitStatModifier, bool doPreview = false)
    {
        Debug.LogWarning("Apply unit stat modifier");
        // init resistance var
        Resistance resistance;
        // 
        switch (unitStatModifier.unitStat)
        {
            case UnitStat.Leadership:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    if (!doPreview)
                        UnitLeadership += GetUSMStatBonus(unitStatModifier, UnitLeadership);
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStat.Health:
                if (!doPreview)
                    UnitHealthMax += GetUSMStatBonus(unitStatModifier, UnitHealthMax);
                // return usm can be applied as result;
                return true;
            case UnitStat.Defense:
                if (!doPreview)
                    UnitDefense += GetUSMStatBonus(unitStatModifier, UnitDefense);
                // return usm can be applied as result;
                return true;
            case UnitStat.Power:
                if (!doPreview)
                    UnitPower += GetUSMStatBonus(unitStatModifier, UnitPower);
                // return usm can be applied as result;
                return true;
            case UnitStat.Initiative:
                if (!doPreview)
                    UnitInitiative += GetUSMStatBonus(unitStatModifier, UnitInitiative);
                // return usm can be applied as result;
                return true;
            case UnitStat.MovePoints:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    if (!doPreview)
                        MovePointsMax += GetUSMStatBonus(unitStatModifier, MovePointsMax);
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStat.ScoutingRange:
                // verify if unit is a party leader 
                if (IsLeader)
                {
                    if (!doPreview)
                        ScoutingRange += GetUSMStatBonus(unitStatModifier, ScoutingRange);
                    // return usm can be applied as result;
                    return true;
                }
                // this is not applicable to non-leaders
                return false;
            case UnitStat.DeathResistance:
                if (!doPreview)
                {
                    resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Death);
                    resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
                }
                // return usm can be applied as result;
                return true;
            case UnitStat.FireResistance:
                if (!doPreview)
                {
                    resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Fire);
                    resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
                }
                // return usm can be applied as result;
                return true;
            case UnitStat.WaterResistance:
                if (!doPreview)
                {
                    resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Water);
                    resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
                }
                // return usm can be applied as result;
                return true;
            case UnitStat.MindResistance:
                if (!doPreview)
                {
                    resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Mind);
                    resistance.percent += GetUSMStatBonus(unitStatModifier, resistance.percent);
                }
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

    public void RemoveUSMEffectOnMaxStatValue(UnitStatModifier unitStatModifier)
    {
        Debug.Log("Remove USM's effect on max stat value");
        // init resistance var
        Resistance resistance;
        // 
        switch (unitStatModifier.unitStat)
        {
            case UnitStat.Leadership:
                UnitLeadership += GetUSMStatBonus(unitStatModifier, UnitLeadership);
                break;
            case UnitStat.Health:
                UnitHealthMax -= GetUSMStatBonus(unitStatModifier, UnitHealthMax);
                break;
            case UnitStat.Defense:
                UnitDefense -= GetUSMStatBonus(unitStatModifier, UnitDefense);
                break;
            case UnitStat.Power:
                UnitPower -= GetUSMStatBonus(unitStatModifier, UnitPower);
                break;
            case UnitStat.Initiative:
                UnitInitiative -= GetUSMStatBonus(unitStatModifier, UnitInitiative);
                break;
            case UnitStat.MovePoints:
                MovePointsMax -= GetUSMStatBonus(unitStatModifier, MovePointsMax);
                break;
            case UnitStat.ScoutingRange:
                ScoutingRange -= GetUSMStatBonus(unitStatModifier, ScoutingRange);
                break;
            case UnitStat.DeathResistance:
                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Death);
                resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
                break;
            case UnitStat.FireResistance:
                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Fire);
                resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
                break;
            case UnitStat.WaterResistance:
                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Water);
                resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
                break;
            case UnitStat.MindResistance:
                resistance = Array.Find(UnitResistances, element => element.source == UnitPowerSource.Mind);
                resistance.percent -= GetUSMStatBonus(unitStatModifier, resistance.percent);
                break;
            default:
                Debug.LogError("Unknown unit stat " + unitStatModifier.unitStat.ToString());
                // by default item is not applicable
                break;
        }
    }

    bool MatchScope(ModifierScope modifierScope)
    {
        // verify if we are in edit party screen
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            switch (modifierScope)
            {
                case ModifierScope.Self:
                case ModifierScope.FriendlyUnit:
                case ModifierScope.FriendlyParty:
                    return true;
                case ModifierScope.EnemyUnit:
                case ModifierScope.EnemyParty:
                    return false;
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
                    return false;
            }
        }
        // verify if we are in battle screen
        else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        {
            // get party leader who has this item equipped
            PartyUnit itemWearerLeaderUnit = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.transform.parent.GetComponent<PartyUnit>();
            switch (modifierScope)
            {
                case ModifierScope.Self:
                    // verify if itemWearerLeaderUnit matches current unit
                    if (itemWearerLeaderUnit.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ModifierScope.FriendlyUnit:
                case ModifierScope.FriendlyParty:
                    // verify if faction matches
                    if (itemWearerLeaderUnit.GetComponentInParent<HeroParty>().Faction == GetComponentInParent<HeroParty>().Faction)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ModifierScope.EnemyUnit:
                case ModifierScope.EnemyParty:
                    // verify if faction does not match
                    if (itemWearerLeaderUnit.GetComponentInParent<HeroParty>().Faction != GetComponentInParent<HeroParty>().Faction)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    Debug.LogError("Unknown modifier scope: " + modifierScope.ToString());
                    return false;
            }
        }
        else
        {
            Debug.LogError("Unknown screen active");
            // by default return false
            return false;
        }
    }

    // doPreview - use item without actually using it, just verify if item can be used by this unit
    public bool UseItem(InventoryItem inventoryItem, bool doPreview = false)
    {
        // consumable items can have different duration, for example
        // healing poition - instant usm (0)
        // apply scroll with harmful spell on the enemy - instant upm (0)
        // potion of agility - duration 1 day (>=1), increases unit's inititative by x
        // potion of Titan might - duration is permanent (<0), increases unit's strength by 10%
        // voodoo doll - instant upm, with 3 usages
        // resurection stone - instant usm, with 3 usages
        // to be more generic we assume that somebody may create item with different UPMs and USMs with differnet durations
        // init list of UPMs and USMs ids which are one-time and should be removed from the list of UPMs and USMs after being used
        //List<int> upmIDsTobeRemoved = new List<int>();
        //List<int> usmIDsTobeRemoved = new List<int>();
        // init is applicable by default with false, if at least one modifier is applicable it should reset this flag to true
        bool isApplicable = false;
        // verify if the same item is not already applying its UPMs and USMs, because bonuses from the same items are not stackable
        // normally this check is not needed for the entire-party scope items, which are equipped on the party leader, that is why we do not do this additional check against partly-leader-equipped items
        if ((GetUnitItemByName(inventoryItem.ItemName) != null)
            // and that item is not stackable
            && (!inventoryItem.ItemIsStackable)
            // and that item is not in equipment slot
            && (inventoryItem.HeroEquipmentSlot == HeroEquipmentSlot.None))
        {
            // unit already has this item
            // verify if item is stackable
            Debug.LogWarning("Unit already has " + inventoryItem.name + " item or its bonuses applied. The same item cannot be applied or consumed again.");
            // same item cannot be applied once again
            isApplicable = false;
        }
        else
        {
            // unit does not have this item yet or item is stackable, do additional checks
            // consume unique power modifiers
            for (int i = 0; i < inventoryItem.UniquePowerModifiers.Count; i++)
            {
                // verify if this is active modifier
                if ((inventoryItem.UniquePowerModifiers[i].modifierApplied == ModifierAppliedHow.Active)
                // verify if UPM required statuses match current unit status
                && (MatchStatuses(inventoryItem.UniquePowerModifiers[i].canBeAppliedToTheUnitsWithStatuses))
                // verify if UPM scope matches
                && (MatchScope(inventoryItem.UniquePowerModifiers[i].modifierScope)))
                {
                    // upm has instant one-time effect

                    // or temporary effect defined by the Duration in number of game or battle turns
                    // it is destroyed before start of the next turn if duration reaches
                    // it is applied automatically during calculations (for example damage or defence calculations)

                    // upm is normally applied only during the battle and to the enemy
                    // if we are in this situation, then it means that enemy has applied item with upm on this hero

                    // verify whether it is applied to base, max or current stat values
                    // apply upm
                    bool modifierCanBeApplied = ApplyActiveUPM(inventoryItem.UniquePowerModifiers[i], doPreview);
                    // if at least one upm is applicable, then set applicable flag to true
                    if (modifierCanBeApplied)
                        isApplicable = true;
                    //    upmIDsTobeRemoved.Add(i);
                }
                else
                {
                    // this is passive modifier
                    // it is applied automatically during calculations (for example damage or defence calculations)
                }
            }
            for (int i = 0; i < inventoryItem.UnitStatModifiers.Count; i++)
            {
                // verify if this is active USM
                if ((inventoryItem.UnitStatModifiers[i].modifierAppliedHow == ModifierAppliedHow.Active)
                // verify if USM required statuses match current unit status
                && (MatchStatuses(inventoryItem.UnitStatModifiers[i].canBeAppliedToTheUnitsWithStatuses))
                // verify if USM scope matches
                && (MatchScope(inventoryItem.UnitStatModifiers[i].modifierScope)))
                {
                    // verify if usm applies to current stats
                    if (inventoryItem.UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.CurrentStat)
                    {
                        // apply usm
                        bool modifierCanBeApplied = ApplyUSMToCurrentStatValue(inventoryItem.UnitStatModifiers[i], doPreview);
                        // if at least one usm is applicable, then set applicable flag to true
                        if (modifierCanBeApplied)
                            isApplicable = true;
                    }
                    else if (inventoryItem.UnitStatModifiers[i].modifierAppliedTo == ModifierAppliedTo.MaxStat)
                    {
                        // apply usm, without actually applying it, because it will be applied passively
                        bool modifierCanBeApplied = ApplyUSMToMaxStatValue(inventoryItem.UnitStatModifiers[i], true);
                        // if at least one usm is applicable, then set applicable flag to true
                        if (modifierCanBeApplied)
                            isApplicable = true;
                    }
                    else
                    {
                        Debug.LogError("Do not know to which stat (current or max) unit stat modifier is applied to");
                    }
                    // usm has instant one-time effect
                    // instant modifiers can applied only to the current stat value (example current health)
                    // but they cannot be applied to the base or max value (example to max health)

                    // or temporary effect defined by the Duration in number of game or battle turns
                    // it is destroyed before start of the next turn if duration reaches
                    // it is applied automatically during calculations (for example damage or defence calculations)

                    // verify whether it is applied to base, max or current stat values
                    // verify if it is applicable and that item is not usable any more
                    //if (isApplicable && (inventoryItem.LeftUsagesCount <= 1))
                    //    usmIDsTobeRemoved.Add(i);
                }
                else
                {
                    // usm applied passively from being placed in equipment slot
                    // or it does not apply
                    // it is applied automatically during calculations (for example damage or defence calculations)
                }
            }
            // verify if there are unit status modifiers
            if (inventoryItem.UnitStatusModifiers.Count >= 1)
            {
                // verify if unit status matches
                if ((MatchStatuses(inventoryItem.UnitStatusModifiers[0].canBeAppliedToTheUnitsWithStatuses))
                // verify if scope matches
                && (MatchScope(inventoryItem.UnitStatusModifiers[0].modifierScope)))
                {
                    // set is applicable flag
                    isApplicable = true;
                    // verify if this is not preview
                    if (!doPreview)
                    {
                        // Apply unit Status
                        UnitStatus = inventoryItem.UnitStatusModifiers[0].modifierSetStatus;
                    }
                }
            }
        }
        // verify if at leats one UPM or USM has been applied
        if (isApplicable)
        {
            // verify if this is not preview
            if (!doPreview)
            {
                //// destroy one-time instant USMs and UPMs
                //foreach (int upmIDTobeRemoved in upmIDsTobeRemoved)
                //{
                //    inventoryItem.UniquePowerModifiers.RemoveAt(upmIDTobeRemoved);
                //}
                //foreach (int usmIDTobeRemoved in usmIDsTobeRemoved)
                //{
                //    inventoryItem.UnitStatModifiers.RemoveAt(usmIDTobeRemoved);
                //}
                // decrement usages left counter
                inventoryItem.LeftUsagesCount -= 1;
            }
            // return item is consumable
            return true;
        }
        else
        {
            Debug.LogWarning("Item is not applicable to this unit");
            // this item could not be applied to that unit
            // item will return back automatically
            // return item is not consumable
            return false;
        }
    }

    public int GetUnitEffectiveMaxHealth()
    {
        return UnitHealthMax + GetItemsHealthBonus();
    }

    #region Attributes accessors
    public string UnitName
    {
        get
        {
            return partyUnitData.unitName;
        }

        set
        {
            partyUnitData.unitName = value;
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
            return partyUnitData.unitLeadership;
        }

        set
        {
            partyUnitData.unitLeadership = value;
        }
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

    public int UnitExperienceRequiredToReachNewLevel
    {
        get
        {
            return partyUnitData.unitExperienceRequiredToReachNewLevel;
        }

        set
        {
            partyUnitData.unitExperienceRequiredToReachNewLevel = value;
        }
    }

    public int UnitExperienceReward
    {
        get
        {
            return partyUnitData.unitExperienceReward;
        }

        set
        {
            partyUnitData.unitExperienceReward = value;
        }
    }

    public int UnitExperienceRewardIncrementOnLevelUp
    {
        get
        {
            return partyUnitData.unitExperienceRewardIncrementOnLevelUp;
        }

        set
        {
            partyUnitData.unitExperienceRewardIncrementOnLevelUp = value;
        }
    }

    public int UnitHealthCurr
    {
        get
        {
            return partyUnitData.unitHealthCurr;
        }

        set
        {
            partyUnitData.unitHealthCurr = value;
        }
    }

    public int UnitHealthMax
    {
        get
        {
            return partyUnitData.unitHealthMax;
        }

        set
        {
            partyUnitData.unitHealthMax = value;
        }
    }

    public int UnitHealthMaxIncrementOnLevelUp
    {
        get
        {
            return partyUnitData.unitHealthMaxIncrementOnLevelUp;
        }

        set
        {
            partyUnitData.unitHealthMaxIncrementOnLevelUp = value;
        }
    }

    public int UnitHealthRegenPercent
    {
        get
        {
            return partyUnitData.unitHealthRegenPercent;
        }

        set
        {
            partyUnitData.unitHealthRegenPercent = value;
        }
    }

    public int UnitDefense
    {
        get
        {
            return partyUnitData.unitDefense;
        }

        set
        {
            partyUnitData.unitDefense = value;
        }
    }

    public Resistance[] UnitResistances
    {
        get
        {
            return partyUnitData.unitResistances;
        }

        set
        {
            partyUnitData.unitResistances = value;
        }
    }

    public UnitAbility UnitAbility
    {
        get
        {
            return partyUnitData.unitAbility;
        }

        set
        {
            partyUnitData.unitAbility = value;
        }
    }

    public int UnitPower
    {
        get
        {
            return partyUnitData.unitPower;
        }

        set
        {
            partyUnitData.unitPower = value;
        }
    }

    public int UnitPowerIncrementOnLevelUp
    {
        get
        {
            return partyUnitData.unitPowerIncrementOnLevelUp;
        }

        set
        {
            partyUnitData.unitPowerIncrementOnLevelUp = value;
        }
    }

    public UnitPowerSource UnitPowerSource
    {
        get
        {
            return partyUnitData.unitPowerSource;
        }

        set
        {
            partyUnitData.unitPowerSource = value;
        }
    }

    public UnitPowerDistance UnitPowerDistance
    {
        get
        {
            return partyUnitData.unitPowerDistance;
        }

        set
        {
            partyUnitData.unitPowerDistance = value;
        }
    }

    public UnitPowerScope UnitPowerScope
    {
        get
        {
            return partyUnitData.unitPowerScope;
        }

        set
        {
            partyUnitData.unitPowerScope = value;
        }
    }

    public int UnitInitiative
    {
        get
        {
            return partyUnitData.unitInitiative;
        }

        set
        {
            partyUnitData.unitInitiative = value;
        }
    }

    public string UnitRole
    {
        get
        {
            return partyUnitData.unitRole;
        }

        set
        {
            partyUnitData.unitRole = value;
        }
    }

    public string UnitBriefDescription
    {
        get
        {
            return partyUnitData.unitBriefDescription;
        }

        set
        {
            partyUnitData.unitBriefDescription = value;
        }
    }

    public string UnitFullDescription
    {
        get
        {
            return partyUnitData.unitFullDescription;
        }

        set
        {
            partyUnitData.unitFullDescription = value;
        }
    }

    public int UnitCost
    {
        get
        {
            return partyUnitData.unitCost;
        }

        set
        {
            partyUnitData.unitCost = value;
        }
    }

    public UnitSize UnitSize
    {
        get
        {
            return partyUnitData.unitSize;
        }

        set
        {
            partyUnitData.unitSize = value;
        }
    }

    public bool IsInterpartyMovable
    {
        get
        {
            return partyUnitData.isInterpartyMovable;
        }

        set
        {
            partyUnitData.isInterpartyMovable = value;
        }
    }

    public bool IsDismissable
    {
        get
        {
            return partyUnitData.isDismissable;
        }

        set
        {
            partyUnitData.isDismissable = value;
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
        }
    }

    public UnitDebuff[] UnitDebuffs
    {
        get
        {
            return partyUnitData.unitDebuffs;
        }

        set
        {
            partyUnitData.unitDebuffs = value;
        }
    }

    public UnitBuff[] UnitBuffs
    {
        get
        {
            return partyUnitData.unitBuffs;
        }

        set
        {
            partyUnitData.unitBuffs = value;
        }
    }

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
            return partyUnitData.classIsUpgradable;
        }

        set
        {
            partyUnitData.classIsUpgradable = value;
        }
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
            return partyUnitData.canLearnSkills;
        }

        set
        {
            partyUnitData.canLearnSkills = value;
        }
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
            return partyUnitData.requiresUnitType;
        }

        set
        {
            partyUnitData.requiresUnitType = value;
        }
    }

    public UnitType[] UnlocksUnitTypes
    {
        get
        {
            return partyUnitData.unlocksUnitTypes;
        }

        set
        {
            partyUnitData.unlocksUnitTypes = value;
        }
    }

    public int UpgradeCost
    {
        get
        {
            return partyUnitData.upgradeCost;
        }

        set
        {
            partyUnitData.upgradeCost = value;
        }
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
        }
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
            return partyUnitData.movePointsMax;
        }

        set
        {
            partyUnitData.movePointsMax = value;
        }
    }

    public int ScoutingRange
    {
        get
        {
            return partyUnitData.scoutingRange;
        }

        set
        {
            partyUnitData.scoutingRange = value;
        }
    }

    public UnitSkill[] UnitSkills
    {
        get
        {
            return partyUnitData.unitSkills;
        }

        set
        {
            partyUnitData.unitSkills = value;
        }
    }

    public List<UniquePowerModifier> UniquePowerModifiers
    {
        get
        {
            return partyUnitData.uniquePowerModifiers;
        }

        set
        {
            partyUnitData.uniquePowerModifiers = value;
        }
    }

    public string UnitCellAddress
    {
        get
        {
            return partyUnitData.unitCellAddress;
        }

        set
        {
            partyUnitData.unitCellAddress = value;
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
}
