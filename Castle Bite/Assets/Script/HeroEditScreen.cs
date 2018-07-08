using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEditScreen : MonoBehaviour {

    public void DisableAdv()
    {
        // This is done to avoid this error when I use this on OnDisable() function:
        // Cannot change GameObject hierarchy while activating or deactivating the parent.
        // Move hero party back to map
        Transform partiesOnMapTr = transform.root.Find("PartiesOnMap");
        // there might be more than 1 party, when we exchange items between 2 parties,
        // that is why find all move all parties to map
        HeroParty[] heroParties = GetComponentsInChildren<HeroParty>();
        foreach (HeroParty heroParty in heroParties)
        {
            heroParty.transform.SetParent(partiesOnMapTr);
        }
        //// disable this screen
        //gameObject.SetActive(false);
        // activate exit city function to correctly exit city
        GetComponent<City>().ExitCity();
    }

}
