using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPanelCell : MonoBehaviour
{
    [SerializeField]
    PartyPanel.Cell cell;

    public PartyPanel.Cell Cell
    {
        get
        {
            return cell;
        }
    }
}
