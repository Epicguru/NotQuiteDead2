using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CommandOutput : MonoBehaviour
{
    public bool Open;
    public RectTransform Rect;
    public float TargetHeight = 400f;
    public float TransitionTime = 0.3f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float timer;

    public void Update()
    {
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

        float h = Mathf.Lerp(0f, TargetHeight, x);
        var s = Rect.sizeDelta;
        s.y = h;
        Rect.sizeDelta = s;
    }
}
