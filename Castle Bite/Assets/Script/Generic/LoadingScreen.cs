using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    [SerializeField]
    Text loadingScreenText;
    public void SetActive(bool doActivate, string textString = "")
    {
        // activate this game object
        gameObject.SetActive(doActivate);
        // set text
        loadingScreenText.text = textString;
    }
}
