using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEquipment : MonoBehaviour {
    UnitEquipmentButton callingUnitEquipmentButton;

    public void ActivateAdvance(UnitEquipmentButton unitEquipmentButton)
    {
        callingUnitEquipmentButton = unitEquipmentButton;
        // Activate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(true);
        // activate this menu
        gameObject.SetActive(true);
        // deactivate other uneeded menus
        callingUnitEquipmentButton.SetRequiredMenusActive(false);
        // bring left and right hero parties with inventories to te front
        transform.root.Find("MiscUI/LeftHeroParty").SetAsLastSibling();
        transform.root.Find("MiscUI/RightHeroParty").SetAsLastSibling();
    }

    public void DeactivateAdvance()
    {
        // Deactivate intermediate background
        transform.root.Find("MiscUI/BackgroundIntermediate").gameObject.SetActive(false);
        // deactivate this menu
        gameObject.SetActive(false);
        // activate other menus back
        callingUnitEquipmentButton.SetRequiredMenusActive(true);
    }
}
