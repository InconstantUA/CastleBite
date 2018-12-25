using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeroInfo : MonoBehaviour
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text classText;

    public void SetActive(PartyUnit partyUnit)
    {
        // set name
        nameText.text = partyUnit.GivenName;
        // set level
        levelText.text = partyUnit.UnitLevel.ToString();
        // set class
        classText.text = partyUnit.UnitName;
        // activate it
        gameObject.SetActive(true);
    }
}
