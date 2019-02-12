using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier limiter (used by unique power modifier)
public class ModifierLimiter : ScriptableObject
{

    // default verify if modifier has to be discarded
    public virtual bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    {
        // by default do limit
        return true;
    }

    public virtual bool DoDiscardModifierInContextOf(System.Object context)
    {
        // by default do limit
        return true;
    }

    // Get message
    public virtual string OnLimitMessage
    {
        get
        {
            return "";
        }
    }

}
