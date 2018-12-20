using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBackButton : MonoBehaviour {
    [SerializeField]
    EditPartyScreen editPartyScreen;

    public void OnClick()
    {
        // disable edit party screen
        editPartyScreen.gameObject.SetActive(false);
        // enable map menu
        MapMenuManager.Instance.gameObject.SetActive(true);
        // enable world map
        MapManager.Instance.gameObject.SetActive(true);
    }
}
