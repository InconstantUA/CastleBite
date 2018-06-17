using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    public void ActivateAdvance(PartyUnit partyUnit)
    {
        // verify party unit is not null, if it null,
        // then user right clicked on a cell without unit
        if(partyUnit)
        {
            gameObject.SetActive(true);
            // Fill in unit name
            if (partyUnit.GetGivenName() != partyUnit.GetUnitName())
            {
                transform.Find("UnitName").GetComponent<Text>().text = partyUnit.GetGivenName();
                // activate unit class, because unit has his own name
                transform.Find("NamedUnitClass").gameObject.SetActive(true);
                transform.Find("NamedUnitClass").GetComponent<Text>().text = partyUnit.GetUnitName();
            }
            else
            {
                transform.Find("UnitName").GetComponent<Text>().text = partyUnit.GetUnitName();
                // deactivate unit class, because it is the same as name
                transform.Find("NamedUnitClass").gameObject.SetActive(false);
            }
            // Fill in unit level
            transform.Find("Panel/UnitLevel/Value").GetComponent<Text>().text = partyUnit.GetLevel().ToString();
            // Fill in unit current and experience required to reach next level
            transform.Find("Panel/UnitExperience/Value").GetComponent<Text>().text = partyUnit.GetExperience().ToString() + "/" + partyUnit.GetExperienceRequiredToReachNewLevel().ToString();
            // Fill in leadership information
            // Verify if unit is leader
            if (partyUnit.GetIsLeader())
            {
                // unit is leader
                transform.Find("Panel/UnitLeadership").gameObject.SetActive(true);
                // verify if unit is member of party
                // structure 5PartyPanel-4Row-3Cell-2UnitSlot-1UnitCanvas-Unit
                PartyPanel partyPanel = partyUnit.transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
                if (partyPanel)
                {
                    // set current and max leadership
                    int currentLeadership = partyPanel.GetNumberOfPresentUnits() - 1;
                    transform.Find("Panel/UnitLeadership/Value").GetComponent<Text>().text = currentLeadership.ToString() + "/" + partyUnit.GetLeadership().ToString();
                }
                else
                {
                    // set only max leadership
                    transform.Find("Panel/UnitLeadership/Value").GetComponent<Text>().text = partyUnit.GetLeadership().ToString();
                }
            }
            else
            {
                // unit is common unit
                // deactivate leadership info
                transform.Find("Panel/UnitLeadership").gameObject.SetActive(false);
            }
            // Fill in unit health current and max
            transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = partyUnit.GetHealthCurr().ToString() + "/" + partyUnit.GetHealthMax().ToString();
            // Fill in unit effective defence including additional defence mondifiers:
            //  - city
            //  - items
            transform.Find("Panel/UnitDefence/Value").GetComponent<Text>().text = partyUnit.GetEffectiveDefence().ToString();
            // Fill in resistances
            string resistances = "";
            // verify if there are any rezistances
            if (partyUnit.GetResistances().Length > 0)
            {
                for (int i = 0; i < partyUnit.GetResistances().Length; i++)
                {
                    //  PartyUnit.UnitPowerSource resistance in partyUnit.GetResistances())
                    // if this is first resistance then do not add "/" at the end
                    if (0 == i)
                    {
                        resistances = partyUnit.GetResistances()[i].ToString();
                    }
                    else
                    {
                        resistances = resistances + ", " + partyUnit.GetResistances()[i].ToString();
                    }
                }
            }
            transform.Find("Panel/UnitResistances/Value").GetComponent<Text>().text = resistances;
            // Fill in immunities
            string immunities = "";
            // verify if there are any rezistances
            if (partyUnit.GetImmunities().Length > 0)
            {
                for (int i = 0; i < partyUnit.GetImmunities().Length; i++)
                {
                    //  PartyUnit.UnitPowerSource resistance in partyUnit.GetResistances())
                    // if this is first resistance then do not add "/" at the end
                    if (0 == i)
                    {
                        immunities = partyUnit.GetImmunities()[i].ToString();
                    }
                    else
                    {
                        immunities = immunities + ", " + partyUnit.GetImmunities()[i].ToString();
                    }
                }
            }
            transform.Find("Panel/UnitImmunities/Value").GetComponent<Text>().text = immunities;
            // Fill in unit ability
            transform.Find("Panel/UnitAbility/Value").GetComponent<Text>().text = partyUnit.GetAbility().ToString();
            // Fill in unit power
            transform.Find("Panel/UnitPower/Value").GetComponent<Text>().text = partyUnit.GetPower().ToString();
            // Fill in unit power source
            transform.Find("Panel/UnitPowerSource/Value").GetComponent<Text>().text = partyUnit.GetPowerSource().ToString();
            // Fill in unit distance
            transform.Find("Panel/UnitPowerDistance/Value").GetComponent<Text>().text = partyUnit.GetPowerDistance().ToString();
            // Fill in unit power scope
            transform.Find("Panel/UnitPowerScope/Value").GetComponent<Text>().text = partyUnit.GetPowerScope().ToString();
            // Fill in unit initiative
            transform.Find("Panel/UnitInitiative/Value").GetComponent<Text>().text = partyUnit.GetInitiative().ToString();
            // There is no power modifiers yet, so disable it
            // ..
            transform.Find("Panel/UniquePowerModifiersTable").gameObject.SetActive(false);
            // Fill in description
            transform.Find("Panel/UnitDescription/Value").GetComponent<Text>().text = partyUnit.GetFullDescription().ToString();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate unit info
            transform.root.Find("MiscUI/UnitInfoPanel").gameObject.SetActive(false);
        }
    }

    //// Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    void Update()
    {
        // as coroutine fade away information

    }
}
