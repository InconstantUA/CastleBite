using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsVideoFontSizeControl : MonoBehaviour, IDragHandler {
    Text txt;
    Slider sld;

    private void Start()
    {
        txt = transform.parent.gameObject.GetComponentInChildren<Text>();
        sld = gameObject.GetComponent<Slider>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        txt.text = sld.value.ToString();
        txt.fontSize = (int)sld.value;
    }
}
