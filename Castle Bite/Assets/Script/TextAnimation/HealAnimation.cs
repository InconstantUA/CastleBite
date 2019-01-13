using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HealAnimation", menuName = "Config/Text/Animations/HealAnimation")]
public class HealAnimation : TextAnimation
{
    void SetTextColorTransparency(Text text, float transparency)
    {
        Color c = text.color;
        c.a = transparency;
        text.color = c;
    }

    IEnumerator FadeOutText(Text text, float duration)
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

    public override void Run(Text text, CoroutineQueue coroutineQueue)
    {
        Debug.Log("Running " + GetType().Name);
        // set text color
        text.color = textAnimationConfig.textColor;
        // fade text over time
        coroutineQueue.Run(FadeOutText(text, textAnimationConfig.duration));
    }
}
