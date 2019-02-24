using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/BasePropagatedUPM")]
public class BasePropagatedUPM : UniquePowerModifier
{
    public override void Apply(System.Object context)
    {
        // This type of UPM is calculated automatically based on UPMs inheritance and propagation
    }

    public override void Trigger(PartyUnit dstPartyUnit, UniquePowerModifierData uniquePowerModifierData)
    {
        // This type of UPM is calculated automatically based on UPMs inheritance and propagation
    }
}
