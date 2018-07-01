using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PartiesInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    Transform duplicate;

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

    public void ActivateAdvance(MapHero mapHero)
    {
        Debug.Log("Show Party Info");
        // remove previous duplicate if it was present
        if (duplicate)
        {
            Destroy(duplicate.gameObject);
        }
        // activate placeholder
        Transform singlePartyPlaceholder = transform.Find("SinglePartyPlaceholder");
        singlePartyPlaceholder.gameObject.SetActive(true);
        // create duplicate of the party panel and place it into placeholder
        gameObject.SetActive(true);
        Transform partyPanelTr = mapHero.LinkedPartyTr.Find("PartyPanel");
        duplicate = Instantiate(partyPanelTr, singlePartyPlaceholder);
        // stretch duplicate transform
        RectTransform rectTransform = duplicate.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        // Reset to 0, 0, 0, 0 position
        rectTransform.offsetMin = new Vector2(0, 0); // left, bottom
        rectTransform.offsetMax = new Vector2(0, 0); // -right, -top
    }

    public void ActivateAdvance(MapCity mapCity)
    {
        Debug.Log("Show City info");
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
