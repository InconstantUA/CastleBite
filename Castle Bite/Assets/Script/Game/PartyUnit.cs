using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum UnitType
{
    None,
    Archangel,
    Knight, Ranger, Archmage, Seraphim, Thief, Warrior, Mage, Priest, Colossus, Archer,
    OrcWarrior, Goblin, Ogre, Cyclop, Troll,
    Chevalier, Gladiator, Templar, Lancer, Warlord, Paladin, Champion, // Warrior upgrades
    GiantToad, OrcShaman,
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
public class UniquePowerModifier
{
    // define possible origins (who is the source of unique power modifier)
    public UnitDebuff upmAppliedDebuff;
    public int upmPower;
    public int upmPowerIncrementOnLevelUp;
    public int upmDuration;
    public int upmChance;
    public int upmChanceIncrementOnLevelUp;
    public UnitPowerSource upmSource;
    public ModifierOrigin upmOrigin;

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
            + "\r\n" + "Each level increases shards aura bonus by 5."
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
    public string unitCellAddress;  // used only during game save and load
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

    public int GetUnitEffectivePower()
    {
        // get unit power plus skill bonus
        return UnitPower + GetOffenceSkillPowerBonus();
    }

    public int GetEffectiveLeadership()
    {
        // get current skill leadership bonus = skill level
        UnitSkill skill = Array.Find(UnitSkills, element => element.mName == UnitSkill.SkillName.Leadership);
        int skillBonus = skill.mLevel.mCurrent;
        return UnitLeadership + skillBonus;
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
        // get total move points
        int totalMovePoints = GetAdditiveMovePoints() + GetPathfindingSkillBonus();
        // return result
        return totalMovePoints;
    }

    public int GetEffectiveScoutingRange()
    {
        // get and return sum of default scouting range plus skill bonus
        return ScoutingRange + GetScoutingSkillBonus();
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

    public float GetUnitEffectiveHealthRegen()
    {
        return ((float)UnitHealthRegenPercent / 100f + GetUnitHealSkillHealthRegenModifier());
    }

    public int GetUnitEffectiveHealthRegenPerDay()
    {
        return (int)Math.Floor((float)UnitHealthMax * GetUnitEffectiveHealthRegen());
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
        // get base resistance
        int baseResistance = GetUnitBaseResistance(source);
        // get skill resistance
        int skillResistance = GetUnitResistanceSkillBonus(source);
        // get and return effective resistance
        return baseResistance + skillResistance;
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
