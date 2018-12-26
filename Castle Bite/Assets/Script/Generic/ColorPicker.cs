using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour
{
    [SerializeField]
    TextButton saveButton;
    [SerializeField]
    TextButton cancelButton;
    [SerializeField]
    TextToggle colorToggleTemplate;
    [SerializeField]
    TextToggleGroup colorsToggleGroup;

    public void SetActive(ColorsConfig colorsConfig, UnityAction SaveEvent, UnityAction CancelEvent)
    {
        // remove possible previous listeners attached to this button
        saveButton.OnClick.RemoveAllListeners();
        // setup our listener
        saveButton.OnClick.AddListener(SaveEvent);
        saveButton.OnClick.AddListener(Close);
        // same for Cancel button
        cancelButton.OnClick.RemoveAllListeners();
        cancelButton.OnClick.AddListener(CancelEvent);
        cancelButton.OnClick.AddListener(Close);
        // loop through all colors in config
        foreach(Color color in colorsConfig.colors)
        {
            // create color toggle
            TextToggle colorToggle = Instantiate(colorToggleTemplate.gameObject, colorsToggleGroup.transform).GetComponent<TextToggle>();
            // set color toggle group
            colorToggle.toggleGroup = colorsToggleGroup;
            // activate toggle
            colorToggle.gameObject.SetActive(true);
            // set color
            colorToggle.GetComponentInChildren<RawImage>().color = color;
        }
        // preselect first toggle by simulating mouse click on it
        colorsToggleGroup.GetComponentsInChildren<TextToggle>()[0].ActOnLeftMouseClick();
        // activate this menu
        gameObject.SetActive(true);
    }

    void Close()
    {
        // remove listeners attached to this button
        saveButton.OnClick.RemoveAllListeners();
        cancelButton.OnClick.RemoveAllListeners();
        // remove colors
        foreach(TextToggle textToggle in colorsToggleGroup.GetComponentsInChildren<TextToggle>())
        {
            Destroy(textToggle.gameObject);
        }
        // disable this menu
        gameObject.SetActive(false);
    }

    public Color GetSelectedColor()
    {
        return colorsToggleGroup.GetSelectedToggle().GetComponentInChildren<RawImage>().color;
    }
}
