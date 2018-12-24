using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueAbilitySelector : MonoBehaviour
{
    [SerializeField]
    UniqueAbilityConfig uniqueAbilityConfig;

    public UniqueAbilityConfig UniqueAbilityConfig
    {
        get
        {
            return uniqueAbilityConfig;
        }

        set
        {
            uniqueAbilityConfig = value;
        }
    }
}
