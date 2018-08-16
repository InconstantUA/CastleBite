using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class BattleExit : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Text txt;
    Button btn;
    Color tmpColor;
    public enum ExitOption { FleePlayer, FleeEnemy, DestroyPlayer, DestroyEnemy, EnterCity };
    ExitOption exitOption;

    public void SetExitOption(ExitOption value)
    {
        exitOption = value;
    }

    public ExitOption GetExitOption()
    {
        return exitOption;
    }

    void Start()
    {
        // init text object
        txt = GetComponentInChildren<Text>();
        // baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        SetPressedStatus();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        // keep state On
        ActOnClick();
    }

    void SetPressedStatus()
    {
        if (btn.interactable)
        {
            tmpColor = btn.colors.pressedColor;
        }
        else
        {
            tmpColor = btn.colors.disabledColor;
        }
        tmpColor.a = 1;
        txt.color = tmpColor;
        // Debug.Log("SetPressedStatus " + btn.name + " button");
    }

    void ActOnClick()
    {
        // activate map view
        Debug.Log("Exit");
        BattleScreen battleScreen = transform.parent.GetComponent<BattleScreen>();
        switch (exitOption)
        {
            case ExitOption.DestroyPlayer:
                battleScreen.DestroyPlayer();
                break;
            case ExitOption.DestroyEnemy:
                battleScreen.DestroyEnemy();
                break;
            case ExitOption.FleePlayer:
                battleScreen.FleePlayer();
                break;
            case ExitOption.FleeEnemy:
                battleScreen.FleeEnemy();
                break;
            case ExitOption.EnterCity:
                battleScreen.CaptureAndEnterCity();
                break;
            default:
                Debug.LogError("Unknown exit option.");
                break;
        }
        //// Activate map screen
        //transform.root.Find("MapScreen").gameObject.SetActive(true);
    }

}