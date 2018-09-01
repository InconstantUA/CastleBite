using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapFocusPanel : MonoBehaviour {
    GameObject focusedObject;

    void OnEnable()
    {
        // check if focused object is not defined yet
        if (focusedObject == null)
        {
            ReleaseFocus();
        }
    }

    public void SetActive(MapHero mapHero)
    {
        Debug.Log("Update map focus panel with " + mapHero.name);
        focusedObject = mapHero.gameObject;
        transform.Find("Name").GetComponent<Text>().text = mapHero.LHeroParty.GetPartyLeader().GivenName + "\r\n<size=12>" + mapHero.LHeroParty.GetPartyLeader().UnitName + "</size>";
    }

    public void SetActive(MapCity mapCity)
    {
        focusedObject = mapCity.gameObject;
        Debug.Log("Update map focus panel with " + mapCity.name);
        transform.Find("Name").GetComponent<Text>().text = mapCity.LCity.CityName + "\r\n<size=12>" + mapCity.LCity.CityType + "</size>";
    }

    public void ReleaseFocus()
    {
        Debug.Log("Realease map focus panel focus");
        // clear focused object
        focusedObject = null;
        // clear name
        transform.Find("Name").GetComponent<Text>().text = "";
        // dimm left and right arrows
        transform.Find("Previous").GetComponent<TextButton>().SetDisabledStatus();
        transform.Find("Previous").GetComponent<TextButton>().interactable = false;
        transform.Find("Next").GetComponent<TextButton>().SetDisabledStatus();
        transform.Find("Next").GetComponent<TextButton>().interactable = false;
        //transform.Find("Next").gameObject.SetActive(false);
        // disable all focused object controls
        foreach (Transform childTransform in transform.Find("FocusedObjectControl"))
        {
            childTransform.gameObject.SetActive(false);
        }
        // activate empty control
        transform.Find("FocusedObjectControl/EmptyControl").gameObject.SetActive(true);
    }

}
