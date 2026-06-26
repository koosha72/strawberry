using Strawberry.Core;
using Strawberry.EventSystem;

namespace Strawberry.Components.Lifecycle;

/// <summary>
/// This event occurs every time an entity gets killed. (The health of the HealthComponent reaches 0 or below)
/// </summary>
public struct KilledEvent : IStrawberryEvent
{
    /// <summary>
    /// The HealthComponent that got killed (The event caller).
    /// </summary>
    public HealthComponent HealthComponent;
}

/// <summary>
/// This event occurs every time an entity is damaged.
/// </summary>
public struct DamagedEvent : IStrawberryEvent
{
    /// <summary>
    /// The HealthComponent that got damaged (The event caller).
    /// </summary>
    public HealthComponent HealthComponent;

    /// <summary>
    /// Gets the amount of damage dealt to this entity.
    /// </summary>
    public float Amount { get; init; }
}


/// <summary>
/// This event occurs every time an entity is healed.
/// </summary>
public struct HealedEvent : IStrawberryEvent
{
    /// <summary>
    /// The HealthComponent that got healed (The event caller).
    /// </summary>
    public HealthComponent HealthComponent;

    /// <summary>
    /// Gets the amount of health that was healed to the entity by this event
    /// </summary>
    public float Amount { get; init; }
}

/// <summary>
/// A component that manages the health of an entity and raises events when the health changes.
/// </summary>
public class HealthComponent : BaseComponent
{
    /// <summary>
    /// Gets or sets the amount of health the entity has, if it reaches 0 or less than 0, then the entity will be destroyed by the system and the KilledEvent will raise. (Default value is 100)
    /// </summary>
    public float Health { get; set; } = 100.0f;

    float maxHealth = 100f;
    /// <summary>
    /// Gets or sets the maximum health the entity can have. (Default value is 100). If the Health is greater than MaxHealth, then it will be set to MaxHealth.
    /// </summary>
    public float MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            if (Health > maxHealth)
            {
                Health = maxHealth;
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the entity should be destroyed when it gets killed. (Default value is true)
    /// </summary>
    public bool DestroyOnKill { get; set; } = true;

    /// <summary>
    /// Gets whether the entity is dead.
    /// </summary>
    public bool IsDead { get; private set; }

    /// <summary>
    /// Heals the entity by the given amount.
    /// </summary>
    /// <param name="amount">The amount to heal the entity by.</param>
    public void Heal(float amount)
    {
        if (amount <= 0 || IsDead)
            return;
        Health += amount;
        if (Health > MaxHealth)
            Health = MaxHealth;

        EventManager.Invoke(this, new HealedEvent() { Amount = amount, HealthComponent = this });
    }

    /// <summary>
    /// Damages the entity by the given amount. If the health is less than or equal to 0, it will invoke the KilledEvent and destroy the owner if DestroyOnKill is true.
    /// </summary>
    /// <param name="amount">The amount to damage the entity by.</param>
    public void Damage(float amount)
    {
        if (amount <= 0 || IsDead)
            return;
        Health -= amount;
        EventManager.Invoke(this, new DamagedEvent() { Amount = amount, HealthComponent = this });

        if (Health <= 0.0f)
        {
            EventManager.Invoke(this, new KilledEvent() { HealthComponent = this });
            if (DestroyOnKill)
                Owner.Destroy();
            Health = 0;
            IsDead = true;
        }
    }
}