using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour {

    public void SetActive(bool doActivate, int colorAlpha = 255)
    {
        gameObject.SetActive(doActivate);
        if (doActivate)
        {
            Color color = GetComponent<Image>().color;
            color = new Color(color.r, color.g, color.b, colorAlpha);
        }
    }
}
