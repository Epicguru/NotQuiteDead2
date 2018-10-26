
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CompundLevelObjective : LevelObjective
{
    public List<LevelObjective> Requirements = new List<LevelObjective>();

    [Tooltip("If true, the objectives don't have any specfic order and can be completed independently of each other." +
             "If false, the objectives must be completed in the order that they are defined in the list in order for this compund objective to be considered completed.")]
    public bool Unordered = true;

    /// <summary>
    /// Returns the index of the first uncompleted requirement objective, or -1 if all objectives have been completed.
    /// </summary>
    public int FirstUncompletedIndex
    {
        get
        {
            for (int i = 0; i < Requirements.Count; i++)
            {
                if (!Requirements[i].IsComplete())
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// If this compund objective is set to be ordered, then this will return the first uncompleted objective.
    /// If in unordered mode, this will return null.
    /// If all objectives are completed, will return null.
    /// </summary>
    public LevelObjective CurrentObjective
    {
        get
        {
            if (Unordered)
                return null;

            int i = FirstUncompletedIndex;
            if(i != -1)
            {
                return Requirements[i];
            }
            else
            {
                return null;
            }
        }
    }

    public override bool IsComplete()
    {
        foreach (var item in Requirements)
        {
            if (item == null || item == this)
                continue;
            if (!item.IsComplete())
                return false;
        }
        return true;
    }

    public override float GetProgress()
    {
        int count = 0;
        float p = 0;
        foreach (var item in Requirements)
        {
            if (item == null || item == this)
                continue;

            count++;            
            if (item.IsComplete())
            {
                p += 1f;
            }
            else
            {
                float progress = Mathf.Min(item.GetProgress(), 1f);
                if(progress > 0f)
                {
                    p += progress;
                }
            }
        }

        return Mathf.Clamp01(p / count);
    }

    private static StringBuilder str = new StringBuilder();
    public override string GetPrompt()
    {
        if (!Unordered)
        {
            var current = CurrentObjective;
            if (current == null)
                return "None";

            return current.GetPrompt();
        }
        else
        {
            str.Clear();
            foreach (var item in Requirements)
            {
                if(item != null)
                    str.AppendLine(item.GetPrompt().Trim());
            }
            return str.ToString().TrimEnd();
        }
    }
}
