using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopUp : MonoBehaviour {
    [SerializeField]
    GameObject notificationPopUpGO;
    [SerializeField]
    Text text;
    [SerializeField]
    TextMeshPro textMeshPro;

    // make sure that we have only one instance of this Confirmation menu
    // and have easy access to it
    private static NotificationPopUp notificationPopUp;

    public GameObject NotificationPopUpGO
    {
        get
        {
            return notificationPopUpGO;
        }
    }

    public static NotificationPopUp Instance()
    {
        if (!notificationPopUp)
        {
            notificationPopUp = FindObjectOfType(typeof(NotificationPopUp)) as NotificationPopUp;
            if (!notificationPopUp)
            {
                // Debug.LogError("There needs to be only one ConfirmationPopUp script on a GameObject in your scene.");
            }
        }
        return notificationPopUp;
    }

    public void DisplayMessage(string message)
    {
        notificationPopUpGO.SetActive(true);
        SetMessage(message);
    }

    public void DisplayMessage(NotificationPopUpMessageConfig messageConfig)
    {
        notificationPopUpGO.SetActive(true);
        SetProMessage(messageConfig.message);
    }

    public void SetMessage(string message)
    {
        text.text = message;
        // enable normal text object
        text.gameObject.SetActive(true);
        // disable text mesh pro
        textMeshPro.gameObject.SetActive(false);
    }

    public void SetProMessage(string message)
    {
        textMeshPro.text = message;
        // enable text mesh pro
        textMeshPro.gameObject.SetActive(true);
        // disable normal text object
        text.gameObject.SetActive(false);
    }

}
