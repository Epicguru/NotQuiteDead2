
using UnityEngine;

public class ObjGoToRect : LevelObjective
{
    [Tooltip("The user friendly prompt that will tell the player what to do.")]
    public string Prompt = "Go to the [name]. {0}m";
    [Tooltip("The target position, in world units.")]
    public Rect Bounds = new Rect(0f, 0f, 5f, 5f);
    [Tooltip("If true, then the Prompt string is formatted with the remaining distance in meters." +
         "For example, do 'Go to the door. {0}m away.' to display 'Go to the door. 20m away.'")]
    public bool Format = true;

    private bool completed;

    public float AproxDistance
    {
        get
        {
            var c = Player.Character;
            if (c == null)
                return 34404;
            var clamped = GetClampedPos(c);
            return Vector2.Distance(c.transform.position, clamped);
        }
    }

    private Vector2 GetClampedPos(Character c)
    {
        return new Vector2(Mathf.Clamp(c.transform.position.x, Bounds.xMin, Bounds.xMax), Mathf.Clamp(c.transform.position.y, Bounds.yMin, Bounds.yMax));
    }

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
        if (Format)
            return Prompt.Form(AproxDistance.ToString("N0"));
        else
            return Prompt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsComplete() ? Color.cyan : Color.yellow;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        if(Player.Character != null)
        {
            Gizmos.color = Color.blue;
            var pos = GetClampedPos(Player.Character);
            Gizmos.DrawWireCube(pos, Vector3.one * 0.15f);
        }
    }
}
