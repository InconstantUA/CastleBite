using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UniquePowerModifierInfoMenu : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    Text upmNameText;
    [SerializeField]
    Text upmDescriptionText;
    [SerializeField]
    Transform upmAdditionalInfoArea;
    [SerializeField]
    Text additionalInfoTextTemplate;

    public void SetActive(bool doActivate, UniquePowerModifierInfoData uniquePowerModifierInfoData = null)
    {
        if (doActivate)
        {
            // set upm name
            upmNameText.text = uniquePowerModifierInfoData.name;
            // set upm description
            upmDescriptionText.text = uniquePowerModifierInfoData.description;
            // set upm additional information
            foreach(string textString in uniquePowerModifierInfoData.additionalInfo)
            {
                // create new text element in additional info and set its value
                Instantiate(additionalInfoTextTemplate.gameObject, upmAdditionalInfoArea).GetComponent<Text>().text = textString;
            }
            // enable this menu
            gameObject.SetActive(true);
        }
        else
        {
            // disable this menu
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        // remove all created additional info texts
        RecycleBin.RecycleChildrenOf(upmAdditionalInfoArea.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        // deactivate this menu
        gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //// Debug.Log("OnPointerUp");
        //if (Input.GetMouseButtonUp(1))
        //{
            // on right mouse click
            // deactivate this menu
            gameObject.SetActive(false);
        //}
    }
}
