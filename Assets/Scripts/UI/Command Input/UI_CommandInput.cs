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

    private int autoIndex = 0;

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

        autoIndex = 0;
    }

    public void Update()
    {
        bool focoused = Input.isFocused;
        string typed = Input.text.Trim();
        bool isCmd = typed.StartsWith("/");
        
        if(InputManager.IsDown("Complete Command", true))
        {
            if (isCmd && Suggestions.Matches.Count > 0)
            {
                string complete = this.Suggestions.Matches[autoIndex++].Name;
                if(typed.Length - 1 < complete.Length)
                {
                    Input.text = '/' + complete;
                    Input.caretPosition = Input.text.Length;
                }
            }
        }

        if(InputManager.IsDown("Enter Command", true))
        {
            if (isCmd)
            {
                // Execute here...
                string error;
                bool clear = Commands.TryExecute(Input.text.Trim(), out error);

                if (clear)
                {
                    // Assume this means it executed successfuly.
                    Commands.AddCommandAsExectued(Input.text);
                    Input.text = "";
                    Suggestions.Text.text = "";
                    autoIndex = 0;
                }

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
