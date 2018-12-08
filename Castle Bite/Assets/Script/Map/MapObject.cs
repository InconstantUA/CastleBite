using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MapObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // For UI user interraction
    [SerializeField]
    private float labelDimTimeout;
    MapObjectLabel label;
    Text labelTxt;
    // Colors for Label
    [SerializeField]
    Color hiddenLabelColor;
    [SerializeField]
    Color alwaysOnLabelColor;
    [SerializeField]
    Color notHighlightedLabelColor;
    [SerializeField]
    Color highlightedLabelColor;
    [SerializeField]
    Color pressedLabelColor;
    // For dimm
    [SerializeField]
    bool isMouseOver;
    // For always On lable
    [SerializeField]
    bool labelAlwaysOn;

    void Start()
    {
        // set label
        label = GetComponentInChildren<MapObjectLabel>(true);
        // set label text object
        labelTxt = label.GetComponent<Text>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.LogWarning("OnPointerDown");
            // on left mouse click
            labelTxt.color = pressedLabelColor;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
            // verify which component is being linked
            // verify if it is MapCit or MapHero
            if (GetComponent<MapCity>() != null || GetComponent<MapHero>() != null)
            {
                // show unit info
                UIRoot.Instance.transform.Find("MiscUI/PartiesInfoPanel").GetComponent<PartiesInfoPanel>().ActivateAdvance(this);
            }
            // verify if it is MapChest
            else if (GetComponent<MapItemsContainer>() != null)
            {
                // show treasure chest info
                UIRoot.Instance.transform.Find("MiscUI").GetComponentInChildren<ContextInfoPopUp>(true).SetActive(true, "<b>Treasure chest</b>.\r\n\r\nWho knows what is hidden inside...");
            }
            else
            {
                Debug.LogWarning("Unknown linked component");
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // verify which component is being linked
            // verify if it is MapCit or MapHero
            if (GetComponent<MapCity>() != null || GetComponent<MapHero>() != null)
            {
                // deactivate unit info
                UIRoot.Instance.transform.Find("MiscUI/PartiesInfoPanel").gameObject.SetActive(false);
            }
            // verify if it is MapChest
            else if (GetComponent<MapItemsContainer>() != null)
            {
                // disable treasure chest info
                UIRoot.Instance.transform.Find("MiscUI").GetComponentInChildren<ContextInfoPopUp>(true).SetActive(false);
            }
            else
            {
                Debug.LogWarning("Unknown linked component");
            }
        }
    }

    MapObject GetLinkedMapObject()
    {
        // verify if there is linked object (stacked objects on the same tile and trigger its OnPointerEnter too)
        // verify if this map object is linked to city on map
        if (GetComponent<MapCity>())
        {
            // verify if there is linked party = hero inside of this city
            if (GetComponent<MapCity>().LMapHero)
            {
                // return map object of linked hero
                return GetComponent<MapCity>().LMapHero.GetComponent<MapObject>();
            }
        }
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // highlight this menu
        SetHighlightedStatus();
        isMouseOver = true;
        if (label.Interactable)
            labelTxt.raycastTarget = true;
        // give control on actions to map manager
        // MapManager mapManager = transform.parent.GetComponent<MapManager>();
        MapManager.Instance.OnPointerEnterChildObject(gameObject, eventData);
        // tigger on pointer enter event on linked object, if it is present
        MapObject linkedMapHeroMapObj = GetLinkedMapObject();
        if (GetLinkedMapObject())
        {
            linkedMapHeroMapObj.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        if (labelAlwaysOn)
        {
            labelTxt.color = alwaysOnLabelColor;
        }
        else
        {
            labelTxt.color = notHighlightedLabelColor;
            // Dimm label
            StartCoroutine(DimmLabelWithDelay());
        }
        // give control on actions to map manager
        // MapManager mapManager = transform.parent.GetComponent<MapManager>();
        MapManager.Instance.OnPointerExitChildObject(gameObject, eventData);
        // tigger on pointer exit event on linked object, if it is present
        MapObject linkedMapHeroMapObj = GetLinkedMapObject();
        if (GetLinkedMapObject())
        {
            linkedMapHeroMapObj.OnPointerExit(eventData);
        }
    }

    IEnumerator DimmLabelWithDelay()
    {
        yield return new WaitForSeconds(labelDimTimeout);
        // verify if mouse is not entered again after we started to wait
        if (!isMouseOver && !label.IsMouseOver)
        {
            label.HideLabel();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            // change city pressed status to city highlighted color
            // so it is not in pressed status any more
            SetHighlightedStatus();
            // give control on actions to map manager
            // MapManager mapManager = transform.parent.GetComponent<MapManager>();
            MapManager.Instance.ActOnClick(gameObject, eventData);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click

        }
    }

    public void SetHighlightedStatus()
    {
        // change to highlighted color
        labelTxt.color = highlightedLabelColor;
    }

    public void SetAlwaysOn(bool doActivate)
    {
        // turn on label always on flag
        LabelAlwaysOn = doActivate;
        // verify if we need to show or hide all labels
        if (doActivate)
        {
            GetComponentInChildren<MapObjectLabel>(true).SetAlwaysOnLabelColor();
        }
        else
        {
            GetComponentInChildren<MapObjectLabel>(true).HideLabel();
        }
    }

    public bool IsMouseOver
    {
        get
        {
            return isMouseOver;
        }

        set
        {
            isMouseOver = value;
        }
    }

    public Color HiddenLabelColor
    {
        get
        {
            return hiddenLabelColor;
        }

        set
        {
            hiddenLabelColor = value;
        }
    }

    public Color NotHighlightedLabelColor
    {
        get
        {
            return notHighlightedLabelColor;
        }

        set
        {
            notHighlightedLabelColor = value;
        }
    }

    public Color HighlightedLabelColor
    {
        get
        {
            return highlightedLabelColor;
        }

        set
        {
            highlightedLabelColor = value;
        }
    }

    public Color PressedLabelColor
    {
        get
        {
            return pressedLabelColor;
        }

        set
        {
            pressedLabelColor = value;
        }
    }

    public float LabelDimTimeout
    {
        get
        {
            return labelDimTimeout;
        }

        set
        {
            labelDimTimeout = value;
        }
    }

    public bool LabelAlwaysOn
    {
        get
        {
            return labelAlwaysOn;
        }

        set
        {
            labelAlwaysOn = value;
        }
    }

    public Color AlwaysOnLabelColor
    {
        get
        {
            return alwaysOnLabelColor;
        }

        set
        {
            alwaysOnLabelColor = value;
        }
    }
}