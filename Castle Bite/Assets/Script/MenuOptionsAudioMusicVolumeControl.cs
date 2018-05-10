using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsAudioMusicVolumeControl : MonoBehaviour, IDragHandler
{
    Text txt;
    Slider sld;

    private void Start()
    {
        txt = transform.parent.gameObject.GetComponentInChildren<Text>();
        sld = gameObject.GetComponent<Slider>();
        //Adds a listener to the main slider and invokes a method when the value changes.
        sld.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void OnDrag(PointerEventData eventData)
    {
        ValueChangeCheck();
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        txt.text = sld.value.ToString();
    }
}
