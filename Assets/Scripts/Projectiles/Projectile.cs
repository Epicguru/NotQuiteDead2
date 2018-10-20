using System.Collections.Generic;
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
    [Tooltip("The maximum number of bounces that the projectile can make off of solid surfaces.")]
    public int MaxBounces = 0;
    [Tooltip("The maximum penetration count against objects that have Health, such as characters.")]
    public int MaxPenetration = 0;
    [Tooltip("The baseline damage that the projectile deals against objects.")]
    public float Damage = 50f;
    [Tooltip("The amout of armour penetration that the projectile has when damaging Health objects. Does not affect real penetration.")]
    [Range(0f, 1f)]
    public float ArmourPenetration = 0f;

    private Vector2 startPos;
    private int remainingBounces;
    private int remainingPenetrations;
    private bool destroyed = false;

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
        if (destroyed)
        {
            Pool.Return(this.PoolableObject);
            return;
        }

        ResolveAndMove(CurrentSpeed * Time.deltaTime);
        EnsureRange();
    }

    private List<Health> damaged = new List<Health>();
    private void ResolveAndMove(float distance)
    {
        // Takes the projectile's current position, direction, and moves the projectile whilst reporting and solving collisions.

        Vector2 currentPos = transform.position;
        Vector2 currentDirection = Direction;

        if (currentDirection.sqrMagnitude != 1f)
            currentDirection.Normalize();

        const int MAX_ITTER = 100;
        int j = 0;
        bool runLoop = true;
        while (runLoop && j < MAX_ITTER)
        {
            j++;

            int count;
            int interactions = 0;
            var hits = GetHits(currentPos, currentDirection, distance, out count);
            // For each collision, which will be in order, see if it has a health object.
            for (int i = 0; i < count; i++)
            {
                var hit = hits[i];
                Health h = Health.GetHealthOf(hit.transform, false);

                if (h == null)
                {
                    // No health object, so it is an indestructible object, such as a wall.
                    // Bounce off it, or just stop.

                    // If it is a trigger, pass right through it, like a ghost.
                    if (hit.collider.isTrigger)
                        continue;                        

                    if (remainingBounces > 0)
                    {
                        // Calculate the distance to the bounce point, and subtract that from the pending distance.
                        float distanceToWall = Vector2.Distance(currentPos, hit.point);
                        float offset = 0.025f;
                        distance -= distanceToWall - offset;
                        currentPos = hit.point + (offset * hit.normal);

                        // Calculate the new reflected direction, ready for the next itteration.
                        currentDirection = CalculateReflection(currentDirection, hit.normal).normalized;

                        // Have one less bounce.
                        remainingBounces--;

                        var r = GetRigidbody(hit.transform);
                        if (r != null)
                        {
                            r.AddForceAtPosition(Direction * Damage * 0.1f, hit.point, ForceMode2D.Impulse);
                        }

                        SendMessage("UponProjectileBounce", hit, SendMessageOptions.DontRequireReceiver);
                        interactions++;
                        break;
                    }
                    else
                    {
                        currentPos = hit.point;
                        runLoop = false;
                        destroyed = true; // Destroy the whole projectile.
                        interactions++;
                        SendMessage("UponProjectileDestroyed", SendMessageOptions.DontRequireReceiver);
                        break;
                    }
                }
                else
                {
                    if (damaged.Contains(h))
                    {
                        continue;
                    }

                    if (h.IsDead) // Ignore dead objects.
                        continue;
                    if (h.Invunerable)
                        continue; // Ignore invunerable objects.

                    // TODO hit (and penetrate?) health objects.

                    if(hit.collider.isTrigger && hit.collider.GetComponent<Health>() == null)
                    {
                        // If the collider hit is a trigger, and the health component is not attached directly to it, ignore this collision.
                        continue;
                    }

                    // Can we penetrate this object?
                    if(remainingPenetrations == 0)
                    {
                        // Can't penetrate it but still deal damage.
                        if (!damaged.Contains(h))
                        {
                            float d = GetDamage();
                            h.DealDamage(d, ArmourPenetration);
                            damaged.Add(h);
                            SendMessage("UponProjectileDamage", hit, SendMessageOptions.DontRequireReceiver);
                        }

                        // Nope, stop right here.
                        currentPos = hit.point;
                        runLoop = false;
                        destroyed = true;
                        interactions++;
                        SendMessage("UponProjectileDestroyed", SendMessageOptions.DontRequireReceiver);
                        break;
                    }
                    else
                    {
                        // Go through it and deal damage...
                        remainingPenetrations--;

                        // Can we damage it or has it already been hit?
                        if (damaged.Contains(h))
                        {
                            continue;
                        }
                        else
                        {
                            damaged.Add(h);
                            interactions++;

                            // Deal the damage against the health component.
                            float d = GetDamage();
                            h.DealDamage(d, ArmourPenetration);

                            SendMessage("UponProjectilePenetrate", hit, SendMessageOptions.DontRequireReceiver);
                            SendMessage("UponProjectileDamage", hit, SendMessageOptions.DontRequireReceiver);

                            continue;
                        }
                    }
                }
            }

            if(count == 0 || interactions == 0)
            {
                // Clear path ahead, no need to do more processing.
                currentPos = currentPos + currentDirection * distance;
                break;
            }            
        }

        transform.position = currentPos;
        Direction = currentDirection;
    }

    private float GetDamage()
    {
        return this.Damage;
    }

    private const int MAX_HITS = 30;
    private static RaycastHit2D[] hits = new RaycastHit2D[MAX_HITS];
    private static RaycastHit2D[] GetHits(Vector2 start, Vector2 direction, float distance, out int count)
    {
        int realHits = Physics2D.RaycastNonAlloc(start, direction, hits, distance);
        if (realHits > MAX_HITS)
        {
            Debug.LogError("There were more raycast hits than are supported: {0} compared to max {1}. Change the max value to fix this.".Form(realHits, MAX_HITS));
            realHits = MAX_HITS;
        }

        count = realHits;
        return hits;
    }

    private Rigidbody2D GetRigidbody(Transform t)
    {
        if (t == null)
            return null;

        return t.GetComponentInParent<Rigidbody2D>();
    }

    private Vector2 CalculateReflection(Vector2 inDirection, Vector2 normal)
    {
        // Assumes that vectors are normalized. The normal MUST be normalized, and the input direction's magnitude will always equal that of the output (reflected) vector's magnitude, so
        // technically it doesn't have to be normalized.

        return inDirection - 2 * normal * (Vector2.Dot(inDirection, normal));
    }

    private bool EnsureRange()
    {
        // Automatically destroys this projectile if it goes out of range. Returns true if destroyed.

        if (DistanceTravelled > MaxRange)
        {
            transform.position -= -(Vector3)Direction * (DistanceTravelled - MaxRange);
            destroyed = true;
            SendMessage("UponProjectileDestroyed", SendMessageOptions.DontRequireReceiver);
            return true;
        }
        return false;
    }

    private void UponSpawned()
    {
        startPos = transform.position;
        remainingBounces = MaxBounces;
        remainingPenetrations = MaxPenetration;
        destroyed = false;
        damaged.Clear();
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

        if (spawned.UponFired != null)
            spawned.UponFired.Invoke();
        spawned.UponSpawned();

        return spawned;
    }
}
