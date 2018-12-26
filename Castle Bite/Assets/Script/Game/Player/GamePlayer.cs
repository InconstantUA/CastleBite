using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum PlayerType
{
    Human,
    AI
}

[Serializable]
public enum PlayerUniqueAbility {
    Powerful,
    Tough,
    Blessed,
    Cursed,
    Leech,
    Hardcore
};

[Serializable]
public class UniqueAbilityData : System.Object
{
    public PlayerUniqueAbility playerUniqueAbility;
    public int currentLevel;
    public int currentLearningPoints;
}

[Serializable]
public enum PlayerTurnState
{
    Active,
    Waiting,
    HasMoved
}

[Serializable]
public class PlayerData : System.Object
{
    public UniqueAbilityData playerUniqueAbilityData;
    public PlayerType playerType;
    public string givenName;
    public int totalGold;
    public int totalMana;
    public Faction faction;
    public PlayerTurnState playerTurnState = PlayerTurnState.Waiting;
    public int focusedObjectID = 0;
    public int[,] tilesDiscoveryState;
    public AchievementData[] achievements;
    public float age;           // For Achievements and stats: time spend playing game
    public int battlesWon;      // For Achievements and stats
    public int battlesLost;     // For Achievements and stats
    public int battlesEscaped;  // For Achievements and stats
    public int citiesCaptured;  // For Achievements and stats
}

// class for events references
[Serializable]
public class Gold : System.Object
{

}

// class for events references
[Serializable]
public class Mana : System.Object
{

}

public class GamePlayer : MonoBehaviour {
    [SerializeField]
    PlayerData playerData;
    // for player time tracking
    float timeOfTheLastTakenMeasurement;

    //[SerializeField]
    //int totalGold;
    //[SerializeField]
    //Faction faction;

    // for Debug when player are not create on game start
    void Awake()
    {
        // init time
        timeOfTheLastTakenMeasurement = Time.time;
        // Get map manager
        // MapManager mapManager = transform.root.Find("MapScreen").GetComponentInChildren<MapManager>(true);
        // init tiles discovery array
        // playerData.tilesDiscoveryState = new int[MapManager.Instance.TileMapWidth, MapManager.Instance.TileMapHeight];
        // playerData.tilesDiscoveryState = new int[MapManager.Instance.TileMapWidth, MapManager.Instance.TileMapHeight];
    }

    public PlayerType PlayerType
    {
        get
        {
            return playerData.playerType;
        }

        set
        {
            playerData.playerType = value;
        }
    }

    public Faction Faction
    {
        get
        {
            return playerData.faction;
        }

        set
        {
            playerData.faction = value;
        }
    }

    public string GivenName
    {
        get
        {
            return playerData.givenName;
        }

        set
        {
            playerData.givenName = value;
        }
    }

    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }

        set
        {
            playerData = value;
        }
    }

    public int TotalGold
    {
        get
        {
            return playerData.totalGold;
        }

        set
        {
            playerData.totalGold = value;
            // trigger event
            EventsAdmin.Instance.IHasChanged(this, new Gold());
        }
    }

    public int TotalMana
    {
        get
        {
            return playerData.totalMana;
        }

        set
        {
            playerData.totalMana = value;
            // trigger event
            EventsAdmin.Instance.IHasChanged(this, new Mana());
        }
    }

    public PlayerTurnState PlayerTurnState
    {
        get
        {
            return playerData.playerTurnState;
        }

        set
        {
            playerData.playerTurnState = value;
        }
    }

    public int FocusedObjectID
    {
        get
        {
            return playerData.focusedObjectID;
        }

        set
        {
            playerData.focusedObjectID = value;
        }
    }

    public int[,] TilesDiscoveryState
    {
        get
        {
            return playerData.tilesDiscoveryState;
        }

        set
        {
            playerData.tilesDiscoveryState = value;
        }
    }

    public float Age
    {
        get
        {
            return playerData.age;
        }

        set
        {
            playerData.age = value;
        }
    }

    public UniqueAbilityData PlayerUniqueAbilityData
    {
        get
        {
            return playerData.playerUniqueAbilityData;
        }

        set
        {
            playerData.playerUniqueAbilityData = value;
        }
    }

    public int BattlesWon
    {
        get
        {
            return playerData.battlesWon;
        }

        set
        {
            playerData.battlesWon = value;
        }
    }

    public int BattlesLost
    {
        get
        {
            return playerData.battlesLost;
        }

        set
        {
            playerData.battlesLost = value;
        }
    }

    public int BattlesEscaped
    {
        get
        {
            return playerData.battlesEscaped;
        }

        set
        {
            playerData.battlesEscaped = value;
        }
    }

    public int CitiesCaptured
    {
        get
        {
            return playerData.citiesCaptured;
        }

        set
        {
            playerData.citiesCaptured = value;
        }
    }

    public AchievementData[] Achievements
    {
        get
        {
            return playerData.achievements;
        }

        set
        {
            playerData.achievements = value;
        }
    }

    public string GetAge()
    {
        // Get time difference between last measurement and current time
        float timeDelta = Time.time - timeOfTheLastTakenMeasurement;
        // Add player delta to the player age value
        playerData.age += timeDelta;
        // Get timespan
        TimeSpan timeSpan = TimeSpan.FromSeconds(playerData.age);
        // Calculate automatically number of years/months/days/hours/minutes and return this as a result
        return timeSpan.Days + " day(s) " + timeSpan.Hours + " hour(s) " + timeSpan.Minutes + " minute(s)";
    }

    public int GetTotalGoldIncomePerDay()
    {
        // set total gold income per day to 0
        int totalGoldIncomePerDay = 0;
        // loop through all cities
        foreach(City city in ObjectsManager.Instance.GetComponentsInChildren<City>(true))
        {
            // verify if city belongs to the player's faction
            if (city.CityFaction == Faction)
            {
                totalGoldIncomePerDay += city.GoldIncomePerDay;
            }
        }
        // return result
        return totalGoldIncomePerDay;
    }

    public int GetTotalManaIncomePerDay()
    {
        // set total gold income per day to 0
        int totalManaIncomePerDay = 0;
        // loop through all cities
        foreach (City city in ObjectsManager.Instance.GetComponentsInChildren<City>(true))
        {
            // verify if city belongs to the player's faction
            if (city.CityFaction == Faction)
            {
                totalManaIncomePerDay += city.ManaIncomePerDay;
            }
        }
        // return result
        return totalManaIncomePerDay;
    }
}
