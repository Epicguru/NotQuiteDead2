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

    public void UponFired()
    {
        LineRenderer.positionCount = 2;
        LineRenderer.SetPosition(0, transform.position);
        LineRenderer.SetPosition(1, transform.position);
    }

    public void Update()
    {
        SetFirstVertex(transform.position);

        //if(Random.Range(0, 100) == 0)
        //{
        //    AddWaypoint();
        //    Projectile.Direction = Random.insideUnitCircle.normalized;
        //}
    }

    private void SetFirstVertex(Vector3 pos)
    {
        LineRenderer.SetPosition(0, pos);
    }

    private void AddWaypoint()
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
