using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {
    public string cityName;
    public string cityDescription;
    public int cityLevel;
    public enum CityType { Normal, Capital };
    public CityType cityType;
    int maxCityLevel = 5;
    public enum CityViewActiveState { Normal, ActiveHeal, ActiveResurect, ActiveDismiss, ActiveHeroEquipment };
    [SerializeField]
    CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;
    public enum CityOccupationState { NoHeroIn, HeroIn };
    [SerializeField]
    CityOccupationState cityOccupationState;
    public enum CityInventoryState { HeroInvOff, HeroInvOn };
    [SerializeField]
    CityInventoryState cityInventoryState;

    // City view state is required to effectively change between different states
    // and do not forget to enable or disable something
    // we need to correct transition to normal state after leaving city view
    // to view it normally again after we enter it
    // and do not have heal/resurect/dismiss states active after we enter city next time
    //
    // For the same purpose we have City inventory state
    // If we leave city, then we should deactivate inventory view
    //
    // The same for city occupation state
    // If hero leaves city, the we should return city state o NoHeroIn
    // and activate hire hero button


    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public int GetDefence()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + bonus;
    }

    public int GetHealPerDay()
    {
        int bonus = 0;
        if (cityType == CityType.Capital)
        {
            bonus = 20;
        }
        return (cityLevel * 5) + 5 + bonus;
    }

    public int GetUnitsCapacity()
    {
        return cityLevel;
    }

    public bool HasCityReachedMaximumLevel()
    {
        bool result = false;
        // note city level for capital is 6, which is higher than for normal city
        if  (cityLevel >= maxCityLevel)
        {
            // city has reached its maximum level
            result = true;
        }
        return result;
    }

    public void SetActiveState(CityViewActiveState requiredState, bool doActivate)
    {
        // if doActivate is true,
        // then we are really swiching to this new state
        // otherwise it means that we are deactivating old state and returning to Normal state
        // this should not happen for Normal state, so here is check, for this
        if  (requiredState == CityViewActiveState.Normal)
        {
            Debug.LogError("Unexpected condition");
        }
        // 1st return previous state to Normal, if it is already not Normal
        // this only needed if we are activating new state, while previous state was not normal
        if ( (cityViewActiveState != CityViewActiveState.Normal) && (requiredState != cityViewActiveState) && (doActivate == true))
        {
            SetActiveState(cityViewActiveState, false);
        }
        // Update 
        cityViewActiveState = requiredState;
        // instruct party and garnizon panels to
        // highlight differently cells with and without units
        // and disable hire buttons
        switch (cityViewActiveState)
        {
            case CityViewActiveState.ActiveDismiss:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                break;
            case CityViewActiveState.ActiveHeal:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                break;
            case CityViewActiveState.ActiveResurect:
                transform.Find("CityGarnizon").GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                break;
            case CityViewActiveState.ActiveHeroEquipment:
                // this is not applicable to garnizon, garnizon does not have hero or eqipment
                // it is only appicable to hero party
                break;
        }
        // if hero party is present,
        // then activate active state highligh
        // if not - then disable hire hero button
        if (transform.GetComponentInChildren<HeroParty>())
        {
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                    break;
                case CityViewActiveState.ActiveHeal:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                    break;
                case CityViewActiveState.ActiveResurect:
                    transform.GetComponentInChildren<HeroParty>().GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                    break;
                case CityViewActiveState.ActiveHeroEquipment:
                    SetActiveHeroEquipment(doActivate);
                    break;
            }
        }
        else
        {
            transform.Find("HireHeroPanel/HireHeroPanelBtn").gameObject.SetActive(!doActivate);
        }
        // Update cursor
        CursorController.Instance.SetCityActiveViewStateCursor(cityViewActiveState, doActivate);
        // if doActivate was false
        // this means that we need to return to normal state.
        // that what we did by passing doActivate variable to other functions
        // so we only need to set here state to normal
        if (false == doActivate)
        {
            cityViewActiveState = CityViewActiveState.Normal;
        }
    }

    void SetActiveHeroEquipment(bool doActivate)
    {
        // Disable Hero equipment menu if it was enabled and enable it otherwise
        // Also disable / enable hero party and city garnizon
        // Structure:   [city]->[HeroParty/CityGarnizon]
        Transform heroParty = transform.GetComponentInChildren<HeroParty>().transform;
        GameObject heroEquipmentMenu = heroParty.Find("HeroEquipment").gameObject;
        GameObject heroUnitsPanel = heroParty.Find("PartyPanel").gameObject;
        GameObject cityGarnizon = transform.Find("CityGarnizon").gameObject;
        heroEquipmentMenu.SetActive(doActivate);
        heroUnitsPanel.SetActive(!doActivate);
        cityGarnizon.SetActive(!doActivate);
    }

    public void ExitCity()
    {
        // return states to normal
        if (CityViewActiveState.Normal != cityViewActiveState)
        {
            // 1st deactivate button which is linked to this state
            // because this is not done by user
            // we should do it ourselves
            // we should do this before deactivating state,
            // because state is reset to normal by SetActiveState
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    // find and deactivate toggle
                    // simulate user mouse click
                    transform.Find("CtrlPnlCity/Dismiss").GetComponent<ActionToggle>().OnPointerDown(null);
                    // also we need to manually deactivate Toggle
                    transform.Find("CtrlPnlCity/Dismiss").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveHeal:
                    transform.Find("CtrlPnlCity/Heal").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.Find("CtrlPnlCity/Heal").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveResurect:
                    transform.Find("CtrlPnlCity/Resurect").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.Find("CtrlPnlCity/Resurect").GetComponent<Toggle>().isOn = false;
                    break;
                case CityViewActiveState.ActiveHeroEquipment:
                    // SetActiveState(cityViewActiveState, false);
                    transform.GetComponentInChildren<HeroParty>().transform.Find("HeroEquipmentBtn").GetComponent<ActionToggle>().OnPointerDown(null);
                    transform.GetComponentInChildren<HeroParty>().transform.Find("HeroEquipmentBtn").GetComponent<Toggle>().isOn = false;
                    break;
            }
            // deactive currently active state
            // SetActiveState(cityViewActiveState, false);

        }
        // deactivate this component
        transform.gameObject.SetActive(false);
    }
}
