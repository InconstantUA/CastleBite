using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplatesManager : MonoBehaviour {
    public GameObject GetPartyUnitTemplateByType(UnitType unitType)
    {
        // loop through all party units in Templates
        foreach (PartyUnit partyUnit in transform.Find("Obj").GetComponentsInChildren<PartyUnit>(true))
        {
            // verify if party unit type is of required type
            if (unitType == partyUnit.PartyUnitData.unitType)
            {
                return partyUnit.gameObject;
            }
        }
        // if nothing found, then log error and return null
        Debug.LogError("Cannot find Party Unit template matching [" + unitType.ToString() + "] UnitType");
        return null;
    }
}
