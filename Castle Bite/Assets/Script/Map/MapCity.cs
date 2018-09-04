using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//public class MapCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
public class MapCity : MonoBehaviour
{
    //enum State { NotSelected, Selected };

    [SerializeField]
    private City lCity;
    private MapHero lMapHero;
    //public float labelDimTimeout;
    //City linkedCity;
    MapObjectLabel label;
    //Text labelTxt;
    Image markerImage;
    // Colors for Marker
    [SerializeField]
    Color hiddenMarkerColor;
    [SerializeField]
    Color highlightedMarkerColor;

    public MapHero LMapHero
    {
        get
        {
            return lMapHero;
        }

        set
        {
            lMapHero = value;
        }
    }

    public City LCity
    {
        get
        {
            return lCity;
        }

        set
        {
            lCity = value;
        }
    }

    void Start()
    {
        // init linkedCity object
        //linkedCity = lCity.GetComponent<City>();
        // update link in opposite direction
        lCity.GetComponent<City>().LMapCity = GetComponent<MapCity>();
        // set markerImage
        markerImage = GetComponent<Image>();
        // set label
        label = GetComponentInChildren<MapObjectLabel>();
        // set label text
        label.LabelTxt.text = "[" + lCity.CityName + "]\n\r <size=12>" + lCity.CityDescription + "</size>";
    }

    public IEnumerator EnterCityEditMode()
    {
        //Debug.Log("EnterCityEditMode");
        // Trigger on mapobject exit to Hide label(s - + hide hero's lable, if it is in city)
        // verify if MapObject's labe is still active and mouse over it
        if (GetComponentInChildren<MapObjectLabel>().GetComponent<Text>().raycastTarget && GetComponentInChildren<MapObjectLabel>().IsMouseOver)
        {
            // disable it
            GetComponent<MapObject>().OnPointerExit(null);
        }
        // Block mouse input
        InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        inputBlocker.SetActive(true);
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(GetComponent<MapObject>().LabelDimTimeout + 0.1f); 
        // Unblock mouse input
        inputBlocker.SetActive(false);
        // map manager change to browse mode back
        // . - this is done by OnDisable() automatically in MapManager
        //MapManager mapManager = transform.parent.GetComponent<MapManager>();
        //mapManager.SetMode(MapManager.Mode.Browse);
        // Deactivate map
        GameObject mapScreen = transform.root.Find("MapScreen").gameObject;
        mapScreen.SetActive(false);
        // everything below related to mapManager or mapScreen will not be processed
        // because map manager is disabled
        // Activate city view = go to city edit mode
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).SetEditPartyScreenActive(lCity);
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