using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AdditionalInfoPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    public void ActivateAdvance(AdditionalInfo additionalInfo)
    {
        // Debug.Log("Activate AdditionalInfoPanel");
        gameObject.SetActive(true);
        transform.Find("Panel/Header").GetComponent<Text>().text = additionalInfo.GetHeader();
        string[] lines = additionalInfo.GetLines();
        transform.Find("Panel/InfoText/Line1").GetComponent<Text>().text = lines[0];
        transform.Find("Panel/InfoText/Line2").GetComponent<Text>().text = lines[1];
        transform.Find("Panel/InfoText/Line3").GetComponent<Text>().text = lines[2];
        transform.Find("Panel/InfoText/Line4").GetComponent<Text>().text = lines[3];
        transform.Find("Panel/InfoText/Line5").GetComponent<Text>().text = lines[4];
        transform.Find("Panel/InfoText/Line6").GetComponent<Text>().text = lines[5];
        transform.Find("Panel/InfoText/Line7").GetComponent<Text>().text = lines[6];
        transform.Find("Panel/InfoText/Line8").GetComponent<Text>().text = lines[7];
    }

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
            // deactivate additional info
            gameObject.SetActive(false);
        }
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // as coroutine fade away information

    //}
}
