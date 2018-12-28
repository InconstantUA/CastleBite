using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour {
    const byte DefaultColorAlpha = 255;

    void SetTransparency(byte a)
    {
        // get current 32 bit color
        Color32 currentColor32 = (Color32)GetComponent<Image>().color;
        // set background image transparency
        GetComponent<Image>().color = new Color32(currentColor32.r, currentColor32.g, currentColor32.b, a);
        //Color color = GetComponent<Image>().color;
        //color = new Color(color.r, color.g, color.b, a);
    }

    public void SetActive(bool doActivate, byte colorAlpha = DefaultColorAlpha)
    {
        gameObject.SetActive(doActivate);
        if (doActivate)
        {
            SetTransparency(colorAlpha);
        }
        else
        {
            // reset alpha color back to default, because I didn't change everywhere usage of Background to use this function, some may just activate it as game object expecting non-transparent background
            SetTransparency(DefaultColorAlpha);
        }
    }
}
