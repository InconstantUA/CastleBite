using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrologAnimation : MonoBehaviour {
    enum State { Start, Header, BrifeLineAnimation, BriefNexLine, Objective, End };
    State state;
    // float curColAlpha;
    public float headerAnimationDuration = 3f; // in seconds
    public float briefLineAnimationDuration = 1f; // in seconds
    public float objectiveAnimationDuration = 2f; // in seconds
    float animationDuration;
    float animationStartTime;
    float previousAnimationTime;
    Text header;
    Text[] brief;
    int animatedBriefLine = 0;
    Text objective;

	// Use this for initialization
	void Start () {
        // set first state
        state = State.Start;
        // init variables
        header = gameObject.transform.Find("Header").gameObject.GetComponentInChildren<Text>();
        objective = gameObject.transform.Find("Objective").gameObject.GetComponentInChildren<Text>();
        // brief = gameObject.transform.Find("Brief").GetComponentInChildren<VerticalLayoutGroup>().GetComponentsInChildren<Text>();
        brief = gameObject.transform.Find("Brief").gameObject.GetComponentsInChildren<Text>();
        // Hide all text
        SetAllTextAlphaColor(0);
    }

    void SetAllTextAlphaColor(float alphaColor)
    {
        //  Hide header and objective
        Color tmpClr;
        tmpClr = new Color(header.color.r, header.color.g, header.color.b, alphaColor);
        header.color = tmpClr;
        tmpClr = new Color(objective.color.r, objective.color.g, objective.color.b, alphaColor);
        objective.color = tmpClr;
        //  hide brief
        foreach (Text line in brief)
        {
            tmpClr = new Color(line.color.r, line.color.g, line.color.b, alphaColor);
            line.color = tmpClr;
        }
    }

    // Update is called once per frame
    void Update () {
        // Animate only when it is activated
        if (gameObject.activeSelf)
        {
            // Start animation
            if (state == State.Start)
            {
                SwitchToTheNextState(State.Header);
            }
            // Activate header
            if (state == State.Header)
            {
                Animate(header, State.BrifeLineAnimation, headerAnimationDuration);
            }
            // Activate brief
            if (state == State.BrifeLineAnimation)
            {
                if (animatedBriefLine < brief.Length)
                {
                    Animate(brief[animatedBriefLine], State.BriefNexLine, briefLineAnimationDuration);
                }
                else
                {
                    // last line has been animated, switch to the objective animation
                    state = State.Objective;
                }
            }
            if (state == State.BriefNexLine)
            {
                animatedBriefLine += 1;
                state = State.BrifeLineAnimation;
            }
            // Activate objective
            if (state == State.Objective)
            {
                Animate(objective, State.End, objectiveAnimationDuration);
            }
        }
    }

    void Animate(Text txt, State nextState, float animDrtn)
    {
        animationDuration = animDrtn;
        // Add alpha based on the time passed untill we reach full alpha
        // On full alpha go to the next state
        float timePassedSinceAnimationStart = Time.time - animationStartTime;
        if (timePassedSinceAnimationStart >= animationDuration)
        {
            SetTextAlfaToMax(txt);
            SwitchToTheNextState(nextState);
        }
        else
        {
            IncreaseTextAlfa(txt);
        }
    }

    void SetTextAlfaToMax(Text txt)
    {
        // just in case make collor a to 1, if it was not done by animation yet
        Color tmpClr = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
        txt.color = tmpClr;
    }

    void IncreaseTextAlfa(Text txt)
    {
        // Debug.Log(txt.color.a + " - " + txt.text);
        float timePassedSincePreviousAnimation = Time.time - previousAnimationTime;
        float alphaColDelta = timePassedSincePreviousAnimation / animationDuration;
        txt.color += new Color(0, 0, 0, alphaColDelta);
        // curColAlpha = txt.color.a;
        // just in case - verify that alpha is not greater than 1
        if (txt.color.a > 1)
        {
            Color tmpClr = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
            txt.color = tmpClr;
        }
        previousAnimationTime = Time.time;
    }

    void SwitchToTheNextState(State nextState)
    {
        state = nextState;
        // curColAlpha = 0;
        animationStartTime = Time.time;
        previousAnimationTime = Time.time;
    }

    public void Skip()
    {
        Debug.Log("Skip");
        state = State.End;
        SetAllTextAlphaColor(1);
    }
}
