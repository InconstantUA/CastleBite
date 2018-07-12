using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PartiesInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    Dictionary<Transform, Transform> originalParent;
    Dictionary<Transform, Vector3> originalPosition;

    void OnEnable()
    {
        originalParent = new Dictionary<Transform, Transform>();
        originalPosition = new Dictionary<Transform, Vector3>();
    }

    void OnDisable()
    {
        //// destroy clonned panels
        //foreach (PartyPanel partyPanel in transform.GetComponentsInChildren<PartyPanel>())
        //{
        //    Destroy(partyPanel.gameObject);
        //}
        // return to original parent
        foreach (KeyValuePair<Transform, Transform> original in originalParent)
        {
            original.Key.SetParent(original.Value);
        }
        // return to original position
        foreach (KeyValuePair<Transform, Vector3> original in originalPosition)
        {
            original.Key.position = original.Value;
        }
        // deactivate all child panels
        transform.Find("SinglePartyPlaceholder").gameObject.SetActive(false);
        transform.Find("LeftPartyInfoPlaceHolder").gameObject.SetActive(false);
        transform.Find("RightPartyInfoPlaceHolder").gameObject.SetActive(false);
    }

    public void ActivateAdvance(GameObject gameObject)
    {
        if (gameObject.GetComponent<MapHero>())
        {
            ActivateAdvance(gameObject.GetComponent<MapHero>());
            return;
        }
        if (gameObject.GetComponent<MapCity>())
        {
            ActivateAdvance(gameObject.GetComponent<MapCity>());
            return;
        }
    }

    void CloneAndPlacePartyPanel(Transform sourcePartyPanelTr, Transform partyPanelPlaceholder)
    {
        // activate placeholder
        partyPanelPlaceholder.gameObject.SetActive(true);
        // Activate PartiesInfoPanel
        gameObject.SetActive(true);
        // create duplicate of the party panel and place it into placeholder
        //Transform destinationPartyPanelTr = Instantiate(sourcePartyPanelTr.gameObject, partyPanelPlaceholder).transform;
        // save orignal parent
        originalParent.Add(sourcePartyPanelTr, sourcePartyPanelTr.parent);
        // save original position
        originalPosition.Add(sourcePartyPanelTr, sourcePartyPanelTr.position);
        // change parent
        sourcePartyPanelTr.SetParent(partyPanelPlaceholder);
        // stretch heroPartyPanelClone transform
        RectTransform rectTransform = sourcePartyPanelTr.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        // Reset to 0, 0, 0, 0 position
        rectTransform.offsetMin = new Vector2(0, 0); // left, bottom
        rectTransform.offsetMax = new Vector2(0, 0); // -right, -top
    }

    public void ActivateAdvance(MapHero mapHero)
    {
        Debug.Log("Show Party Info");
        // clone hero panel and place it into the middle placeholder
        CloneAndPlacePartyPanel(mapHero.LinkedPartyTr.Find("PartyPanel"), transform.Find("SinglePartyPlaceholder"));
    }

    public void ActivateAdvance(MapCity mapCity)
    {
        Debug.Log("Show City info");
        // verify if there is a hero in the city
        if (mapCity.LinkedPartyOnMapTr)
        {
            // clone hero in city panel and place it into the left placeholder
            MapHero mapHero = mapCity.LinkedPartyOnMapTr.GetComponent<MapHero>();
            CloneAndPlacePartyPanel(mapHero.LinkedPartyTr.Find("PartyPanel"), transform.Find("LeftPartyInfoPlaceHolder"));
            Debug.Log("Show linked party panel");
        }
        else
        {
            // No hero in city
            // Display black box
            transform.Find("LeftPartyInfoPlaceHolder").gameObject.SetActive(true);
        }
        // clone city panel and place it into the right placeholder
        CloneAndPlacePartyPanel(mapCity.LinkedCityTr.Find("CityGarnizon/PartyPanel"), transform.Find("RightPartyInfoPlaceHolder"));
        Debug.Log("Show city garnizon party panel " + mapCity.LinkedCityTr.name);
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
