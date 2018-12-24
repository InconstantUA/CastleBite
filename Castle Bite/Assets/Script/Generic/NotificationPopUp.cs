using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopUp : MonoBehaviour {
    [SerializeField]
    Text text;
    [SerializeField]
    TextMeshPro textMeshPro;
    // public Text myText;

    // Use this for initialization
    //void Start()
    //{
    //    myText = transform.GetComponentInChildren<Transform>().GetComponentInChildren<Text>();
    //    Debug.Log(myText.name);
    //}

    public void DisplayMessage(string message)
    {
        gameObject.SetActive(true);
        SetMessage(message);
    }

    public void DisplayMessage(NotificationPopUpMessageConfig messageConfig)
    {
        gameObject.SetActive(true);
        SetProMessage(messageConfig.message);
    }

    public void SetMessage(string message)
    {
        // This does not work
        // Debug.Log(transform.Find("Panel").name);
        // Debug.Log(transform.Find("Panel").Find("Text").name);
        // Debug.Log(txt.name);
        text.text = message;
        // enable normal text object
        text.gameObject.SetActive(true);
        // disable text mesh pro
        textMeshPro.gameObject.SetActive(false);
    }

    public void SetProMessage(string message)
    {
        // This does not work
        // Debug.Log(transform.Find("Panel").name);
        // Debug.Log(transform.Find("Panel").Find("Text").name);
        // Debug.Log(txt.name);
        textMeshPro.text = message;
        // enable text mesh pro
        textMeshPro.gameObject.SetActive(true);
        // disable normal text object
        text.gameObject.SetActive(false);
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
