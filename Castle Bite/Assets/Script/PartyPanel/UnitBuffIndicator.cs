using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitBuffIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Text symbol;
    AdditionalInfo additionalInfo;
    Image backgroundImage;

    private void Awake()
    {
        symbol = GetComponent<Text>();
        additionalInfo = GetComponent<AdditionalInfo>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (Input.GetMouseButtonDown(0))
        {
            // on left mouse click
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // on right mouse click
            // show unit info
            transform.root.Find("MiscUI/AdditionalInfoPanel").GetComponent<AdditionalInfoPanel>().ActivateAdvance(additionalInfo);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            transform.root.Find("MiscUI/AdditionalInfoPanel").gameObject.SetActive(false);
        }
    }

    IEnumerator FadeUnitCellInfo()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = backgroundImage.color;
            c.a = f;
            backgroundImage.color = c;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void SetActiveAdvance()
    {
        gameObject.SetActive(true);
        StartCoroutine("FadeUnitCellInfo");
    }

}
