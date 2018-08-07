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
            if (partyUnit.GivenName != partyUnit.UnitName)
            {
                transform.Find("UnitName").GetComponent<Text>().text = partyUnit.GivenName;
                // activate unit class, because unit has his own name
                transform.Find("NamedUnitClass").gameObject.SetActive(true);
                transform.Find("NamedUnitClass").GetComponent<Text>().text = partyUnit.UnitName;
            }
            else
            {
                transform.Find("UnitName").GetComponent<Text>().text = partyUnit.UnitName;
                // deactivate unit class, because it is the same as name
                transform.Find("NamedUnitClass").gameObject.SetActive(false);
            }
            // Fill in unit level
            transform.Find("Panel/UnitLevel/Value").GetComponent<Text>().text = partyUnit.UnitLevel.ToString();
            // Fill in unit current and experience required to reach next level
            transform.Find("Panel/UnitExperience/Value").GetComponent<Text>().text = partyUnit.UnitExperience.ToString() + "/" + partyUnit.UnitExperienceRequiredToReachNewLevel.ToString();
            // Verify if unit is leader
            if (partyUnit.IsLeader)
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
            SetUnitResistancesInfo(partyUnit);
            // Fill in immunities
            // .. - replaced with resistance 100% = immune
            //string immunities = "";
            //// verify if there are any rezistances
            //if (partyUnit.GetImmunities().Length > 0)
            //{
            //    for (int i = 0; i < partyUnit.GetImmunities().Length; i++)
            //    {
            //        //  UnitPowerSource resistance in partyUnit.GetResistances())
            //        // if this is first resistance then do not add "/" at the end
            //        if (0 == i)
            //        {
            //            immunities = partyUnit.GetImmunities()[i].ToString();
            //        }
            //        else
            //        {
            //            immunities = immunities + ", " + partyUnit.GetImmunities()[i].ToString();
            //        }
            //    }
            //}
            //transform.Find("Panel/UnitImmunities/Value").GetComponent<Text>().text = immunities;
            // Fill in unit initiative
            transform.Find("Panel/UnitInitiative/Value").GetComponent<Text>().text = partyUnit.UnitInitiative.ToString();
            // Fill in unit ability
            transform.Find("Panel/UnitAbility/Name").GetComponent<Text>().text = partyUnit.UnitAbility.ToString();
            // Fill in unit power
            SetUnitPowerInfo(partyUnit);
            // Fill in unit power source
            transform.Find("Panel/AbilityParameters/Source").GetComponent<Text>().text = partyUnit.UnitPowerSource.ToString();
            // Fill in unit distance
            transform.Find("Panel/AbilityParameters/Distance").GetComponent<Text>().text = partyUnit.UnitPowerDistance.ToString();
            // Fill in unit power scope
            transform.Find("Panel/AbilityParameters/Scope").GetComponent<Text>().text = partyUnit.UnitPowerScope.ToString();
            // Fill in information about unique power modifiers
            FillInUniquePowerModifiersInformation(partyUnit);
            // Fill in description
            transform.Find("Panel/UnitDescription/Value").GetComponent<Text>().text = partyUnit.UnitFullDescription.ToString();
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

    int GetCurrentNumberOfUnitsInParty(PartyUnit partyUnit)
    {
        // verify if unit is member of party
        // structure 5PartyPanel-4Row-3Cell-2UnitSlot-1UnitCanvas-Unit
        PartyPanel partyPanel = null;
        if (partyUnit.transform.parent)
            if (partyUnit.transform.parent.parent)
                if (partyUnit.transform.parent.parent.parent)
                    if (partyUnit.transform.parent.parent.parent.parent)
                        if (partyUnit.transform.parent.parent.parent.parent.parent)
                            partyPanel = partyUnit.transform.parent.parent.parent.parent.parent.GetComponent<PartyPanel>();
        if (partyPanel != null)
        {
            return partyPanel.GetNumberOfPresentUnits() - 1;
        }
        else
        {
            return 0;
        }
    }

    void SetUnitLeadershipInfo(PartyUnit partyUnit)
    {
        transform.Find("Panel/LeaderAttributes").gameObject.SetActive(true);
        // get attribute text
        Text attributeText = transform.Find("Panel/LeaderAttributes/Values/Leadership").GetComponent<Text>();
        // set current and max leadership
        attributeText.text = GetCurrentNumberOfUnitsInParty(partyUnit).ToString() + "/" + partyUnit.GetEffectiveLeadership().ToString();
        // verify if base leadership does not equal to effective leadership
        if (partyUnit.GetEffectiveLeadership() != partyUnit.UnitLeadership)
        {
            // Display how leadership is calculated
            attributeText.text += "(";
            // set default unit power without bonuses (without just upgraded stats power bonus)
            attributeText.text += baseStatPreviewStyleStart + partyUnit.UnitLeadership.ToString() + baseStatPreviewStyleEnd;
            // get and add skill bonus to text if upgrade unit panel is active
            AddBonusInfoToText(attributeText, partyUnit.GetLeadershipSkillBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
            // close brackets
            attributeText.text += ")";
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
            return partyUnit.UnitPowerIncrementOnLevelUp * statsUpgradeCount;
        }
        else
        {
            return 0;
        }
    }

    int GetUnitStatsBonusHealthRegenPerDay(PartyUnit partyUnit)
    {
        // get stats upgrade menu
        UpgradeUnit upgradeUnit = transform.root.Find("MiscUI/UpgradeUnit").GetComponent<UpgradeUnit>();
        // verify if it is active now
        if (upgradeUnit.gameObject.activeInHierarchy)
        {
            //  get stats upgrade count during current upgrade session
            int statsUpgradeCount = upgradeUnit.StatsUpgradeCount;
            float baseRegenMultiplier = (float)partyUnit.UnitHealthRegenPercent / 100f;
            return (int)Math.Floor(partyUnit.UnitHealthMaxIncrementOnLevelUp * statsUpgradeCount * baseRegenMultiplier);
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
        // Display effective power
        powerText.text = partyUnit.GetUnitEffectivePower().ToString();
        // get base power
        int basePower = partyUnit.UnitPower - GetStatsPowerBonus(partyUnit);
        // verify if effective power does not equal to base power
        if (partyUnit.GetUnitEffectivePower() != basePower)
        {
            // Display additional modifiers between brackets
            powerText.text += "(";
            // set default unit power without bonuses (without just upgraded stats power bonus)
            powerText.text += baseStatPreviewStyleStart + basePower.ToString() + baseStatPreviewStyleEnd;
            // get and add stats power bonus if it is present
            AddBonusInfoToText(powerText, GetStatsPowerBonus(partyUnit), statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
            // get and add offence skill bonus to text if upgrade unit panel is active
            AddBonusInfoToText(powerText, partyUnit.GetOffenceSkillPowerBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
            // close brackets
            powerText.text += ")";
        }
    }

    void SetUnitDefenseInfo(PartyUnit partyUnit)
    {
        // Get defense text
        Text attributeText = transform.Find("Panel/UnitDefense/Value").GetComponent<Text>();
        // display effective defense
        attributeText.text = partyUnit.GetEffectiveDefense().ToString();
        // verify if base unit defense does not equal to effective defense
        if (partyUnit.GetEffectiveDefense() != partyUnit.UnitDefense)
        {
            // Display how defense is calculated
            // open brackets
            attributeText.text += "(";
            // set base unit defense without bonuses
            attributeText.text += baseStatPreviewStyleStart + partyUnit.UnitDefense.ToString() + baseStatPreviewStyleEnd;
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
    }

    public void SetUnitMovePointsInfo(PartyUnit partyUnit)
    {
        // Get move points text
        Text attributeText = transform.Find("Panel/LeaderAttributes/Values/MovePoints").GetComponent<Text>();
        // display effective move points
        attributeText.text = partyUnit.MovePointsCurrent.ToString() + "/" + partyUnit.GetEffectiveMaxMovePoints().ToString();
        // verify if base move points do not equal to effective move points
        if (partyUnit.GetEffectiveMaxMovePoints() != partyUnit.MovePointsMax)
        {
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
    }

    public void SetUnitScoutingRangeInfo(PartyUnit partyUnit)
    {
        // Get move points text
        Text attributeText = transform.Find("Panel/LeaderAttributes/Values/ScoutingRange").GetComponent<Text>();
        // display effective scouting range
        attributeText.text = partyUnit.GetEffectiveScoutingRange().ToString();
        // verify if base scouting range does not equal to effective scouting range
        if (partyUnit.GetEffectiveScoutingRange() != partyUnit.ScoutingRange)
        {
            // Display how scouting range is calculated
            // open brackets
            attributeText.text += "(";
            // set base unit move points without bonuses
            attributeText.text += baseStatPreviewStyleStart + partyUnit.ScoutingRange.ToString() + baseStatPreviewStyleEnd;
            // get and add skill bonus to text
            AddBonusInfoToText(attributeText, partyUnit.GetScoutingSkillBonus(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
            // close brackets
            attributeText.text += ")";
        }
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
            return partyUnit.UnitHealthMaxIncrementOnLevelUp * statsUpgradeCount;
        }
        else
        {
            return 0;
        }
    }

    //public void SetHealthPreview(int currentHealth, int maxHealth, int maxHealthBonus)
    //{
    //    Text healthText = transform.Find("Panel/UnitHealth/Value").GetComponent<Text>();
    //    // display without bonus
    //    healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    //    // show bonuses calculation
    //    healthText.text += "(";
    //    // show original health without bonuses
    //    healthText.text += maxHealth - maxHealthBonus;
    //    // show health bonus
    //    AddBonusInfoToText(healthText, maxHealthBonus, statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
    //    healthText.text += ")";
    //    //// verify if max health bonus is not zero (this is possible, when we downgrade to the initial point)
    //    //if (0 == maxHealthBonus)
    //    //{
    //    //    // do not display + info
    //    //    transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString();
    //    //}
    //    //else
    //    //{
    //    //    // display + bonus info
    //    //    transform.Find("Panel/UnitHealth/Value").GetComponent<Text>().text = currentHealth.ToString() + "/" + maxHealth.ToString() + statsBonusPreviewStyleStart + maxHealthBonus.ToString() + statsBonusPreviewStyleEnd;
    //    //}
    //}

    public void SetUnitHealthInfo(PartyUnit partyUnit)
    {
        // Get health value text
        Text attributeText = transform.Find("Panel/UnitHealth/Value").GetComponent<Text>();
        // Display effective current and max health
        attributeText.text = partyUnit.UnitHealthCurr.ToString() + "/" + partyUnit.UnitHealthMax.ToString();
        // get base max health
        int baseMaxHealth = partyUnit.UnitHealthMax - GetStatsHealthBonus(partyUnit);
        // verify if effective resistance does not equal base resistance
        if (baseMaxHealth != partyUnit.UnitHealthMax)
        {
            // Display how max health is calculated
            // open brackets
            attributeText.text += "(";
            // set default unit health without bonuses
            attributeText.text += baseStatPreviewStyleStart + (baseMaxHealth).ToString() + baseStatPreviewStyleEnd;
            // get and add stats bonus to text
            AddBonusInfoToText(attributeText, GetStatsHealthBonus(partyUnit), statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
            // close brackets
            attributeText.text += ")";
        }
        // Display effective health regeneration per day
        attributeText.text += " " + partyUnit.GetUnitEffectiveHealthRegenPerDay().ToString();
        // add per day indication
        attributeText.text += "/day";
        // get base health regen
        int baseHealthRegen = partyUnit.GetUnitHealthRegenPerDay() - GetUnitStatsBonusHealthRegenPerDay(partyUnit);
        // verify if base health regen does not equal effecitve health regen
        if (partyUnit.GetUnitEffectiveHealthRegenPerDay() != baseHealthRegen)
        {
            // Display how regen is calculated
            // open brackets
            attributeText.text += "(";
            // set base unit regen without bonuses
            attributeText.text += baseStatPreviewStyleStart + baseHealthRegen.ToString() + baseStatPreviewStyleEnd;
            // get and add stats bonus regen
            //attributeText.text += statsBonusPreviewStyleStart + "+" + GetUnitStatsBonusHealthRegenPerDay(partyUnit) + statsBonusPreviewStyleEnd;
            AddBonusInfoToText(attributeText, GetUnitStatsBonusHealthRegenPerDay(partyUnit), statsBonusPreviewStyleStart, statsBonusPreviewStyleEnd);
            // get and add skill bonus to text
            AddBonusInfoToText(attributeText, partyUnit.GetUnitHealSkillHealthRegenBonusPerDay(), skillBonusPreviewStyleStart, skillBonusPreviewStyleEnd);
            // close brackets
            attributeText.text += ")";
        }
    }

    public void SetUnitResistancesInfo(PartyUnit partyUnit)
    {
        // initialise resistances string
        string resistances = "";
        string resistance = "";
        UnitPowerSource source; // source = resistance
        bool firstResistanceInAString = true;
        // loop throug all power sources and find if unit has resistances
        for (int i = 0; i < (int)UnitPowerSource.None; i++) // None is the last element
        {
            source = (UnitPowerSource)i;
            // get effective resistance
            int effectiveResistance = partyUnit.GetUnitEffectiveResistance(source);
            // verify if effective resistance is higher than 0 and it is not equal to base resistance 
            if (effectiveResistance > 0)
            {
                // form resistance string
                // set resistance name
                resistance = source.ToString();
                // add effective resistance to string
                resistance += ":" + effectiveResistance;
                // get base resistance
                int baseResistance = partyUnit.GetUnitBaseResistance(source);
                // verify if effective resistance does not equal base resistance
                if (baseResistance != effectiveResistance)
                {
                    // Display how resistance is calculated
                    // open brackets
                    resistance += "(";
                    // verify if base resistance is higher than 0
                    if (baseResistance > 0)
                    {
                        // add base resistance to string
                        resistance += baseStatPreviewStyleStart + baseResistance + baseStatPreviewStyleEnd;
                    }
                    // get skill resistance
                    int skillResistance = partyUnit.GetUnitResistanceSkillBonus(source);
                    // verify if skill resistance is higher than 0
                    if (skillResistance > 0)
                    {
                        // add skill resistance to string
                        resistance += skillBonusPreviewStyleStart + "+" + skillResistance + skillBonusPreviewStyleEnd;
                    }
                    // close brackets
                    resistance += ")";
                }
                // verify if this is first resistace in a string
                if (firstResistanceInAString)
                {
                    // this is first resistance
                    // add it to the list of resistances
                    resistances = resistance;
                    firstResistanceInAString = false;
                }
                else
                {
                    // this is not first resistance
                    // add it to the list of resistances after ', ' separator
                    resistances += ", " + resistance;
                }
            }
            //for (int i = 0; i < partyUnit.Resistances.Length; i++)
            //{
            //    source = partyUnit.Resistances[i].source;
            //    // set resistance name
            //    resistance = source.ToString();
            //    // get hero skill resistance bonus for the same resistance type
            //    int resistanceSkillBonus = partyUnit.GetUnitResistanceSkillBonus(source);
            //    // verify if hero knows resistance skill with the same type
            //    if (resistanceSkillBonus != 0)
            //    {
            //        // add skill bonus to the total value
            //        resistance += ":" + baseStatPreviewStyleStart + partyUnit.Resistances[i].percent.ToString() + baseStatPreviewStyleEnd;
            //    }
            //    // get and add base resistance to string
            //    resistance += ":" + baseStatPreviewStyleStart + partyUnit.Resistances[i].percent.ToString() + baseStatPreviewStyleEnd;
            //}
        }
        transform.Find("Panel/UnitResistances/Value").GetComponent<Text>().text = resistances;
    }

    public void SetLearnedSkillsBonusPreview(UnitSkill learnedSkill, PartyUnit partyUnit)
    {
        switch (learnedSkill.mName)
        {
            case UnitSkill.SkillName.Leadership:
                // update normal values
                SetUnitLeadershipInfo(partyUnit);
                break;
            case UnitSkill.SkillName.Offence:
                //SetUnitPowerInfo(partyUnit, learnedSkill);
                SetUnitPowerInfo(partyUnit);
                break;
            case UnitSkill.SkillName.Defense:
                SetUnitDefenseInfo(partyUnit);
                break;
            case UnitSkill.SkillName.Pathfinding:
                SetUnitMovePointsInfo(partyUnit);
                break;
            case UnitSkill.SkillName.Scouting:
                SetUnitScoutingRangeInfo(partyUnit);
                break;
            case UnitSkill.SkillName.Healing:
                SetUnitHealthInfo(partyUnit);
                break;
            case UnitSkill.SkillName.DeathResistance:
            case UnitSkill.SkillName.FireResistance:
            case UnitSkill.SkillName.WaterResistance:
            case UnitSkill.SkillName.MindResistance:
                SetUnitResistancesInfo(partyUnit);
                break;
            case UnitSkill.SkillName.ShardAura:
                break;
            case UnitSkill.SkillName.LifelessContinuation:
                break;
            default:
                Debug.LogError("Unknown skill: " + learnedSkill.mName);
                break;
        }
    }

    void ActivateModifier(string modifierUIName, UniquePowerModifier uniquePowerModifier)
    {
        Transform modifier = transform.Find("Panel/UniquePowerModifiersTable/" + modifierUIName);
        modifier.Find("Name").GetComponent<Text>().text = uniquePowerModifier.GetDisplayName();
        modifier.Find("Power").GetComponent<Text>().text = uniquePowerModifier.upmPower.ToString();
        modifier.Find("Duration").GetComponent<Text>().text = uniquePowerModifier.upmDuration.ToString();
        modifier.Find("Chance").GetComponent<Text>().text = uniquePowerModifier.upmChance.ToString();
        modifier.Find("Source").GetComponent<Text>().text = uniquePowerModifier.upmSource.ToString();
        modifier.Find("Origin").GetComponent<Text>().text = uniquePowerModifier.upmOrigin.ToString();
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
        List<UniquePowerModifier> uniquePowerModifiers = partyUnit.UniquePowerModifiers;
        Transform uniquePowerModifiersTable = transform.Find("Panel/UniquePowerModifiersTable");
        if (uniquePowerModifiers.Count > 0)
        {
            // Activate unique power modifiers table
            uniquePowerModifiersTable.gameObject.SetActive(true);
            // Activate and fill in or deactivate power modifiers
            for (int i = 1; i <= maxModifiers; i++)
            {
                // Activate first modifier
                if (i <= uniquePowerModifiers.Count)
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
