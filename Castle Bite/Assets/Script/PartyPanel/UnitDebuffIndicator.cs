using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitDebuffIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    PartyUnit.UnitDebuff unitDeuff;
    [SerializeField]
    int totalDuration;
    int currentDuration;
    Text symbol;
    AdditionalInfo additionalInfo;
    Image backgroundImage;

    private void Awake()
    {
        symbol = GetComponent<Text>();
        additionalInfo = GetComponent<AdditionalInfo>();
        backgroundImage = transform.Find("Background").GetComponent<Image>();
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

    IEnumerator FadeBackground()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = backgroundImage.color;
            c.a = f;
            backgroundImage.color = c;
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator FadeForegroundAndDestroyBuff()
    {
        // Fade Foreground
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = symbol.color;
            c.a = f;
            symbol.color = c;
            yield return new WaitForSeconds(.05f);
        }
        // Destroy buff
    }

    public void SetActiveAdvance(bool doActivate)
    {
        if (doActivate)
        {
            gameObject.SetActive(true);
            StartCoroutine("FadeBackground");
        }
        else
        {
            // fade away foreground text and at the end of fade destroy buff
            StartCoroutine("FadeForegroundAndDestroyBuff");
        }
    }

}
