using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
[RequireComponent(typeof(LineRenderer))]
public class Bullet : MonoBehaviour
{
    public Projectile Projectile
    {
        get
        {
            if (_proj == null)
                _proj = GetComponent<Projectile>();
            return _proj;
        }
    }
    private Projectile _proj;

    public LineRenderer LineRenderer
    {
        get
        {
            if (_lrn == null)
                _lrn = GetComponent<LineRenderer>();
            return _lrn;
        }
    }
    private LineRenderer _lrn;

    public float TrailLength = 5f;

    [ReadOnly]
    public float LENGTH;

    public void UponFired()
    {
        LineRenderer.positionCount = 2;
        LineRenderer.SetPosition(0, transform.position);
        LineRenderer.SetPosition(1, transform.position);
    }

    public void Update()
    {
        SetFirstVertex(transform.position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Projectile.Direction = Random.insideUnitCircle.normalized;
            AddNewVertex();
        }

        EnsureLineSize();
    }

    private void SetFirstVertex(Vector3 pos)
    {
        LineRenderer.SetPosition(0, pos);
    }

    private void EnsureLineSize()
    {
        if (LineRenderer.positionCount < 2)
            return;

        float MAX = TrailLength;
        float MAX_SQAURED = MAX*MAX;

        // Makes sure that the entirety of the line is no more than MAX units long.
        float dst = 0f;
        Vector2 previousPoint = LineRenderer.GetPosition(0);
        for (int i = 1; i < LineRenderer.positionCount; i++)
        {
            // Grab the position, get square distance to the next vertex.
            Vector2 pos = LineRenderer.GetPosition(i);

            dst += (pos - previousPoint).sqrMagnitude;
        }

        bool corrected = false;
        // Is it more than the max length?
        if(dst >= MAX_SQAURED)
        {
            // Turn the square distance into the real distance.
            dst = Mathf.Sqrt(dst);

            // Now we need to remove this excess from the line, starting from the last vertex moving towards the first vertex.
            float excess = dst - MAX;

            // Trim all the excess from the line.
            // Normally the excess is no more than one frame's worth of movement, but this is the only way to ensure very fast, bouncing projectiles can maintain a continuous line.
            RemoveFromLine(excess, -1);

            corrected = true;
        }

        dst = 0f;
        previousPoint = LineRenderer.GetPosition(0);
        for (int i = 1; i < LineRenderer.positionCount; i++)
        {
            // Grab the position, get square distance to the next vertex.
            Vector2 pos = LineRenderer.GetPosition(i);

            dst += (pos - previousPoint).sqrMagnitude;
        }
        LENGTH = Mathf.Sqrt(dst);

        if(corrected)
            Debug.Assert(Mathf.Abs(TrailLength - LENGTH) < 0.1f, "Target: {0}, got: {1}".Form(TrailLength, LENGTH));
    }

    /// <summary>
    /// Removes the toRemove distance, in units, from the edge (index, index - 1) and returns the amount that could not be removed, if any.
    /// This method uses recursion to remove the inital toRemove value from the whole line, assuming that the inital index supplied had a value of (vertexCount - 1) or simply -1.
    /// </summary>
    /// <param name="toRemove">The target length to remove from the edge. The distance is removed from index towards (index - 1).</param>
    /// <param name="index">The index of the vertex to start removing from.</param>
    /// <returns>The remiaining length that could not be removed, if any.</returns>
    private float RemoveFromLine(float toRemove, int index)
    {
        if (index == 0)
            return 0f;
        if (index < 0)
            index = LineRenderer.positionCount - 1;

        Vector2 current = LineRenderer.GetPosition(index);
        Vector2 next = LineRenderer.GetPosition(index - 1);

        float dst = Vector2.Distance(current, next);
        if(toRemove > dst)
        {
            // Remove the last point (this one).
            LineRenderer.positionCount -= 1;
            return RemoveFromLine(toRemove - dst, index - 1);
        }
        else
        {
            // The distance between the current point and the next one is less that the target length to remove.
            // Therefore, the resulting new vertex position will line alone the line (current <-> next).
            // The return value should also logically at this point be zero.

            Vector2 diff = current - next;
            diff.Normalize();

            // Now that we have the direction, set it's magnitude to:
            // Distance - toRemove
            // to achieve the final position of the last vertex.

            diff *= dst - toRemove;
            Vector2 finalPos = next + diff;
            Debug.Log("Removed {0}/{1} [~{4}] from point ({2} <-> {3}) on frame {5}".Form(Vector2.Distance(current, finalPos), toRemove, index, index - 1, Mathf.Abs(toRemove - Vector2.Distance(current, finalPos)), Time.frameCount));
            LineRenderer.SetPosition(index, finalPos);
            return 0f;
        }
    }

    /// <summary>
    /// Adds the current position as a new vertex, placed immediately after the first vertex.
    /// </summary>
    private void AddNewVertex()
    {
        // Say we have two points:
        // The start and the current point, where 0 is current and 1 is start.
        // Now 0 must become 1 and 1 become 2.
        LineRenderer.positionCount++;
        for (int i = LineRenderer.positionCount - 1; i >= 1; i--)
        {
            LineRenderer.SetPosition(i, LineRenderer.GetPosition(i - 1));
        }
    }
}
