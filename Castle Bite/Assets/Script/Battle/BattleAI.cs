﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMove
{
    public enum Option
    {
        Defend,
        Wait,
        Flee,
        ApplyPower
    }

    public Option Action { get; set; }

    public UnitSlot TargetUnitSlot { get; set; }

    public BattleMove(Option option, UnitSlot unitSlot = null)
    {
        Action = option;
        TargetUnitSlot = unitSlot;
    }
}

public class BattleAI : MonoBehaviour {

    [SerializeField]
    GameEvent onDefendEvent;
    [SerializeField]
    GameEvent onRetreatEvent;
    [SerializeField]
    GameEvent onWaitEvent;

    BattleScreen battleScreen;

    void Awake()
    {
        battleScreen = GetComponent<BattleScreen>();
    }

    List<BattleMove> GetAllMoves()
    {
        // init list of battle move options
        // it is always possible to defend, that why we will predefine this option during initialization
        List<BattleMove> battleMoves = new List<BattleMove> { new BattleMove(BattleMove.Option.Defend) };
        // verify if it is possible to flee
        // this only possible if hero party was on map
        if (battleScreen.ActiveUnitUI.LPartyUnit.GetUnitParty().CanEscapeFromBattle)
        {
            // add flee option
            battleMoves.Add(new BattleMove(BattleMove.Option.Flee));
        }
        // verify if it is possible to wait
        // this is only possible if hero did not wait yet = TurnPhase does not equal PostWait
        if (battleScreen.BattleTurnPhase!= BattleTurnPhase.PostWait)
        {
            // add wait option
            battleMoves.Add(new BattleMove(BattleMove.Option.Wait));
        }
        // get all attack targets options
        //  for damaging powers
        List<UnitSlot> powerApplyTargets = battleScreen.GetEnemyPartyPanel().GetAllPowerTargetableUnitSlots();
        foreach (UnitSlot unitSlot in powerApplyTargets)
        {
            battleMoves.Add(new BattleMove(BattleMove.Option.ApplyPower, unitSlot));
        }
        powerApplyTargets = battleScreen.GetPlayerPartyPanel().GetAllPowerTargetableUnitSlots();
        //  for buffing and healing powers
        foreach (UnitSlot unitSlot in powerApplyTargets)
        {
            battleMoves.Add(new BattleMove(BattleMove.Option.ApplyPower, unitSlot));
        }
        return battleMoves;
    }

    BattleMove GetBestMove(List<BattleMove> battleMoves)
    {
        // Draft:
        // Give priority to power actions
        //  return first power action
        foreach (BattleMove battleMove in battleMoves)
        {
            if ((battleMove.Action == BattleMove.Option.ApplyPower) && (battleMove.TargetUnitSlot != null))
            {
                return battleMove;
            }
        }
        // Do not flee or wait for now
        // ..
        // It always possible to defend, so return it as last option
        return new BattleMove(BattleMove.Option.Defend);
    }

    void ExecuteMove(BattleMove battleMove)
    {
        Debug.Log("Execute move");
        // execute based on the move action
        switch (battleMove.Action)
        {
            case BattleMove.Option.ApplyPower:
                // simulate click on a unit slot
                battleMove.TargetUnitSlot.OnLeftMouseClick();
                break;
            case BattleMove.Option.Defend:
                // simulate button click
                // transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleDefend>().ActOnClick();
                // trigger Event
                onDefendEvent.Raise();
                break;
            case BattleMove.Option.Flee:
                // simulate button click
                // transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleRetreat>().ActOnClick();
                // trigger Event
                onRetreatEvent.Raise();
                break;
            case BattleMove.Option.Wait:
                // simulate button click
                // transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleWait>().ActOnClick();
                // trigger Event
                onWaitEvent.Raise();
                break;
            default:
                Debug.LogError("Unknown battle move option");
                break;
        }
    }

    public IEnumerator Act()
    {
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.SetActive(true);
        // Wait for animation
        yield return new WaitForSeconds(0.25f);
        // Get all possible moves
        List<BattleMove> battleMoves = GetAllMoves();
        // Display all moves
        foreach(BattleMove battleMove in battleMoves)
        {
            if (battleMove.TargetUnitSlot != null)
            {
                Debug.Log("Move: " + battleMove.Action.ToString() + " " + battleMove.TargetUnitSlot.GetUnit().UnitName);
            }
            else
            {
                Debug.Log("Move: " + battleMove.Action.ToString());
            }
        }
        // Find best move
        BattleMove bestMove = GetBestMove(battleMoves);
        if (bestMove.TargetUnitSlot != null)
        {
            Debug.Log("Best move: " + bestMove.Action.ToString() + " " + bestMove.TargetUnitSlot.GetUnit().UnitName);
        }
        else
        {
            Debug.Log("Best move: " + bestMove.Action.ToString());
        }
        // Execute best move
        ExecuteMove(bestMove);
        // Wait for animation
        yield return new WaitForSeconds(0.25f);
        // Unblock mouse input
        InputBlocker.SetActive(false);
    }
}
