using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It is prototype and not used
[Serializable]
public class UnitStat : System.Object
{
    public UnitStatData unitStatData; // we have to make it public to be serializable, but values should be changed only via Properties accessors

    public virtual UnitStatID UnitStatID
    {
        get
        {
            return unitStatData.unitStatID;
        }
    }

    public virtual int CurrentValue
    {
        get
        {
            return unitStatData.currentValue;
        }

        set
        {
            unitStatData.currentValue = value;
        }
    }
}
