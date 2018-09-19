using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CommandInput : MonoBehaviour
{
    public static UI_CommandInput Instance;
    public UI_CommandOutput Output;
    public UI_CommandSuggestions Suggestions;
    public InputField Input;

    public void Awake()
    {
        Instance = this;
    }

    public void UponTyped()
    {
        string typed = Input.text.Trim();
        bool isCmd = typed.StartsWith("/");

        if (!isCmd)
            return;

        var s = Suggestions;
        string key = Input.text.Trim().ToLower().Split(' ')[0].Replace("/", "");
        s.Keyword = string.IsNullOrWhiteSpace(key) ? "NOTTHISFORSURESERIOUSLY" : key;        
        s.UpdateSuggestions();
    }

    public void UponEndType()
    {
        //Debug.Log("End Type!");
    }

    public void Update()
    {
        bool focoused = Input.isFocused;
        string typed = Input.text.Trim();
        bool isCmd = typed.StartsWith("/");

        Output.Open = focoused;
        Suggestions.Open = isCmd && focoused;
        
        if(InputManager.IsDown("Enter Command"))
        {
            if (isCmd)
            {
                // Execute here...
                string error;
                bool clear = Commands.TryExecute(Input.text.Trim(), out error);

                if(clear)
                    Input.text = "";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    Output.Log(RichText.InColour(error.Trim(), Color.red));
                }
            }

            // Give focous back to the input field. Slightly hackish, very effective.
            EventSystem.current.SetSelectedGameObject(Input.gameObject, null);
            Input.ActivateInputField();
        }
    }
}
