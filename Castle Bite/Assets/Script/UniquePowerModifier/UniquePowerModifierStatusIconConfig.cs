using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/StatusIconConfig")]
public class UniquePowerModifierStatusIconConfig : ScriptableObject
{
    public float fadeBackgroundAnimationDuration;
    public float onDisableFadeOutTextDuration;
    public float blinkTextDuration;
    public float blinkTextTransparencyThreshold;
}
