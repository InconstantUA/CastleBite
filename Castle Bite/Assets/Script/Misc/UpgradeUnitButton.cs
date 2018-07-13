using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitButton : MonoBehaviour {

    public void ShowUpgradeUnitMenu()
    {
        // structure: 2UnitCanvas-1UpgradeUnitPanel-PlusButton(this), UnitCanvas-PartyUnit
        PartyUnit partyUnit = transform.parent.parent.GetComponentInChildren<PartyUnit>();
        Debug.Log("ShowUpgradeUnitMenu for " + partyUnit.name + " unit");
        transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>().ActivateAdvance(partyUnit);
    }
}
