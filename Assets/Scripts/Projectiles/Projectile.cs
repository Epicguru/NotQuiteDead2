﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PoolableObject))]
public class Projectile : MonoBehaviour
{
    public static Dictionary<byte, Projectile> LoadedPrefabs;

    public PoolableObject PoolableObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolableObject>();
            return _po;
        }
    }
    private PoolableObject _po;

    public byte ID;

    [Tooltip("The normalized direction vector.")]
    public Vector2 Direction = new Vector2(1f, 0f);
    [Tooltip("The speed at which the projectile travels, in units per second.")]
    public float CurrentSpeed = 10f;
    [Tooltip("The distance from the origial firing point at which the projectile is automatically destroyed.")]
    public float MaxRange = 150f;

    private Vector2 startPos;

    public float SquareDistanceTravelled
    {
        get
        {
            // Should just be used for comparisons.
            return ((Vector2)(transform.position) - startPos).sqrMagnitude;
        }
    }

    public float DistanceTravelled
    {
        get
        {
            // Good ol' pythagoras.
            return Mathf.Sqrt(SquareDistanceTravelled);
        }
    }

    public UnityEvent UponFired;

    public void Update()
    {
        Vector2 currentPos = transform.position;

        // Apply transformation here.
        currentPos += Direction.normalized * CurrentSpeed * Time.deltaTime;

        transform.position = currentPos;

        EnsureRange();
    }

    private bool EnsureRange()
    {
        // Automatically destroys this projectile if it goes out of range. Returns true if destroyed.

        if(DistanceTravelled >= MaxRange)
        {
            Pool.Return(PoolableObject);
            return true;
        }
        return false;
    }

    private void UponSpawned()
    {
        startPos = transform.position;
    }

    public static void LoadAll()
    {
        if (LoadedPrefabs != null)
            return;

        LoadedPrefabs = new Dictionary<byte, Projectile>();
        byte highest = 0;

        var fromDisk = Resources.LoadAll<Projectile>("Projectiles");
        foreach (var p in fromDisk)
        {
            if (LoadedPrefabs.ContainsKey(p.ID))
            {
                Debug.LogError("Duplicate projectile ID {0}".Form(p.ID));
            }
            else
            {
                LoadedPrefabs.Add(p.ID, p);
                if (highest < p.ID)
                    highest = p.ID;
            }
        }

        Debug.Log("Loaded {0} projectile prefabs, highest ID is {1}".Form(LoadedPrefabs.Count, highest));
    }

    public static void UnloadAll()
    {
        LoadedPrefabs.Clear();
    }

    public static bool IsLoaded(byte id)
    {
        return LoadedPrefabs != null && LoadedPrefabs.ContainsKey(id);
    }

    public static Projectile GetPrefab(byte id)
    {
        if (IsLoaded(id))
        {
            return LoadedPrefabs[id];
        }
        else
        {
            Debug.LogWarning("No projectile prefab found for ID {0}. Returning null.".Form(id));
            return null;
        }
    }

    public static Projectile Spawn(byte id, Vector2 pos, Vector2 direction)
    {
        var prefab = GetPrefab(id);
        if (prefab == null)
            return null;

        var spawned = Pool.Get(prefab.PoolableObject).GetComponent<Projectile>();
        spawned.transform.position = pos;
        spawned.Direction = direction;

        if(spawned.UponFired != null)
            spawned.UponFired.Invoke();
        spawned.UponSpawned();

        return spawned;
    }
}
