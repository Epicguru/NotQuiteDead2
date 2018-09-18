﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public string Keyword;
    public List<string> Matches = new List<string>();

    private float timer;
    private StringBuilder str = new StringBuilder();
    private bool dirty = false;

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

        float h = Mathf.Lerp(0f, Height, x);
        float y = Mathf.Lerp(0f, YPos, x);

        Rect.anchoredPosition = new Vector2(0f, y);
        var s = Rect.sizeDelta;
        s.y = h;
        Rect.sizeDelta = s;

        // Do real text update here, so it can never be done more than once per frame.
        if (dirty)
        {
            dirty = false;

            Matches.Clear();
            foreach (var cmd in Commands.Loaded.Keys)
            {
                if (cmd.Contains(Keyword))
                {
                    Matches.Add(cmd);
                }
            }

            str.Clear();
            const string WHITESPACE = "  ";

            foreach (var item in Matches)
            {
                str.Append(RichText.Highlight(item, Keyword, Color.black, true));
                str.Append(WHITESPACE);
            }

            Text.text = str.ToString();
            str.Clear();
        }
    }

    public void UpdateSuggestions()
    {
        dirty = true;
    }
}
