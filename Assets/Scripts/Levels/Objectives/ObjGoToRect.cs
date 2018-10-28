
using UnityEngine;

public class ObjGoToRect : LevelObjective
{
    [Tooltip("The user friendly prompt that will tell the player what to do.")]
    public string Prompt = "Go to the [name].";
    [Tooltip("The target position, in world units.")]
    public Rect Bounds;

    private bool completed;

    public override bool IsComplete()
    {
        if (Player.Character == null)
            return false;

        if (!completed && Bounds.Contains(Player.Character.transform.position))
            completed = true;
        return completed;
    }

    public override string GetPrompt()
    {
        return Prompt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsComplete() ? Color.cyan : Color.yellow;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}
