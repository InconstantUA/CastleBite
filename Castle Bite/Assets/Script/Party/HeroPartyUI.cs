using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPartyUI : MonoBehaviour {
    [SerializeField]
    HeroParty lHeroParty;

    public HeroParty LHeroParty
    {
        get
        {
            return lHeroParty;
        }

        set
        {
            lHeroParty = value;
        }
    }

    void LinkPartyUnitToUI(PartyUnit partyUnit)
    {
        // Get unit slot Transform by unit address
        Transform unitSlotTransform = transform.GetComponentInChildren<PartyPanel>().transform.Find(partyUnit.UnitCellAddress).GetComponentInChildren<UnitSlot>().transform;
        // Get unit canvas template
        GameObject unitCanvasTemplate = transform.root.Find("Templates/UI/UnitCanvas").gameObject;
        // Create new unit canvas in unit slot
        GameObject newUnitCanvasGO = Instantiate(unitCanvasTemplate, unitSlotTransform);
        // link party Unit to canvas
        newUnitCanvasGO.GetComponent<PartyUnitUI>().LPartyUnit = partyUnit;
        // enable new unit canvas
        newUnitCanvasGO.SetActive(true);
    }

    void LinkPartyUnitsToUI()
    {
        foreach(PartyUnit partyUnit in LHeroParty.GetComponentsInChildren<PartyUnit>())
        {
            LinkPartyUnitToUI(partyUnit);
        }
    }

    void OnEnable()
    {
        LinkPartyUnitsToUI();
    }
}
