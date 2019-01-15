using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DefaultTextAnimation", menuName = "Config/Text/Animations/DefaultAnimation")]
public class TextAnimation : ScriptableObject
{
    public TextAnimationConfig textAnimationConfig;

    protected void SetTextColorTransparency(Text text, float transparency)
    {
        Color c = text.color;
        c.a = transparency;
        text.color = c;
    }

    protected IEnumerator FadeOutText(Text text, float duration)
    {
        // loop over duration in seconds backwards
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            SetTextColorTransparency(text, i / duration);
            // Debug.Log("Fade text, transparency: " + Mathf.RoundToInt(i / duration * 255));
            yield return null;
        }
        // hide text completely
        // Debug.Log("Hide text");
        SetTextColorTransparency(text, 0);
        yield return null;
    }

    //public virtual void Run(Text text, CoroutineQueue coroutineQueue)
    //{
    //    Debug.Log("Running " + GetType().Name);
    //    // set text color
    //    text.color = textAnimationConfig.textColor;
    //    // fade text over time
    //    coroutineQueue.Run(FadeOutText(text, textAnimationConfig.duration));
    //}
    public virtual void Run(Text text)
    {
        Debug.Log("Running " + textAnimationConfig.name);
        // set text color
        text.color = textAnimationConfig.textColor;
        // fade text over time and 
        // pass animation script
        // and coroutine group id parameter for the animation which should be running at he same time whith other animation 
        // (example: life drain ability deals damage and heals or magic damage which deals damage to entire party)
        // verify if group id is set
        if (textAnimationConfig.coroutineGroupID != null)
        {
            CoroutineQueueManager.Run(FadeOutText(text, textAnimationConfig.duration), textAnimationConfig.coroutineGroupID.GUID);
        }
        else
        {
            CoroutineQueueManager.Run(FadeOutText(text, textAnimationConfig.duration));
        }
    }
}
