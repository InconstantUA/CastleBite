using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScreen : MonoBehaviour {
    FocusPanel leftFocusPanel;
    FocusPanel rightFocusPanel;

    PartyPanel playerPartyPanel;
    PartyPanel enemyPartyPanel;

    PartyUnit activeUnit;

    public enum BattlePlace { Map, CityOutside, CityInside };
    BattlePlace battlePlace;

    bool battleHasEnded;

    CoroutineQueue queue;

    public enum TurnPhase
    {
        Main,       // moving all units
        PostWait    // moving units which activated Wait before hand
    };
    TurnPhase turnPhase;


    //public BattlePlace GetBattlePlace()
    //{
    //    return battlePlace;
    //}

    //public void SetBattlePlace(BattlePlace value)
    //{
    //    battlePlace = value;
    //}

    public bool GetBattleHasEnded()
    {
        return battleHasEnded;
    }

    // Use this for initialization
    void Awake() {
        // initialize internal "static" (non-changeable) resources
        leftFocusPanel = transform.Find("LeftFocus").GetComponent<FocusPanel>();
        rightFocusPanel = transform.Find("RightFocus").GetComponent<FocusPanel>();
        // Create a coroutine queue that can run max 1 coroutine at once
        queue = new CoroutineQueue(1, StartCoroutine);
    }

    BattlePlace GetBattlePlace(HeroParty playerHeroParty, HeroParty enemyHeroParty)
    {
        // Return battle place based on:
        //  - position of units before battle start
        //  - who is initiating battle
        // ..
        return BattlePlace.Map;
    }

    public void EnterBattle(MapHero playerOnMap, MapHero enemyOnMap)
    {
        // activate this battle sreen
        gameObject.SetActive(true);
        // Set turn phase to main phase
        SetTurnPhase(TurnPhase.Main);
        // get hero's parties
        HeroParty playerHeroParty = playerOnMap.linkedPartyTr.GetComponent<HeroParty>();
        HeroParty enemyHeroParty = enemyOnMap.linkedPartyTr.GetComponent<HeroParty>();
        // get battle place
        battlePlace = GetBattlePlace(playerHeroParty, enemyHeroParty);
        // move hero parties to the battle screen
        playerHeroParty.transform.SetParent(transform);
        enemyHeroParty.transform.SetParent(transform);
        // disable player party inventory and equipment
        playerHeroParty.transform.Find("PartyInventory").gameObject.SetActive(false);
        playerHeroParty.transform.Find("HeroEquipment").gameObject.SetActive(false);
        playerHeroParty.transform.Find("HeroEquipmentBtn").gameObject.SetActive(false);
        // Get parties panels
        playerPartyPanel = playerHeroParty.GetComponentInChildren<PartyPanel>();
        enemyPartyPanel = enemyHeroParty.GetComponentInChildren<PartyPanel>();
        // Get parties leaders
        PartyUnit playerPartyLeader = playerPartyPanel.GetPartyLeader();
        PartyUnit enemyPartyLeader = enemyPartyPanel.GetPartyLeader();
        // Link parties leaders to the focus panels
        leftFocusPanel.focusedObject = playerPartyLeader.gameObject;
        rightFocusPanel.focusedObject = enemyPartyLeader.gameObject;
        // Initialize focus panel with information from linked leaders
        leftFocusPanel.OnChange(FocusPanel.ChangeType.Init);
        rightFocusPanel.OnChange(FocusPanel.ChangeType.Init);
        // start turn based battle
        StartBattle();
    }

    void StartBattle()
    {
        Debug.Log("StartBattle");
        // set battle has started
        battleHasEnded = false;
        // deactivate hero edit click and drag handler
        playerPartyPanel.SetOnEditClickHandler(false);
        enemyPartyPanel.SetOnEditClickHandler(false);
        // activate battle click handler, which will react on clicks
        playerPartyPanel.SetOnBattleClickHandler(true);
        enemyPartyPanel.SetOnBattleClickHandler(true);
        // do battle until some party wins or other party flee
        if (!StartTurn())
        {
            // it is not possible to start new turn
            EndBattle();
        }
        // Deactivate exit battle button;
        transform.Find("Exit").gameObject.SetActive(false);
        // Activate all other battle buttons;
        transform.Find("CtrlPnlFight").gameObject.SetActive(true);
    }

    void DefaultOnBattleExit()
    {
        // if hero is still alive, then set click handler to edit mode
        BattleExit exitButton = transform.Find("Exit").GetComponent<BattleExit>();
        if (exitButton.GetExitOption() != BattleExit.ExitOption.DestroyPlayer)
        {
            // activate hero edit click and drag handler
            playerPartyPanel.SetOnEditClickHandler(true);
            // deactivate battle click handler, which will react on clicks
            playerPartyPanel.SetOnBattleClickHandler(false);
        }
        // Activate other required screen based on the original parties location
        switch (battlePlace)
        {
            case BattlePlace.Map:
                // Enable map screen
                Transform mapScreen = transform.root.Find("MapScreen");
                mapScreen.gameObject.SetActive(true);
                // Change map mode to browse
                MapManager mapManager = mapScreen.Find("Map").GetComponent<MapManager>();
                mapManager.SetMode(MapManager.Mode.Browse);
                break;
            case BattlePlace.CityOutside:
                break;
            case BattlePlace.CityInside:
                break;
            default:
                Debug.LogError("Unknown battle place");
                break;
        }
        // Reset almost all units statuses and info for panels, if they are still present
        // Exceptions: Dead
        if (playerPartyPanel)
        {
            playerPartyPanel.ResetUnitCellInfoPanel(playerPartyPanel.transform);
            playerPartyPanel.ResetUnitCellStatus(new string[] { playerPartyPanel.deadStatus });
        }
        if (enemyPartyPanel)
        {
            enemyPartyPanel.ResetUnitCellInfoPanel(enemyPartyPanel.transform);
            enemyPartyPanel.ResetUnitCellStatus(new string[] { enemyPartyPanel.deadStatus });
        }
        // Close battle sreen
        gameObject.SetActive(false);
    }

    public void DestroyPlayer()
    {
        // set variables
        HeroParty heroParty = playerPartyPanel.transform.parent.GetComponent<HeroParty>();
        MapHero heroOnMapRepresentation = heroParty.GetLinkedPartyOnMap();
        // destroy on map party representation
        Destroy(heroOnMapRepresentation.gameObject);
        // destroy party
        Destroy(heroParty.gameObject);
        DefaultOnBattleExit();
    }

    public void DestroyEnemy()
    {
        Destroy(enemyPartyPanel.transform.parent.gameObject);
        DefaultOnBattleExit();
    }

    public void FleePlayer()
    {
        DefaultOnBattleExit();
    }

    public void FleeEnemy()
    {
        DefaultOnBattleExit();
    }

    void EndBattle()
    {
        Debug.Log("EndBattle");
        // set battle has ended
        battleHasEnded = true;
        // Remove highlight from active unit
        queue.Run(activeUnit.HighlightActiveUnitInBattle(false));
        // Clear units info and status information
        enemyPartyPanel.ResetUnitCellStatus(new string[] { enemyPartyPanel.deadStatus, enemyPartyPanel.levelUpStatus });
        // Set exit button variable
        BattleExit exitButton = transform.Find("Exit").GetComponent<BattleExit>();
        // Activate exit battle button;
        exitButton.gameObject.SetActive(true);
        // Deactivate all other battle buttons;
        transform.Find("CtrlPnlFight").gameObject.SetActive(false);
        // Check who win battle
        if (!playerPartyPanel.CanFight())
        {
            // player cannot fight anymore
            // verify if player has flee from battle
            if (playerPartyPanel.HasEscapedBattle())
            {
                // player has escaped battle
                // On exit: move it 2 tiles away from other party
                exitButton.SetExitOption(BattleExit.ExitOption.FleePlayer);
            }
            else
            {
                // player lost battle and was destroyed
                // On exit: destroy player party
                exitButton.SetExitOption(BattleExit.ExitOption.DestroyPlayer);
            }
            // Show how much experience was earned by enemy party
            enemyPartyPanel.GrantAndShowExperienceGained(playerPartyPanel);
        }
        else
        {
            // enemy cannot fight anymore
            // verify if enemy has flee from battle
            if (enemyPartyPanel.HasEscapedBattle())
            {
                // enemy has escaped battle
                // On exit: move it 2 tiles away from other party
                exitButton.SetExitOption(BattleExit.ExitOption.FleeEnemy);
            }
            else
            {
                // player lost battle and was destroyed
                // On exit: destroy enemy party
                // Destroy(enemyPartyPanel.transform.parent.gameObject);
                exitButton.SetExitOption(BattleExit.ExitOption.DestroyEnemy);
            }
            // Show how much experience was earned by player party
            playerPartyPanel.GrantAndShowExperienceGained(enemyPartyPanel);
        }
        // Remove all buffs and debuffs
        enemyPartyPanel.RemoveAllBuffsAndDebuffs();
        playerPartyPanel.RemoveAllBuffsAndDebuffs();
    }

    void ResetHasMovedFlag()
    {
        // Reset hasMoved flag on all units, so they can now move again;
        playerPartyPanel.ResetHasMovedFlag();
        enemyPartyPanel.ResetHasMovedFlag();
    }

    void SetTurnPhase(TurnPhase newPhase)
    {
        // Todo: Consider just unsing bool, if there will be no other phases
        // act based on the previous phase value
        switch (turnPhase)
        {
            case TurnPhase.Main:
                switch (newPhase)
                {
                    case TurnPhase.Main:
                        // this can happen on the start of the battle
                        // Debug.LogError("Phase is already " + newPhase.ToString());
                        break;
                    case TurnPhase.PostWait:
                        // Deactivate Wait button
                        transform.Find("CtrlPnlFight/Wait").gameObject.SetActive(false);
                        break;
                    default:
                        Debug.LogError("Unknown newPhase" + newPhase.ToString());
                        break;
                }
                break;
            case TurnPhase.PostWait:
                switch (newPhase)
                {
                    case TurnPhase.Main:
                        // Enable Wait button again
                        transform.Find("CtrlPnlFight/Wait").gameObject.SetActive(true);
                        break;
                    case TurnPhase.PostWait:
                        Debug.LogError("Phase is already " + newPhase.ToString());
                        break;
                    default:
                        Debug.LogError("Unknown newPhase" + newPhase.ToString());
                        break;
                }
                break;
            default:
                Debug.LogError("Unknown turnPhase" + turnPhase.ToString());
                break;
        }
        // set turn phase
        turnPhase = newPhase;
    }

    bool StartTurn()
    {
        Debug.Log("StartTurn");
        bool canStart = false;
        // Verify if battle has not ended yet, if there are still units which can fight on both sides
        if (CanContinueBattle())
        {
            // Reset hasMoved flag on all units, so they can now move again;
            ResetHasMovedFlag();
            // Reset turn phase to main
            SetTurnPhase(TurnPhase.Main);
            // loop through all units according to their initiative
            // Activate unit with the highest initiative
            canStart = ActivateNextUnit();
        }
        return canStart;
    }

    bool CanContinueBattle()
    {
        bool canContinue = false;
        // verify if there are units in both parties that can fight
        if (playerPartyPanel.CanFight() && enemyPartyPanel.CanFight())
        {
            canContinue = true;
        }
        return canContinue;
    }

    PartyUnit FindNextUnit()
    {
        // Find unit with the highest initiative, which can still move during this turn in battle
        PartyUnit playerNextUnit = playerPartyPanel.GetActiveUnitWithHighestInitiative(turnPhase);
        PartyUnit enemyNextUnit = enemyPartyPanel.GetActiveUnitWithHighestInitiative(turnPhase);
        // verify if player and enemy has more units to move
        if (playerNextUnit && enemyNextUnit)
        {
            // both parties still have units to move
            if (playerNextUnit.GetInitiative() > enemyNextUnit.GetInitiative())
            {
                // player has higher initiative
                return playerNextUnit;
            }
            else if (playerNextUnit.GetInitiative() == enemyNextUnit.GetInitiative())
            {
                // player and enemy has equal initiative
                // randomly choose between player and enemy units
                // Random.value resturns a value between 0 and 1, 
                // so by shifting 0.5 you could also modify the probability of the two numbers.
                if (Random.value < 0.5f)
                {
                    return playerNextUnit;
                }
                else
                {
                    return enemyNextUnit;
                }
            }
            else
            {
                // enemy has higher initiative
                return enemyNextUnit;
            }
        } else
        {
            // some parties do not have units to move
            // find which party has and which does not have
            if (playerNextUnit)
            {
                // player has next unit, return it
                return playerNextUnit;
            } else if (enemyNextUnit)
            {
                // player has next unit, return it
                return enemyNextUnit;
            } else
            {
                // no any pary has units to move
                // verify current phase
                if (TurnPhase.Main == turnPhase)
                {
                    // do the same check but for units, which are in waiting status
                    SetTurnPhase(TurnPhase.PostWait);
                    return FindNextUnit();
                } else
                {
                    // return null, this should initiate new battle turn
                    return null;
                }
            }
        }
    }

    public bool ActivateNextUnit()
    {
        Debug.Log("ActivateNextUnit");
        bool canActivate = false;
        if (CanContinueBattle())
        {
            // find next unit, which can act in the battle
            PartyUnit nextUnit = FindNextUnit();
            // save it for later needs
            activeUnit = nextUnit;
            if (nextUnit)
            {
                // found next unit
                // activate it
                // Highlight it and reset all other highlights
                // first reset all cells do default values
                playerPartyPanel.ResetAllCellsCanBeTargetedStatus();
                enemyPartyPanel.ResetAllCellsCanBeTargetedStatus();
                // Highlight next unit
                // queue.Run(nextUnit.HighlightActiveUnitInBattle(true));
                queue.Run(nextUnit.HighlightActiveUnitInBattle(true));
                // first trigger process buffs and debuffs
                // Then trigger buffs and debuffs before applying highlights
                // Verify if unit has buffs which should be removed, example: defence
                nextUnit.DeactivateExpiredBuffs();
                // Verify if unit has debuffs which should be applied, example: poison
                // Deactivate debuffs which has expired, example: poison duration may last 2 turns
                // This is checked and done after debuff trigger
                nextUnit.TriggerAppliedDebuffs();
                // do not activate it immediately, instead put it to the queue
                // to be activated after other animations are finished
                queue.Run(playerPartyPanel.SetActiveUnitInBattle(nextUnit));
                queue.Run(enemyPartyPanel.SetActiveUnitInBattle(nextUnit));
                canActivate = true;
            }
            else
            {
                // no other units can be activated
                // go the next turn
                canActivate = StartTurn();
            }
        }
        else
        {
            EndBattle();
        }
        return canActivate;
    }

    public PartyUnit GetActiveUnit()
    {
        return activeUnit;
    }

    public CoroutineQueue GetQueue()
    {
        return queue;
    }

    // Update is called once per frame
    //void Update () {
    //}
}
