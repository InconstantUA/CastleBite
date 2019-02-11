using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/UIConfigs/StatusIcon")]
public class UniquePowerModifierStatusIconUIConfig : ScriptableObject
{
    public string symbol;
    public Color statusIconTextNormalColor;
    public Color statusIconTextHighlightedColor;
    public Color statusIconTextPressedColor;
    public Color statusIconTextDisabledColor;
    public Color statusIconBackgroundColor;
    public TextAnimation onTriggerTextAnimation;
}
