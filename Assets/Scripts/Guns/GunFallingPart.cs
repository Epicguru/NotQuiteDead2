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


    public void Mimic(SpriteRenderer spr)
    {
        if (spr == null)
            return;

        transform.position = spr.transform.position;
        transform.rotation = spr.transform.rotation;
        transform.localScale = transform.lossyScale;
        SpriteRenderer.sprite = spr.sprite;
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
