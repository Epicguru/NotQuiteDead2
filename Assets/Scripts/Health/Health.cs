
using UnityEngine;

/// <summary>
/// Contains information for a single character about their Health state, including current HP, max HP and armour.
/// </summary>
public class Health : MonoBehaviour
{
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
        private set
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
        private set
        {
            _currentArmour = Mathf.Clamp(value, 0f, MaxArmour);
        }
    }
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
    private float _maxArmour;

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

    public Vector2 DealDamage(float baseDamage, float armourPen)
    {
        const float DAMAGE_AGAINST_ARMOUR = 0.5f;

        /* 
         * Base rules:
         * - Armour takes half the damage from hits. So a base 100 attack would only remove 50 armour points.
         * - Health does not drop until armour is gone, unless armour penetration is used.
         * - When armour penetration is used, damage against armour is reduced depending on the penetration.
         */

        float damage = baseDamage;

        float damageToHealth = 0f;
        float damageToArmour = 0f;

        while(damage > 0f)
        {
            // Can't distribute the damage if we have no health left. No point.
            if (CurrentHealth <= 0f)
                break;

            if(CurrentArmour > 0f)
            {
                float d2a = damage * DAMAGE_AGAINST_ARMOUR;
                if(d2a > CurrentArmour)
                {
                    float remainder = d2a - CurrentArmour;
                }
            }
        }

        return new Vector2(damageToHealth, damageToArmour);
    }
}