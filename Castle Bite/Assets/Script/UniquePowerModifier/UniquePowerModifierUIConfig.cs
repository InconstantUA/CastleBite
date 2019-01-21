using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/UIConfig")]
public class UniquePowerModifierUIConfig : ScriptableObject
{
    public string symbol;
    public Color statusIconTextNormalColor;
    public Color statusIconTextHighlightedColor;
    public Color statusIconTextPressedColor;
    public Color statusIconTextDisabledColor;
    public Color statusIconBackgroundColor;
}
