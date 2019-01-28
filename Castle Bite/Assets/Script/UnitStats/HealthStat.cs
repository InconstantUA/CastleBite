using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It is prototype and not used
[Serializable]
public class HealthStat : UnitStat
{
    public override int CurrentValue
    {
        get
        {
            return unitStatData.currentValue;
        }

        set
        {
            // here goes custom logic on set
            unitStatData.currentValue = value;
        }
    }
}
