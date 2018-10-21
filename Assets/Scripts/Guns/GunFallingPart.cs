using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableObject))]
public class GunFallingPart : MonoBehaviour
{
    public Vector2 Velocity;
    public float AngularVelocity;
    public float Lifetime = 2f;
    public bool UseGravity = true;

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spr == null)
                _spr = GetComponent<SpriteRenderer>();
            return _spr;
        }
    }
    private SpriteRenderer _spr;

    public PoolableObject PoolableObject
    {
        get
        {
            if (_poolable == null)
                _poolable = GetComponent<PoolableObject>();
            return _poolable;
        }
    }
    private PoolableObject _poolable;


    public bool Mimic(SpriteRenderer spr)
    {
        if (spr == null)
            return true;

        transform.position = spr.transform.position;
        transform.rotation = spr.transform.rotation;
        transform.localScale = transform.localScale;
        var c = spr.GetComponentInParent<Character>();
        if(c != null)
        {
            var s = transform.localScale;
            s.x = c.Direction.Right ? 1f : -1f;
            transform.localScale = s;
        }
        SpriteRenderer.sprite = spr.sprite;
        return c == null ? true : c.Direction.Right;
    }

    public void MimicWithVel(SpriteRenderer spr, Vector2 vel, float angular, bool useGravity, Vector2 inherited)
    {
        bool right = Mimic(spr);
        var velocity = vel;
        if (!right)
        {
            velocity.x *= -1f;
            angular *= -1f;
        }
        SetVelocity(velocity + inherited, angular, useGravity);
    }

    public void SetVelocity(Vector2 velocity, float angular, bool useGravity)
    {
        this.Velocity = velocity;
        this.AngularVelocity = angular;
        this.UseGravity = useGravity;
    }

    public void StartLife(float time)
    {
        Lifetime = time;
    }

    private void Update()
    {
        transform.position += (Vector3)Velocity * Time.deltaTime;
        var angles = transform.localEulerAngles;
        angles.z += AngularVelocity * Time.deltaTime;
        transform.localEulerAngles = angles;

        if (UseGravity)
        {
            Velocity.y -= 9.8f * Time.deltaTime;
        }

        Lifetime -= Time.deltaTime;
        if(Lifetime <= 0f)
        {
            Pool.Return(this.PoolableObject);
        }
    }
}
