using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    public void SetActive(bool doActivate)
    {
        // activate background
        transform.root.Find("MiscUI").GetComponentInChildren<BackgroundUI>(true).SetActive(doActivate);
        // activate this game object
        gameObject.SetActive(doActivate);
    }
}
