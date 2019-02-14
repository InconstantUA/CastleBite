using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Animations/None")]
public class UniquePowerModifierNoneAnimation : UniquePowerModifierAnimation
{
    public override void Run(System.Object context)
    {
        Debug.Log("Run None Animation");
    }
}
