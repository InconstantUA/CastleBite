using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    GameObject uniquePowerModifierItemInfoTemplate;
    [SerializeField]
    GameObject unitStatsModifierItemInfoTemplate;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
            // deactivate this menu
            gameObject.SetActive(false);
        }
    }

    public void SetActive(InventoryItem inventoryItem)
    {
        Debug.Log("Show " + inventoryItem.ItemName + " item information");
        // activate this object
        gameObject.SetActive(true);
        // set item cost
        transform.Find("ItemCost/Value").GetComponent<Text>().text = inventoryItem.ItemValue.ToString();
        // Get Max usages UI tranform
        Transform maxUsagesUI = transform.Find("MaxUsages");
        // verify if item has usages
        if (inventoryItem.MaxUsagesCount >= 1)
        {
            // enable usages information
            maxUsagesUI.gameObject.SetActive(true);
            // set item left/max usages count
            transform.Find("MaxUsages/Value").GetComponent<Text>().text = inventoryItem.LeftUsagesCount.ToString() + "/" + inventoryItem.MaxUsagesCount.ToString();
        }
        else
        {
            // disable usages information
            maxUsagesUI.gameObject.SetActive(false);
        }
        // Get unit stat modifiers table UI
        Transform usmsTableUI = transform.Find("Modifiers/UnitStatsModifiersTable");
        // verify if item has unit stats modifiers
        if (inventoryItem.UnitStatModifiers.Count >= 1)
        {
            // enable usms info UI
            usmsTableUI.gameObject.SetActive(true);
            // Get unit stat modifiers list grid transform
            Transform usmsListGrid = usmsTableUI.Find("ListOfUnitStatsModifiers/Grid");
            // remove all previously set unit stat modifiers values
            foreach (Transform childTransform in usmsListGrid)
            {
                Destroy(childTransform.gameObject);
            }
            // set item's unit stat modifiers
            foreach (UnitStatModifier usm in inventoryItem.UnitStatModifiers)
            {
                // create new row in the table from template
                Transform newUSMTransform = Instantiate(unitStatsModifierItemInfoTemplate, usmsListGrid).transform;
                // set values
                newUSMTransform.Find("StatName").GetComponent<Text>().text = "Unit " + usm.unitStat.ToString();
                // init power information text
                string powerText = "";
                // set power information
                switch (usm.modifierAppliedHow)
                {
                    case ModifierAppliedHow.Additively:
                        powerText = "+" + usm.modifierPower.ToString();
                        break;
                    case ModifierAppliedHow.Multiplicatively:
                        powerText = "x" + usm.modifierPower.ToString();
                        break;
                    case ModifierAppliedHow.Percent:
                        powerText = usm.modifierPower.ToString() + "%";
                        break;
                    case ModifierAppliedHow.Toggle:
                        powerText = "-";
                        break;
                    default:
                        Debug.LogError("Do not know how to apply modifier power");
                        break;
                }
                // set power text in UI
                newUSMTransform.Find("Power").GetComponent<Text>().text = powerText;
                // verify if duration is permanent (<0)
                if (usm.duration < 0)
                {
                    newUSMTransform.Find("Duration").GetComponent<Text>().text = "Permanent";
                }
                // verify if it is consumable item with instant duration
                else if (usm.duration == 0)
                {
                    newUSMTransform.Find("Duration").GetComponent<Text>().text = "Instant";
                }
                // verify if it is item with duration
                else
                {
                    newUSMTransform.Find("Duration").GetComponent<Text>().text = usm.duration.ToString();
                }
                newUSMTransform.Find("Scope").GetComponent<Text>().text = usm.modifierScope.ToString();
            }
        }
        else
        {
            // disable usms info UI
            usmsTableUI.gameObject.SetActive(false);
        }
        // Get unique power modifiers table UI
        Transform upmsTableUI = transform.Find("Modifiers/UniquePowerModifiersTable");
        // verify if item has unique power modifiers
        if (inventoryItem.UniquePowerModifiers.Count >= 1)
        {
            // enable upms info UI
            upmsTableUI.gameObject.SetActive(true);
            // Get unique power modifiers list grid transform
            Transform upmsListGrid = upmsTableUI.Find("ListOfUniquePowerModifiers/Grid");
            // remove all previously set unique power modifiers values
            foreach (Transform childTransform in upmsListGrid)
            {
                Destroy(childTransform.gameObject);
            }
            // set item's unique power modifiers
            foreach (UniquePowerModifier upm in inventoryItem.UniquePowerModifiers)
            {
                // create new row in the table from template
                Transform newUPMTransform = Instantiate(uniquePowerModifierItemInfoTemplate, upmsListGrid).transform;
                // set values
                newUPMTransform.Find("Name").GetComponent<Text>().text = upm.GetDisplayName();
                newUPMTransform.Find("Power").GetComponent<Text>().text = upm.upmPower.ToString();
                newUPMTransform.Find("Duration").GetComponent<Text>().text = upm.upmDuration.ToString();
                newUPMTransform.Find("Chance").GetComponent<Text>().text = upm.upmChance.ToString() + "%";
                newUPMTransform.Find("Source").GetComponent<Text>().text = upm.upmSource.ToString();
            }
        }
        else
        {
            // disable upms info UI
            upmsTableUI.gameObject.SetActive(false);
        }
    }

}
