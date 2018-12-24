using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[Serializable]
//public struct UniqueAbilityConfigsMap
//{
//    public PlayerUniqueAbility playerUniqueAbility;
//    public UniqueAbilityConfig uniqueAbilityConfig;
//}

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    [SerializeField]
    UniqueAbilityConfig[] uniqueAbilityConfigs;
    //[SerializeField]
    //UniqueAbilityConfigsMap[] uniqueAbilityConfigsMap;

    void Awake()
    {
        Instance = this;
    }

    public UniqueAbilityConfig[] UniqueAbilityConfigs
    {
        get
        {
            return uniqueAbilityConfigs;
        }
    }

    //public UniqueAbilityConfigsMap[] UniqueAbilityConfigsMap
    //{
    //    get
    //    {
    //        return uniqueAbilityConfigsMap;
    //    }

    //    set
    //    {
    //        uniqueAbilityConfigsMap = value;
    //    }
    //}
}
