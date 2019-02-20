using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Unit/UniquePowerModifiers/ConfigAttributes/ModifierScopeConfig")]
public class ModifierScopeConfig : ScriptableObject
{
    [SerializeField]
    ModifierScopeID modifierScopeID;
    [SerializeField]
    string displayName;
    [SerializeField]
    string description;

    public ModifierScopeID ModifierScopeID
    {
        get
        {
            return modifierScopeID;
        }
    }

    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }
}
