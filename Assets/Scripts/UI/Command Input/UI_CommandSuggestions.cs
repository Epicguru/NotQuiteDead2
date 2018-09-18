using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CommandSuggestions : MonoBehaviour
{
    public bool Open;

    public RectTransform Rect;
    public Text Text;

    public float Height = 26f;
    public float YPos = 10f;
    public float TransitionTime = 0.5f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float timer;

    public void Update()
    {
        Text.text = RichText.Highlight("This is text! Longwordhere!", "word", Color.black, true);

        if (Open)
        {
            timer += Time.unscaledDeltaTime;
        }
        else
        {
            timer -= Time.unscaledDeltaTime;
        }
        timer = Mathf.Clamp(timer, 0f, TransitionTime);
        float p = timer / TransitionTime;
        float x = Curve.Evaluate(p);

        float h = Mathf.Lerp(0f, Height, x);
        float y = Mathf.Lerp(0f, YPos, x);

        Rect.anchoredPosition = new Vector2(0f, y);
        var s = Rect.sizeDelta;
        s.y = h;
        Rect.sizeDelta = s;
    }
}
