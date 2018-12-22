using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrologAnimation : MonoBehaviour {
    enum State { Start, Header, BrifeLineAnimation, BriefNexLine, Objective, End };
    State state;
    // float curColAlpha;
    [SerializeField]
    float headerAnimationDuration = 3f; // in seconds
    [SerializeField]
    float briefLineAnimationDuration = 1f; // in seconds
    [SerializeField]
    float objectiveAnimationDuration = 2f; // in seconds
    [SerializeField]
    Text prologHeaderText;
    [SerializeField]
    Text prologObjectiveHeader;
    [SerializeField]
    Text prologObjectiveText;
    [SerializeField]
    GameObject briefLineTemplate;
    [SerializeField]
    Transform briefRootTransform;
    float animationDuration;
    float animationStartTime;
    float previousAnimationTime;
    Text[] briefTextLines;
    int animatedBriefLine;

	//// Use this for initialization
	//void Start () {
 //       // set first state
 //       state = State.Start;
 //       // init variables
 //       //prologHeaderText = transform.Find("Header").GetComponentInChildren<Text>();
 //       //prologObjectiveText = transform.Find("Objective").GetComponentInChildren<Text>();
 //       // brief = transform.Find("Brief").GetComponentInChildren<VerticalLayoutGroup>().GetComponentsInChildren<Text>();
 //       briefTextLines = null;
 //       // Hide all text
 //       // SetAllTextAlphaColor(0);
 //   }

    void OnDisable()
    {
        // cleanup brief lines
        foreach (Transform childTransform in transform.Find("Brief"))
        {
            Destroy(childTransform.gameObject);
        }
    }

    public void SetActive(ChapterData chapterData)
    {
        Debug.Log("Activating Prolog Animation");
        // set first state
        state = State.Start;
        // init header
        prologHeaderText.text = chapterData.prologHeader;
        // loop through all lines in prolog
        foreach(string briefString in chapterData.prologBrief)
        {
            // create a brief text line in UI and set its text from prolog
            Instantiate(briefLineTemplate, briefRootTransform).GetComponent<Text>().text = briefString;
        }
        // init brief
        briefTextLines = transform.Find("Brief").GetComponentsInChildren<Text>();
        // init objective
        prologObjectiveText.text = chapterData.prologObjective;
        // Hide all text
        SetAllTextAlphaColor(0);
        // reset animated brief line number
        animatedBriefLine = 0;
        // activating this object
        gameObject.SetActive(true);
    }

    void SetTextAlphaColor(Text text, float alphaColor)
    {
        Color tmpClr = new Color(text.color.r, text.color.g, text.color.b, alphaColor);
        text.color = tmpClr;
    }

    void SetAllTextAlphaColor(float alphaColor)
    {
        //  Hide header and objective
        SetTextAlphaColor(prologHeaderText, alphaColor);
        SetTextAlphaColor(prologObjectiveHeader, alphaColor);
        SetTextAlphaColor(prologObjectiveText, alphaColor);
        //  hide brief
        foreach (Text line in briefTextLines)
        {
            SetTextAlphaColor(line, alphaColor);
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
                Animate(prologHeaderText, State.BrifeLineAnimation, headerAnimationDuration);
            }
            // Activate brief
            if (state == State.BrifeLineAnimation)
            {
                if (animatedBriefLine < briefTextLines.Length)
                {
                    Animate(briefTextLines[animatedBriefLine], State.BriefNexLine, briefLineAnimationDuration);
                }
                else
                {
                    // last line has been animated, switch to the objective animation
                    state = State.Objective;
                    // show objective header
                    SetTextAlfaToMax(prologObjectiveHeader);
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
                Animate(prologObjectiveText, State.End, objectiveAnimationDuration);
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
        Debug.Log("Switching to the " + nextState.ToString() + " state");
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
