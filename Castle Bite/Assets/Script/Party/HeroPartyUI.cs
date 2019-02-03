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
        // Get PartyPanel
        PartyPanel partyPanel = GetComponentInChildren<PartyPanel>(true);
        // Get unit slot Transform by unit address
        Transform unitSlotTransform = partyPanel.transform.Find(partyUnit.UnitPPRow + "/" + partyUnit.UnitPPCell).GetComponentInChildren<UnitSlot>(true).transform;
        // Get unit canvas template
        GameObject unitCanvasTemplate = transform.root.Find("Templates/UI/UnitCanvas").gameObject;
        // Create new unit canvas in unit slot
        PartyUnitUI newUnitCanvas = Instantiate(unitCanvasTemplate, unitSlotTransform).GetComponent<PartyUnitUI>();
        // link party Unit to canvas
        newUnitCanvas.LPartyUnit = partyUnit;
        // enable new unit canvas
        newUnitCanvas.gameObject.SetActive(true);
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
        // Enable PartyPanel
        GetComponentInChildren<PartyPanel>(true).gameObject.SetActive(true);
        // Enable Party Inventory
        GetComponentInChildren<PartyInventoryUI>(true).gameObject.SetActive(true);
    }

    void OnDisable()
    {
        // reset hero party link to null
        LHeroParty = null;
        // Remove all PartyUnitUIs
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>(true))
        {
            Destroy(partyUnitUI.gameObject);
        }
        // Disable PartyPanel
        GetComponentInChildren<PartyPanel>(true).gameObject.SetActive(false);
        // Disable Party Inventory
        GetComponentInChildren<PartyInventoryUI>(true).gameObject.SetActive(false);
    }

    public PartyUnitUI GetPartyLeaderUI()
    {
        foreach (PartyUnitUI partyUnitUI in GetComponentsInChildren<PartyUnitUI>())
        {
            // verify if this is leader
            if (partyUnitUI.LPartyUnit.IsLeader)
            {
                return partyUnitUI;
            }
        }
        return null;
    }

}
