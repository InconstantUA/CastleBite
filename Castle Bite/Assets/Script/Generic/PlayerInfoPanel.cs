using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField]
    BackgroundUI backgroundUI;
    [SerializeField]
    int backgroundAlfaTransparency = 222;
    [SerializeField]
    TextButton backButton;
    [SerializeField]
    Text playerNameText;
    [SerializeField]
    Text factionLeaderInfoText;
    [SerializeField]
    Text playerAgeText;
    [SerializeField]
    Text uniqueAbilityText;
    [SerializeField]
    Text battlesWonText;
    [SerializeField]
    Text goldTotalText;
    [SerializeField]
    Text goldIncomePerDayText;
    [SerializeField]
    Text manaTotalText;
    [SerializeField]
    Text manaIncomePerDayText;
    [SerializeField]
    PlayerCitiesInfoPanel citiesInfoPanel;
    [SerializeField]
    PlayerHeroesInfoPanel heroesInfoPanel;
    [SerializeField]
    PlayerAchievementsInfoPanel achievementsInfoPanel;

    void SetPlayerGeneralInfo()
    {
        // get active player
        GamePlayer activeGamePlayer = TurnsManager.Instance.GetActivePlayer();
        // set active player name
        playerNameText.text = activeGamePlayer.GivenName;
        // set active player faction
        factionLeaderInfoText.text = activeGamePlayer.Faction.ToString() + " Leader";
        // set player age
        playerAgeText.text = activeGamePlayer.GetAge();
        // set player unique ability display name information
        uniqueAbilityText.text = Array.Find(ConfigManager.Instance.UniqueAbilityConfigs, element => element.playerUniqueAbility == activeGamePlayer.PlayerUniqueAbilityData.playerUniqueAbility).displayName;
        // set active player battles won
        battlesWonText.text = activeGamePlayer.BattlesWon.ToString();
        // set total gold
        goldTotalText.text = activeGamePlayer.TotalGold.ToString();
        // set gold income per day
        goldIncomePerDayText.text = activeGamePlayer.GetTotalGoldIncomePerDay().ToString() + "/day";
        // set total mana
        manaTotalText.text = activeGamePlayer.TotalMana.ToString();
        // set mana income per day
        manaIncomePerDayText.text = activeGamePlayer.GetTotalManaIncomePerDay().ToString() + "/day";
    }

    public void SetActive(bool doActivate)
    {
        // Activate/Deactivate background
        backgroundUI.SetActive(doActivate, backgroundAlfaTransparency);
        // Activate player general info
        if (doActivate)
        {
            SetPlayerGeneralInfo();
        }
        // Activate/Deactivate cities info panel
        citiesInfoPanel.SetActive(doActivate);
        // Activate/Deactivate cities info panel
        heroesInfoPanel.SetActive(doActivate);
        // Activate/Deactivate cities info panel
        achievementsInfoPanel.SetActive(doActivate);
        // Activate/Deactivate this game object
        gameObject.SetActive(doActivate);
        // Activate/Deactivate back button
        backButton.gameObject.SetActive(doActivate);
    }
}
