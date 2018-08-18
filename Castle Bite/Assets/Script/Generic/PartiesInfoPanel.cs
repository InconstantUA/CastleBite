using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PartiesInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    bool positionHasBeenChanged = false;


    void OnDisable()
    {

        // deactivate all child panels (placeholders)
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(false);
        }
        // Deactivate what was activated
        // Deactivate left hero party UI
        transform.root.Find("MiscUI/LeftHeroParty").gameObject.SetActive(false);
        // Deactivate right hero party UI
        transform.root.Find("MiscUI/RightHeroParty").gameObject.SetActive(false);
        // Get Party Panel
        PartyPanel partyPanel = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>().GetComponentInChildren<PartyPanel>(true);
        // verify if party panel position has changed
        if (positionHasBeenChanged)
        {
            // Change PartyPanel Position back to original
            SetPartyPanelPosition(partyPanel, transform.Find("LeftPartyInfoPlaceHolder").GetComponent<RectTransform>());
            // Reset flag to false
            positionHasBeenChanged = false;
        }
    }

    public void ActivateAdvance(MapObject mapObject)
    {
        // Activate this (it has transparent background image attached, which blocks raycasts)
        gameObject.SetActive(true);
        // And set it as background
        transform.SetAsFirstSibling();
        // Activate required party panels
        if (mapObject.GetComponent<MapHero>())
        {
            ActivateAdvance(mapObject.GetComponent<MapHero>());
            return;
        }
        if (mapObject.GetComponent<MapCity>())
        {
            ActivateAdvance(mapObject.GetComponent<MapCity>());
            return;
        }
    }

    void SetRectTransform(RectTransform src, RectTransform dst)
    {
        dst.anchorMin = new Vector2
        {
            x = src.anchorMin.x,
            y = src.anchorMin.y
        };
        dst.anchorMax = new Vector2
        {
            x = src.anchorMax.x,
            y = src.anchorMax.y
        };
        dst.offsetMin = new Vector2
        {
            x = src.offsetMin.x,
            y = src.offsetMin.y
        };
        dst.offsetMax = new Vector2
        {
            x = src.offsetMax.x,
            y = src.offsetMax.y
        };
    }

    void SetPartyPanelPosition(PartyPanel partyPanel, RectTransform newPosition)
    {
        // get current position
        RectTransform currentPosition = partyPanel.GetComponent<RectTransform>();
        // set position has been changed flag
        positionHasBeenChanged = true;
        // Change to the new position
        SetRectTransform(newPosition, currentPosition);
    }

    public void ActivateAdvance(MapHero mapHero)
    {
        Debug.Log("Show Party Info");
        //// clone hero panel and place it into the middle placeholder
        //SetPartyPanelPosition(mapHero.LHeroParty.Find("PartyPanel"), transform.Find("SinglePartyPlaceholder"));
        // Get Lefto Hero Party UI
        HeroPartyUI leftHeroPartyUI = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>();
        // assign HeroParty to left hero party UI
        leftHeroPartyUI.LHeroParty = mapHero.LHeroParty;
        // Get Party Panel
        PartyPanel partyPanel = leftHeroPartyUI.GetComponentInChildren<PartyPanel>(true);
        // Get new position
        RectTransform newPosition = transform.Find("SinglePartyPlaceholder").GetComponent<RectTransform>();
        // Change Party Panel position to new
        SetPartyPanelPosition(partyPanel, newPosition);
        // activate left hero party UI
        leftHeroPartyUI.gameObject.SetActive(true);
        // deactivate inventory
        leftHeroPartyUI.GetComponentInChildren<PartyInventoryUI>(true).gameObject.SetActive(false);
        //// Activate placeholder to be used as background
        //transform.Find("SinglePartyPlaceholder").gameObject.SetActive(true);
    }

    public void ActivateAdvance(MapCity mapCity)
    {
        Debug.Log("Show City info");
        // verify if there is a hero in the city
        if (mapCity.LMapHero != null)
        {
            // Get Lefto Hero Party UI
            HeroPartyUI leftHeroPartyUI = transform.root.Find("MiscUI/LeftHeroParty").GetComponent<HeroPartyUI>();
            // assign HeroParty to left hero party UI
            leftHeroPartyUI.LHeroParty = mapCity.LCity.GetHeroPartyByMode(PartyMode.Party);
            // activate left hero party UI
            leftHeroPartyUI.gameObject.SetActive(true);
            // deactivate inventory
            leftHeroPartyUI.GetComponentInChildren<PartyInventoryUI>(true).gameObject.SetActive(false);
            //// Activate placeholder to be used as background
            //transform.Find("LeftPartyInfoPlaceHolder").gameObject.SetActive(true);
        }
        else
        {
            // No hero in city
            // Display black box
            transform.Find("LeftPartyInfoPlaceHolder").gameObject.SetActive(true);
        }
        // Get Right Hero Party UI
        HeroPartyUI rightHeroPartyUI = transform.root.Find("MiscUI/RightHeroParty").GetComponent<HeroPartyUI>();
        // assign City garnizon HeroParty to right hero party UI
        rightHeroPartyUI.LHeroParty = mapCity.LCity.GetHeroPartyByMode(PartyMode.Garnizon);
        // activate right hero party UI
        rightHeroPartyUI.gameObject.SetActive(true);
        // deactivate inventory
        rightHeroPartyUI.GetComponentInChildren<PartyInventoryUI>(true).gameObject.SetActive(false);
        //// Activate placeholder to be used as background
        //transform.Find("RightPartyInfoPlaceHolder").gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            gameObject.SetActive(false);
        }
    }

}
