
using UnityEngine;

public class ObjGoToPoint : LevelObjective
{
    [Tooltip("The user friendly prompt that will tell the player what to do.")]
    public string Prompt = "Go to the name. {0}m away.";
    [Tooltip("The target position, in world units.")]
    public Vector2 Position;
    [Tooltip("The range around the target position in which the objective is considered completed when the player character" +
             "stands there. Should not be less than 0.5 units.")]
    public float Range = 5f;
    [Tooltip("If true, then the Prompt string is formatted with the remaining distance in meters." +
             "For example, do 'Go to the door. {0}m away.' to display 'Go to the door. 20m away.'")]
    public bool DisplayProximity = true;

    private bool completed;

    public float DistanceToCompletion
    {
        get
        {
            var playerChar = Player.Character;
            if (playerChar == null)
                return 34404f;
            float dst = Vector2.Distance(Position, playerChar.transform.position);
            float adjusted = Mathf.Max(dst - Mathf.Max(Range, 0.5f), 0f);

            return adjusted;
        }
    }

    public override bool IsComplete()
    {
        if (!completed && DistanceToCompletion == 0f)
            completed = true;
        return completed;
    }

    public override string GetPrompt()
    {
        if (DisplayProximity)
            return Prompt.Form(DistanceToCompletion.ToString("N1"));
        else
            return Prompt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsComplete() ? Color.cyan : Color.yellow;
        Gizmos.DrawWireSphere(Position, Mathf.Max(Range, 0.5f));
        if (!IsComplete() && Player.Character != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 start = Player.Character.transform.position;
            Vector2 end = Position;
            end += (start - end).normalized * Range;
            Gizmos.DrawLine(start, end);
        }
    }
}
