using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Limiters/Limit Modifier Propagation To Other Units")]
public class LimitModifierPropagationToOtherUnits : ModifierLimiter
{
    [SerializeField]
    bool doPropagateUPM;

    public bool DoesContextMatch(System.Object context)
    {
        // verify if context matches battle context
        if (context is PropagationContext)
        {
            // context match
            return true;
        }
        // by default context doesn't match
        return false;
    }

    public override ValidationResult DoDiscardModifierInContextOf(System.Object context)
    {
        // verify if context doesn't match requirements of this limiter
        if (!DoesContextMatch(context))
        {
            // context is not in scope of this limiter
            // don't limit
            return ValidationResult.Pass();
        }
        // verify if context matches battle context
        if (context is PropagationContext)
        {
            // Get propagation context
            PropagationContext propagationContext = (PropagationContext)context;
            // verify if destination unit is set
            if (propagationContext.DestinationPartyUnit != null)
            {
                // verify if we need to propagate UPM
                if (doPropagateUPM)
                {
                    // don't limit
                    return ValidationResult.Pass();
                }
                else
                {
                    // limit (do not propagate this UPM)
                    return ValidationResult.Discard(onDiscardMessage);
                }
            }
            else
            {
                // limit (do not propagate this UPM)
                return ValidationResult.Discard(onDiscardMessage);
            }
        }
        // don't limit
        return ValidationResult.Pass();
    }

}
