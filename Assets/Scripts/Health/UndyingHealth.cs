
using UnityEngine;

public class UndyingHealth : Health
{
    protected override Vector3 PostDealDamage(Vector3 calculated, float baseDamage, float armourPen)
    {
        // Reset health and armour. No resultant damage!
        CurrentHealth = MaxHealth;
        CurrentArmour = MaxArmour;

        // Return the mock damage to the world. To everyone else it will appear as though the
        // object has taken damage. Useful for enemies or NPC that are never meant to be killed.
        return calculated;
    }
}