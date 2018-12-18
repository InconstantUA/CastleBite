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
        // set label text
        GetComponent<MapObject>().Label.LabelTxt.text = "[" + lCity.CityName + "]\n\r <size=12>" + lCity.CityDescription + "</size>";
    }

    public void DimmLabel()
    {
        // Trigger on mapobject exit to Hide label(s - + hide hero's lable, if it is in city)
        // verify if MapObject's labe is still active and mouse over it
        if (GetComponent<MapObject>().Label.GetComponent<Text>().raycastTarget && GetComponent<MapObject>().Label.IsMouseOver)
        {
            // disable it
            GetComponent<MapObject>().OnPointerExit(null);
        }
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