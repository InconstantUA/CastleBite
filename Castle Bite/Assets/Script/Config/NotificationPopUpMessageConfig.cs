using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NotificationPopUpMessage", menuName = "Config/Notification Pop-Up Message")]
public class NotificationPopUpMessageConfig : ScriptableObject
{
    [TextArea]
    public string message;
}
