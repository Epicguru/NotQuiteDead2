using System.Collections.Generic;
using UnityEngine;

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

    public void Update()
    {
        Vector2 currentPos = transform.position;

        // Apply transformation here.
        currentPos += Direction.normalized * CurrentSpeed * Time.deltaTime;

        transform.position = currentPos;
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
}
