using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BackgroundIntermediate : MonoBehaviour
{
    public enum Mode
    {
        FullScreen,
        MiddleScreen
    }
    [SerializeField]
    Image fullScreenBackgroundImage;
    [SerializeField]
    Image middleScreenBackgroundImage;

    Image GetBackgroundImageByMode(Mode mode)
    {
        // verify which background to activate
        // Upgrade, Equipment and HireUnit uses Intermediate background, which allows to see control buttons
        // Just right click in EditParty or City view should show full screen background to cover control buttons
        switch (mode)
        {
            case Mode.FullScreen:
                return fullScreenBackgroundImage;
            case Mode.MiddleScreen:
                return middleScreenBackgroundImage;
            default:
                Debug.LogWarning("Unknown image mode: " + mode + " . Falling back to MiddleScreen background");
                return middleScreenBackgroundImage;
        }
    }

    Color GetNewColorWithTransparency(Color color, int a)
    {
        return new Color(color.r, color.g, color.b, a);
    }

    public void SetActive(bool doActivate, Mode mode = Mode.MiddleScreen, byte colorAlpha = 222)
    {
        if (doActivate)
        {
            // get background image by mode
            Image backgroundImage = GetBackgroundImageByMode(mode);
            // get current 32 bit color
            Color32 currentColor32 = (Color32)backgroundImage.color;
            // set background image transparency
            backgroundImage.color = new Color32(currentColor32.r, currentColor32.g, currentColor32.b, colorAlpha);
            // activate background image
            // Debug.Log("Activate " + backgroundImage.name + " background");
            backgroundImage.gameObject.SetActive(doActivate);
        }
        else
        {
            // disable both backgrounds
            fullScreenBackgroundImage.gameObject.SetActive(false);
            middleScreenBackgroundImage.gameObject.SetActive(false);
        }
        // activate/deactivate this object
        gameObject.SetActive(doActivate);
    }
}
