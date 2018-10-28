using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public Health Health;
    public FallingPart Part;

    private void Awake()
    {
        if(Health != null)
        {
            Health.UponDeath.AddListener(UponDeath);
        }
    }

    private void UponDeath()
    {
        const float magnitude = 6.5f;
        const float gravity = -12f;
        const float time = 2f;

        float angle = 90f + Random.Range(-50f, 50f);
        Vector2 vel = angle.ToDirection() * magnitude;
        float aVel = Random.Range(360f, 1000f) * (Random.value > 0.5f ? 1f : -1f);

        Part.Release(vel, aVel, time, gravity);
    }
}
