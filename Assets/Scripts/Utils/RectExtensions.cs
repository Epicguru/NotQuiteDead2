
using UnityEngine;

public static class RectExtensions
{
    public static bool Intersects(this RectInt rect, RectInt other)
    {
        // If the max is greater than the min.
        if (rect.max.x > other.min.x && rect.max.y > other.min.y && rect.max.x <= other.max.x && rect.max.y <= other.max.y)
            return true;

        if (rect.min.x < other.max.x && rect.min.y < other.max.y && rect.min.x >= other.min.x && rect.min.y >= other.min.y)
            return true;

        return false;
    }

    public static RectInt Rotated(this RectInt rect)
    {
        return new RectInt(rect.position, new Vector2Int(rect.height, rect.width));
    }
}