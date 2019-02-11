using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/Validations/ValidationUIConfig")]
public class UniquePowerModifierValidationUIConfig : ScriptableObject
{
    public Color upmIsApplicableForUnitSlotColor;
    public Color upmIsApplicableButNotAdvisedForUnitSlotColor;
    public Color upmIsNotApplicableForUnitSlotColor;
}
