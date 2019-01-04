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
    [SerializeField]
    ColorsConfig playersObjectsOnMapColor;
    [SerializeField]
    UnitSkillConfig[] unitSkillConfigs;
    [SerializeField]
    GameSaveConfig gameSaveConfig;
    [SerializeField]
    PartyUnitConfig[] partyUnitConfigs;
    [SerializeField]
    InventoryItemConfig[] inventoryItemConfigs;

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

    public ColorsConfig PlayersObjectsOnMapColor
    {
        get
        {
            return playersObjectsOnMapColor;
        }

        set
        {
            playersObjectsOnMapColor = value;
        }
    }

    public UnitSkillConfig[] UnitSkillConfigs
    {
        get
        {
            return unitSkillConfigs;
        }

        set
        {
            unitSkillConfigs = value;
        }
    }

    public GameSaveConfig GameSaveConfig
    {
        get
        {
            return gameSaveConfig;
        }

        set
        {
            gameSaveConfig = value;
        }
    }

    public PartyUnitConfig[] PartyUnitConfigs
    {
        get
        {
            return partyUnitConfigs;
        }

        set
        {
            partyUnitConfigs = value;
        }
    }

    public InventoryItemConfig[] InventoryItemConfigs
    {
        get
        {
            return inventoryItemConfigs;
        }

        set
        {
            inventoryItemConfigs = value;
        }
    }
}
