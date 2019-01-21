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
    Text text;
    [SerializeField]
    TextButton textButton;
    [SerializeField]
    Text durationLeft;
    UniquePowerModifierInfoMenu uniquePowerModifierInfoMenu;

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
        durationLeft.text = uniquePowerModifierData.DurationLeft.ToString();
    }

    IEnumerator FadeBackground()
    {
        for (float f = 1f; f >= 0; f -= 0.3f)
        {
            Color c = backgroundImage.color;
            c.a = f;
            backgroundImage.color = c;
            yield return new WaitForSeconds(.05f);
        }
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
        textButton.NormalColor = uniquePowerModifierConfig.UniquePowerModifierUIConfig.statusIconTextNormalColor;
        textButton.HighlightedColor = uniquePowerModifierConfig.UniquePowerModifierUIConfig.statusIconTextHighlightedColor;
        textButton.PressedColor = uniquePowerModifierConfig.UniquePowerModifierUIConfig.statusIconTextPressedColor;
        textButton.DisabledColor = uniquePowerModifierConfig.UniquePowerModifierUIConfig.statusIconTextDisabledColor;
        // update text current color
        text.color = textButton.NormalColor;
        // update text symbol
        text.text = uniquePowerModifierConfig.UniquePowerModifierUIConfig.symbol;
        // update background image color
        backgroundImage.color = uniquePowerModifierConfig.UniquePowerModifierUIConfig.statusIconBackgroundColor;
        // activate game object
        gameObject.SetActive(true);
        // Start animation
        CoroutineQueueManager.Run(FadeBackground());
    }

    public void TriggerUniquePowerModifier()
    {
        // display animation
    }

    public void OnUniquePowerModifierDurationHasBeenResetToMax(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log("Display on UPM Duration reset to Max animation");
            // update current duration left value
            UpdateDurationLeftText();
        }
    }

    public void OnUniquePowerModifierPowerHasBeenChanged(UniquePowerModifierData uniquePowerModifierData)
    {
        // verify if this is the same as this UPM data
        if (uniquePowerModifierData.UniquePowerModifierID == this.uniquePowerModifierData.UniquePowerModifierID)
        {
            Debug.Log("Display on UPM Power change animation");
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
                "Power: " + uniquePowerModifierData.CurrentPower,
                "Duration: " + uniquePowerModifierData.DurationLeft + "/" + uniquePowerModifierConfig.UpmDurationMax,
                "Origin: " + uniquePowerModifierData.GetOriginDisplayName() + " " + uniquePowerModifierConfig.upmOrigin
            }
        };
    }
}
