using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnit : MonoBehaviour {
    public int cost;
    public int healthCurr;
    public int healthMax;
    public bool isLeader;
    public string unitName;

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


    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

}
