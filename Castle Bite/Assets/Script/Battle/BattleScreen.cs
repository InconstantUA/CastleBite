using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleTurnPhase
{
    Main,       // moving all units
    PostWait    // moving units which activated Wait before hand
};

public class BattleScreen : MonoBehaviour {
    [SerializeField]
    GameEvent battleHasStarted;
    [SerializeField]
    GameEvent battleHasEnded;
    [SerializeField]
    GameEvent battleEnterMainPhase;
    [SerializeField]
    GameEvent battleEnterPostWaitPhase;
    [SerializeField]
    GameEvent onActiveUnitChange;
    [SerializeField]
    GameEvent battleScreenEnable;
    [SerializeField]
    GameEvent battleScreenDisable;

    //[SerializeField]
    //MapManager mapManager;
    PartyPanel playerPartyPanel;
    PartyPanel enemyPartyPanel;

    // save active unit parent, because active unit may change
    Transform activeUnitParent;
    // bool canActivate = false; // if it is possible to activate next unit
    public bool BattleHasEnded { get; private set; }

    CoroutineQueue queue;

    BattleAI battleAI;

    BattleTurnPhase battleTurnPhase;

    public enum ExitOption { FleePlayer, FleeEnemy, DestroyPlayer, DestroyEnemy, EnterCity };
    ExitOption exitOption;

    public PartyUnitUI ActiveUnitUI
    {
        get
        {
            return activeUnitParent.GetComponent<PartyUnitUI>();
        }

        set
        {
            activeUnitParent = value.transform;
            // trigger event on active unit change
            onActiveUnitChange.Raise(value.LPartyUnit.gameObject);
        }
    }

    public CoroutineQueue Queue
    {
        get
        {
            // verify if queue is not initiated == null
            if (queue == null)
            {
                // Create a coroutine queue that can run max 1 coroutine at once
                queue = new CoroutineQueue(1, StartCoroutine);
            }
            return queue;
        }
    }

    public BattleTurnPhase BattleTurnPhase
    {
        get
        {
            return battleTurnPhase;
        }

        private set
        {
            //// Todo: Consider just unsing bool, if there will be no other phases
            //// act based on the previous (current) phase value
            //switch (turnPhase)
            //{
            //    case TurnPhase.Main:
            //        switch (newPhase)
            //        {
            //            case TurnPhase.Main:
            //                // this can happen on the start of the battle
            //                // Debug.LogError("Phase is already " + newPhase.ToString());
            //                break;
            //            case TurnPhase.PostWait:
            //                // Deactivate Wait button
            //                SetBattleControlButtonActive("Wait", false);
            //                break;
            //            default:
            //                Debug.LogError("Unknown newPhase" + newPhase.ToString());
            //                break;
            //        }
            //        break;
            //    case TurnPhase.PostWait:
            //        switch (newPhase)
            //        {
            //            case TurnPhase.Main:
            //                // Enable Wait button again
            //                SetBattleControlButtonActive("Wait", true);
            //                break;
            //            case TurnPhase.PostWait:
            //                Debug.LogError("Phase is already " + newPhase.ToString());
            //                break;
            //            default:
            //                Debug.LogError("Unknown newPhase" + newPhase.ToString());
            //                break;
            //        }
            //        break;
            //    default:
            //        Debug.LogError("Unknown turnPhase" + turnPhase.ToString());
            //        break;
            //}
            switch (value)
            {
                case BattleTurnPhase.Main:
                    // Trigger enter main turn phase event
                    battleEnterMainPhase.Raise();
                    break;
                case BattleTurnPhase.PostWait:
                    // Trigger enter post-wait phase event
                    battleEnterPostWaitPhase.Raise();
                    break;
                default:
                    Debug.LogError("Unknown newPhase" + value.ToString());
                    break;
            }
            // set turn phase
            battleTurnPhase = value;
        }
    }

    public PartyPanel GetPlayerPartyPanel()
    {
        return playerPartyPanel;
    }

    public PartyPanel GetEnemyPartyPanel()
    {
        return enemyPartyPanel;
    }

    // Use this for initialization
    void Awake() {
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

    void OnDisable()
    {
        // trigger event
        battleScreenDisable.Raise();
        SetCommonBattleUIActive(false);
    }

    //void SetBattleControlButtonActive(string buttonName, bool doActivate)
    //{
    //    transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/" + buttonName).gameObject.SetActive(doActivate);
    //}

    //void SetBattleControlsActive(bool doActivate)
    //{
    //    // SetBattleControlButtonActive("Defend", doActivate);
    //    SetBattleControlButtonActive("Wait", doActivate);
    //    SetBattleControlButtonActive("Retreat", doActivate);
    //    SetBattleControlButtonActive("AutoBattle", doActivate);
    //    SetBattleControlButtonActive("InstantResolve", doActivate);
    //    // transform.root.Find("MiscUI/BottomControlPanel/MiddleControls").gameObject.SetActive(doActivate);
    //}

    //void SetBattleExitButtonActive(bool doActivate)
    //{
    //    transform.root.Find("MiscUI/BottomControlPanel/RightControls/BattleExit").gameObject.SetActive(doActivate);
    //}

    //BattleExit GetBattleExitButton()
    //{
    //    return transform.root.Find("MiscUI/BottomControlPanel/RightControls/BattleExit").GetComponent<BattleExit>();
    //}

    void SetCommonBattleUIActive(bool doActivate)
    {
        // Activate/Deactivate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(doActivate);
        // Activate/Deactivate Hero parties UIs
        transform.root.Find("MiscUI/LeftHeroParty").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/RightHeroParty").gameObject.SetActive(doActivate);
        // Deactivate/Activate Hero parties Inventory
        transform.root.Find("MiscUI/LeftHeroParty/PartyInventory").gameObject.SetActive(!doActivate);
        transform.root.Find("MiscUI/RightHeroParty/PartyInventory").gameObject.SetActive(!doActivate);
        // Activate/Deactivate Focus panels
        transform.root.Find("MiscUI/LeftFocus").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/RightFocus").gameObject.SetActive(doActivate);
        // Activate/Deactivate city controls
        //transform.root.Find("MiscUI/BottomControlPanel").gameObject.SetActive(doActivate);
        // SetBattleControlsActive(doActivate);
        //SetBattleControlButtonActive("Defend", doActivate);
        //SetBattleControlButtonActive("Wait", doActivate);
        //SetBattleControlButtonActive("Retreat", doActivate);
        //SetBattleControlButtonActive("AutoBattle", doActivate);
        //SetBattleControlButtonActive("InstantResolve", doActivate);
        // Always Deactivate Exit button
        //SetBattleExitButtonActive(false);
    }

    void EnterBattleCommon(HeroParty playerHeroParty, HeroParty enemyHeroParty)
    {
        // activate this battle sreen
        gameObject.SetActive(true);
        // get Left Hero Party UI
        HeroPartyUI playerHeroPartyUI = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>();
        // link Player to the Left UI
        playerHeroPartyUI.LHeroParty = playerHeroParty;
        // get Right Hero Party UI
        HeroPartyUI enemyHeroPartyUI = transform.root.Find("MiscUI/RightHeroParty").GetComponent<HeroPartyUI>();
        // link Enemy to the Right UI
        enemyHeroPartyUI.LHeroParty = enemyHeroParty;
        // Record original poisitions
        playerHeroParty.PreBattleParentTr = playerHeroParty.transform.parent;
        enemyHeroParty.PreBattleParentTr = enemyHeroParty.transform.parent;
        // set if party can escape based on its original position
        playerHeroParty.CanEscapeFromBattle = CanEscape(playerHeroParty.PreBattleParentTr);
        enemyHeroParty.CanEscapeFromBattle = CanEscape(enemyHeroParty.PreBattleParentTr);
        // Activate Hero parties UIs upfront so that PartyLeaders UIs are created
        playerHeroPartyUI.gameObject.SetActive(true);
        enemyHeroPartyUI.gameObject.SetActive(true);
        // assign party leader to the left focus panel
        transform.root.Find("MiscUI/LeftFocus").GetComponent<FocusPanel>().focusedObject = playerHeroPartyUI.GetPartyLeaderUI().gameObject;
        // assign party leader to the right focus panel
        transform.root.Find("MiscUI/RightFocus").GetComponent<FocusPanel>().focusedObject = enemyHeroPartyUI.GetPartyLeaderUI().gameObject;
        // trigger battle screen enable event
        battleScreenEnable.Raise();
        // Activate all needed UI at once
        SetCommonBattleUIActive(true);
        // Get parties panels
        playerPartyPanel = playerHeroPartyUI.GetComponentInChildren<PartyPanel>();
        enemyPartyPanel = enemyHeroPartyUI.GetComponentInChildren<PartyPanel>();
        // Set if parties panels are AI or player controllable
        // .. do it automatically in future, based on ...
        playerPartyPanel.IsAIControlled = false;
        enemyPartyPanel.IsAIControlled = false;
        // start turn based battle
        StartBattle();
    }

    public void EnterBattle(MapHero playerOnMap, MapHero enemyOnMap)
    {
        // get hero's parties
        HeroParty playerHeroParty = playerOnMap.LHeroParty;
        HeroParty enemyHeroParty = enemyOnMap.LHeroParty;
        EnterBattleCommon(playerHeroParty, enemyHeroParty);
    }

    HeroParty GetPartyInCityWhichCanFight(MapCity enemyCityOnMap)
    {
        // Firt verify if city is protected by Hero's party
        // Player cannot attack city untill there a party protecting it
        HeroParty enemyHeroParty = enemyCityOnMap.LCity.GetHeroPartyByMode(PartyMode.Party);
        if (enemyHeroParty)
        {
            // enemy hero is protecting city
            // battle place is on a city gates
            // verify if there are units which can fight, because it is possible that player left only dead units in party
            if (enemyHeroParty.HasUnitsWhichCanFight())
            {
                return enemyHeroParty;
            }
            else
            {
                Debug.LogWarning("Party was found, but there is no unit which can fight. Checking city Garnizon.");
            }
        }
        // if we are here, then there is no enemy hero protecting city
        // get garnizon party
        // there should be garnizon in every city
        enemyHeroParty = enemyCityOnMap.LCity.GetHeroPartyByMode(PartyMode.Garnizon);
        // verify if there are units in party protecting this city, which can fight
        // it is possible that city is not protected, because all units are dead
        if (enemyHeroParty.HasUnitsWhichCanFight())
        {
            return enemyHeroParty;
        }
        else
        {
            Debug.LogWarning("There is no unit in city Garnizon which can fight");
        }
        // if no party can fight, then return null
        return null;
    }

    public void EnterBattle(MapHero playerOnMap, MapCity enemyCityOnMap)
    {
        // gen enemy party, which can fight
        HeroParty enemyHeroParty = GetPartyInCityWhichCanFight(enemyCityOnMap);
        // verify if there are units in party protecting this city, which can fight
        // it is possible that city is not protected
        if (enemyHeroParty != null)
        {
            // city is protected
            // get hero's parties
            HeroParty playerHeroParty = playerOnMap.LHeroParty;
            // proceed with preparations for the battle
            EnterBattleCommon(playerHeroParty, enemyHeroParty);
        }
        else
        {
            // city is not protected
            // no need to battle
            // move to and enter city
            CaptureAndEnterCity();
        }
    }

    void StartBattle()
    {
        //Debug.Log("StartBattle");
        // set battle has ended to false
        BattleHasEnded = false;
        // trigger start battle event
        battleHasStarted.Raise();
        // Start turn
        StartTurn();
        //// do battle until some party wins or other party flee
        //if (!StartTurn())
        //{
        //    // it is not possible to start new turn
        //    Queue.Run(EndBattle());
        //}
    }

    //MapManager GetMapManager()
    //{
    //    return transform.root.Find("MapScreen/Map").GetComponent<MapManager>();
    //}

    void DefaultOnBattleExit()
    {
        //// if hero is still alive, then set click handler to edit mode
        //if (GetBattleExitButton().GetExitOption() != BattleExit.ExitOption.DestroyPlayer)
        //{
        //    // activate hero edit click and drag handler
        //    playerPartyPanel.SetOnEditClickHandler(true);
        //    // deactivate battle click handler, which will react on clicks
        //    playerPartyPanel.SetOnBattleClickHandler(false);
        //}
        // Activate other required screen based on the original parties location
        // Always start with map screen, no matter where battle took place
        // Enable map screen
        MapMenuManager.Instance.gameObject.SetActive(true);
        // Activate map
        MapManager.Instance.gameObject.SetActive(true);
        // Change map mode to browse
        //MapManager mapManager = GetMapManager();
        MapManager.Instance.SetMode(MapManager.Mode.Browse);
        // Move heroes parties to thier initial positions before battle
        // Verify if player party is not destroyed
        //if (playerPartyPanel)
        //{
        //    playerPartyPanel.GetHeroParty().transform.SetParent(playerPartyPanel.GetHeroParty().PreBattleParentTr);
        //}
        // Verify if enemy party is not destroyed
        //if (enemyPartyPanel)
        //{
        //    enemyPartyPanel.GetHeroParty().transform.SetParent(enemyPartyPanel.GetHeroParty().PreBattleParentTr);
        //}
        // Reset almost all units statuses and info for panels, if they are still present
        // Exceptions: Dead
        // Verify if player party is not destroyed
        if (playerPartyPanel)
        {
            playerPartyPanel.ResetUnitCellInfoPanel();
            playerPartyPanel.ResetUnitCellStatus(new string[] { playerPartyPanel.deadStatusText });
            playerPartyPanel.ResetUnitCellHighlight();
            // set map focus
            MapMenuManager.Instance.GetComponentInChildren<MapFocusPanel>().SetActive(playerPartyPanel.GetHeroParty().LMapHero);
        }
        // Verify if enemy party is not destroyed
        if (enemyPartyPanel)
        {
            enemyPartyPanel.ResetUnitCellInfoPanel();
            enemyPartyPanel.ResetUnitCellStatus(new string[] { enemyPartyPanel.deadStatusText });
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
        MapHero heroOnMapRepresentation = heroParty.LMapHero;
        MapObjectLabel heroOnMapLabel = heroOnMapRepresentation.GetComponent<MapObject>().Label;
        // destroy label
        Destroy(heroOnMapLabel.gameObject);
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

    void FleeXFromY(HeroParty fleeingHeroParty, HeroParty otherHeroParty)
    {
        Debug.Log("Flee");
        // Grant fleeing hero party leader 1 bonus move point
        fleeingHeroParty.GetPartyLeader().MovePointsCurrent += 1;
        // get fleeing party transform on map
        // it is not possible to flee if your party is not on map, that is why there is no additional checks here
        Transform fleeingPartyTransform = fleeingHeroParty.LMapHero.transform;
        // get other party or city transform on map
        Transform oppositeTransform;
        // verify if prebattle parent was city
        if (otherHeroParty.PreBattleParentTr.GetComponent<City>())
        {
            // player was in city or it was city garnizon
            // get city on map transform
            oppositeTransform = otherHeroParty.PreBattleParentTr.GetComponent<City>().LMapCity.transform;
        }
        else
        {
            // player was on map
            // get linked party on map transform
            oppositeTransform = otherHeroParty.LMapHero.transform;
        }
        // give control to map manager to flee
        MapManager.Instance.EscapeBattle(fleeingPartyTransform, oppositeTransform);
    }

    public void FleePlayer()
    {
        // get parties before UI is destoyed
        HeroParty playerHeroParty = playerPartyPanel.GetHeroParty();
        HeroParty enemyHeroParty = enemyPartyPanel.GetHeroParty();
        // exit battle
        DefaultOnBattleExit();
        // start flee animation
        FleeXFromY(playerHeroParty, enemyHeroParty);
    }

    public void FleeEnemy()
    {
        // get parties before UI is destoyed
        HeroParty playerHeroParty = playerPartyPanel.GetHeroParty();
        HeroParty enemyHeroParty = enemyPartyPanel.GetHeroParty();
        // exit battle
        DefaultOnBattleExit();
        // start flee animation
        FleeXFromY(enemyHeroParty, playerHeroParty);
    }

    public void CaptureAndEnterCity()
    {
        Debug.Log("BattleScreen: EnterCity");
        // remove dead units from city garnizon's heroParty
        enemyPartyPanel.transform.parent.GetComponent<HeroPartyUI>().LHeroParty.RemoveDeadPartyUnits();
        // Change city faction to player's faction
        enemyPartyPanel.GetCity().CityCurrentFaction = playerPartyPanel.GetHeroParty().Faction;
        // Prepare variables to be used later
        MapHero mapHero = playerPartyPanel.GetHeroParty().LMapHero;
        MapCity destinationCityOnMap = enemyPartyPanel.GetCity().LMapCity;
        //MapManager mapManager = GetMapManager();
        // execute default on battle exit function
        DefaultOnBattleExit();
        // Trigger map hero move to and enter city
        MapManager.Instance.MapHeroMoveToAndEnterCity(mapHero, destinationCityOnMap);
    }

    IEnumerator EndBattle()
    {
        Debug.Log("EndBattle");
        // set battle has ended
        BattleHasEnded = true;
        battleHasEnded.Raise();
        // Remove highlight from active unit
        ActiveUnitUI.HighlightActiveUnitInBattle(false);
        //// Set exit button variable
        //BattleExit exitButton = GetBattleExitButton();
        //// Activate exit battle button;
        //exitButton.gameObject.SetActive(true);
        // Deactivate all other battle buttons;
        // SetBattleControlsActive(false);
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
                exitOption = ExitOption.FleePlayer;
            }
            else
            {
                Debug.Log("Player lost battle and was destroyed");
                // verify if battle was with city Garnizon:
                // .. this is not needed here because city garnizon cannot initiate fight
                // On exit: destroy player party
                exitOption = ExitOption.DestroyPlayer;
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
                exitOption = ExitOption.FleeEnemy;
            }
            else
            {
                Debug.Log("Enemy lost battle and was destroyed");
                // verify if battle was with city Garnizon:
                if (enemyPartyPanel.GetHeroParty().PartyMode == PartyMode.Garnizon)
                {
                    // On exit: enter city
                    Debug.Log("Enter city on exit");
                    exitOption = ExitOption.EnterCity;
                }
                else if (enemyPartyPanel.GetHeroParty().PartyMode == PartyMode.Party)
                {
                    // On exit: destroy enemy party
                    exitOption = ExitOption.DestroyEnemy;
                }
                else
                {
                    Debug.LogError("Unknown party mode " + enemyPartyPanel.GetHeroParty().PartyMode.ToString());
                }
            }
            // Show how much experience was earned by player party
            playerPartyPanel.GrantAndShowExperienceGained(enemyPartyPanel);
        }
        // Remove all buffs and debuffs
        enemyPartyPanel.RemoveAllBuffsAndDebuffs();
        playerPartyPanel.RemoveAllBuffsAndDebuffs();
        // unblock input
        InputBlocker.SetActive(false);
        yield return null;
    }

    void ResetHasMovedFlag()
    {
        // Reset hasMoved flag on all units, so they can now move again;
        playerPartyPanel.ResetHasMovedFlag();
        enemyPartyPanel.ResetHasMovedFlag();
    }

    //void StartTurn()
    //{
    //    //Debug.Log("StartTurn");
    //    //bool canStart = false;
    //    // Verify if battle has not ended yet, if there are still units which can fight on both sides
    //    if (CanContinueBattle())
    //    {
    //        // Reset hasMoved flag on all units, so they can now move again;
    //        ResetHasMovedFlag();
    //        // Reset turn phase to main
    //        SetTurnPhase(TurnPhase.Main);
    //        // loop through all units according to their initiative
    //        // Activate unit with the highest initiative
    //        ActivateNextUnit();
    //        //canStart = ActivateNextUnit();
    //    }
    //    else
    //    {
    //        Queue.Run(EndBattle());
    //    }
    //    //return canStart;
    //}

    void StartTurn()
    {
        // Reset hasMoved flag on all units, so they can now move again;
        ResetHasMovedFlag();
        // Reset turn phase to main
        BattleTurnPhase = BattleTurnPhase.Main;
        // Activate unit with the highest initiative
        ActivateNextUnit();
    }

    bool CanContinueBattle()
    {
        // verify if there are units in both parties that can fight
        if (playerPartyPanel.CanFight() && enemyPartyPanel.CanFight())
        {
            // can continue battle
            return true;
        }
        // cannot continue battle
        return false;
    }

    PartyUnitUI GetPartyUnitUIWithHighestInitiative(PartyUnitUI partyUnitUI1, PartyUnitUI partyUnitUI2)
    {
        if (partyUnitUI1.LPartyUnit.GetEffectiveInitiative() > partyUnitUI2.LPartyUnit.GetEffectiveInitiative())
        {
            // player has higher initiative
            return partyUnitUI1;
        }
        else if (partyUnitUI1.LPartyUnit.GetEffectiveInitiative() == partyUnitUI2.LPartyUnit.GetEffectiveInitiative())
        {
            // player and enemy has equal initiative
            // randomly choose between player and enemy units
            // Random.value resturns a value between 0 and 1, 
            // so by shifting 0.5 you could also modify the probability of the two numbers.
            if (UnityEngine.Random.value < 0.5f)
            {
                return partyUnitUI1;
            }
            else
            {
                return partyUnitUI2;
            }
        }
        else
        {
            // enemy has higher initiative
            return partyUnitUI2;
        }
    }

    PartyUnitUI FindNextUnit()
    {
        // Find unit with the highest initiative, which can still move during this turn in battle
        PartyUnitUI playerNextUnitUI = playerPartyPanel.GetActiveUnitUIWithHighestInitiativeWhichHasNotMoved(BattleTurnPhase);
        PartyUnitUI enemyNextUnitUI = enemyPartyPanel.GetActiveUnitUIWithHighestInitiativeWhichHasNotMoved(BattleTurnPhase);
        // verify if player and enemy has more units to move
        if (playerNextUnitUI && enemyNextUnitUI)
        {
            // both parties still have units to move
            return GetPartyUnitUIWithHighestInitiative(playerNextUnitUI, enemyNextUnitUI);
        }
        else
        {
            // some parties do not have units to move
            // find which party has and which does not have
            if (playerNextUnitUI)
            {
                // player has next unit, return it
                return playerNextUnitUI;
            } else if (enemyNextUnitUI)
            {
                // player has next unit, return it
                return enemyNextUnitUI;
            } else
            {
                // no any pary has units to move
                // verify current phase
                if (BattleTurnPhase.Main == BattleTurnPhase)
                {
                    // do the same check but for units, which are in waiting status
                    BattleTurnPhase = BattleTurnPhase.PostWait;
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
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        //// Set Queue is active flag
        //queueIsActive = true;
        // Highlight it and reset all other highlights
        // first reset all cells do default values
        playerPartyPanel.ResetAllCellsCanBeTargetedStatus();
        enemyPartyPanel.ResetAllCellsCanBeTargetedStatus();
        // Highlight next unit
        // If unit had waiting status in the past, then reset it back to active
        if (ActiveUnitUI.LPartyUnit.UnitStatus == UnitStatus.Waiting)
        {
            // Change unit status to Active
            ActiveUnitUI.LPartyUnit.UnitStatus = UnitStatus.Active;
        }
        //switch (ActiveUnitUI.LPartyUnit.UnitStatus)
        //{
        //    case UnitStatus.Waiting:
        //        // Change unit status to Active
        //        ActiveUnitUI.LPartyUnit.UnitStatus = UnitStatus.Active;
        //        break;
        //    case UnitStatus.Active:
        //    case UnitStatus.Escaping:
        //        // nothing to do here
        //        break;
        //    case UnitStatus.Dead:
        //    case UnitStatus.Escaped:
        //        Debug.LogError("Unit with [" + ActiveUnitUI.LPartyUnit.UnitStatus + "] status should not get turn.");
        //        break;
        //    default:
        //        Debug.LogError("Unknown unit status " + ActiveUnitUI.LPartyUnit.UnitStatus);
        //        break;
        //}
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
        if ((ActiveUnitUI.LPartyUnit.UnitStatus == UnitStatus.Active)
           || (ActiveUnitUI.LPartyUnit.UnitStatus == UnitStatus.Escaping))
        {
            ActiveUnitUI.HighlightActiveUnitInBattle(true);
            // Trigger buffs and debuffs before applying highlights
            // Verify if unit has buffs which should be removed, example: defense
            // ActiveUnitUI.DeactivateExpiredBuffs();
            // Verify if unit has debuffs which should be applied, example: poison
            // Deactivate debuffs which has expired, example: poison duration may last 2 turns
            // This is checked and done after debuff trigger
            ActiveUnitUI.TriggerAppliedUniquePowerModifiers();
        }
        //yield return null;
    }

    IEnumerator EscapeUnit()
    {
        yield return new WaitForSeconds(0.25f);
        ActiveUnitUI.LPartyUnit.HasMoved = true;
        ActiveUnitUI.LPartyUnit.UnitStatus = UnitStatus.Escaped;
        // This unit can't act any more
        // Skip post-move actions and Activate next unit
        // canActivate = ActivateNextUnit();
        ActivateNextUnit();
        yield return new WaitForSeconds(0.25f);
    }

    public void SetHighlight()
    {
        CoroutineQueueManager.Run(playerPartyPanel.SetActiveUnitInBattle(ActiveUnitUI));
        CoroutineQueueManager.Run(enemyPartyPanel.SetActiveUnitInBattle(ActiveUnitUI));
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
        UnitStatus unitStatus = ActiveUnitUI.LPartyUnit.UnitStatus;
        switch (unitStatus)
        {
            case UnitStatus.Active:
                // Activate highlights of which cells can or cannot be targeted
                SetHighlight();
                // verify if active unit's party panel is AI controlled => faction not equal to player's faction
                if (ActiveUnitUI.GetUnitPartyPanel().IsAIControlled)
                {
                    // give control to battle AI
                    CoroutineQueueManager.Run(battleAI.Act());
                }
                else
                {
                    // wait for user to act
                }
                // canActivate = true;
                break;
            case UnitStatus.Escaping:
                // If there were debuffs applied and unit has survived,
                // then unit may escape now
                // Escape unit
                CoroutineQueueManager.Run(EscapeUnit());
                break;
            case UnitStatus.Dead:
                // This unit can't act any more
                // This can happen here due to applied debuffs
                // Skip post-move actions and Activate next unit
                // canActivate = ActivateNextUnit();
                ActivateNextUnit();
                break;
            case UnitStatus.Waiting:
            case UnitStatus.Escaped:
                Debug.LogError("This status [" + unitStatus.ToString() + "] should not be here.");
                break;
            default:
                Debug.LogError("Unknown unit status " + unitStatus.ToString());
                break;
        }
        // Unblock mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.SetActive(false);
        Debug.Log("Unit has been activated");
        yield return null;
    }

    void UpdateBattleControlPanelAccordingToUnitPossibilities()
    {
        // replaced with event
        //verify if party can flee
        //if (ActiveUnitUI.LPartyUnit.GetUnitParty().CanEscapeFromBattle)
        //{
        //    // activate flee button
        //    SetBattleControlButtonActive("Retreat", true);
        //}
        //else
        //{
        //    // deactivate flee button
        //    SetBattleControlButtonActive("Retreat", false);
        //}
        // reset all party inventory panels to disabled state
        transform.root.Find("MiscUI/LeftHeroParty/PartyInventory").gameObject.SetActive(false);
        transform.root.Find("MiscUI/RightHeroParty/PartyInventory").gameObject.SetActive(false);
        // verify if active unit is party leader
        if (ActiveUnitUI.LPartyUnit.IsLeader)
        {
            // activate party leader battle inventory
            ActiveUnitUI.GetComponentInParent<HeroPartyUI>().transform.Find("PartyInventory").gameObject.SetActive(true);
        }
    }

    IEnumerator SetNextUnitActive(PartyUnitUI nextUnitUI)
    {
        // save it for later needs
        ActiveUnitUI = nextUnitUI;
        UpdateBattleControlPanelAccordingToUnitPossibilities();
        ExecutePreActivateActions();
        ProcessBuffsAndDebuffs();
        CoroutineQueueManager.Run(ActivateUnit());
        yield return null;
    }

    //public bool ActivateNextUnit()
    //{
    //    //Debug.Log("ActivateNextUnit");
    //    canActivate = false;
    //    if (CanContinueBattle())
    //    {
    //        // find next unit, which can act in the battle
    //        PartyUnitUI nextUnitUI = FindNextUnit();
    //        //Debug.Log("Next unit is " + nextUnit);
    //        if (nextUnitUI)
    //        {
    //            // found next unit
    //            // save it for later needs
    //            ActiveUnitUI = nextUnitUI;
    //            // activate it
    //            Queue.Run(NextUnit());
    //            canActivate = true;
    //        }
    //        else
    //        {
    //            // no other units can be activated
    //            // go the next turn
    //            canActivate = StartTurn();
    //        }
    //    }
    //    else
    //    {
    //        Queue.Run(EndBattle());
    //    }
    //    return canActivate;
    //}

    public void ActivateNextUnit()
    {
        //Debug.Log("ActivateNextUnit");
        if (CanContinueBattle())
        {
            // find next unit, which can act in the battle
            PartyUnitUI nextUnitUI = FindNextUnit();
            //Debug.Log("Next unit is " + nextUnit);
            if (nextUnitUI)
            {
                // found next unit
                // activate it
                CoroutineQueueManager.Run(SetNextUnitActive(nextUnitUI));
            }
            else
            {
                // no other units can be activated
                // start next turn
                StartTurn();
            }
        }
        else
        {
            CoroutineQueueManager.Run(EndBattle());
        }
    }

    void Proceed()
    {
        // set unit has moved flag
        ActiveUnitUI.LPartyUnit.HasMoved = true;
        // activate next unit
        ActivateNextUnit();
    }

    // Used in Editor in BattleScreen
    public void OnDefend()
    {
        Debug.Log("OnDefend");
        // Get active unit party panel
        //PartyPanel partyPanel = ActiveUnitUI.GetUnitPartyPanel();
        // Apply defense stance status
        //partyPanel.SetUnitDefenseBuffActive(ActiveUnitUI);
        ActiveUnitUI.SetDefenseBuffActive();
        // proceed with default post-move actions
        Proceed();
    }

    IEnumerator ReatreatingAnimation()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void OnRetreat()
    {
        Debug.Log("OnRetreat");
        // set unit is escaping status
        ActiveUnitUI.LPartyUnit.UnitStatus = UnitStatus.Escaping;
        // execute animation
        CoroutineQueueManager.Run(ReatreatingAnimation());
        // proceed with default post-move actions
        Proceed();
    }

    IEnumerator WaitingAnimation()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void OnWait()
    {
        Debug.Log("OnWait");
        // set unit is waiting status
        ActiveUnitUI.LPartyUnit.UnitStatus = UnitStatus.Waiting;
        // execute wait animation
        CoroutineQueueManager.Run(WaitingAnimation());
        // activate next unit without setting hasMoved flag
        ActivateNextUnit();
    }

    public void OnInstantResolve()
    {
        Debug.Log("OnInstantResolve");
    }

    public void OnAuto()
    {
        Debug.Log("OnAuto");
    }

    public void OnExit()
    {
        Debug.Log("OnExit");
        switch (exitOption)
        {
            case ExitOption.DestroyPlayer:
                DestroyPlayer();
                break;
            case ExitOption.DestroyEnemy:
                DestroyEnemy();
                break;
            case ExitOption.FleePlayer:
                FleePlayer();
                break;
            case ExitOption.FleeEnemy:
                FleeEnemy();
                break;
            case ExitOption.EnterCity:
                CaptureAndEnterCity();
                break;
            default:
                Debug.LogError("Unknown exit option.");
                break;
        }
    }

    public void OnEndItemDrag()
    {
        Debug.LogWarning("OnEndItemDrag");
        // Instruct Battle screen to update units highlight
        SetHighlight();
    }

    public void OnItemHasBeenDroppedIntoEquipmentSlot(System.Object inventorySlotDropHandler)
    {
        // note: this event is handled here, because it should be handled like this only when Battle screen is active
        // take equipment slot parameter from item (in case if new slot has been created or empty slot has been reused)
        ((ItemSlotDropHandler)inventorySlotDropHandler).EquipmentSlot = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CurrentHeroEquipmentSlot;

    }

    public void OnItemHasBeenDroppedIntoInventorySlot(System.Object inventorySlotDropHandler)
    {
        // note: this event is handled here, because it should be handled like this only when Battle screen is active
        // take equipment slot parameter from item (in case if new slot has been created or empty slot has been reused)
        ((ItemSlotDropHandler)inventorySlotDropHandler).EquipmentSlot = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.CurrentHeroEquipmentSlot;
    }

    public void PartyInventoryUIHasBeenEnabledEvent(System.Object partyInventoryUI)
    {
        // verify if object type is correct
        if (partyInventoryUI is PartyInventoryUI)
        {
            // display party inventory
            ((PartyInventoryUI)partyInventoryUI).DisplayHeroEquipmentUsableInventory();
        }
    }
}
