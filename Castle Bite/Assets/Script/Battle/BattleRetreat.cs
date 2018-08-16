using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class BattleRetreat : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    Text txt;
    Button btn;
    Color tmpColor;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            // on left mouse click
            ActOnClick();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // on right mouse click
        }
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

    IEnumerator StartRetreating()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void ActOnClick()
    {
        Debug.Log("Retreat");
        // get battle screen, structure: BattleScreen-CtrlPnlFight-This
        BattleScreen battleScreen = transform.parent.parent.GetComponent<BattleScreen>();
        // set unit is waiting status
        battleScreen.ActiveUnitUI.SetUnitStatus(UnitStatus.Escaping);
        // set unit has moved flag
        battleScreen.ActiveUnitUI.LPartyUnit.HasMoved = true;
        // execute wait animation
        battleScreen.GetQueue().Run(StartRetreating());
        // activate next unit
        battleScreen.ActivateNextUnit();
    }

}