using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitButton : MonoBehaviour {

    public void ShowUpgradeUnitMenu()
    {
        //// structure: 
        //// 2[Front/Back/Wide]Cell-1UpgradeUnitPanel-PlusButton(this), UnitCanvas-PartyUnit
        //// [Front/Back/Wide]Cell-UnitSlot-UnitCanvas-PartyUnit
        //PartyUnit partyUnit;
        //// check whether we are in battle or in other mode
        //if (transform.parent.parent.GetComponentInChildren<UnitSlot>().GetComponentInChildren<UnitOnBattleMouseHandler>())
        //{
        //    // we are in battle mode
        //    partyUnit = transform.parent.parent.GetComponentInChildren<UnitSlot>().GetComponentInChildren<UnitOnBattleMouseHandler>().GetComponent<PartyUnitUI>().LPartyUnit;
        //}
        //else
        //{
        //    // we are in other mode
        //    partyUnit = transform.parent.parent.GetComponentInChildren<UnitSlot>().GetComponentInChildren<UnitDragHandler>().GetComponent<PartyUnitUI>().LPartyUnit;
        //}
        // structure: 2PartyUnitUI-1UpgradeUnitPanel-PlusButton(this)
        PartyUnitUI partyUnitUI = transform.parent.GetComponentInParent<PartyUnitUI>();
        Debug.Log("ShowUpgradeUnitMenu for " + partyUnitUI.LPartyUnit.name + " unit");
        transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>().ActivateAdvance(partyUnitUI);
    }
}
