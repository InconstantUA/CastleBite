using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class UnitInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable;
    public readonly int maxModifiers = 4;

    string baseStatPreviewStyleStart = "<color=#606060>";
    string baseStatPreviewStyleEnd = "</color>";

    string statsBonusPreviewStyleStart = "<color=green>";
    string statsBonusPreviewStyleEnd = "</color>";

    string skillBonusPreviewStyleStart = "<color=blue>";
    string skillBonusPreviewStyleEnd = "</color>";

    string cityBonusPreviewStyleStart = "<color=white>";
    string cityBonusPreviewStyleEnd = "</color>";

    string statusBonusPreviewStyleStart = "<color=yellow>";
    string statusBonusPreviewStyleEnd = "</color>";

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
            // Verify if unit is leader
            if (partyUnit.GetIsLeader())
            {
                // unit is leader
                // Fill in leadership information
                SetUnitLeadershipInfo(partyUnit);
                // Set unit move points info
                SetUnitMovePointsInfo(partyUnit);
                // Set scouting range info
                SetUnitScoutingRangeInfo(partyUnit);
            }
            else
            {
                // unit is common unit
                // deactivate leadership info
                transform.Find("Panel/LeaderAttributes").gameObject.SetActive(false);
            }
            // Fill in unit health current and max
            SetUnitHealthInfo(partyUnit);
            // Fill in unit effective defense including additional defense mondifiers:
            SetUnitDefenseInfo(partyUnit);
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
            // Fill in unit initiative
            transform.Find("Panel/UnitInitiative/Value").GetComponent<Text>().text = partyUnit.GetInitiative().ToString();
            // Fill in unit ability
            transform.Find("Panel/UnitAbility/Name").GetComponent<Text>().text = partyUnit.GetAbility().ToString();
            // Fill in unit power
            SetUnitPowerInfo(partyUnit);
            // Fill in unit power source
            transform.Find("Panel/AbilityParameters/Source").GetComponent<Text>().text = partyUnit.GetPowerSource().ToString();
            // Fill in unit distance
            transform.Find("Panel/AbilityParameters/Distance").GetComponent<Text>().text = partyUnit.GetPowerDistance().ToString();
            // Fill in unit power scope
            transform.Find("Panel/AbilityParameters/Scope").GetComponent<Text>().text = partyUnit.GetPowerScope().ToString();
            // Fill in information about unique power modifiers
            FillInUniquePowerModifiersInformation(partyUnit);
            // Fill in description
            transform.Find("Panel/UnitDescription/Value").GetComponent<Text>().text = partyUnit.GetFullDescription().ToString();
        }
    }

    public void SetHealthPreview(int currentHealth, int maxHealth, int maxHealthBonus)
    {
        Text healthText = transform.Find("Panel/UnitHealth/Value").GetComponent<Text>();
        // display without bonus
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        // show bonuses calculation
        healthText.text += "(";
        // show original health without bonuses
        healthText.text += maxHealth - maxHealthBonus;
        // show health bonus
        AddBonusInfoToText(healthText, maxHealthBonus, statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
        healthText.text += ")";
        //// verify if max health bonus is not zero (this is possible, when we downgrade to the initial point)
        //if (0 == maxHealthBonus)
        //{
        //    // do not display + info
        //    transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString();
        //}
        //else
        //{
        //    // display + bonus info
        //    transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString() + statsBonusPreviewStyleStart + maxHealthBonus.ToString() + statsBonusPreviewStyleEnd;
        //}
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
            modifier.Find("Power").GetComponent<Text>().text = power.ToString() + statsBonusPreviewStyleStart + "+" + powerBonus.ToString() + statsBonusPreviewStyleEnd;
        }
        if (0 == chanceBonus)
        {
            // do not display + info
            modifier.Find("Chance").GetComponent<Text>().text = chance.ToString();
        }
        else
        {
            // display + bonus info
            modifier.Find("Chance").GetComponent<Text>().text = chance.ToString() + statsBonusPreviewStyleStart + "+" + chanceBonus.ToString() + statsBonusPreviewStyleEnd;
        }
    }

    void SetUnitLeadershipInfo(PartyUnit partyUnit)
    {
        transform.Find("Panel/LeaderAttributes").gameObject.SetActive(true);
        // verify if unit is member of party
        // structure 5PartyPanel-4Row-3Cell-2UnitSlot-1UnitCanvas-Unit
        PartyPanel partyPanel = partyUnit.transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
        if (partyPanel)
        {
            // set current and max leadership
            int currentLeadership = partyPanel.GetNumberOfPresentUnits() - 1;
            transform.Find("Panel/LeaderAttributes/Values/Leadership").GetComponent<Text>().text = currentLeadership.ToString() + "/" + partyUnit.GetLeadership().ToString();
        }
        else
        {
            // set only max leadership
            transform.Find("Panel/LeaderAttributes/Values/Leadership").GetComponent<Text>().text = partyUnit.GetLeadership().ToString();
        }
        if (partyUnit.GetLeadershipSkillBonus() > 0)
        {
            // add bonus value to the text
            transform.Find("Panel/LeaderAttributes/Values/Leadership").GetComponent<Text>().text += skillBonusPreviewStyleStart + "+" + partyUnit.GetLeadershipSkillBonus().ToString() + skillBonusPreviewStyleEnd;
        }
    }

    void AddBonusInfoToText(Text statText, int skillBonus, string styleStart, string styleEnd)
    {
        // verify if skill bonus is higher than 0
        if (skillBonus > 0)
        {
            // show skill bonus
            statText.text += styleStart + "+" + skillBonus.ToString() + styleEnd;
        }
    }

    int GetStatsPowerBonus(PartyUnit partyUnit)
    {
        // get stats upgrade menu
        UpgradeUnit upgradeUnit = transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>();
        // verify if it is active now
        if (upgradeUnit.gameObject.activeInHierarchy)
        {
            //  get stats upgrade count during current upgrade session
            int statsUpgradeCount = upgradeUnit.StatsUpgradeCount;
            return partyUnit.GetPowerIncrementOnLevelUp() * statsUpgradeCount;
        }
        else
        {
            return 0;
        }
    }

    public void SetUnitPowerInfo(PartyUnit partyUnit)
    {
        // Get power text
        Text powerText = transform.Find("Panel/AbilityParameters/Power").GetComponent<Text>();
        // Display total power
        powerText.text = partyUnit.GetUnitEffectivePower().ToString();
        // Display additional modifiers between brackets
        powerText.text += "(";
        // set default unit power without bonuses (without just upgraded stats power bonus)
        powerText.text += baseStatPreviewStyleStart + (partyUnit.GetPower() - GetStatsPowerBonus(partyUnit)).ToString() + baseStatPreviewStyleEnd;
        // get and add stats power bonus if it is present
        AddBonusInfoToText(powerText, GetStatsPowerBonus(partyUnit), statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
        // get and add offence skill bonus to text if upgrade unit panel is active
        AddBonusInfoToText(powerText, partyUnit.GetOffenceSkillPowerBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
        // close brackets
        powerText.text += ")";
    }

    void SetUnitDefenseInfo(PartyUnit partyUnit)
    {
        // Get defense text
        Text attributeText = transform.Find("Panel/UnitDefense/Value").GetComponent<Text>();
        // display total defense
        attributeText.text = partyUnit.GetEffectiveDefense().ToString();
        // Display how defense is calculated
        // open brackets
        attributeText.text += "(";
        // set default unit defense without bonuses
        attributeText.text += baseStatPreviewStyleStart + partyUnit.GetDefense().ToString() + baseStatPreviewStyleEnd;
        // get and add city bonus to text
        AddBonusInfoToText(attributeText, partyUnit.GetCityDefenseBonus(), cityBonusPreviewStyleStart, cityBonusPreviewStyleEnd);
        // get and add offence skill bonus to text
        AddBonusInfoToText(attributeText, partyUnit.GetSkillDefenseBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
        // get and add status bonus (example: defense stance) to text
        AddBonusInfoToText(attributeText, (int)(partyUnit.GetStatusDefenseBonus() * (100 - partyUnit.GetTotalAdditiveDefense())), statusBonusPreviewStyleStart, statusBonusPreviewStyleEnd);
        // not needed - stats do no give defense bonus
        // close brackets
        attributeText.text += ")";
    }

    public void SetUnitMovePointsInfo(PartyUnit partyUnit)
    {
        // Get move points text
        Text attributeText = transform.Find("Panel/LeaderAttributes/Values/MovePoints").GetComponent<Text>();
        // display total move points
        attributeText.text = partyUnit.MovePointsCurrent.ToString() + "/" + partyUnit.GetEffectiveMaxMovePoints().ToString();
        // Display how move points are calculated
        // open brackets
        attributeText.text += "(";
        // set default unit  move points without bonuses
        attributeText.text += baseStatPreviewStyleStart + partyUnit.MovePointsMax.ToString() + baseStatPreviewStyleEnd;
        // get and add path finding skill bonus to text
        AddBonusInfoToText(attributeText, partyUnit.GetPathfindingSkillBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
        // close brackets
        attributeText.text += ")";
    }

    public void SetUnitScoutingRangeInfo(PartyUnit partyUnit)
    {
        // Get move points text
        Text attributeText = transform.Find("Panel/LeaderAttributes/Values/ScoutingRange").GetComponent<Text>();
        // display total move points
        attributeText.text = partyUnit.GetEffectiveScoutingRange().ToString();
        // Display how move points are calculated
        // open brackets
        attributeText.text += "(";
        // set default unit  move points without bonuses
        attributeText.text += baseStatPreviewStyleStart + partyUnit.ScoutingRange.ToString() + baseStatPreviewStyleEnd;
        // get and add skill bonus to text
        AddBonusInfoToText(attributeText, partyUnit.GetScoutingSkillBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
        // close brackets
        attributeText.text += ")";
    }

    int GetStatsHealthBonus(PartyUnit partyUnit)
    {
        // get stats upgrade menu
        UpgradeUnit upgradeUnit = transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>();
        // verify if it is active now
        if (upgradeUnit.gameObject.activeInHierarchy)
        {
            //  get stats upgrade count during current upgrade session
            int statsUpgradeCount = upgradeUnit.StatsUpgradeCount;
            return partyUnit.GetHealthMaxIncrementOnLevelUp() * statsUpgradeCount;
        }
        else
        {
            return 0;
        }
    }

    public void SetUnitHealthInfo(PartyUnit partyUnit)
    {
        // Get health value text
        Text attributeText = transform.Find("Panel/UnitHealth/Value").GetComponent<Text>();
        // Display effective current and max health
        attributeText.text = partyUnit.GetHealthCurr().ToString() + "/" + partyUnit.GetHealthMax().ToString();
        // Display how max health is calculated
        // open brackets
        attributeText.text += "(";
        // set default unit health without bonuses
        attributeText.text += baseStatPreviewStyleStart + (partyUnit.GetHealthMax() - GetStatsHealthBonus(partyUnit)).ToString() + baseStatPreviewStyleEnd;
        // get and add stats bonus to text
        AddBonusInfoToText(attributeText, GetStatsHealthBonus(partyUnit), statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
        // close brackets
        attributeText.text += ")";
        // Display effective health regeneration per day
        attributeText.text += " " + partyUnit.GetUnitEffectiveHealthRegenPerDay().ToString();
        // add per day indication
        attributeText.text += "/day";
        // Display how regen is calculated
        // open brackets
        attributeText.text += "(";
        // set default unit regen without bonuses
        attributeText.text += baseStatPreviewStyleStart + partyUnit.GetUnitEffectiveHealthRegenPerDay().ToString() + baseStatPreviewStyleEnd;
        // get and add skill bonus to text
        AddBonusInfoToText(attributeText, partyUnit.GetUnitHealSkillHealthRegenBonusPerDay(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
        // close brackets
        attributeText.text += ")";
    }

    public void SetLearnedSkillsBonusPreview(PartyUnit.UnitSkill learnedSkill, PartyUnit partyUnit)
    {
        switch (learnedSkill.Name)
        {
            case PartyUnit.UnitSkill.SkillName.Leadership:
                // update normal values
                SetUnitLeadershipInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.Offence:
                //SetUnitPowerInfo(partyUnit, learnedSkill);
                SetUnitPowerInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.Defense:
                SetUnitDefenseInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.Pathfinding:
                SetUnitMovePointsInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.Scouting:
                SetUnitScoutingRangeInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.Healing:
                SetUnitHealthInfo(partyUnit);
                break;
            case PartyUnit.UnitSkill.SkillName.DeathResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.FireResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.WaterResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.MindResistance:
                break;
            case PartyUnit.UnitSkill.SkillName.ShardAura:
                break;
            case PartyUnit.UnitSkill.SkillName.LifelessContinuation:
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
