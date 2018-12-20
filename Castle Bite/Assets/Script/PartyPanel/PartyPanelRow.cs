using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPanelRow : MonoBehaviour
{
    [SerializeField]
    PartyPanel.Row row;

    public PartyPanel.Row Row
    {
        get
        {
            return row;
        }
    }
}
