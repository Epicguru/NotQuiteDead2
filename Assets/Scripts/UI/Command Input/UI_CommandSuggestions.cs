using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UI_CommandSuggestions : MonoBehaviour
{
    public UI_CommandInput Input;

    public Text Text;

    public string Keyword;
    public List<DebugCmd> Matches = new List<DebugCmd>();

    private float timer;
    private StringBuilder str = new StringBuilder();
    private bool dirty = false;

    public void Update()
    {
        // Do real text update here, so it can never be done more than once per frame.
        if (dirty)
        {
            dirty = false;

            Matches.Clear();
            var suggested = Commands.GetAutocompleteCommand(Keyword);

            foreach(var thing in suggested)
            {
                foreach(var comm in Commands.Loaded[thing.CommandName])
                {
                    Matches.Add(comm);
                }
            }

            str.Clear();
            const string WHITESPACE = "  ";

            int index = 0;
            foreach (var item in Matches)
            {
                string text = item.ToString();
                string comm = RichText.Highlight(text, Keyword, Color.black, true);
                str.Append(comm);
                str.Append(WHITESPACE);
                index++;
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
