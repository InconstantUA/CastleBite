using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScreen : MonoBehaviour {
    FocusPanel leftFocusPanel;
    FocusPanel rightFocusPanel;

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
        // Get parties leaders
        PartyUnit playerPartyLeader = playerHeroParty.GetComponentInChildren<PartyPanel>().GetPartyLeader();
        PartyUnit enemyPartyLeader = enemyHeroParty.GetComponentInChildren<PartyPanel>().GetPartyLeader();
        // Link parties leaders to the focus panels
        leftFocusPanel.focusedObject = playerPartyLeader.gameObject;
        rightFocusPanel.focusedObject = enemyPartyLeader.gameObject;
        // Initialize focus panel with information from linked leaders
        leftFocusPanel.OnChange(FocusPanel.ChangeType.Init);
        rightFocusPanel.OnChange(FocusPanel.ChangeType.Init);
    }

    // Update is called once per frame
    //void Update () {
    //}
}
