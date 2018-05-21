using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnit : MonoBehaviour {
    public int cost;
    public int healthCurr;
    public int healthMax;
    public bool isLeader;
    public string unitName;
    public string givenName;
    public int unitLevel;
    public int unitLeadership;

    public enum HeroType { Knight, Ranger, Archmage, Archangel, Thief, Unknown};
    public HeroType heroType;

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

    public void SetName(string requiredName)
    {
        unitName = requiredName;
    }

    public string GetName()
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


    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

}
