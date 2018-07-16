using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PartyUnit : MonoBehaviour {
    // Custom types
    public enum UnitType
    {
        CapitalGuard,
        Knight, Ranger, Archmage, Seraphim, Thief, Warrior, Mage, Priest, Colossus, Archer,
        Orc, Goblin, Ogre, Cyclop, Troll,
        Chevalier, Gladiator, Templar, Lancer, Warlord, Paladin, Champion, // Warrior upgrades
        Unknown
    };

    public enum UnitSize {
        Single,
        Double
    };

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

    public enum UnitPowerSource
    {
        Physical,   // Attack with metal weapons
        Water,
        Fire,
        Earth,
        Wind,
        Life,       // Heal
        Death,      // Poison
        Pure,       // Cannot be resisted
        None
    }

    public enum UnitPowerDistance
    {
        Mele,
        Ranged
    }

    public enum UnitPowerScope
    {
        OneUnit,
        EntireParty
    }

    public enum UnitStatus
    {
        Active, // not Dead and not Escaped = can fight
        Waiting,
        Escaping,
        Escaped,
        Dead
    }

    public enum UnitDebuff:int
    {
        None,
        Poisoned,
        Burned,
        Chilled,
        Paralyzed,
        ArrSize
    }

    public enum UnitBuff:int
    {
        None,
        DefenceStance,
        ArrSize // for dynamic resizing of UnitBuffs array
    }

    public class UnitSkill
    {
        public string Name { get; set; }
        public class SkillLevel
        {
            public int Current { get; set; }
            public int Max { get; set; }
            public SkillLevel(int current, int max)
            {
                Current = current;
                Max = max;
            }
        }
        public SkillLevel Level { get; set; }
        public int RequiredHeroLevel { get; set; }
        public int LevelUpIncrementStep { get; set; } // skill can be learned only after this number of levels has passed
        public string Description { get; set; }
        public UnitSkill(string name, int currentLevel, int maxLevel, int requiredHeroLevel, int levelUpIncrementStep, string description)
        {
            Name = name;
            Level = new SkillLevel(currentLevel, maxLevel);
            RequiredHeroLevel = requiredHeroLevel;
            LevelUpIncrementStep = levelUpIncrementStep;
            Description = description;
        }
    }

    public UnitSkill[] skills = new UnitSkill[]
    {
        new UnitSkill(
            "Leadership",
            0, 3,
            3, 2,
            "Allows hero to have 1 multiplied by skill level additional unit(s) in his party."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            "Offence",
            0, 3,
            2, 2,
            "Increase hero attack power by 15% multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            "Defence",
            0, 3,
            2, 2,
            "Increase hero defence from all sources by adding 10 defence points multiplied by skill level to current hero defence value."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned after 2 hero level ups."
        ),
        new UnitSkill(
            "Pathfinding",
            0, 3,
            2, 1,
            "Increase hero move points by 50% multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
        ),
        new UnitSkill(
            "Scouting",
            0, 3,
            2, 1,
            "Increase hero scouting range in the fog of war by 1 tile multiplied by skill level."
            + "\r\n" + "Maximum level: 3."
            //+ "\r\n" + "2nd and higher skill levels can be learned one each hero level up."
        ),
        new UnitSkill(
            "Healing",
            0, 2,
            2, 3,
            "Increase hero and its party members daily healing rate by 20% from total health multiplied by skill level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 3 hero level ups."
        ),
        new UnitSkill(
            "Death Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Death-based attacks, for example Poison."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            "Fire Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Fire-based attacks, for example Burning."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            "Water Resistance",
            0, 2,
            5, 5,
            "Increase hero change to resist Water-based attacks, for example Chill."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 5 hero level ups."
        ),
        new UnitSkill(
            "Mind Resistance",
            0, 2,
            7, 7,
            "Increase hero change to resist Mind-based attacks, for example paralyze."
            + "\r\n" + "Chance to resist on the first skill level is 50%. Grants complete immunity on the 2nd level."
            + "\r\n" + "Maximum level: 2."
            //+ "\r\n" + "2nd skill level can be learned after 7 hero level ups."
        ),
    };

    // Misc attributes
    [SerializeField]
    string unitName;
    [SerializeField]
    UnitType unitType;
    // Leader attributes
    [SerializeField]
    bool isLeader;
    [SerializeField]
    int unitLeadership;
    [SerializeField]
    string givenName;
    // Level and experience
    [SerializeField]
    int unitLevel;
    [SerializeField]
    int unitExperience;
    [SerializeField]
    int unitExperienceRequiredToReachNewLevel;
    [SerializeField]
    int unitExperienceReward;
    [SerializeField]
    int unitExperienceRewardIncrementOnLevelUp;
    // Defensive attributes
    [SerializeField]
    int healthCurr;
    [SerializeField]
    int healthMax;
    [SerializeField]
    int healthMaxIncrementOnLevelUp;
    //[SerializeField]
    //bool isAlive = true;
    [SerializeField]
    int unitDefence;
    [SerializeField]
    UnitPowerSource[] unitResistances;
    [SerializeField]
    UnitPowerSource[] unitImmunities;
    // Offensive attributes
    [SerializeField]
    UnitAbility unitAbility;
    [SerializeField]
    int unitPower;
    [SerializeField]
    int unitPowerIncrementOnLevelUp;
    [SerializeField]
    UnitPowerSource unitPowerSource;
    [SerializeField]
    UnitPowerDistance unitPowerDistance;
    [SerializeField]
    UnitPowerScope unitPowerScope;
    [SerializeField]
    int unitInitiative = 10;
    // Unique power modifiers
    //[SerializeField]
    //UniquePowerModifier[] uniquePowerModifiers;
    // Misc Description
    [SerializeField]
    string unitRole;
    [SerializeField]
    string unitBriefDescription;
    [SerializeField]
    string unitFullDescription;
    // Misc Battle attributes
    //[SerializeField]
    //bool hasEscaped = false;
    [SerializeField]
    bool hasMoved = false;
    // Misc hire and edit unit attributes
    [SerializeField]
    int cost;
    [SerializeField]
    UnitSize unitSize;
    [SerializeField]
    bool isInterpartyMovable;
    [SerializeField]
    bool isDismissable;
    [SerializeField]
    UnitStatus unitStatus;
    [SerializeField]
    UnitDebuff[] unitDebuffs;
    [SerializeField]
    UnitBuff[] unitBuffs;
    // For Upgrade
    [SerializeField]
    int unitUpgradePoints;
    [SerializeField]
    int unitStatPoints;
    [SerializeField]
    bool classIsUpgradable;
    [SerializeField]
    int unitClassPoints;
    [SerializeField]
    bool canLearnSkills;
    [SerializeField]
    int unitSkillPoints;
    // For class upgrade
    [SerializeField]
    PartyUnit requiresUnit;
    [SerializeField]
    PartyUnit[] unlocksUnits;
    [SerializeField]
    int upgradeCost;


    void InitUnitBuffs()
    {
        unitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
        for (int i = 0; i < (int)UnitBuff.ArrSize; i++)
        {
            unitBuffs[i] = UnitBuff.None;
        }
    }

    void InitUnitDebuffs()
    {
        unitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
        for (int i = 0; i < (int)UnitDebuff.ArrSize; i++)
        {
            unitDebuffs[i] = UnitDebuff.None;
        }
    }

    private void Awake()
    {
        InitUnitBuffs();
        InitUnitDebuffs();
        //Debug.Log("Awake " + GetUnitName() + " " + GetGivenName() + " " + unitBuffs.Length.ToString());
    }

    private void Start()
    {
        //unitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
        //unitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
        //Debug.Log("Start " + GetUnitName() + " " + GetGivenName() + " " + unitBuffs.Length.ToString());
    }

    public Transform GetUnitCell()
    {
        // structure: 3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
        return transform.parent.parent.parent;
    }

    public Text GetUnitStatusText()
    {
        return GetUnitCell().Find("Status").GetComponent<Text>();
    }

    public Text GetUnitInfoPanelText()
    {
        return GetUnitCell().Find("InfoPanel").GetComponent<Text>();
    }

    public Transform GetUnitBuffsPanel()
    {
        return GetUnitCell().Find("Status/Buffs");
    }

    public Transform GetUnitDebuffsPanel()
    {
        return GetUnitCell().Find("Status/Debuffs");
    }

    public HeroParty GetUnitParty()
    {
        // structure: 6HeroParty/CityGarnizon-5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
        // verify if unit is member of party
        if (transform.parent)
            if (transform.parent.parent)
                if (transform.parent.parent.parent)
                    if (transform.parent.parent.parent.parent)
                        if (transform.parent.parent.parent.parent.parent.parent)
                            return transform.parent.parent.parent.parent.parent.parent.GetComponent<HeroParty>();
        return null;
    }

    public PartyPanel GetUnitPartyPanel()
    {
        // structure: 5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
        // verify if unit is member of party
        if (transform.parent.parent.parent)
        {
            if (transform.parent.parent.parent.parent)
            {
                return transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
            }
        }
        return null;
    }

    public City GetUnitCity()
    {
        // structure: 7city-6HeroParty/CityGarnizon-5PartyPanel-4Row-3UnitCell[Front/Back/Wide]-2UnitSlot-1UnitCanvas-Unit
        // verify if unit is member of party and city
        if (transform.parent)
            if (transform.parent.parent)
                if (transform.parent.parent.parent)
                    if (transform.parent.parent.parent)
                        if (transform.parent.parent.parent.parent)
                            if (transform.parent.parent.parent.parent.parent.parent)
                                if (transform.parent.parent.parent.parent.parent.parent.parent)
                                    return transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<City>();
        return null;
    }

    public int GetEffectiveDefence()
    {
        // Get additional defence mondifiers:
        //  - city
        //  - items
        // Verify if unit is in a friendly city and get city defence modifier
        PartyUnit unit = GetComponent<PartyUnit>();
        City city = unit.GetUnitCity();
        HeroParty party = unit.GetUnitParty();
        int cityDefenceModifier = 0;
        if (city)
        {
            // verify if city is friendly
            if (city.GetFaction() == party.GetFaction())
            {
                cityDefenceModifier = city.GetDefence();
            }
        }
        // Get items modifier
        // no items yet, skip
        // ..
        int itemsDefenceModifier = 0;
        // Get total defence modifier
        int totalDefenceModifier = unit.GetDefence() + cityDefenceModifier + itemsDefenceModifier;
        // Get status modifiers, example: defence stance buff
        if (unitBuffs != null)
        {
            Debug.Log(GetUnitName() + " " + GetGivenName() + " " + unitBuffs.Length.ToString());
            if (unitBuffs.Length != (int)UnitBuff.ArrSize)
            {
                InitUnitBuffs();
            }
        }
        else
        {
            Debug.Log("Unit buffs array is null");
        }
        if (UnitBuff.DefenceStance == unitBuffs[(int)UnitBuff.DefenceStance])
        {
            // reduce damage by half
            totalDefenceModifier = totalDefenceModifier + (100 - totalDefenceModifier) / 2;
        }
        return (totalDefenceModifier);
    }

    public void RemoveAllBuffs()
    {
        //Debug.Log("RemoveAllBuffs");
        // in unit properties
        for (int i = 0; i < unitBuffs.Length; i++)
        {
            unitBuffs[i] = UnitBuff.None;
        }
        // in UI
        UnitBuffIndicator[] allBuffs = GetUnitCell().Find("Status/Buffs").GetComponentsInChildren<UnitBuffIndicator>();
        foreach (UnitBuffIndicator buff in allBuffs)
        {
            Destroy(buff.gameObject);
        }
    }

    public void RemoveAllDebuffs()
    {
        // in unit properties
        for (int i = 0; i < unitDebuffs.Length; i++)
        {
            unitDebuffs[i] = UnitDebuff.None;
        }
        // in UI
        UnitDebuffIndicator[] allDebuffs = GetUnitCell().Find("Status/Debuffs").GetComponentsInChildren<UnitDebuffIndicator>();
        foreach (UnitDebuffIndicator buff in allDebuffs)
        {
            Destroy(buff.gameObject);
        }
    }

    public void RemoveAllBuffsAndDebuffs()
    {
        RemoveAllBuffs();
        RemoveAllDebuffs();
    }


    public int GetAbilityDamageDealt(PartyUnit activeBattleUnit)
    {
        int damageDealt = 0;
        int srcUnitDamage = activeBattleUnit.GetPower();
        int dstUnitDefence = GetEffectiveDefence();
        // calculate damage dealt
        damageDealt = (int)Math.Round((((float)srcUnitDamage * (100f - (float)dstUnitDefence)) / 100f));
        return damageDealt;
    }

    public void ApplyDestructiveAbility(int damageDealt)
    {
        int healthAfterDamage = GetHealthCurr() - damageDealt;
        // make sure that we do not set health less then 0
        if (healthAfterDamage <= 0)
        {
            healthAfterDamage = 0;
        }
        SetHealthCurr(healthAfterDamage);
        // update current health in UI
        // structure: 3[Front/Back/Wide]cell-2UnitSlot/HPPanel-1UnitCanvas-dstUnit
        // structure: [Front/Back/Wide]cell-UnitSlot/HPPanel-HPcurr
        // Transform cell = dstUnit.GetUnitCell();
        Text currentHealth = GetUnitCurrentHealthText();
        currentHealth.text = healthAfterDamage.ToString();
        // verify if unit is dead
        if (0 == healthAfterDamage)
        {
            // set unit is dead attribute
            SetUnitStatus(UnitStatus.Dead);
        }
        // display damage dealt in info panel
        Text infoPanel = GetUnitInfoPanelText();
        infoPanel.text = "-" + damageDealt + " health";
        infoPanel.color = Color.red;
    }

    public int GetDebuffDamageDealt(UniquePowerModifier appliedUniquePowerModifier)
    {
        return appliedUniquePowerModifier.Power;
    }

    // Note: animation should be identical to the function with the same name in PartyPanel
    public void FadeUnitCellInfo(float alpha)
    {
        Text infoPanel = GetUnitCell().Find("InfoPanel").GetComponent<Text>();
        Color c = infoPanel.color;
        c.a = alpha;
        infoPanel.color = c;
    }

    public void DeactivateExpiredBuffs()
    {
        // Deactivate expired buffs in UI
        // PartyUnit unit = GetComponent<PartyUnit>();
        UnitBuffIndicator[] buffsUI = GetUnitBuffsPanel().GetComponentsInChildren<UnitBuffIndicator>();
        foreach (UnitBuffIndicator buffUI in buffsUI)
        {
            // First decrement buff current duration
            buffUI.DecrementCurrentDuration();
            // Verify if it has timed out;
            if (buffUI.GetCurrentDuration() == 0)
            {
                // buff has timed out
                // deactivate it (it will be destroyed at the end of animation)
                buffUI.SetActiveAdvance(false);
                // deactivate it in unit properties too
                unitBuffs[(int)buffUI.GetUnitBuff()] = UnitBuff.None;
            }
        }
    }

    public void TriggerAppliedDebuffs()
    {
        //Debug.Log("TriggerAppliedDebuffs");
        UnitDebuffIndicator[] debuffsIndicators = GetUnitDebuffsPanel().GetComponentsInChildren<UnitDebuffIndicator>();
        //UnitDebuffsUI unitDebuffsUI = unit.GetUnitDebuffsPanel().GetComponent<UnitDebuffsUI>();
        foreach (UnitDebuffIndicator debuffIndicator in debuffsIndicators)
        {
            Debug.Log(name);
            // as long as we cannot initiate all debuffs at the same time
            // we add debuffs to the queue and they will be triggered one after another
            // CoroutineQueue queue = unitDebuffsUI.GetQueue();
            CoroutineQueue queue = transform.root.Find("BattleScreen").GetComponent<BattleScreen>().GetQueue();
            //if (queue == null)
            //{
            //    Debug.LogError("No queue");
            //}
            //if (debuffIndicator == null)
            //{
            //    Debug.LogError("No debuffIndicator");
            //}
            IEnumerator coroutine = debuffIndicator.TriggerDebuff(GetComponent<PartyUnit>());
            //if (coroutine == null)
            //{
            //    Debug.LogError("No coroutine");
            //}
            queue.Run(coroutine);
            // Trigger debuff against player
            // Decrement buff current duration
            debuffIndicator.DecrementCurrentDuration();
        }
    }

    public void HighlightActiveUnitInBattle(bool doHighlight)
    {
        // Highlight
        Color highlightColor;
        if (doHighlight)
        {
            //Debug.Log(" HighlightActiveUnitInBattle");
            highlightColor = Color.white;
        }
        else
        {
            //Debug.Log(" Remove highlight from active unit in battle");
            highlightColor = Color.grey;
        }
        // highlight unit canvas with required color
        Text canvasText = GetUnitCell().Find("Br").GetComponent<Text>();
        canvasText.color = highlightColor;
    }

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

    public void SetUnitStatus(UnitStatus value)
    {
        Debug.Log("Set unit " + unitName + " status " + value.ToString());
        unitStatus = value;
        // get new UI color according ot unit status
        Color32 newUIColor;
        // set dead in status
        string statusString;
        switch (value)
        {
            case UnitStatus.Active:
                newUIColor = new Color32(160, 160, 160, 255);
                statusString = "";
                break;
            case UnitStatus.Waiting:
                newUIColor = new Color32(96, 96, 96, 96);
                statusString = "Waiting";
                break;
            case UnitStatus.Escaping:
                newUIColor = new Color32(96, 96, 96, 255);
                statusString = "Escaping";
                break;
            case UnitStatus.Escaped:
                newUIColor = new Color32(64, 64, 64, 255);
                statusString = "Escaped";
                // clear unit buffs and debuffs
                RemoveAllBuffsAndDebuffs();
                break;
            case UnitStatus.Dead:
                newUIColor = new Color32(64, 64, 64, 255);
                statusString = "Dead";
                // clear unit buffs and debuffs
                RemoveAllBuffsAndDebuffs();
                break;
            default:
                Debug.LogError("Unknown status " + value.ToString());
                newUIColor = Color.red;
                statusString = "Error";
                break;
        }
        GetUnitCurrentHealthText().color = newUIColor;
        GetUnitMaxHealthText().color = newUIColor;
        GetUnitCanvasText().color = newUIColor;
        GetUnitStatusText().color = newUIColor;
        GetUnitStatusText().text = statusString;
    }


    public int GetPower()
    {
        return unitPower;
    }

    public void SetPower(int value)
    {
        unitPower = value;
    }

    public int GetPowerIncrementOnLevelUp()
    {
        return unitPowerIncrementOnLevelUp;
    }

    public void SetPowerIncrementOnLevelUp(int value)
    {
        unitPowerIncrementOnLevelUp = value;
    }

    public int GetExperienceReward()
    {
        return unitExperienceReward;
    }

    public void SetExperienceReward(int value)
    {
        unitExperienceReward = value;
    }

    public int GetExperienceRewardIncrementOnLevelUp()
    {
        return unitExperienceRewardIncrementOnLevelUp;
    }

    public void SetExperienceRewardIncrementOnLevelUp(int value)
    {
        unitExperienceRewardIncrementOnLevelUp = value;
    }

    public int GetExperience()
    {
        return unitExperience;
    }

    public void SetExperience(int value)
    {
        unitExperience = value;
    }

    public int GetExperienceRequiredToReachNewLevel()
    {
        return unitExperienceRequiredToReachNewLevel;
    }

    public void SetExperienceRequiredToReachNewLevel(int value)
    {
        unitExperienceRequiredToReachNewLevel = value;
    }

    public int GetDefence()
    {
        return unitDefence;
    }

    public void SetDefence(int value)
    {
        unitDefence = value;
    }

    public UnitAbility GetAbility()
    {
        return unitAbility;
    }

    public void SetAbility(UnitAbility value)
    {
        unitAbility = value;
    }

    public UnitPowerDistance GetPowerDistance()
    {
        return unitPowerDistance;
    }

    public void SetPowerDistance(UnitPowerDistance value)
    {
        unitPowerDistance = value;
    }

    public UnitPowerScope GetPowerScope()
    {
        return unitPowerScope;
    }

    public void SetPowerScope(UnitPowerScope value)
    {
        unitPowerScope = value;
    }

    public UnitPowerSource GetPowerSource()
    {
        return unitPowerSource;
    }

    public void SetPowerSource(UnitPowerSource value)
    {
        unitPowerSource = value;
    }

    public UnitPowerSource[] GetResistances()
    {
        return unitResistances;
    }

    public void SetResistances(UnitPowerSource[] value)
    {
        unitResistances = value;
    }

    public UnitPowerSource[] GetImmunities()
    {
        return unitImmunities;
    }

    public void SetImmunities(UnitPowerSource[] value)
    {
        unitImmunities = value;
    }

    public void SetCost(int requiredCost)
    {
        cost = requiredCost;
    }

    public int GetCost()
    {
        return cost;
    }

    public void SetHealthCurr(int requiredHealth)
    {
        healthCurr = requiredHealth;
    }

    public int GetHealthCurr()
    {
        return healthCurr;
    }

    public void SetHealthMax(int requiredHealth)
    {
        healthMax = requiredHealth;
    }

    public int GetHealthMax()
    {
        return healthMax;
    }

    public void SetHealthMaxIncrementOnLevelUp(int value)
    {
        healthMaxIncrementOnLevelUp = value;
    }

    public int GetHealthMaxIncrementOnLevelUp()
    {
        return healthMaxIncrementOnLevelUp;
    }

    public void SetUnitName(string requiredName)
    {
        unitName = requiredName;
    }

    public string GetUnitName()
    {
        return unitName;
    }

    public void SetGivenName(string requiredGivenName)
    {
        givenName = requiredGivenName;
    }

    public string GetGivenName()
    {
        return givenName;
    }

    public void SetLevel(int requiredLevel)
    {
        unitLevel = requiredLevel;
    }

    public int GetLevel()
    {
        return unitLevel;
    }

    public void SetLeadership(int requiredLeadership)
    {
        unitLeadership = requiredLeadership;
    }

    public int GetLeadership()
    {
        return unitLeadership;
    }

    public void SetRole(string requiredRole)
    {
        unitRole = requiredRole;
    }

    public string GetRole()
    {
        return unitRole;
    }

    public void SetBriefDescription(string requiredBriefDescription)
    {
        unitBriefDescription = requiredBriefDescription;
    }

    public string GetBriefDescription()
    {
        return unitBriefDescription;
    }

    public string GetFullDescription()
    {
        return unitFullDescription;
    }

    public void SetFullDescription(string requiredFullDescription)
    {
        unitFullDescription = requiredFullDescription;
    }

    public void SetUnitType(UnitType requiredUnitType)
    {
        unitType = requiredUnitType;
    }

    public UnitType GetUnitType()
    {
        return unitType;
    }

    public void SetUnitSize(UnitSize requiredUnitSize)
    {
        unitSize = requiredUnitSize;
    }

    public UnitSize GetUnitSize()
    {
        return unitSize;
    }

    public bool GetIsLeader()
    {
        return isLeader;
    }

    public void SetIsLeader(bool isLdr)
    {
        isLeader = isLdr;
    }

    public bool GetIsDismissable()
    {
        return isDismissable;
    }

    public void SetIsDismissable(bool isLdr)
    {
        isDismissable = isLdr;
    }

    public bool GetIsInterpartyMovable()
    {
        return isInterpartyMovable;
    }

    public void SetIsInterpartyMovable(bool isLdr)
    {
        isInterpartyMovable = isLdr;
    }

    //public bool GetIsAlive()
    //{
    //    return isAlive;
    //}

    //public void SetIsAlive(bool value)
    //{
    //    isAlive = value;
    //}

    //public bool GetHasEscaped()
    //{
    //    return hasEscaped;
    //}

    //public void SetHasEscaped(bool value)
    //{
    //    hasEscaped = value;
    //}

    public bool GetHasMoved()
    {
        return hasMoved;
    }

    public void SetHasMoved(bool value)
    {
        hasMoved = value;
    }

    public int GetInitiative()
    {
        return unitInitiative;
    }

    public void SetInitiative(int value)
    {
        unitInitiative = value;
    }

    public UnitStatus GetUnitStatus()
    {
        return unitStatus;
    }

    public Text GetUnitCurrentHealthText()
    {
        return GetUnitCell().Find("HPPanel/HPcurr").GetComponent<Text>();
    }

    public Text GetUnitMaxHealthText()
    {
        return GetUnitCell().Find("HPPanel/HPmax").GetComponent<Text>();
    }

    public Text GetUnitCanvasText()
    {
        return GetUnitCell().Find("Br").GetComponent<Text>();
    }

    public UnitDebuff[] GetUnitDebuffs()
    {
        return unitDebuffs;
    }

    public UnitBuff[] GetUnitBuffs()
    {
        return unitBuffs;
    }

    public int UnitUpgradePoints
    {
        get
        {
            return unitUpgradePoints;
        }

        set
        {
            unitUpgradePoints = value;
        }
    }

    public int UnitStatPoints
    {
        get
        {
            return unitStatPoints;
        }

        set
        {
            unitStatPoints = value;
        }
    }

    public bool ClassIsUpgradable
    {
        get
        {
            return classIsUpgradable;
        }

        set
        {
            classIsUpgradable = value;
        }
    }

    public int UnitClassPoints
    {
        get
        {
            return unitClassPoints;
        }

        set
        {
            unitClassPoints = value;
        }
    }

    public bool CanLearnSkills
    {
        get
        {
            return canLearnSkills;
        }

        set
        {
            canLearnSkills = value;
        }
    }

    public int UnitSkillPoints
    {
        get
        {
            return unitSkillPoints;
        }

        set
        {
            unitSkillPoints = value;
        }
    }

    public PartyUnit RequiresUnit
    {
        get
        {
            return requiresUnit;
        }

        set
        {
            requiresUnit = value;
        }
    }

    public PartyUnit[] UnlocksUnits
    {
        get
        {
            return unlocksUnits;
        }

        set
        {
            unlocksUnits = value;
        }
    }

    public int UpgradeCost
    {
        get
        {
            return upgradeCost;
        }

        set
        {
            upgradeCost = value;
        }
    }

    public void SetUnitCellInfoUI()
    {
        // set Name
        GetUnitCell().Find("UnitSlot").GetChild(0).Find("Name").GetComponent<Text>().text = GetUnitPartyPanel().GetUnitDisplayName(GetComponent<PartyUnit>());
        // set Health
        GetUnitCell().Find("HPPanel/HPcurr").GetComponent<Text>().text = healthCurr.ToString();
        GetUnitCell().Find("HPPanel/HPmax").GetComponent<Text>().text = healthMax.ToString();
    }
    //public UniquePowerModifier[] GetUniquePowerModifiers()
    //{
    //    return uniquePowerModifiers;
    //}

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

}
