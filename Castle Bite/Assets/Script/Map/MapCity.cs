using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//public class MapCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
public class MapCity : MonoBehaviour
{
    enum State { NotSelected, Selected };

    public Transform linkedCityTr;
    private Transform linkedPartyOnMapTr;
    //[SerializeField]
    //public float labelDimTimeout;
    [SerializeField]
    City linkedCity;
    MapObjectLabel label;
    //Text labelTxt;
    Image markerImage;
    // Colors for Marker
    [SerializeField]
    Color hiddenMarkerColor;
    [SerializeField]
    Color highlightedMarkerColor;

    public Transform LinkedPartyOnMapTr
    {
        get
        {
            return linkedPartyOnMapTr;
        }

        set
        {
            linkedPartyOnMapTr = value;
        }
    }

    // Colors for Label
    //[SerializeField]
    //Color hiddenLabelColor;
    //[SerializeField]
    //Color notHighlightedLabelColor;
    //[SerializeField]
    //Color highlightedLabelColor;
    //[SerializeField]
    //Color pressedLabelColor;
    // For dimm
    //[SerializeField]
    //bool isMouseOver;

    void Start()
    {
        // init linkedCity object
        linkedCity = linkedCityTr.GetComponent<City>();
        // set markerImage
        markerImage = GetComponent<Image>();
        // set label
        label = GetComponentInChildren<MapObjectLabel>();
        // set label text
        label.LabelTxt.text = "[" + linkedCity.GetCityName() + "]\n\r <size=12>" + linkedCity.GetCityDescription() + "</size>";
    }

    //void Update()
    //{
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    // Debug.Log("MapCity OnPointerDown");
    //    labelTxt.color = pressedLabelColor;
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    // Debug.Log("MapCity OnPointerUp");
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    // Debug.Log("MapCity OnPointerEnter");
    //    // highlight this menu
    //    SetHighlightedStatus();
    //    isMouseOver = true;
    //    labelTxt.raycastTarget = true;
    //    // give control on actions to map manager
    //    MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //    mapManager.OnPointerEnterChildObject(gameObject, eventData);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    // Debug.Log("MapCity OnPointerExit");
    //    isMouseOver = false;
    //    // Dimm label
    //    StartCoroutine(DimmLabelWithDelay());
    //    // verify if there is hero in this city
    //    if (linkedPartyOnMapTr)
    //    {
    //        // do the same for hero's object
    //        linkedPartyOnMapTr.GetComponent<MapHero>().SetNormalStatus();
    //    }
    //    // give control on actions to map manager
    //    MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //    mapManager.OnPointerExitChildObject(gameObject, eventData);
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // Debug.Log("MapCity OnPointerClick");
    //    // change city pressed status to city highlighted color
    //    // so it is not in pressed status any more
    //    SetHighlightedStatus();
    //    // give control on actions to map manager
    //    MapManager mapManager = transform.parent.GetComponent<MapManager>();
    //    mapManager.ActOnClick(gameObject, eventData);
    //}

    //void SetHighlightedStatus()
    //{
    //    // Debug.Log("SetHighlightedStatus " + btn.name + " button");
    //    // change to highlighted color
    //    labelTxt.color = highlightedLabelColor;
    //    // also highlight party in the city if it is present
    //    // but do it a little dimmed to show that it will not be activated on press
    //    // but to indicate that you can move mouse left to activate it
    //    if (linkedPartyOnMapTr)
    //    {
    //        linkedPartyOnMapTr.GetComponent<MapHero>().SetNormalStatus();
    //        // also show hero label
    //        linkedPartyOnMapTr.Find("HeroLabel").GetComponent<MapHeroLabel>().LabelTxt.raycastTarget = true;
    //        // stop courutine, if it was running to prevent it to dimm out hero lable
    //        // this does not work
    //        // StopCoroutine(DimmHeroLabelWithDelay());
    //    }
    //}

    

    public IEnumerator EnterCityEditMode()
    {
        Debug.Log("EnterCityEditMode");
        // Trigger on mapobject exit to Hide label(s - + hide hero's lable, if it is in city)
        //label.HideLabel();
        GetComponent<MapObject>().OnPointerExit(null);
        //Debug.Log("1 - triggered on exit");
        //yield return new WaitForSeconds(2f);
        // Block mouse input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(true);
        //Debug.Log("2 - blocked input");
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(GetComponent<MapObject>().LabelDimTimeout + 0.1f); 
        // go to city edit mode
        // get variables
        GameObject mapScreen = transform.root.Find("MapScreen").gameObject;
        GameObject cityMenu = linkedCity.gameObject;
        // Unblock mouse input
        inputBlocker.SetActive(false);
        //Debug.Log("3 unblocked mouse input");
        // map manager change to browse mode back
        MapManager mapManager = transform.parent.GetComponent<MapManager>();
        mapManager.SetMode(MapManager.Mode.Browse);
        // Deactivate map and activate city
        mapScreen.SetActive(false);
        cityMenu.SetActive(true);
        //Debug.Log("4 activated map screen");
        //yield return new WaitForSeconds(2f);
    }

    public void SetSelectedState(bool doActivate)
    {
        // select this city
        if (doActivate)
        {
            // higlight it with red blinking
            markerImage.color = highlightedMarkerColor;
        }
        else
        {
            // exit selected mode
            markerImage.color = hiddenMarkerColor;
        }
    }

    //public bool IsMouseOver
    //{
    //    get
    //    {
    //        return isMouseOver;
    //    }

    //    set
    //    {
    //        isMouseOver = value;
    //    }
    //}
}