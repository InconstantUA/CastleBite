using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitBuffIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    UnitBuff unitBuff;
    [SerializeField]
    int totalDuration;
    [SerializeField]
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

    public UnitBuff GetUnitBuff()
    {
        return unitBuff;
    }

    public int GetCurrentDuration()
    {
        return currentDuration;
    }

    public void DecrementCurrentDuration()
    {
        currentDuration -= 1;
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
        // Block mouse input
        // InputBlocker inputBlocker = transform.root.Find("MiscUI/InputBlocker").GetComponent<InputBlocker>();
        InputBlocker.Instance.SetActive(true);
        // Fade
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = backgroundImage.color;
            c.a = f;
            backgroundImage.color = c;
            yield return new WaitForSeconds(.05f);
        }
        // Unblock mouse input
        InputBlocker.Instance.SetActive(false);
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
        Destroy(gameObject);
    }

    public void SetActiveAdvance(bool doActivate)
    {
        // CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).Queue;
        if (doActivate)
        {
            gameObject.SetActive(true);
            //StartCoroutine("FadeBackground");
            CoroutineQueueManager.Run(FadeBackground());
            // reset currentDuration
            currentDuration = totalDuration;
        }
        else
        {
            // fade away foreground text and at the end of fade destroy buff
            //StartCoroutine("FadeForegroundAndDestroyBuff");
            CoroutineQueueManager.Run(FadeForegroundAndDestroyBuff());
        }
    }

}
