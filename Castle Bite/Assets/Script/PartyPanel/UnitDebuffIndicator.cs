using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class UnitDebuffIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    UnitDebuff unitDebuff;
    [SerializeField]
    int totalDuration;
    [SerializeField]
    int currentDuration;
    [SerializeField]
    int power;
    Text symbol;
    AdditionalInfo additionalInfo;
    Image backgroundImage;
    UniquePowerModifierConfig appliedUniquePowerModifier;

    public int CurrentDuration
    {
        get
        {
            return currentDuration;
        }

        set
        {
            currentDuration = value;
        }
    }

    public int TotalDuration
    {
        get
        {
            return totalDuration;
        }

        set
        {
            totalDuration = value;
        }
    }

    private void Awake()
    {
        symbol = GetComponent<Text>();
        additionalInfo = GetComponent<AdditionalInfo>();
        backgroundImage = transform.Find("Background").GetComponent<Image>();
    }

    public UnitDebuff GetUnitDebuff()
    {
        return unitDebuff;
    }

    public int GetCurrentDuration()
    {
        return CurrentDuration;
    }

    public void DecrementCurrentDuration()
    {
        CurrentDuration -= 1;
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



    public IEnumerator TriggerDebuff(PartyUnitUI dstUnitUI)
    {
        // Trigger debuff within unit
        dstUnitUI.ApplyDestructiveAbility(dstUnitUI.LPartyUnit.GetDebuffDamageDealt(appliedUniquePowerModifier));
        // Proceed if unit is still alive
        if (UnitStatus.Dead != dstUnitUI.LPartyUnit.UnitStatus)
        {
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
                dstUnitUI.FadeUnitCellInfo(f);
                yield return new WaitForSeconds(.1f); // note: timing should be the same as for FadeUnitCellInfo function
            }
            // Verify if it has timed out;
            if (GetCurrentDuration() == 0)
            {
                // buff has timed out
                // deactivate it (it will be destroyed at the end of animation)
                SetActiveAdvance(false);
                // deactivate it in unit properties too
                dstUnitUI.LPartyUnit.UnitDebuffs[(int)GetUnitDebuff()] = UnitDebuff.None;
            }
        }
        else
        {
            // all debuffs should be already removed by SetUnitStatus(status)
            // Unit cannot move any more
            // And even if it will be resurected, then his cannot move during this turn
            dstUnitUI.LPartyUnit.HasMoved = true;
            // Fade unit cell info
            for (float f = 1f; f >= 0; f -= 0.1f)
            {
                dstUnitUI.FadeUnitCellInfo(f);
                yield return new WaitForSeconds(.1f); // note: timing should be the same as for FadeUnitCellInfo function
            }
        }
    }

    void FillInAdditionalInfo(UniquePowerModifierConfig uniquePowerModifier, PartyUnit partyUnit)
    {
        string[] infoLines = additionalInfo.GetLines();
        // line 1(0 index in array) already filled in in prefab
        // fill in next lines
        infoLines[1] = "Damage type: " + uniquePowerModifier.UpmSource.ToString();
        infoLines[2] = "Damage dealt: " + Math.Abs(uniquePowerModifier.GetUpmCurrentPower(partyUnit.StatsUpgradesCount)).ToString();
        if (uniquePowerModifier.UpmDurationMax >= 2)
        {
            // duration is 2 or more
            // add s in turns word
            infoLines[3] = "Duration: " + uniquePowerModifier.UpmDurationMax.ToString() + " turns";
        }
        else
        {
            // when duration is 1 turn
            // do not add s in turn word
            infoLines[3] = "Duration: " + uniquePowerModifier.UpmDurationMax.ToString() + " turn";
        }
    }

    // uniquePowerModifier parameter is optional on deactivation
    public void SetActiveAdvance(bool doActivate, UniquePowerModifierConfig uniquePowerModifier = null, PartyUnit partyUnit = null)
    {
        // CoroutineQueue queue = transform.root.GetComponentInChildren<UIManager>().GetComponentInChildren<BattleScreen>(true).Queue;
        if (doActivate)
        {
            // Activate object
            gameObject.SetActive(true);
            // Save appliedUniquePowerModifier
            appliedUniquePowerModifier = uniquePowerModifier;
            // Fill in additionalInfo
            FillInAdditionalInfo(uniquePowerModifier, partyUnit);
            // Start animation
            CoroutineQueueManager.Run(FadeBackground());
            //StartCoroutine("FadeBackground");
            // reset currentDuration
            CurrentDuration = TotalDuration;
        }
        else
        {
            // fade away foreground text and at the end of fade destroy buff
            CoroutineQueueManager.Run(FadeForegroundAndDestroyBuff());
            //StartCoroutine("FadeForegroundAndDestroyBuff");
        }
    }

}
