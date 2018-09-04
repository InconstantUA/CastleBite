﻿using System;
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
    Text turnNumberTextUI;

    public void Reset(Faction playerFaction = Faction.Unknown)
    {
        // verify if activePlayerID is set
        if (playerFaction != Faction.Unknown)
        {
            // loop through list of all players
            foreach(GamePlayer gamePlayer in transform.root.Find("GamePlayers").GetComponentsInChildren<GamePlayer>())
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
        // .. Fix
        foreach (GamePlayer gamePlayer in transform.root.Find("GamePlayers").GetComponentsInChildren<GamePlayer>())
        {
            if (PlayerTurnState.Active == gamePlayer.PlayerTurnState)
            {
                return gamePlayer;
            }
        }
        Debug.LogError("No active player");
        return null;
    }

    public void UpdateTurnNumberText()
    {
        Debug.Log("Set turn number text UI to " + TurnNumber.ToString());
        turnNumberTextUI.text = TurnNumber.ToString();
    }

    GamePlayer GetNextPlayer()
    {
        // get all players
        GamePlayer[] allPlayers = transform.root.Find("GamePlayers").GetComponentsInChildren<GamePlayer>();
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

    GameObject GetGameObjectOnMapByID(int id)
    {
        // loop through all objects on map
        foreach(MapObject mapObject in transform.root.Find("MapScreen/Map").GetComponentsInChildren<MapObject>())
        {
            // verify if id is the same as we are searching for
            if (mapObject.gameObject.GetInstanceID() == id)
            {
                return mapObject.gameObject;
            }
        }
        Debug.LogWarning("Failed to find game object by ID");
        return null;
    }

    public void EndTurn()
    {
        Debug.Log("End turn");
        // Get active player
        GamePlayer activePlayer = GetActivePlayer();
        // Get next player
        GamePlayer nextPlayer = GetNextPlayer();
        // Loop through each hero Party and execute needed actions
        foreach (HeroParty heroParty in transform.root.GetComponentsInChildren<HeroParty>())
        {
            // verify if party belongs to active player
            if (heroParty.Faction == activePlayer.Faction)
            {
                // verify if this is not city harnizon party
                if (heroParty.PartyMode != PartyMode.Garnizon)
                {
                    // reset move points to max
                    // Note: this should be done before giving control to other player, so during his turn he can make impact on the other parties move points
                    heroParty.GetPartyLeader().MovePointsCurrent = heroParty.GetPartyLeader().MovePointsMax;
                }
                // .. decrement daily debuffs
            }
            // verify if party belongs to the next player
            else if (heroParty.Faction == nextPlayer.Faction)
            {
                // loop through all party units
                foreach (PartyUnit partyUnit in heroParty.GetComponentsInChildren<PartyUnit>())
                {
                    // apply daily heal to all party members
                    // verify if health is not max already
                    if (partyUnit.UnitHealthCurr != partyUnit.UnitHealthMax)
                    {
                        // apply daily health regen
                        // Note: this should be done before taking control
                        partyUnit.UnitHealthCurr += partyUnit.GetUnitEffectiveHealthRegenPerDay();
                        // verify if health is not higher than max
                        if (partyUnit.UnitHealthCurr > partyUnit.UnitHealthMax)
                        {
                            // reset current health to max
                            partyUnit.UnitHealthCurr = partyUnit.UnitHealthMax;
                        }
                    }
                    // .. decrement daily buffs
                }
            }
        }
        // Change active player
        // Change current active player turn state to "HasMoved"
        activePlayer.PlayerTurnState = PlayerTurnState.HasMoved;
        // Change next active player turn state to active
        nextPlayer.PlayerTurnState = PlayerTurnState.Active;
        // Get map focus panel
        MapFocusPanel mapFocusPanel = transform.root.Find("MapScreen/MapMenu").GetComponentInChildren<MapFocusPanel>();
        // Reset map focus panel
        // Note it will also reset focused object ID for active player
        mapFocusPanel.ReleaseFocus();
        // Get map manager
        MapManager mapManager = transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
        // Reset map selection
        mapManager.SetSelection(MapManager.Selection.None);
        // verify if next player had focused object in the past
        if (nextPlayer.FocusedObjectID != 0)
        {
            // get previously focused ame object on map by id
            GameObject previouslyFocusedGameObjectOnMap = GetGameObjectOnMapByID(nextPlayer.FocusedObjectID);
            // verify if it is not null
            if (previouslyFocusedGameObjectOnMap != null)
            {
                // verify if it is MapHero
                if (previouslyFocusedGameObjectOnMap.GetComponent<MapHero>() != null)
                {
                    // init focus panel with map hero
                    mapFocusPanel.SetActive(previouslyFocusedGameObjectOnMap.GetComponent<MapHero>());
                    // select hero on map
                    mapManager.SetSelection(MapManager.Selection.PlayerHero, previouslyFocusedGameObjectOnMap.GetComponent<MapHero>());
                }
                // verify if it is MapCity
                else if (previouslyFocusedGameObjectOnMap.GetComponent<MapCity>() != null)
                {
                    // init focus panel with map city
                    mapFocusPanel.SetActive(previouslyFocusedGameObjectOnMap.GetComponent<MapCity>());
                    // select city on map
                    mapManager.SetSelection(MapManager.Selection.PlayerCity, previouslyFocusedGameObjectOnMap.GetComponent<MapCity>());
                }
                else
                {
                    Debug.LogWarning("Failed to find focused game object type");
                }
            }
        }
        // Update top player income info panel
        transform.root.Find("MiscUI/TopInfoPanel").GetComponentInChildren<TextBoxDisplayCurrentGoldValue>(true).UpdateGoldValue();
        // reset cursor to normal, because it is changed by MapManager on mapManager.SetSelection
        transform.root.Find("CursorController").GetComponent<CursorController>().SetNormalCursor();
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
