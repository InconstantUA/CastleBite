using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionSelectionGroup : MonoBehaviour {

    [SerializeField]
    GameObject factionSelectionTemplate;

    int GetSelectedFactionIndex(FactionSelection selectedFaction, FactionSelection[] factions)
    {
        for (int i = 0; i < factions.Length; i++)
        {
            if (selectedFaction.Faction == factions[i].Faction)
            {
                return i;
            }
        }
        Debug.LogError("Error: selected faction is not in the list of all possible factions");
        return 0;
    }

    public void SelectNext()
    {
        // Get currently selected faction
        FactionSelection selectedFaction = gameObject.GetComponentInChildren(typeof(FactionSelection), false) as FactionSelection;
        // get all factions
        FactionSelection[] factions = GetComponentsInChildren<FactionSelection>(true);
        // get currently selected element index
        int selectedFactionIndex = GetSelectedFactionIndex(selectedFaction, factions);
        Debug.Log("Current Faction index: " + selectedFactionIndex);
        // select next faction in a list
        int nextFactionIndex;
        // verify if we have reached last element
        if (selectedFactionIndex == factions.Length - 1)
        {
            // select first element
            nextFactionIndex = 0;
        }
        else
        {
            // increment index
            nextFactionIndex = selectedFactionIndex + 1;
        }
        Debug.Log("Next Faction index: " + nextFactionIndex);
        // deactivate previous selection
        selectedFaction.gameObject.SetActive(false);
        // activate next selection
        factions[nextFactionIndex].gameObject.SetActive(true);
        // Update Heroes selection
        GetComponentInParent<ChooseYourFirstHero>().SetFaction(factions[nextFactionIndex].Faction);
    }

    public void SelectPrevious()
    {
        // select previous faction in a list
        // Get currently selected faction
        FactionSelection selectedFaction = gameObject.GetComponentInChildren(typeof(FactionSelection), false) as FactionSelection;
        // get all factions
        FactionSelection[] factions = GetComponentsInChildren<FactionSelection>(true);
        // get currently selected element index
        int selectedFactionIndex = GetSelectedFactionIndex(selectedFaction, factions);
        Debug.Log("Current Faction index: " + selectedFactionIndex);
        // select next faction in a list
        int previousFactionIndex;
        // verify if we have reached the first (0) element
        if (selectedFactionIndex == 0)
        {
            // select first element
            previousFactionIndex = factions.Length - 1;
        }
        else
        {
            // increment index
            previousFactionIndex = selectedFactionIndex - 1;
        }
        Debug.Log("Previous Faction index: " + previousFactionIndex);
        // deactivate previous selection
        selectedFaction.gameObject.SetActive(false);
        // activate next selection
        factions[previousFactionIndex].gameObject.SetActive(true);
        // Update Heroes selection
        GetComponentInParent<ChooseYourFirstHero>().SetFaction(factions[previousFactionIndex].Faction);
    }

    void Awake () {
        // init "is any faction enabled" flag
        bool isAnyFactionEnabled = false;
        // Create Faction Selection for each faction
        foreach (Faction faction in Enum.GetValues(typeof(Faction)))
        {
            // skip Unknown faction
            if (faction != Faction.Unknown)
            {
                // Create faction selection from template
                FactionSelection factionSelection = Instantiate(factionSelectionTemplate, transform).GetComponent<FactionSelection>();
                // assign it with faction type
                factionSelection.Faction = faction;
                // set text to faction name
                factionSelection.GetComponent<Text>().text = faction.ToString();
                // verify we has not enabled already some faction
                if (!isAnyFactionEnabled)
                {
                    // enable first faction
                    factionSelection.gameObject.SetActive(true);
                    // reset flag
                    isAnyFactionEnabled = true;
                }
            }
        }
    }
	
}
