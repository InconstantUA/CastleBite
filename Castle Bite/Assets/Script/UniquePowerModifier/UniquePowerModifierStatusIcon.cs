using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniquePowerModifierInfoData
{
    public string name;
    public string description;
    public string[] additionalInfo;
}

public class UniquePowerModifierStatusIcon : MonoBehaviour
{
    [SerializeField]
    UniquePowerModifierData uniquePowerModifierData;
    [SerializeField]
    Image backgroundImage;
    [SerializeField]
    Text symbolText;
    [SerializeField]
    TextButton textButton;
    [SerializeField]
    Text durationLeftText;
    [SerializeField]
    UniquePowerModifierStatusIconConfig config;
    UniquePowerModifierInfoMenu uniquePowerModifierInfoMenu;
    private bool animationHasFinished;

    public UniquePowerModifierInfoMenu UniquePowerModifierInfoMenu
    {
        get
        {
            // verify if it is not set yet
            if (uniquePowerModifierInfoMenu == null)
            {
                // get the reference from UI
                uniquePowerModifierInfoMenu = UIRoot.Instance.GetComponentInChildren<UIManager>(true).GetComponentInChildren<UniquePowerModifierInfoMenu>(true);
            }
            return uniquePowerModifierInfoMenu;
        }
    }

    void UpdateDurationLeftText()
    {
        // update current duration left value
        durationLeftText.text = uniquePowerModifierData.DurationLeft.ToString();
    }

    private void SetImageColorTransparency(Image image, float transparency)
    {
        Color c = image.color;
        c.a = transparency;
        image.color = c;
    }

    IEnumerator FadeBackground()
    {
        //// set final transparency
        //float finalTransparency = 0;
        //// verify if durationleft is not 0
        //if (uniquePowerModifierData.DurationLeft != 0)
        //{
        //    // set final transparency to more than 0, so it indicates that it still has ome duration
        //    finalTransparency = 0.3f;
        //}
        // reset animation has finished flag
        animationHasFinished = false;
        //for (float f = 1f; f >= finalTransparency; f -= 0.05f)
        //{
        //    Color c = backgroundImage.color;
        //    c.a = f;
        //    backgroundImage.color = c;
        //    yield return new WaitForSeconds(.05f);
        //}
        float duration = config.fadeBackgroundAnimationDuration;
        // loop over duration in seconds backwards
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            SetImageColorTransparency(backgroundImage, i / duration);
            // Debug.Log("Fade text, transparency: " + Mathf.RoundToInt(i / duration * 255));
            yield return null;
        }
        // set animation has finished flag
        animationHasFinished = true;
    }

    protected void SetTextColorTransparency(Text text, float transparency)
    {
        Color c = text.color;
        c.a = transparency;
        text.color = c;
    }

    protected IEnumerator FadeOutText(Text text, float duration)
    {
        // loop over duration in seconds backwards
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            SetTextColorTransparency(text, i / duration);
            // Debug.Log("Fade text, transparency: " + Mathf.RoundToInt(i / duration * 255));
            yield return null;
        }
        // hide text completely
        // Debug.Log("Hide text");
        SetTextColorTransparency(text, 0);
        yield return null;
    }

    protected IEnumerator FadeOutTexts(Text[] texts, float duration)
    {
        // loop over duration in seconds backwards
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            foreach(Text text in texts)
            {
                SetTextColorTransparency(text, i / duration);
            }
            // Debug.Log("Fade text, transparency: " + Mathf.RoundToInt(i / duration * 255));
            yield return null;
        }
        // hide text completely
        // Debug.Log("Hide text");
        foreach (Text text in texts)
        {
            SetTextColorTransparency(text, 0);
        }
        yield return null;
    }

    protected IEnumerator BlinkText(Text text, float duration, float transparensyThreshold)
    {
        // loop over duration in seconds backwards
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            // next transparency
            // time Tr
            // 0.5  0.5
            // 0.4  (0.4/0.5-0.5)*0.8+0.2
            float timeFromTotalLeft = i / duration;
            float transparencyRange = 1 - transparensyThreshold;
            float nextTransparency = transparensyThreshold + timeFromTotalLeft * transparencyRange;
            // Debug.Log("Next transparency is " + nextTransparency);
            SetTextColorTransparency(text, nextTransparency);
            // Debug.Log("Fade text, transparency: " + Mathf.RoundToInt(i / duration * 255));
            yield return null;
        }
    }

    IEnumerator RemoveThisStatusIcon()
    {
        // hide symbol and duration left text
        yield return FadeOutTexts(new Text[] { symbolText, durationLeftText }, config.onDisableFadeOutTextDuration);
        // wait while animation has not finished
        while (!animationHasFinished)
        {
            // skip to the next frame
            yield return null;
        }
        // remove this status icon
        RecycleBin.Recycle(this.gameObject);
    }

    public void SetActive(UniquePowerModifierData uniquePowerModifierData)
    {
        // save UPM data
        this.uniquePowerModifierData = uniquePowerModifierData;
        // update current duration left value
        UpdateDurationLeftText();
        // get UPM UI config
        UniquePowerModifierConfig uniquePowerModifierConfig = uniquePowerModifierData.GetUniquePowerModifierConfig();
        // update text button colors
        textButton.NormalColor = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.statusIconTextNormalColor;
        textButton.HighlightedColor = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.statusIconTextHighlightedColor;
        textButton.PressedColor = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.statusIconTextPressedColor;
        textButton.DisabledColor = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.statusIconTextDisabledColor;
        // update text current color
        symbolText.color = textButton.NormalColor;
        // update text symbol
        symbolText.text = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.symbol;
        // update background image color
        backgroundImage.color = uniquePowerModifierConfig.UniquePowerModifierStausIconUIConfig.statusIconBackgroundColor;
        // activate game object
        gameObject.SetActive(true);
        // Start animation
        CoroutineQueueManager.Run(FadeBackground());
    }

    public void OnUniquePowerModifierHasBeenRemovedEvent(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            // Remove this UPM status icon
            Debug.Log("Remove this UPM status icon");
            // Do it via coroutine and global queue, to make sure that it is removed after all other animation has finished
            CoroutineQueueManager.Run(RemoveThisStatusIcon());
        }
    }

    public void OnUniquePowerModifierHasBeenTriggeredEvent(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log(".. Display on UPM has been triggered animation");
            // Start animation
            StartCoroutine(FadeBackground());
        }
    }

    public void OnUniquePowerModifierDurationHasBeenResetToMax(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log(".. Display on UPM Duration reset to Max animation");
            // update current duration left value
            UpdateDurationLeftText();
            // display animation
            StartCoroutine(BlinkText(durationLeftText, config.blinkTextDuration, config.blinkTextTransparencyThreshold));
        }
    }

    public void OnUniquePowerModifierDurationHasChanged(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log(".. Display on UPM Duration has changed animation");
            // update current duration left value
            UpdateDurationLeftText();
            // display animation
            StartCoroutine(BlinkText(durationLeftText, config.blinkTextDuration, config.blinkTextTransparencyThreshold));
        }
    }

    public void OnUniquePowerModifierPowerHasBeenChanged(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log(".. Display on UPM Power change animation");
        }
    }

    // on right mouse click
    public void DisplayAdditionalInfo()
    {
        Debug.Log("Display additional information about UPM");
        UniquePowerModifierInfoMenu.SetActive(true, GetInfo());
    }

    public void HideAdditionalInfo()
    {
        UniquePowerModifierInfoMenu.SetActive(false);
    }

    public UniquePowerModifierInfoData GetInfo()
    {
        UniquePowerModifierConfig uniquePowerModifierConfig = uniquePowerModifierData.GetUniquePowerModifierConfig();
        return new UniquePowerModifierInfoData
        {
            name = uniquePowerModifierConfig.DisplayName,
            description = uniquePowerModifierConfig.Description,
            additionalInfo = new string[] {
                "Power: " + Math.Abs(uniquePowerModifierData.CurrentPower),
                "Duration: " + uniquePowerModifierData.DurationLeft + "/" + uniquePowerModifierConfig.UpmDurationMax,
                "Origin: " + uniquePowerModifierData.GetOriginDisplayName() + " " + uniquePowerModifierData.UniquePowerModifierID.modifierOrigin
            }
        };
    }


}
