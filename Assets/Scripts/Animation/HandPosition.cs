using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPosition : MonoBehaviour
{
    public Hand Hand = Hand.LEFT;
    public bool BehindItem = false;
    public bool Flipped = false;

    public void OnDrawGizmos()
    {
        const float WIDTH = 0.125f;
        const float HEIGHT = 0.1875f;
        var topLeft = transform.TransformPoint(-WIDTH / 2f, HEIGHT / 2f, 0f);
        var bottomLeft = transform.TransformPoint(-WIDTH / 2f, -HEIGHT / 2f, 0f);
        var topRight = transform.TransformPoint(WIDTH / 2f, HEIGHT / 2f, 0f);
        var bottomRight = transform.TransformPoint(WIDTH / 2f, -HEIGHT / 2f, 0f);
        var fingerPos = transform.TransformPoint(WIDTH * (Hand == Hand.LEFT ? 0.15f : -0.15f) * (Flipped ? -1f : 1f), -HEIGHT * 0.25f, 0f);

        Gizmos.color = Hand == Hand.LEFT ? Color.green * 0.7f : Color.red * 0.7f;
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawWireSphere(fingerPos, 0.05f);
    }
}

public enum Hand : byte
{
    LEFT,
    RIGHT
}