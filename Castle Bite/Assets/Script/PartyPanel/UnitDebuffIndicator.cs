using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitDebuffIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    PartyUnit.UnitDebuff unitDebuff;
    [SerializeField]
    int totalDuration;
    [SerializeField]
    int currentDuration;
    [SerializeField]
    int power;
    Text symbol;
    AdditionalInfo additionalInfo;
    Image backgroundImage;
    UniquePowerModifier appliedUniquePowerModifier;

    private void Awake()
    {
        symbol = GetComponent<Text>();
        additionalInfo = GetComponent<AdditionalInfo>();
        backgroundImage = transform.Find("Background").GetComponent<Image>();
    }

    public PartyUnit.UnitDebuff GetUnitDebuff()
    {
        return unitDebuff;
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
        Destroy(gameObject);
    }



    public IEnumerator TriggerDebuff(PartyUnit dstUnit)
    {
        // Trigger debuff within unit
        dstUnit.ApplyDestructiveAbility(dstUnit.GetDebuffDamageDealt(appliedUniquePowerModifier));
        // reset background image color to be visible
        Color cx = backgroundImage.color;
        cx.a = 1;
        backgroundImage.color = cx;
        // trigger animation
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = backgroundImage.color;
            c.a = f;
            backgroundImage.color = c;
            dstUnit.FadeUnitCellInfo(f);
            yield return new WaitForSeconds(.1f); // note: timing should be the same as for FadeUnitCellInfo function
        }
        // Verify if it has timed out;
        if (GetCurrentDuration() == 0)
        {
            // buff has timed out
            // deactivate it (it will be destroyed at the end of animation)
            SetActiveAdvance(false);
            // deactivate it in unit properties too
            dstUnit.GetUnitDebuffs()[(int)GetUnitDebuff()] = PartyUnit.UnitDebuff.None;
        }
    }

    void FillInAdditionalInfo(UniquePowerModifier uniquePowerModifier)
    {
        string[] infoLines = additionalInfo.GetLines();
        // line 1(0 index in array) already filled in in prefab
        // fill in next lines
        infoLines[1] = "Damage type: " + uniquePowerModifier.Source.ToString();
        infoLines[2] = "Damage dealt: " + uniquePowerModifier.Power.ToString();
        if (uniquePowerModifier.Duration >= 2)
        {
            // duration is 2 or more
            // add s in turns word
            infoLines[3] = "Duration: " + uniquePowerModifier.Duration.ToString() + " turns";
        }
        else
        {
            // when duration is 1 turn
            // do not add s in turn word
            infoLines[3] = "Duration: " + uniquePowerModifier.Duration.ToString() + " turn";
        }
    }

    // uniquePowerModifier parameter is optional on deactivation
    public void SetActiveAdvance(bool doActivate, UniquePowerModifier uniquePowerModifier = null)
    {
        if (doActivate)
        {
            // Activate object
            gameObject.SetActive(true);
            // Save appliedUniquePowerModifier
            appliedUniquePowerModifier = uniquePowerModifier;
            // Fill in additionalInfo
            FillInAdditionalInfo(uniquePowerModifier);
            // Start animation
            StartCoroutine("FadeBackground");
            // reset currentDuration
            currentDuration = totalDuration;
        }
        else
        {
            // fade away foreground text and at the end of fade destroy buff
            StartCoroutine("FadeForegroundAndDestroyBuff");
        }
    }

}
