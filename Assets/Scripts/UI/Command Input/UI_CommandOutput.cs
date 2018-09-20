using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CommandOutput : MonoBehaviour
{
    public Text Text;
    public ScrollRect Scroll;
    public Transform[] Bars;

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
