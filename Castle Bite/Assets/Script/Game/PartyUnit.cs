using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnit : MonoBehaviour {
    [SerializeField]
    int cost;
    [SerializeField]
    int healthCurr;
    [SerializeField]
    int healthMax;
    [SerializeField]
    bool isLeader;
    [SerializeField]
    string unitName;
    [SerializeField]
    string givenName;
    [SerializeField]
    int unitLevel;
    [SerializeField]
    int unitLeadership;
    [SerializeField]
    string unitRole;
    [SerializeField]
    string unitBriefDescription;
    [SerializeField]
    string unitFullDescription;
    [SerializeField]
    bool isInterpartyMovable;
    [SerializeField]
    bool isDismissable;
    [SerializeField]
    bool isAlive = true;
    [SerializeField]
    bool hasEscaped = false;
    [SerializeField]
    bool hasMoved = false;
    [SerializeField]
    int initiative = 10;



    public enum UnitType {
        CapitalGuard,
        Knight, Ranger, Archmage, Seraphim, Thief, Swordsman, Mage, Priest, Colossus, Archer,
        Orc, Goblin, Ogre, Cyclop, Troll,
        Unknown };
    public UnitType unitType;

    public enum UnitSize { Single, Double };
    public UnitSize unitSize;

    public enum UnitPower {
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
        Heal,               // Dominion Priest
        BlowWithMaul,       // Dominion Colossus
        CastChainLightning,    // Dominion Mage
        None
    };
    [SerializeField]
    UnitPower unitPower;

    public enum UnitPowerSource
    {
        Physical,   // Attack with metal weapons
        Water,
        Fire,
        Earth,
        Wind,
        Life,       // Heal
        Pure,       // Cannot be resisted
        None
    }
    [SerializeField]
    UnitPowerSource unitPowerSource;



    public UnitPower GetPower()
    {
        return unitPower;
    }

    public void SetPower(UnitPower value)
    {
        unitPower = value;
    }

    public UnitPowerSource GetPowerSource()
    {
        return unitPowerSource;
    }

    public void SetPowerSource(UnitPowerSource value)
    {
        unitPowerSource = value;
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

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void SetIsAlive(bool value)
    {
        isAlive = value;
    }

    public bool GetHasEscaped()
    {
        return hasEscaped;
    }

    public void SetHasEscaped(bool value)
    {
        hasEscaped = value;
    }

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
        return initiative;
    }

    public void SetInitiative(int value)
    {
        initiative = value;
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

}
