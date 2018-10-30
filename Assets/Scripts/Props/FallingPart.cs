using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPart : MonoBehaviour
{
    public bool Released = false;
    public AnimationCurve AlphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private SpriteRenderer[] renderers;
    private float g;
    private float t;
    private float st;
    private Vector2 vel;
    private float aVel;

    public void Release(Vector2 velocity, float angularVelocity, float time, float gravity)
    {
        Released = true;

        transform.parent = null;

        this.vel = velocity;
        this.aVel = angularVelocity;
        this.t = time;
        this.st = time;
        this.g = gravity;
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!Released)
            return;

        float p = t / st;
        float a = AlphaCurve.Evaluate(1f - Mathf.Clamp01(p));
        SetAlpha(a);

        this.transform.position += (Vector3)vel * Time.deltaTime;
        this.transform.Rotate(0f, 0f, aVel * Time.deltaTime);

        vel.y += Time.deltaTime * g;

        t -= Time.deltaTime;
        if(t <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void SetAlpha(float a)
    {
        foreach (var r in renderers)
        {
            if(r != null)
            {
                var c = r.color;
                c.a = a;
                r.color = c;
            }
        }
    }
}
