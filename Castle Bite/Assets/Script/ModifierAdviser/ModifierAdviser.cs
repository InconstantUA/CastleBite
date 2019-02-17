using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier adviser (used by unique power modifier)
public class ModifierAdviser : ScriptableObject
{

    //// default verify if modifier has to be discarded
    //public virtual bool DoAdviseAgainstUPMUsageInContextOf(System.Object srcContext, System.Object dstContext)
    //{
    //    // by default not advise against UPM usage
    //    return false;
    //}

    public virtual bool DoAdviseAgainstUPMUsageInContextOf(System.Object context)
    {
        // by default not advise against UPM usage
        return false;
    }

}
