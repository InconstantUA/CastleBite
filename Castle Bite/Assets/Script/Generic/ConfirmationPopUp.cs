using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmationPopUp : MonoBehaviour {
    public Text question;
    public Button yesButton;
    public Button noButton;
    public GameObject confirmationPopUpObj;

    // make sure that we have only one instance of this Confirmation menu
    // and have easy access to it
    private static ConfirmationPopUp confirmationPopUp;
    public static ConfirmationPopUp Instance ()
    {
        if (!confirmationPopUp)
        {
            confirmationPopUp = FindObjectOfType(typeof(ConfirmationPopUp)) as ConfirmationPopUp;
            if (!confirmationPopUp)
            {
                // Debug.LogError("There needs to be only one ConfirmationPopUp script on a GameObject in your scene.");
            }
        }
        return confirmationPopUp;
    }

    // Yes/No: A string, a Yes event, a No event
    public void Choice (string question, UnityAction yesEvent, UnityAction noEvent)
    {
        // activate confirmation popup
        confirmationPopUpObj.SetActive(true);
        // remove possible previous listeners attached to this button
        yesButton.onClick.RemoveAllListeners();
        // setup our listener
        yesButton.onClick.AddListener(yesEvent);
        yesButton.onClick.AddListener(ClosePanel);
        // same for No button
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(noEvent);
        noButton.onClick.AddListener(ClosePanel);
        // update text in confiramtion popup
        this.question.text = question;
    }

    void ClosePanel()
    {
        confirmationPopUpObj.SetActive(false);
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
