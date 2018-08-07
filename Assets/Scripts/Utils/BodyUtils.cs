
using UnityEngine;

public static class BodyUtils
{
    public static float GetSquareSpeed(this Rigidbody2D body)
    {
        return body.velocity.x * body.velocity.x + body.velocity.y * body.velocity.y;
    }

    public static float GetSpeed(this Rigidbody2D body)
    {
        return Mathf.Sqrt(body.GetSquareSpeed());
    }
}
