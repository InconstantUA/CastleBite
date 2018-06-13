using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScreen : MonoBehaviour {
    FocusPanel leftFocusPanel;
    FocusPanel rightFocusPanel;

    PartyPanel playerPartyPanel;
    PartyPanel enemyPartyPanel;

    PartyUnit activeUnit;

    // Use this for initialization
    void Awake () {
        // initialize internal "static" (non-changeable) resources
        leftFocusPanel = transform.Find("LeftFocus").GetComponent<FocusPanel>();
        rightFocusPanel = transform.Find("RightFocus").GetComponent<FocusPanel>();
    }
	
    public void EnterBattle(MapHero playerOnMap, MapHero enemyOnMap)
    {
        // activate this battle sreen
        gameObject.SetActive(true);
        // get hero's parties
        HeroParty playerHeroParty = playerOnMap.linkedPartyTr.GetComponent<HeroParty>();
        HeroParty enemyHeroParty = enemyOnMap.linkedPartyTr.GetComponent<HeroParty>();
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
    }

    void EndBattle()
    {
        // Show who win battle
        // ..
        // activate hero edit click and drag handler
        playerPartyPanel.SetOnEditClickHandler(true);
        enemyPartyPanel.SetOnEditClickHandler(true);
        // deactivate battle click handler, which will react on clicks
        playerPartyPanel.SetOnBattleClickHandler(false);
        enemyPartyPanel.SetOnBattleClickHandler(false);
        // Destroy party which lost, unless it has flee from battle
        //  if it has flee, then move it 1-2 tiles avay from winning hero
        Debug.Log("EndBattle");
        // Activate exit battle button;
    }

    void ResetHasMovedFlag()
    {
        // Reset hasMoved flag on all units, so they can now move again;
        playerPartyPanel.ResetHasMovedFlag();
        enemyPartyPanel.ResetHasMovedFlag();
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
        PartyUnit playerNextUnit = playerPartyPanel.GetActiveUnitWithHighestInitiative();
        PartyUnit enemyNextUnit = enemyPartyPanel.GetActiveUnitWithHighestInitiative();
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
                // return null, this should initiate new battle turn
                return null;
            }
        }
    }

    public bool ActivateNextUnit()
    {
        Debug.Log("ActivateNextUnit");
        bool canActivate = false;
        // find next unit, which can act in the battle
        PartyUnit nextUnit = FindNextUnit();
        // save it for later needs
        activeUnit = nextUnit;
        if (nextUnit)
        {
            // found next unit
            // activate it
            playerPartyPanel.SetActiveUnitInBattle(nextUnit);
            enemyPartyPanel.SetActiveUnitInBattle(nextUnit);
            canActivate = true;
        } else
        {
            // no other units can be activated
            // go the next turn
            canActivate = StartTurn();
        }
        return canActivate;
    }

    public PartyUnit GetActiveUnit()
    {
        return activeUnit;
    }

    // Update is called once per frame
    //void Update () {
    //}
}
