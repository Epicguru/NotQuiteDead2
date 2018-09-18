using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CommandInput : MonoBehaviour
{
    public UI_CommandOutput Output;
    public UI_CommandSuggestions Suggestions;
    public InputField Input;

    public void UponTyped()
    {
        string typed = Input.text.Trim();
        bool isCmd = typed.StartsWith("/");

        if (!isCmd)
            return;

        var s = Suggestions;
        string key = Input.text.Trim().ToLower().Replace("/", "");
        s.Keyword = string.IsNullOrWhiteSpace(key) ? "NOTTHISFORSURESERIOUSLY" : key;        
        s.UpdateSuggestions();
    }

    public void Update()
    {
        bool focoused = Input.isFocused;
        string typed = Input.text.Trim();
        bool isCmd = typed.StartsWith("/");

        Output.Open = focoused;
        Suggestions.Open = isCmd && focoused;        
    }
}
