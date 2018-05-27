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
    bool isInterpartyMovable;
    [SerializeField]
    bool isDismissable;
    [SerializeField]


    public enum UnitType { CapitalGuard, Knight, Ranger, Archmage, Seraphim, Thief, Swordsman, Mage, Priest, Colossus, Archer, Unknown };
    public UnitType unitType;

    public enum UnitSize { Single, Double };
    public UnitSize unitSize;

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

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

}
