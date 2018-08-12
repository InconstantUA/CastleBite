using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEquipment : MonoBehaviour {
    UnitEquipmentButton callingUnitEquipmentButton;

    public void ActivateAdvance(UnitEquipmentButton unitEquipmentButton)
    {
        callingUnitEquipmentButton = unitEquipmentButton;
        gameObject.SetActive(true);
        // deactivate other uneeded menus
        callingUnitEquipmentButton.SetRequiredMenusActive(false);
    }

    public void DeactivateAdvance()
    {
        gameObject.SetActive(false);
        // activate other menus back
        callingUnitEquipmentButton.SetRequiredMenusActive(true);
    }
}
