using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/UIConfigs/GeneralUIConfig")]
public class UniquePowerModifierUIConfig : ScriptableObject
{
    [SerializeField]
    private UniquePowerModifierValidationUIConfig validationUIConfig;
    [SerializeField]
    private UniquePowerModifierStatusIconUIConfig statusIconUIConfig;
    [SerializeField]
    private TextAnimation onTriggerUPMTextAnimation;
    [SerializeField]
    private UniquePowerModifierAnimation uniquePowerModifierAnimation;

    public UniquePowerModifierValidationUIConfig ValidationUIConfig
    {
        get
        {
            return validationUIConfig;
        }
    }

    public UniquePowerModifierStatusIconUIConfig StatusIconUIConfig
    {
        get
        {
            return statusIconUIConfig;
        }
    }

    public TextAnimation OnTriggerUPMTextAnimation
    {
        get
        {
            return onTriggerUPMTextAnimation;
        }
    }

    public UniquePowerModifierAnimation UniquePowerModifierAnimation
    {
        get
        {
            return uniquePowerModifierAnimation;
        }
    }
}
