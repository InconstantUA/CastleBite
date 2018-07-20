﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScreen : MonoBehaviour {
    FocusPanel leftFocusPanel;
    FocusPanel rightFocusPanel;

    PartyPanel playerPartyPanel;
    PartyPanel enemyPartyPanel;

    // save active unit parent, because active unit may change
    Transform activeUnitParent;

    bool battleHasEnded;
    bool canActivate = false; // if it is possible to activate next unit
    //bool queueIsActive;

    CoroutineQueue queue;

    BattleAI battleAI;

    public enum TurnPhase
    {
        Main,       // moving all units
        PostWait    // moving units which activated Wait before hand
    };
    TurnPhase turnPhase;

    public PartyUnit ActiveUnit
    {
        get
        {
            return activeUnitParent.GetComponentInChildren<PartyUnit>();
        }

        set
        {
            activeUnitParent = value.transform.parent;
        }
    }

    public TurnPhase GetTurnPhase()
    {
        return turnPhase;
    }

    public PartyPanel GetPlayerPartyPanel()
    {
        return playerPartyPanel;
    }

    public PartyPanel GetEnemyPartyPanel()
    {
        return enemyPartyPanel;
    }

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
        battleAI = GetComponent<BattleAI>();
    }

    //BattlePlace GetBattlePlace(HeroParty playerHeroParty, HeroParty enemyHeroParty)
    //{
    //    // Return battle place based on:
    //    //  - position of units before battle start
    //    //  - who is initiating battle
    //    // ..
    //    return BattlePlace.Map;
    //}

    bool CanEscape(Transform parentLocation)
    {
        Debug.LogWarning("Location " + parentLocation.name);
        if (parentLocation.GetComponent<City>())
        {
            // if party was in city, then it cannot escape
            return false;
        }
        else
        {
            // other options:
            //  - party was on map
            // can escape
            return true;
        }
    }

    void EnterBattleCommon(HeroParty playerHeroParty, HeroParty enemyHeroParty)
    {
        // activate this battle sreen
        gameObject.SetActive(true);
        // Set turn phase to main phase
        SetTurnPhase(TurnPhase.Main);
        // Record original poisitions
        playerHeroParty.PreBattleParentTr = playerHeroParty.transform.parent;
        enemyHeroParty.PreBattleParentTr = enemyHeroParty.transform.parent;
        // set if party can escape based on its original position
        playerHeroParty.CanEscapeFromBattle = CanEscape(playerHeroParty.PreBattleParentTr);
        enemyHeroParty.CanEscapeFromBattle = CanEscape(enemyHeroParty.PreBattleParentTr);
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
        // Set if parties panels are AI or player controllable
        // .. do it automatically in future, based on ...
        playerPartyPanel.IsAIControlled = false;
        enemyPartyPanel.IsAIControlled = false;
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

    public void EnterBattle(MapHero playerOnMap, MapHero enemyOnMap)
    {
        // get hero's parties
        HeroParty playerHeroParty = playerOnMap.LinkedPartyTr.GetComponent<HeroParty>();
        HeroParty enemyHeroParty = enemyOnMap.LinkedPartyTr.GetComponent<HeroParty>();
        EnterBattleCommon(playerHeroParty, enemyHeroParty);
    }

    public void EnterBattle(MapHero playerOnMap, MapCity enemyCityOnMap)
    {
        // get hero's parties
        HeroParty playerHeroParty = playerOnMap.LinkedPartyTr.GetComponent<HeroParty>();
        // Verify if city is protected by Hero's party
        HeroParty enemyHeroParty = enemyCityOnMap.LinkedCityTr.GetComponent<City>().GetHeroPartyByMode(HeroParty.PartyMode.Party);
        if (enemyHeroParty)
        {
            // set battle place on a city gates
        }
        else
        {
            // no enemy hero protecting city
            // get garnizon party
            enemyHeroParty = enemyCityOnMap.LinkedCityTr.GetComponent<City>().GetHeroPartyByMode(HeroParty.PartyMode.Garnizon);
        }
        // verify if there are units in party protecting this city, which can fight
        // it is possible that city is not protected
        if (enemyHeroParty.GetComponentInChildren<PartyPanel>().GetActiveUnitWithHighestInitiative(TurnPhase.Main))
        {
            // city is protected
            // proceed with preparations for the battle
            EnterBattleCommon(playerHeroParty, enemyHeroParty);
        }
        else
        {
            // city is not protected
            // no need to battle
            // move to and enter city
            EnterCity();
        }
    }

    void StartBattle()
    {
        //Debug.Log("StartBattle");
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
            queue.Run(EndBattle());
        }
        // Deactivate exit battle button;
        transform.Find("Exit").gameObject.SetActive(false);
        // Activate all other battle buttons;
        transform.Find("CtrlPnlFight").gameObject.SetActive(true);
    }

    MapManager GetMapManager()
    {
        return transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
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
        // Always start with map screen, no matter where battle took place
        // Enable map screen
        Transform mapScreen = transform.root.Find("MapScreen");
        mapScreen.gameObject.SetActive(true);
        // Change map mode to browse
        MapManager mapManager = GetMapManager();
        mapManager.SetMode(MapManager.Mode.Browse);
        // Move heroes parties to thier initial positions before battle
        // Verify if player party is not destroyed
        if (playerPartyPanel)
        {
            playerPartyPanel.GetHeroParty().transform.SetParent(playerPartyPanel.GetHeroParty().PreBattleParentTr);
        }
        // Verify if enemy party is not destroyed
        if (enemyPartyPanel)
        {
            enemyPartyPanel.GetHeroParty().transform.SetParent(enemyPartyPanel.GetHeroParty().PreBattleParentTr);
        }
        // Reset almost all units statuses and info for panels, if they are still present
        // Exceptions: Dead
        // Verify if player party is not destroyed
        if (playerPartyPanel)
        {
            playerPartyPanel.ResetUnitCellInfoPanel(playerPartyPanel.transform);
            playerPartyPanel.ResetUnitCellStatus(new string[] { playerPartyPanel.deadStatus });
            playerPartyPanel.ResetUnitCellHighlight();
        }
        // Verify if enemy party is not destroyed
        if (enemyPartyPanel)
        {
            enemyPartyPanel.ResetUnitCellInfoPanel(enemyPartyPanel.transform);
            enemyPartyPanel.ResetUnitCellStatus(new string[] { enemyPartyPanel.deadStatus });
            enemyPartyPanel.ResetUnitCellHighlight();
        }
        // Close battle screen
        gameObject.SetActive(false);
    }

    public void DestroyParty(PartyPanel partyPanel)
    {
        Debug.Log("Destroy party");
        // set variables
        HeroParty heroParty = partyPanel.GetHeroParty();
        MapHero heroOnMapRepresentation = heroParty.GetLinkedPartyOnMap();
        // destroy on map party representation
        Destroy(heroOnMapRepresentation.gameObject);
        // destroy party
        Destroy(heroParty.gameObject);
    }

    public void DestroyPlayer()
    {
        DestroyParty(playerPartyPanel);
        DefaultOnBattleExit();
    }

    public void DestroyEnemy()
    {
        DestroyParty(enemyPartyPanel);
        DefaultOnBattleExit();
    }

    void FleeXFromY(PartyPanel fleeingPartyPanel, PartyPanel otherPartyPanel)
    {
        Debug.Log("Flee");
        // get fleeing party transform on map
        // it is not possible to flee if your party is not on map, that is why there is no additional checks here
        Transform fleeingPartyTransform = fleeingPartyPanel.GetHeroParty().GetLinkedPartyOnMap().transform;
        // get other party or city transform on map
        Transform oppositeTransform;
        // verify if prebattle parent was city
        if (otherPartyPanel.GetHeroParty().PreBattleParentTr.GetComponent<City>())
        {
            // player was in city or it was city garnizon
            // get city on map transform
            oppositeTransform = otherPartyPanel.GetHeroParty().PreBattleParentTr.GetComponent<City>().LinkedMapCity.transform;
        }
        else
        {
            // player was on map
            // get linked party on map transform
            oppositeTransform = otherPartyPanel.GetHeroParty().GetLinkedPartyOnMap().transform;
        }
        // give control to map manager to flee
        GetMapManager().EscapeBattle(fleeingPartyTransform, oppositeTransform);
    }

    public void FleePlayer()
    {
        DefaultOnBattleExit();
        FleeXFromY(playerPartyPanel, enemyPartyPanel);
    }

    public void FleeEnemy()
    {
        DefaultOnBattleExit();
        FleeXFromY(enemyPartyPanel, playerPartyPanel);
    }

    public void EnterCity()
    {
        Debug.Log("BattleScreen: EnterCity");
        DefaultOnBattleExit();
        // remove dead units from city garnizon's party panel
        enemyPartyPanel.GetHeroParty().GetComponentInChildren<PartyPanel>().RemoveDeadUnits();
        // Change city faction to player's faction
        enemyPartyPanel.GetCity().SetFaction(playerPartyPanel.GetHeroParty().GetFaction());
        // Trigger map hero move to and enter city
        MapHero mapHero = playerPartyPanel.GetHeroParty().GetLinkedPartyOnMap();
        MapCity destinationCityOnMap = enemyPartyPanel.GetCity().LinkedMapCity;
        MapManager mapManager = GetMapManager();
        mapManager.MapHeroMoveToAndEnterCity(mapHero, destinationCityOnMap);
    }

    IEnumerator EndBattle()
    {
        Debug.Log("EndBattle");
        // set battle has ended
        battleHasEnded = true;
        // Remove highlight from active unit
        ActiveUnit.HighlightActiveUnitInBattle(false);
        //// Clear units info and status information
        //enemyPartyPanel.ResetUnitCellStatus(new string[] { enemyPartyPanel.deadStatus, enemyPartyPanel.levelUpStatus });
        // Set exit button variable
        BattleExit exitButton = transform.Find("Exit").GetComponent<BattleExit>();
        // Activate exit battle button;
        exitButton.gameObject.SetActive(true);
        // Deactivate all other battle buttons;
        transform.Find("CtrlPnlFight").gameObject.SetActive(false);
        // Check who win battle
        if (!playerPartyPanel.CanFight())
        {
            Debug.Log("Player cannot fight anymore");
            // player cannot fight anymore
            // verify if player has flee from battle
            if (playerPartyPanel.HasEscapedBattle())
            {
                Debug.Log("Player has escaped battle");
                // On exit: move it 2 tiles away from other party
                exitButton.SetExitOption(BattleExit.ExitOption.FleePlayer);
            }
            else
            {
                Debug.Log("Player lost battle and was destroyed");
                // verify if battle was with city Garnizon:
                // .. this is not needed here because city garnizon cannot initiate fight
                // On exit: destroy player party
                exitButton.SetExitOption(BattleExit.ExitOption.DestroyPlayer);
            }
            // Show how much experience was earned by enemy party
            enemyPartyPanel.GrantAndShowExperienceGained(playerPartyPanel);
        }
        else
        {
            Debug.Log("Enemy cannot fight anymore");
            // verify if enemy has flee from battle
            if (enemyPartyPanel.HasEscapedBattle())
            {
                Debug.Log("Enemy has escaped battle");
                // On exit: move it 2 tiles away from other party
                exitButton.SetExitOption(BattleExit.ExitOption.FleeEnemy);
            }
            else
            {
                Debug.Log("Enemy lost battle and was destroyed");
                // verify if battle was with city Garnizon:
                if (enemyPartyPanel.GetHeroParty().GetMode() == HeroParty.PartyMode.Garnizon)
                {
                    // On exit: enter city
                    Debug.Log("Enter city on exit");
                    exitButton.SetExitOption(BattleExit.ExitOption.EnterCity);
                }
                else if (enemyPartyPanel.GetHeroParty().GetMode() == HeroParty.PartyMode.Party)
                {
                    // On exit: destroy enemy party
                    exitButton.SetExitOption(BattleExit.ExitOption.DestroyEnemy);
                }
                else
                {
                    Debug.LogError("Unknown party mode " + enemyPartyPanel.GetHeroParty().GetMode().ToString());
                }
            }
            // Show how much experience was earned by player party
            playerPartyPanel.GrantAndShowExperienceGained(enemyPartyPanel);
        }
        // Remove all buffs and debuffs
        enemyPartyPanel.RemoveAllBuffsAndDebuffs();
        playerPartyPanel.RemoveAllBuffsAndDebuffs();
        yield return null;
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
        //Debug.Log("StartTurn");
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
    
    //IEnumerator EmptyQueueIndicator()
    //{
    //    Debug.Log("Queue is empty");
    //    queueIsActive = false;
    //    yield return null;
    //}

    void ExecutePreActivateActions()
    {
        //Debug.Log("ExecutePreActivateActions");
        // Block mouse input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(true);
        //// Set Queue is active flag
        //queueIsActive = true;
        // Highlight it and reset all other highlights
        // first reset all cells do default values
        playerPartyPanel.ResetAllCellsCanBeTargetedStatus();
        enemyPartyPanel.ResetAllCellsCanBeTargetedStatus();
        // Highlight next unit
        // If unit had waiting status in the past, then reset it back to active
        PartyUnit.UnitStatus unitStatus = ActiveUnit.GetUnitStatus();
        switch (unitStatus)
        {
            case PartyUnit.UnitStatus.Active:
                break;
            case PartyUnit.UnitStatus.Waiting:
                // Activate unit
                ActiveUnit.SetUnitStatus(PartyUnit.UnitStatus.Active);
                break;
            case PartyUnit.UnitStatus.Escaping:
                break;
            case PartyUnit.UnitStatus.Dead:
            case PartyUnit.UnitStatus.Escaped:
                Debug.LogError("This status [" + unitStatus.ToString() + "] should not be here.");
                break;
            default:
                Debug.LogError("Unknown unit status " + unitStatus.ToString());
                break;
        }
        //yield return null;
    }

    void ProcessBuffsAndDebuffs()
    {
        //Debug.Log("ProcessBuffsAndDebuffs");
        //// Wait while all previously triggered actions are complete
        //while (queueIsActive)
        //{
        //    Debug.Log("Queue is active");
        //    yield return new WaitForSeconds(1f);
        //}
        // Set Queue is active flag
        //queueIsActive = true;
        // Next actions are applicably only to active or escaping unit
        if (  (ActiveUnit.GetUnitStatus() == PartyUnit.UnitStatus.Active)
           || (ActiveUnit.GetUnitStatus() == PartyUnit.UnitStatus.Escaping) )
        {
            ActiveUnit.HighlightActiveUnitInBattle(true);
            // Trigger buffs and debuffs before applying highlights
            // Verify if unit has buffs which should be removed, example: defence
            ActiveUnit.DeactivateExpiredBuffs();
            // Verify if unit has debuffs which should be applied, example: poison
            // Deactivate debuffs which has expired, example: poison duration may last 2 turns
            // This is checked and done after debuff trigger
            ActiveUnit.TriggerAppliedDebuffs();
        }
        //yield return null;
    }

    IEnumerator EscapeUnit()
    {
        yield return new WaitForSeconds(0.25f);
        ActiveUnit.SetHasMoved(true);
        ActiveUnit.SetUnitStatus(PartyUnit.UnitStatus.Escaped);
        // This unit can't act any more
        // Skip post-move actions and Activate next unit
        canActivate = ActivateNextUnit();
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator ActivateUnit()
    {
        //Debug.Log("ActivateUnit");
        // Wait while all previously triggered actions are complete
        //while (queueIsActive)
        //{
        //    Debug.Log("Queue is active");
        //    yield return new WaitForSeconds(1f);
        //}
        //// Set Queue is active flag
        //queueIsActive = true;
        PartyUnit.UnitStatus unitStatus = ActiveUnit.GetUnitStatus();
        switch (unitStatus)
        {
            case PartyUnit.UnitStatus.Active:
                // Activate highlights of which cells can or cannot be targeted
                queue.Run(playerPartyPanel.SetActiveUnitInBattle(ActiveUnit));
                queue.Run(enemyPartyPanel.SetActiveUnitInBattle(ActiveUnit));
                // verify if active unit's party panel is AI controlled => faction not equal to player's faction
                if (ActiveUnit.GetUnitPartyPanel().IsAIControlled)
                {
                    // give control to battle AI
                    queue.Run(battleAI.Act());
                }
                else
                {
                    // wait for user to act
                }
                canActivate = true;
                break;
            case PartyUnit.UnitStatus.Escaping:
                // If there were debuffs applied and unit has survived,
                // then unit may escape now
                // Escape unit
                queue.Run(EscapeUnit());
                break;
            case PartyUnit.UnitStatus.Dead:
                // This unit can't act any more
                // Skip post-move actions and Activate next unit
                canActivate = ActivateNextUnit();
                break;
            case PartyUnit.UnitStatus.Waiting:
            case PartyUnit.UnitStatus.Escaped:
                Debug.LogError("This status [" + unitStatus.ToString() + "] should not be here.");
                break;
            default:
                Debug.LogError("Unknown unit status " + unitStatus.ToString());
                break;
        }
        // Unblock mouse input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(false);
        Debug.Log("Unit has been activated");
        yield return null;
    }

    void UpdateBattleControlPanelAccordingToUnitPossibilities()
    {
        // verify if party can flee
        if (ActiveUnit.GetUnitParty().CanEscapeFromBattle)
        {
            // activate flee button
            transform.Find("CtrlPnlFight/Retreat").gameObject.SetActive(true);
        }
        else
        {
            // deactivate flee button
            transform.Find("CtrlPnlFight/Retreat").gameObject.SetActive(false);
        }
    }

    IEnumerator NextUnit()
    {
        UpdateBattleControlPanelAccordingToUnitPossibilities();
        ExecutePreActivateActions();
        ProcessBuffsAndDebuffs();
        queue.Run(ActivateUnit());
        yield return null;
    }

    public bool ActivateNextUnit()
    {
        //Debug.Log("ActivateNextUnit");
        canActivate = false;
        if (CanContinueBattle())
        {
            // find next unit, which can act in the battle
            PartyUnit nextUnit = FindNextUnit();
            //Debug.Log("Next unit is " + nextUnit);
            if (nextUnit)
            {
                // found next unit
                // save it for later needs
                ActiveUnit = nextUnit;
                // activate it
                queue.Run(NextUnit());
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
            queue.Run(EndBattle());
        }
        return canActivate;
    }

    //public PartyUnit GetActiveUnit()
    //{
    //    return ActiveUnit;
    //}

    public CoroutineQueue GetQueue()
    {
        return queue;
    }

    // Update is called once per frame
    //void Update () {
    //}
}
