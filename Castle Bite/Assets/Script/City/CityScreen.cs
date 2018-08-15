using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityScreen : MonoBehaviour {
    [SerializeField]
    City mCity;
    [SerializeField]
    CityViewActiveState cityViewActiveState = CityViewActiveState.Normal;

    public City City
    {
        get
        {
            return mCity;
        }
    }

    public void SetCityScreenActive(City city)
    {
        mCity = city;
    }

    void SetRequiredComponentsActive(bool doActivate)
    {
        // Activate/Deactivate background
        transform.root.Find("MiscUI/Background").gameObject.SetActive(doActivate);
        // Activate/Deactivate city controls
        transform.root.Find("MiscUI/BottomControlPanel").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Heal").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Resurect").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/MiddleControls/Dismiss").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/BottomControlPanel/RightControls/CityBackButton").gameObject.SetActive(doActivate);
        // Activate/Deactivate hero Parties menus
        transform.root.Find("MiscUI/LeftHeroParty").gameObject.SetActive(doActivate);
        transform.root.Find("MiscUI/RightHeroParty").gameObject.SetActive(doActivate);
    }

    void OnEnable()
    {
        SetRequiredComponentsActive(true);
    }

    void OnDisable()
    {
        SetRequiredComponentsActive(false);
    }


    public void SetActiveStateDismiss(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveDismiss, doActivate);
    }

    public void SetActiveStateHeal(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveHeal, doActivate);
    }

    public void SetActiveStateResurect(bool doActivate)
    {
        SetActiveState(CityViewActiveState.ActiveResurect, doActivate);
    }

    public void SetActiveState(CityViewActiveState requiredState, bool doActivate)
    {
        // if doActivate is true,
        // then we are really swiching to this new state
        // otherwise it means that we are deactivating old state and returning to Normal state
        // this should not happen for Normal state, so here is check, for this
        if (requiredState == CityViewActiveState.Normal)
        {
            Debug.LogError("Unexpected condition");
        }
        // 1st return previous state to Normal, if it is already not Normal
        // this only needed if we are activating new state, while previous state was not normal
        if ((cityViewActiveState != CityViewActiveState.Normal) && (requiredState != cityViewActiveState) && (doActivate == true))
        {
            SetActiveState(cityViewActiveState, false);
        }
        // Update 
        cityViewActiveState = requiredState;
        // instruct party and garnizon panels to highlight differently cells with and without units
        // loop through all active hero parties
        foreach (HeroParty heroParty in transform.parent.GetComponentsInChildren<HeroParty>())
        {
            // Set active state (highlight)
            switch (cityViewActiveState)
            {
                case CityViewActiveState.ActiveDismiss:
                    heroParty.GetComponentInChildren<PartyPanel>().SetActiveDismiss(doActivate);
                    break;
                case CityViewActiveState.ActiveHeal:
                    heroParty.GetComponentInChildren<PartyPanel>().SetActiveHeal(doActivate);
                    break;
                case CityViewActiveState.ActiveResurect:
                    heroParty.GetComponentInChildren<PartyPanel>().SetActiveResurect(doActivate);
                    break;
                case CityViewActiveState.ActiveUnitDrag:
                    heroParty.GetComponentInChildren<PartyPanel>().SetActiveUnitDrag(doActivate);
                    break;
                default:
                    Debug.LogError("Unknown condition");
                    break;
            }
        }
        // verfiy if there is a city garnizon == we are in city edit mode and not in hero party edit mode
        // and verify if there is already party in city
        if ( (transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Garnizon) != null)
            &&  (transform.GetComponentInParent<UIManager>().GetHeroPartyByMode(PartyMode.Party) == null) )
        {
            // activate hire hero panel
            transform.parent.Find("HireHeroPanel").gameObject.SetActive(true);
        }
        else
        {
            // deactivate hire hero panel
            transform.parent.Find("HireHeroPanel").gameObject.SetActive(false);
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
}
