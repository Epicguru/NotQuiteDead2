using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CommandOutput : MonoBehaviour
{
    public bool Open;
    public Text Text;
    public ScrollRect Scroll;
    public Transform[] Bars;
    public RectTransform Rect;
    public float TargetHeight = 400f;
    public float TransitionTime = 0.3f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float timer;
    private bool showingBars = true;

    public void LateUpdate()
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

        if(x <= 0.15f && showingBars)
        {
            // Is almost or completely closed, hide bar.
            foreach (var bar in Bars)
            {
                bar.gameObject.SetActive(false);
            }
            showingBars = false;
        }
        else if(x > 0.15f && !showingBars)
        {
            // Show bar.
            foreach (var bar in Bars)
            {
                bar.gameObject.SetActive(true);
            }
            showingBars = true;
        }

        float h = Mathf.Lerp(0f, TargetHeight, x);
        var s = Rect.sizeDelta;
        s.y = h;
        Rect.sizeDelta = s;
    }

    public void Log(string lines)
    {
        Text.text += lines.Trim() + '\n';
        Invoke("ScrollToBottom", 0.05f);
    }

    private void ScrollToBottom()
    {
        Scroll.verticalNormalizedPosition = 0f;
    }

    public void ClearLog()
    {
        Text.text = "";
    }
}
