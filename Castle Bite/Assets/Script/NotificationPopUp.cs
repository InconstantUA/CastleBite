using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopUp : MonoBehaviour {
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
        // This does not work
        // Debug.Log(transform.Find("Panel").name);
        // Debug.Log(transform.Find("Panel").Find("Text").name);
        // Debug.Log(txt.name);
        transform.GetComponentInChildren<Transform>().GetComponentInChildren<Text>().text = message;
    }



    //// Update is called once per frame
    //void Update () {

    //}
}
