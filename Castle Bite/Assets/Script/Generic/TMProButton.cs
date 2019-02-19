using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TMProButton : TextButton
{
    public override void SetTextColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }

    public Color GetTextColor()
    {
        return GetComponent<TextMeshProUGUI>().color;
    }

    public Color color
    {
        get
        {
            return GetTextColor();
        }
        set
        {
            // set color
            GetComponent<TextMeshProUGUI>().color = value;
            // set normal color
            NormalColor = value;
        }
    }

    public Material fontSharedMaterial
    {
        set
        {
            GetComponent<TextMeshProUGUI>().fontSharedMaterial = value;
        }
    }
}
