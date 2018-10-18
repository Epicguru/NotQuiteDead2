
using UnityEngine;

public class HealthTests : MonoBehaviour
{
    public Health h;

    public void Start()
    {
        h.Reset(100f, 0f);
        var res = h.DealDamage(100f, 0f);
        Debug.Assert(res == new Vector3(100, 0, 0), res);

        h.Reset(100f, 0f);
        res = h.DealDamage(100f, 0f);
        Debug.Assert(res == new Vector3(100f, 0f, 0f), res);
        Debug.Assert(h.CurrentHealth == 0f);

        h.Reset(100f, 25f);
        res = h.DealDamage(100f, 0f);
        // 25 armour should take 50 damage, leaving 50 health to be taken.
        Debug.Assert(res == new Vector3(50f, 25f, 0f), res);
        Debug.Assert(h.CurrentHealth == 50f);
        Debug.Assert(h.CurrentArmour == 0f);

        h.Reset(100f, 100f);
        res = h.DealDamage(50f, 1f);
        Debug.Assert(res == new Vector3(50f, 0f, 0f), res);
        Debug.Assert(h.CurrentHealth == 50f);

        h.Reset(100f, 100f);
        res = h.DealDamage(100f, 0.5f);
        // Deals half damage against armour, so only 25 armour is removed.
        // Deals half of the total damage to health, so 50 removed.
        Debug.Assert(res == new Vector3(50, 25, 0), res);
        Debug.Assert(h.CurrentHealth == 50f);
        Debug.Assert(h.CurrentArmour == 75f);

        Debug.Log("Done tests.");
    }
}