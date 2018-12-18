using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour {

    public static MapMenuManager Instance { get; private set; }

    [SerializeField]
    bool keepEnabledAfterStart;

    void Awake()
    {
        Instance = this;
        // disable it after initial start
        gameObject.SetActive(keepEnabledAfterStart);
    }

    public IEnumerator EnterCityEditMode(MapCity mapCity)
    {
        //Debug.Log("EnterCityEditMode");
        mapCity.DimmLabel();
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(mapCity.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // map manager change to browse mode back
        // . - this is done by OnDisable() automatically in MapManager
        //MapManager mapManager = transform.parent.GetComponent<MapManager>();
        //mapManager.SetMode(MapManager.Mode.Browse);
        // Deactivate map manager with map
        MapManager.Instance.gameObject.SetActive(false);
        // Deactivate this map menu
        gameObject.SetActive(keepEnabledAfterStart);
        // Note: everything below related to mapManager or mapScreen will not be processed, because map manager is disabled
        // Activate city view = go to city edit mode
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).SetEditPartyScreenActive(mapCity.LCity);
    }

    public IEnumerator EnterHeroEditMode(MapHero mapHero)
    {
        Debug.Log("Enter hero edit mode.");
        mapHero.DimmLabel();
        // Trigger on mapobject exit to Hide label(s - + hide hero's lable, if it is in city)
        // verify if MapObject's labe is still active and mouse over it
        if (mapHero.GetComponent<MapObject>().Label.GetComponent<Text>().raycastTarget && mapHero.GetComponent<MapObject>().Label.IsMouseOver)
        {
            // disable it
            mapHero.GetComponent<MapObject>().OnPointerExit(null);
        }
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Wait for all animations to finish
        // this depends on the labelDimTimeout parameter in MapObject, we add additional 0.1f just in case
        yield return new WaitForSeconds(mapHero.GetComponent<MapObject>().LabelDimTimeout + 0.1f);
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
        // Deactivate map manager with map
        MapManager.Instance.gameObject.SetActive(false);
        // Deactivate this map menu
        gameObject.SetActive(false);
        // Note: everything below related to mapManager or mapScreen will not be processed, because map manager is disabled
        // Activate hero edit menu
        transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<EditPartyScreen>(true).SetEditPartyScreenActive(mapHero.LHeroParty);
    }
}
