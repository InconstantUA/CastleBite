using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEquipmentButton : MonoBehaviour {

    class OriginalState
    {
        public bool[] FocusPanels = new bool[2] { false, false };
        public bool HireHeroPanel;
        public bool HireCommonUnitButtons;
    }

    OriginalState originalState = new OriginalState();

    public void SetRequiredMenusActive(bool doActivate)
    {
        // Get city transform
        // structure: 6City-5HeroParty-4PartyPanel-3Row-2Cell-1UnitEquipmentControl-EquipmentButton(this)
        City city = transform.parent.parent.parent.parent.parent.parent.GetComponent<City>();
        // verify if we are in a city
        if (city)
        {
            // do actions for each HeroParty
            foreach (HeroParty heroParty in city.GetComponentsInChildren<HeroParty>(true))
            {
                // activate/deactivate party panel
                heroParty.GetComponentInChildren<PartyPanel>(true).gameObject.SetActive(doActivate);
            }
            // activate/deactivate city control panel
            city.GetComponentInChildren<CityControlPanel>(true).gameObject.SetActive(doActivate);
            // activate/deactivate ReturnBtn
            city.transform.Find("ReturnBtn").gameObject.SetActive(doActivate);
            // activate/deactivate focus panels
            FocusPanel[] focusPanels = city.GetComponentsInChildren<FocusPanel>(true);
            for (int i = 0; i < focusPanels.Length; i++)
            {
                // verify if focus panel is about to be disabled
                if (!doActivate)
                {
                    // save original state of each focus panel
                    originalState.FocusPanels[i] = focusPanels[i].gameObject.activeSelf;
                    // disable focus panel
                    focusPanels[i].gameObject.SetActive(doActivate);
                }
                else
                {
                    // restore original state of the focus panel (before disable)
                    focusPanels[i].gameObject.SetActive(originalState.FocusPanels[i]);
                }
            }
            // activate/deactivate HireHeroPanel
            if (!doActivate)
            {
                originalState.HireHeroPanel = city.transform.Find("HireHeroPanel").gameObject.activeSelf;
                city.transform.Find("HireHeroPanel").gameObject.SetActive(doActivate);
            }
            else
            {
                city.transform.Find("HireHeroPanel").gameObject.SetActive(originalState.HireHeroPanel);
            }
            // activate/deactivate HireCommonUnitButtons
            if (!doActivate)
            {
                originalState.HireCommonUnitButtons = city.transform.Find("HireCommonUnitButtons").gameObject.activeSelf;
                city.transform.Find("HireCommonUnitButtons").gameObject.SetActive(doActivate);
            }
            else
            {
                city.transform.Find("HireCommonUnitButtons").gameObject.SetActive(originalState.HireCommonUnitButtons);
            }
        }
    }

    public void ShowUnitEquipmentMenu()
    {
        Debug.Log("Show unit equipment menu");
        // activate HeroEquipment menu
        transform.root.Find("MiscUI/HeroEquipment").GetComponent<HeroEquipment>().ActivateAdvance(this);
    }
}
