using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the same colors as defined for the button but for the text
// Button in our case is not visible, only text is visible
// We set alpha in button properties to 0
// Later, before assigning button colors to the text we reset transprancy to 1(255)
[RequireComponent(typeof(Button))]
public class BattleWait : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
    }

    void ActOnClick()
    {
        Debug.Log("Wait");
        // get battle screen, structure: BattleScreen-CtrlPnlFight-This
        BattleScreen battleScreen = transform.parent.parent.GetComponent<BattleScreen>();
        // set unit is waiting status
        PartyUnit activeUnit = battleScreen.GetActiveUnit();
        activeUnit.SetUnitStatus(PartyUnit.UnitStatus.Waiting);
        // execute wait animation
        battleScreen.GetQueue().Run(Wait());
        // activate next unit
        battleScreen.ActivateNextUnit();
    }

}