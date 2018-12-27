using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TurnsData
{
    public int turnNumber;
}

public class TurnsManager : MonoBehaviour {
    public static TurnsManager Instance { get; private set; }
    [SerializeField]
    TurnsData turnsData;
    [SerializeField]
    Text turnNumberText;
    [SerializeField]
    SaveGame saveGame;

    public void Reset(Faction playerFaction = Faction.Unknown)
    {
        // verify if activePlayerID is set
        if (playerFaction != Faction.Unknown)
        {
            // loop through list of all players
            foreach(GamePlayer gamePlayer in ObjectsManager.Instance.GetGamePlayers())
            {
                // verify if there is a player that matches activePlayerID
                if (gamePlayer.Faction == playerFaction)
                {
                    // set required player active
                    gamePlayer.PlayerTurnState = PlayerTurnState.Active;
                }
                else
                {
                    // set all other players to the waiting state
                    gamePlayer.PlayerTurnState = PlayerTurnState.Waiting;
                }
            }
        }
        // reset turn number
        TurnNumber = 1;
        // update turn number UI
        UpdateTurnNumberText();
        // update active player name
        MapMenuManager.Instance.UpdateActivePlayerNameOnMapUI();
    }

    void Awake()
    {
        // verify if instance already initialized
        if (Instance == null)
        {
            // prevent this game object (game options) to be destroyed on scenes change
            // this only works on root objects in a scene, so disabled for now
            // DontDestroyOnLoad(gameObject);
            // initialize instance with this game object
            Instance = this;
        }
        // verify if instance were instantiated by some other scene, when there is already instance present
        else if (Instance != this)
        {
            // destroy this instance of game options to keep only one instance
            Destroy(gameObject);
        }
    }

    public GamePlayer GetActivePlayer()
    {
        foreach (GamePlayer gamePlayer in ObjectsManager.Instance.GetGamePlayers())
        {
            if (PlayerTurnState.Active == gamePlayer.PlayerTurnState)
            {
                return gamePlayer;
            }
        }
        Debug.LogWarning("No active player");
        return null;
    }

    public void UpdateTurnNumberText()
    {
        Debug.Log("Set turn number text UI to " + TurnNumber.ToString());
        turnNumberText.text = TurnNumber.ToString();
    }

    GamePlayer GetNextPlayer()
    {
        // get all players
        GamePlayer[] allPlayers = ObjectsManager.Instance.GetGamePlayers();
        for (int i = 0; i < allPlayers.Length; i++)
        {
            // get current player ID
            if (PlayerTurnState.Active == allPlayers[i].PlayerTurnState)
            {
                int nextPlayerIndex = i + 1;
                // verify if next player index is not higher than number of players
                if (nextPlayerIndex >= allPlayers.Length)
                {
                    // increment turn number
                    TurnNumber += 1;
                    // update turn number UI
                    UpdateTurnNumberText();
                    // start from beginning of array and return first player in array
                    return allPlayers[0];
                }
                else
                {
                    // return player at next index
                    return allPlayers[nextPlayerIndex];
                }
            }
        }
        Debug.LogError("Failed to find next player");
        return null;
    }

    public void EndTurn()
    {
        Debug.Log("End turn");
        // Verify if we need to save game
        if (GameOptions.Instance.gameOpt.autosave >= 1)
        {
            // automatically save game
            saveGame.AutoSave();
        }
        // Get active player
        GamePlayer activePlayer = GetActivePlayer();
        // Get next player
        GamePlayer nextPlayer = GetNextPlayer();
        // Execute post turn actions for the active player
        activePlayer.ExecutePostTurnActions();
        // Execute pre-turn actions for the next player
        nextPlayer.ExecutePreTurnActions();
        // Change active player
        // Change current active player turn state to "HasMoved"
        activePlayer.PlayerTurnState = PlayerTurnState.HasMoved;
        // Change next active player turn state to active
        nextPlayer.PlayerTurnState = PlayerTurnState.Active;
        // execute pre-turn actions for MapMenu
        MapMenuManager.Instance.ExecutePreTurnActions(nextPlayer);
        // Update map tiles data, because some friendly cities are passable and other cities are not passable unless conquerred.
        MapManager.Instance.InitTilesMap();
        // Update player income on the top info panel
        UIRoot.Instance.GetComponentInChildren<UIManager>().GetComponentInChildren<TopInfoPanel>(true).GetComponentInChildren<PlayerIncomeInfo>(true).UpdateInfo();
        // reset cursor to normal, because it is changed by MapManager on mapManager.SetSelection
        CursorController.Instance.SetNormalCursor();
    }

    public int TurnNumber
    {
        get
        {
            return turnsData.turnNumber;
        }

        set
        {
            turnsData.turnNumber = value;
        }
    }

    public TurnsData TurnsData
    {
        get
        {
            return turnsData;
        }

        set
        {
            turnsData = value;
        }
    }
}
