
using UnityEngine;

public class UndyingHealth : Health
{
    public override Vector3 DealDamage(float baseDamage, float armourPen)
    {
        // Deal normal damage.
        Vector3 damage = base.DealDamage(baseDamage, armourPen);

        // Reset health and armour. No resultant damage!
        CurrentHealth = MaxHealth;
        CurrentArmour = MaxArmour;

        // Return the mock damage to the world. To everyone else it will appear as though the
        // object has taken damage. Useful for enemies or NPC that are never meant to be killed.
        return damage;
    }
}