using UnityEngine;

public class ObjectHealth : HealthSystem
{
    private BreakObject breakObject;
    private ObjectProperties properties;

    protected override void Awake()
    {
        breakObject = GetComponent<BreakObject>();
        properties = GetComponent<ObjectProperties>();

        if (properties != null)
        {
            maxHealth = properties.GetDefaultHealth();
        }

        base.Awake();

        if (breakObject == null)
            Debug.LogError($"BreakObject component not found on {gameObject.name}");
    }

    protected override void Die()
    {
        if (breakObject != null)
        {
            breakObject.HandleObjectDestruction();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }
}
