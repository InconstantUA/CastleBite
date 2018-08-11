using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitButton : MonoBehaviour {

    public void ShowUpgradeUnitMenu()
    {
        // structure: 
        // 2[Front/Back/Wide]Cell-1UpgradeUnitPanel-PlusButton(this), UnitCanvas-PartyUnit
        // [Front/Back/Wide]Cell-UnitSlot-UnitCanvas-PartyUnit
        PartyUnit partyUnit = transform.parent.parent.GetComponentInChildren<UnitSlot>().GetComponentInChildren<UnitDragHandler>().GetComponentInChildren<PartyUnit>();
        Debug.Log("ShowUpgradeUnitMenu for " + partyUnit.name + " unit");
        transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>().ActivateAdvance(partyUnit);
    }
}
