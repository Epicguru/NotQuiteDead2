﻿
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public abstract class LevelObjective : MonoBehaviour
{
    /// <summary>
    /// Should return true whenever the objective has been completed. This method will only be called
    /// when in the correct level, and only when that level is completely loaded.
    /// </summary>
    /// <returns>True if the objective is fully completed.</returns>
    public abstract bool IsComplete();

    /// <summary>
    /// Should return a value in the range 0 to 1 indicating the current completion state of the objective.
    /// If this returns a value smaller than zero, such as -1, it indicates the the objective is ongoing or has
    /// no way to measure progress.
    /// </summary>
    /// <returns>A value in the range 0 to 1 or less than zero to indicate unmeasurable progress. By default returns -1.</returns>
    public virtual float GetProgress()
    {
        return -1f;
    }

    /// <summary>
    /// Should return a prompt for the player to complete this objective, such as:
    /// "Destroy the tower" or
    /// "Kill all bandits"
    /// </summary>
    /// <returns></returns>
    public abstract string GetPrompt();

    public override string ToString()
    {
        float p = GetProgress();
        bool comp = IsComplete();
        return name.Trim() + " ({0}): {1}, {2}".Form(GetType().Name, comp ? "complete" : "not complete", comp ? "" : p < 0f ? "ongoing" : Mathf.FloorToInt(p * 100f).ToString() + "%");
    }

    private void OnDestroy()
    {
        var lm = GetComponentInParent<LevelManager>();
        if (lm == null)
            return;

        if (lm.Objectives.Contains(this))
            lm.Objectives.Remove(this);
    }
}
