using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable;
    public readonly int maxModifiers = 4;

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
                // unit is leader, set leadership values
                SetUnitLeadershipInfo(partyUnit);
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
            // Fill in information about unique power modifiers
            FillInUniquePowerModifiersInformation(partyUnit);
            // Fill in description
            transform.Find("Panel/UnitDescription/Value").GetComponent<Text>().text = partyUnit.GetFullDescription().ToString();
        }
    }

    string bonusPreviewStyleStart = "<color=green><size=12>+";
    string bonusPreviewStyleEnd = "</size></color>";

    public void SetHealthPreview(int currentHealth, int maxHealth, int maxHealthBonus)
    {
        // verify if max health bonus is not zero (this is possible, when we downgrade to the initial point)
        if (0 == maxHealthBonus)
        {
            // do not display + info
            transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
        else
        {
            // display + bonus info
            transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString() + bonusPreviewStyleStart + maxHealthBonus.ToString() + bonusPreviewStyleEnd;
        }
    }

    public void SetAbilityPowerPreview(int power, int powerBonus)
    {
        // verify if max health bonus is not zero (this is possible, when we downgrade to the initial point)
        if (0 == powerBonus)
        {
            // do not display + info
            transform.Find("Panel/UnitPower/Value").GetComponent<Text>().text = power.ToString();
        }
        else
        {
            // display + bonus info
            transform.Find("Panel/UnitPower/Value").GetComponent<Text>().text = power.ToString() + bonusPreviewStyleStart + powerBonus.ToString() + bonusPreviewStyleEnd;
        }
    }

    public void SetUniquePowerModifiersPreview(int modifierID, int power, int powerBonus, int chance, int chanceBonus)
    {
        Transform modifier = transform.Find("Panel/UniquePowerModifiersTable/Modifier" + modifierID.ToString());
        if (0 == powerBonus)
        {
            // do not display + info
            modifier.Find("Power").GetComponent<Text>().text = power.ToString();
        }
        else
        {
            // display + bonus info
            modifier.Find("Power").GetComponent<Text>().text = power.ToString() + bonusPreviewStyleStart + powerBonus.ToString() + bonusPreviewStyleEnd;
        }
        if (0 == chanceBonus)
        {
            // do not display + info
            modifier.Find("Chance").GetComponent<Text>().text = chance.ToString();
        }
        else
        {
            // display + bonus info
            modifier.Find("Chance").GetComponent<Text>().text = chance.ToString() + bonusPreviewStyleStart + chanceBonus.ToString() + bonusPreviewStyleEnd;
        }
    }

    void SetUnitLeadershipInfo(PartyUnit partyUnit)
    {
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

    public void SetLearnedSkillsBonusPreview(PartyUnit.UnitSkill learnedSkill, PartyUnit partyUnit)
    {
        switch (learnedSkill.Name)
        {
            case PartyUnit.UnitSkill.SkillName.Leadership:
                // update normal values
                SetUnitLeadershipInfo(partyUnit);
                if (learnedSkill.Level.Current > 0)
                {
                    // display bonus info
                    // get bonus
                    int learnedSkillBonus = learnedSkill.Level.Current;
                    // add bonus value to the text
                    transform.Find("Panel/UnitLeadership/Value").GetComponent<Text>().text += bonusPreviewStyleStart + learnedSkillBonus.ToString() + bonusPreviewStyleEnd;
                }
                break;
            case PartyUnit.UnitSkill.SkillName.Offence:
                break;
            case PartyUnit.UnitSkill.SkillName.Defence:
                break;
            case PartyUnit.UnitSkill.SkillName.Pathfinding:
                break;
            case PartyUnit.UnitSkill.SkillName.Scouting:
                break;
            case PartyUnit.UnitSkill.SkillName.Healing:
                break;
            case PartyUnit.UnitSkill.SkillName.DeathResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.FireResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.WaterResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.MindResistance:
                break;
            default:
                Debug.LogError("Unknown skill: " + learnedSkill.Name);
                break;
        }
    }

    void ActivateModifier(string modifierUIName, UniquePowerModifier uniquePowerModifier)
    {
        Transform modifier = transform.Find("Panel/UniquePowerModifiersTable/" + modifierUIName);
        modifier.Find("Name").GetComponent<Text>().text = uniquePowerModifier.GetDisplayName();
        modifier.Find("Power").GetComponent<Text>().text = uniquePowerModifier.Power.ToString();
        modifier.Find("Duration").GetComponent<Text>().text = uniquePowerModifier.Duration.ToString();
        modifier.Find("Chance").GetComponent<Text>().text = uniquePowerModifier.Chance.ToString();
        modifier.Find("Source").GetComponent<Text>().text = uniquePowerModifier.Source.ToString();
        modifier.Find("Origin").GetComponent<Text>().text = uniquePowerModifier.Origin.ToString();
    }

    void DeactivateModifier(string modifierUIName)
    {
        // transform.Find("Panel/UniquePowerModifiersTable/" + modifierUIName).gameObject.SetActive(false);
        // keep modifier there, but with empty text, so that rows in the table have the same spacing
        // and start from header
        Transform modifier = transform.Find("Panel/UniquePowerModifiersTable/" + modifierUIName);
        modifier.Find("Name").GetComponent<Text>().text = "";
        modifier.Find("Power").GetComponent<Text>().text = "";
        modifier.Find("Duration").GetComponent<Text>().text = "";
        modifier.Find("Chance").GetComponent<Text>().text = "";
        modifier.Find("Source").GetComponent<Text>().text = "";
        modifier.Find("Origin").GetComponent<Text>().text = "";
    }

    void FillInUniquePowerModifiersInformation(PartyUnit partyUnit)
    {
        // get Unique power modifiers
        UniquePowerModifier[] uniquePowerModifiers = partyUnit.GetComponentsInChildren<UniquePowerModifier>();
        Transform uniquePowerModifiersTable = transform.Find("Panel/UniquePowerModifiersTable");
        if (uniquePowerModifiers.Length > 0)
        {
            // Activate unique power modifiers table
            uniquePowerModifiersTable.gameObject.SetActive(true);
            // Activate and fill in or deactivate power modifiers
            for (int i = 1; i <= maxModifiers; i++)
            {
                // Activate first modifier
                if (i <= uniquePowerModifiers.Length)
                {
                    // activate first modifier
                    ActivateModifier(("Modifier" + i.ToString()), uniquePowerModifiers[i - 1]);
                }
                else
                {
                    DeactivateModifier(("Modifier" + i.ToString()));
                }
            }
        }
        else
        {
            // Dectivate unique power modifiers table
            uniquePowerModifiersTable.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (interactable)
        {
            if (Input.GetMouseButtonUp(1))
            {
                // on right mouse click
                // deactivate unit info
                gameObject.SetActive(false);
            }
        }
    }

}
