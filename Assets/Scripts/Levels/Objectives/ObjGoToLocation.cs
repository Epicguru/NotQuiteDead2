
using UnityEngine;

public class ObjGoToLocation : LevelObjective
{
    [Tooltip("The user friendly prompt that will tell the player what to do.")]
    public string Prompt = "Go to the name. {0}m away.";
    [Tooltip("The target position, in world units.")]
    public Vector2 Position;
    [Tooltip("The range around the target position in which the objective is considered completed when the player character" +
             "stands there.")]
    public float Range = 5f;
    [Tooltip("If true, then the Prompt string is formatted with the remaining distance in meters." +
             "For example, do 'Go to the door. {0}m away.' to display 'Go to the door. 20m away.'")]
    public bool DisplayProximity = true;

    public float DistanceToCompletion
    {
        get
        {
            var playerChar = Player.Character;
            float dst = Vector2.Distance(Position, playerChar.transform.position);
            float adjusted = Mathf.Max(dst - Range, 0f);

            return adjusted;
        }
    }

    public override bool IsComplete()
    {
        return DistanceToCompletion == 0f;
    }

    public override string GetPrompt()
    {
        if (DisplayProximity)
            return Prompt.Form(DistanceToCompletion);
        else
            return Prompt;
    }
}
