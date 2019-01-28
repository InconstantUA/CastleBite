using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    UnitStatusConfig[] unitStatusConfigs;
    [SerializeField]
    UnitStatusUIConfig[] unitStatusUIConfigs;
    [SerializeField]
    UnitEventsConfig unitEventsConfig;
    [SerializeField]
    CityConfig[] cityConfigs;
    [SerializeField]
    FactionConfig[] factionConfigs;
    [SerializeField]
    UnitAbilityConfig[] unitAbilityConfigs;

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

    public UnitSkillConfig this[UnitSkillID unitSkillID]
    {
        get
        {
            return unitSkillConfigs.First(e => e.unitSkillID == unitSkillID);
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

    public UnitStatusUIConfig[] UnitStatusUIConfigs
    {
        get
        {
            return unitStatusUIConfigs;
        }

        set
        {
            unitStatusUIConfigs = value;
        }
    }

    public UnitEventsConfig UnitEventsConfig
    {
        get
        {
            return unitEventsConfig;
        }

        set
        {
            unitEventsConfig = value;
        }
    }

    public UnitStatusConfig[] UnitStatusConfigs
    {
        get
        {
            return unitStatusConfigs;
        }

        set
        {
            unitStatusConfigs = value;
        }
    }

    public CityConfig[] CityConfigs
    {
        get
        {
            return cityConfigs;
        }

        set
        {
            cityConfigs = value;
        }
    }

    public FactionConfig[] FactionConfigs
    {
        get
        {
            return factionConfigs;
        }

        set
        {
            factionConfigs = value;
        }
    }

    public FactionConfig this[Faction faction]
    {
        get
        {
            return factionConfigs.First(c => c.faction == faction);
        }
    }

    public CityConfig this[CityID cityID]
    {
        get
        {
            return cityConfigs.First(c => c.cityID == cityID);
            // return Array.Find(cityConfigs, c => c.cityID == cityID);
        }
    }

    public UnitAbilityConfig this[UnitAbilityID unitAbility]
    {
        get
        {
            return unitAbilityConfigs.First(e => e.unitAbilityID == unitAbility);
        }
    }

    public InventoryItemConfig this[InventoryItemID inventoryItemID]
    {
        get
        {
            return inventoryItemConfigs.First(e => e.inventoryItemID == inventoryItemID);
        }
    }

}
