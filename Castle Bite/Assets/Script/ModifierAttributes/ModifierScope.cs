using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierScope
{
    [SerializeField]
    ModifierScopeID modifierScopeID;

    public ModifierScope(ModifierScopeID value)
    {
        modifierScopeID = value;
    }

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
            return ConfigManager.Instance[ModifierScopeID].DisplayName;
        }
    }

    public string Description
    {
        get
        {
            return ConfigManager.Instance[ModifierScopeID].Description;
        }
    }
}
