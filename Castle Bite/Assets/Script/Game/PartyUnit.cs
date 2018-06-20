using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PartyUnit : MonoBehaviour {
    // Custom types
    public enum UnitType
    {
        CapitalGuard,
        Knight, Ranger, Archmage, Seraphim, Thief, Swordsman, Mage, Priest, Colossus, Archer,
        Orc, Goblin, Ogre, Cyclop, Troll,
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
        CutWithAxe,         // Greenskin Orc
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
        Dead,
        Escaped
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

    private void Awake()
    {
        unitDebuffs = new UnitDebuff[(int)UnitDebuff.ArrSize];
        unitBuffs = new UnitBuff[(int)UnitBuff.ArrSize];
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
        if (transform.parent.parent.parent)
        {
            if (transform.parent.parent.parent.parent)
            {
                if (transform.parent.parent.parent.parent.parent.parent)
                {
                    return transform.parent.parent.parent.parent.parent.parent.GetComponent<HeroParty>();
                }
            }
        }
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
        if (transform.parent.parent.parent)
        {
            if (transform.parent.parent.parent.parent)
            {
                if (transform.parent.parent.parent.parent.parent.parent)
                {
                    if (transform.parent.parent.parent.parent.parent.parent.parent)
                    {
                        return transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<City>();
                    }
                }
            }
        }
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
        if (UnitBuff.DefenceStance == unitBuffs[(int)UnitBuff.DefenceStance])
        {
            // reduce damage by half
            totalDefenceModifier = totalDefenceModifier + (100 - totalDefenceModifier) / 2;
        }
        return (totalDefenceModifier);
    }

    public void RemoveAllBuffs()
    {
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

    public string GetUnitStatusString()
    {
        switch (unitStatus)
        {
            case UnitStatus.Active:
                return "";
            case UnitStatus.Dead:
                return "Dead";
            case UnitStatus.Escaped:
                return "Escaped";
            default:
                Debug.LogError("Unknown unit status");
                return null;
        }
    }

    public Color GetUnitStatusColor()
    {
        switch (unitStatus)
        {
            case UnitStatus.Active:
                return Color.gray;
            case UnitStatus.Dead:
                return new Color32(64, 64, 64, 255); // darkest grey
            case UnitStatus.Escaped:
                return new Color32(96, 96, 96, 255); // darker grey
            default:
                Debug.LogError("Unknown unit status");
                return Color.red;
        }
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

    public void SetUnitStatus(UnitStatus value)
    {
        unitStatus = value;
    }

    public UnitDebuff[] GetUnitDebuffs()
    {
        return unitDebuffs;
    }

    public UnitBuff[] GetUnitBuffs()
    {
        return unitBuffs;
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
