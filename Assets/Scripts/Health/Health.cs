
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains information for a single character about their Health state, including current HP, max HP and armour.
/// </summary>
public class Health : MonoBehaviour
{
    public const float ARMOUR_RESISTANCE = 0.5f;

    public bool Invunerable = false;

    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = Mathf.Max(value, 0f);
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        }
    }
    [SerializeField]
    private float _maxHealth = 100f;

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        protected set
        {
            _currentHealth = Mathf.Clamp(value, 0f, MaxHealth);
        }
    }
    [SerializeField]
    private float _currentHealth = 100f;

    public float CurrentArmour
    {
        get
        {
            return _currentArmour;
        }
        protected set
        {
            _currentArmour = Mathf.Clamp(value, 0f, MaxArmour);
        }
    }
    [SerializeField]
    private float _currentArmour = 0f;

    public float MaxArmour
    {
        get
        {
            return _maxArmour;
        }
        set
        {
            _maxArmour = Mathf.Max(value, 0f);
            CurrentArmour = Mathf.Min(CurrentArmour, MaxArmour);
        }
    }
    [SerializeField]
    private float _maxArmour;

    [HideInInspector]
    public UnityEvent UponDeath = new UnityEvent();

    public void Reset(float maxHealth, float health, float maxArmour, float armour)
    {
        MaxHealth = maxHealth;
        CurrentHealth = health;
        MaxArmour = maxArmour;
        CurrentArmour = armour;
    }

    public bool IsDead
    {
        get
        {
            return CurrentHealth <= 0f;
        }
    }

    public bool HasArmour
    {
        get
        {
            return CurrentArmour > 0f;
        }
    }

    public float HealthNormalized
    {
        get
        {
            return Mathf.Clamp01(CurrentHealth / MaxHealth);
        }
    }

    public float HealthPercetage
    {
        get
        {
            return HealthNormalized * 100f;
        }
    }

    public float ArmourNormalized
    {
        get
        {
            return Mathf.Clamp01(CurrentArmour / MaxArmour);
        }
    }

    public float ArmourPercentage
    {
        get
        {
            return ArmourNormalized * 100f;
        }
    }

    /// <summary>
    /// Gets the value of the current health, rounded up to the closest integer. Done to avoid counterintuituve
    /// health display values such as 0 hp still being alive.
    /// </summary>
    /// <returns>The integer current health value, rounded up.</returns>
    public int GetDisplayHealth()
    {
        return Mathf.CeilToInt(CurrentHealth);
    }

    /// <summary>
    /// Gets the value of the current armour, rounded up to the closest integer. Avoids situations where armour can
    /// appear to be gone but there is actually still some armour remaining.
    /// </summary>
    /// <returns>The integer current armour value, rounded up.</returns>
    public int GetDisplayArmour()
    {
        return Mathf.CeilToInt(CurrentArmour);
    }

    /// <summary>
    /// Deals damage to the current Health state, reducing health and/or armour.
    /// </summary>
    /// <param name="baseDamage">The base damage value. With no armour, or 1 armour penetration, this will be the damage dealt to the health directly.</param>
    /// <param name="armourPen">With 1 armour penetration, ignores armour. With 0 armour penetration, will not damage health until armour is destroyed, which could happen in a single hit given enough base damage.</param>
    /// <returns>A vector 3 where the X component is the damage dealt to the health, the Y component is the damage dealt to armour, and the Z component is the damage left over, which would indicate that the character is dead.</returns>
    public Vector3 DealDamage(float baseDamage, float armourPen)
    {
        /* 
         * Base rules:
         * - Armour takes half the damage from hits. So a base 100 attack would only remove 50 armour points.
         * - Health does not drop until armour is gone, unless armour penetration is used.
         * - When armour penetration is used, damage against armour is reduced depending on the penetration.
         */

        if (baseDamage <= 0f)
            return Vector2.zero;
        if (Invunerable)
            return new Vector3(0f, 0f, 0f);
        
        armourPen = Mathf.Clamp01(armourPen);

        float damage = baseDamage;
        float damageToHealth = 0f;
        float damageToArmour = 0f;

        // Distribute damage correctly.
        if (CurrentArmour > 0f && armourPen != 1f)
        {
            // Means that with armour pen of 0, deals full damage to armour, with armour pen 1 deals no damage to armour.
            float d2a = Mathf.Min(damage * (1 - armourPen), CurrentArmour / ARMOUR_RESISTANCE);
            damage -= d2a;

            CurrentArmour -= d2a * ARMOUR_RESISTANCE;
            damageToArmour += d2a * ARMOUR_RESISTANCE;
        }
        if(damage > 0f)
        {
            // We have only health, or full armour pen hit.
            float d2h = Mathf.Min(CurrentHealth, damage);
            CurrentHealth -= d2h;
            damageToHealth += d2h;
            damage -= d2h;
        }

        var calculated = new Vector3(damageToHealth, damageToArmour, damage);
        var final = PostDealDamage(calculated, baseDamage, armourPen);

        if(CurrentHealth <= 0f)
        {
            if(final.x > 0f)
            {
                if(UponDeath != null)
                {
                    UponDeath.Invoke();
                }
            }
        }

        return final;
    }

    protected virtual Vector3 PostDealDamage(Vector3 calculated, float baseDamage, float armourPen)
    {
        return calculated;
    }

    /// <summary>
    /// Gets the health component on a transform or any of it's parents. Will return null if the Health component is not found.
    /// </summary>
    /// <param name="t">The transform to search, including any and all parent transforms.</param>
    /// <param name="ifCanDamage">When true, will return null if the found health component cannot be damaged, for example if it is dead or invunerable.</param>
    /// <returns>The Health component, or null.</returns>
    public static Health GetHealthOf(Transform t, bool ifCanDamage = true)
    {
        if (t == null)
            return null;

        var health = t.GetComponentInParent<Health>();
        if (health == null)
            return null;

        if (health.IsDead && ifCanDamage)
            return null;

        if (health.Invunerable && ifCanDamage)
            return null;

        return health;
    }
}