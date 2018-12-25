using UnityEngine;
using UnityEngine.UI;

public class PlayerCityInfo : MonoBehaviour
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text goldIncomePerDayText;
    [SerializeField]
    Text manaIncomePerDayText;

    public void SetActive(City city)
    {
        // set name
        nameText.text = city.CityName;
        // set level
        levelText.text = city.CityLevelCurrent.ToString();
        // set gold income per day
        goldIncomePerDayText.text = city.GoldIncomePerDay.ToString();
        // set mana income per day
        manaIncomePerDayText.text = city.ManaIncomePerDay.ToString();
        // activate it
        gameObject.SetActive(true);
    }
}
