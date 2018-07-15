using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UniquePowerModifier : MonoBehaviour
{
    // define possible origins (who is the source of unique power modifier)
    public enum PowerOrigin
    {
        Talent,
        Artifact
    }

    [SerializeField]
    PartyUnit.UnitDebuff appliedDebuff;
    [SerializeField]
    int power;
    [SerializeField]
    int powerIncrementOnLevelUp;
    [SerializeField]
    int duration;
    [SerializeField]
    int chance;
    [SerializeField]
    int chanceIncrementOnLevelUp;
    [SerializeField]
    PartyUnit.UnitPowerSource source;
    [SerializeField]
    PowerOrigin origin;

    public PartyUnit.UnitDebuff AppliedDebuff
    {
        get
        {
            return appliedDebuff;
        }

        set
        {
            appliedDebuff = value;
        }
    }

    public int Power
    {
        get
        {
            return power;
        }

        set
        {
            power = value;
        }
    }

    public int Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }

    public int Chance
    {
        get
        {
            return chance;
        }

        set
        {
            chance = value;
        }
    }

    public PartyUnit.UnitPowerSource Source
    {
        get
        {
            return source;
        }

        set
        {
            source = value;
        }
    }

    public PowerOrigin Origin
    {
        get
        {
            return origin;
        }

        set
        {
            origin = value;
        }
    }

    public int PowerIncrementOnLevelUp
    {
        get
        {
            return powerIncrementOnLevelUp;
        }

        set
        {
            powerIncrementOnLevelUp = value;
        }
    }

    public int ChanceIncrementOnLevelUp
    {
        get
        {
            return chanceIncrementOnLevelUp;
        }

        set
        {
            chanceIncrementOnLevelUp = value;
        }
    }

    public string GetDisplayName()
    {
        switch (appliedDebuff)
        {
            case PartyUnit.UnitDebuff.Burned:
                return "Burn";
            case PartyUnit.UnitDebuff.Chilled:
                return "Chill";
            case PartyUnit.UnitDebuff.Paralyzed:
                return "Paralyze";
            case PartyUnit.UnitDebuff.Poisoned:
                return "Poison";
            default:
                Debug.LogError("Unknown debuf");
                return "Error";
        }
    }
}