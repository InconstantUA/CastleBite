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
    Offence,
    Defense,
    Sorcery,
    Hardcore
};

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
    public PlayerUniqueAbility playerUniqueAbility;
    public PlayerType playerType;
    public string givenName;
    public int totalGold;
    public Faction faction;
    public PlayerTurnState playerTurnState = PlayerTurnState.Waiting;
    public int focusedObjectID = 0;
    public int[,] tilesDiscoveryState;
}

[Serializable]
public class Gold : System.Object
{

}

public class GamePlayer : MonoBehaviour {
    [SerializeField]
    PlayerData playerData;
    //[SerializeField]
    //int totalGold;
    //[SerializeField]
    //Faction faction;

    // for Debug when player are not create on game start
    void Awake()
    {
        // Get map manager
        MapManager mapManager = transform.root.Find("MapScreen").GetComponentInChildren<MapManager>(true);
        // init tiles discovery array
        playerData.tilesDiscoveryState = new int[mapManager.TileMapWidth, mapManager.TileMapHeight];
    }
    
    public PlayerUniqueAbility PlayerUniqueAbility
    {
        get
        {
            return playerData.playerUniqueAbility;
        }

        set
        {
            playerData.playerUniqueAbility = value;
        }
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

    public int PlayerGold
    {
        get
        {
            return playerData.totalGold;
        }

        set
        {
            playerData.totalGold = value;
            // trigger event

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

    //public int GetTotalGold()
    //{
    //    return playerData.totalGold;
    //}

    //public void SetTotalGold(int newTotalGoldValue)
    //{
    //    playerData.totalGold = newTotalGoldValue;
    //    // Trigger gold update event to update gold value in UI
    //}
}
