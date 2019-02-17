using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for modifier limiter (used by unique power modifier)
public class ModifierLimiter : ScriptableObject
{
    public string onDiscardMessage;

    public class ValidationResult
    {
        // set default values:
        public bool doDiscardModifier = false;
        public string message = "";

        public static ValidationResult Pass()
        {
            return new ValidationResult
            {
                doDiscardModifier = false,
                message = ""
            };
        }

        public static ValidationResult Discard(string onDiscardMessage)
        {
            return new ValidationResult
            {
                doDiscardModifier = true,
                message = onDiscardMessage
            };
        }
    }
    //// default verify if modifier has to be discarded
    //bool DoDiscardModifierInContextOf(System.Object srcContext, System.Object dstContext)
    //{
    //    // by default do limit
    //    return true;
    //}

    public virtual ValidationResult DoDiscardModifierInContextOf(System.Object context)
    {
        // by default do not discard modifier
        return ValidationResult.Pass();
    }

    //// Get message
    //public virtual string OnLimitMessage
    //{
    //    get
    //    {
    //        return "";
    //    }
    //}

}
