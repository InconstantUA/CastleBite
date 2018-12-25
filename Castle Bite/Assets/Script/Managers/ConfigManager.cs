using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    [SerializeField]
    UniqueAbilityConfig[] uniqueAbilityConfigs;
    [SerializeField]
    CityUpgradeConfig cityUpgradeConfig;

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

    public CityUpgradeConfig CityUpgradeConfig
    {
        get
        {
            return cityUpgradeConfig;
        }

        set
        {
            cityUpgradeConfig = value;
        }
    }

}
