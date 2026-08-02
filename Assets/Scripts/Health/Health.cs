using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float current;

    public bool IsDead => current <= 0f;
    public float CurrentHealth => current;
    public float MaxHealth => maxHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action<float> OnHealed;
    public event Action OnDeath;

    private void Awake() => current = maxHealth;

    public void SetMaxHealth(float value, bool refill = true)
    {
        maxHealth = Mathf.Max(1f, value);
        current = refill ? maxHealth : Mathf.Min(current, maxHealth);
        OnHealthChanged?.Invoke(current, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (current <= 0 || amount <= 0) return;

        float previous = current;
        current = Mathf.Max(0, current - amount);
        OnHealthChanged?.Invoke(current, maxHealth);
        OnDamaged?.Invoke(previous - current);

        if (current <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        float previous = current;
        current = Mathf.Min(maxHealth, current + amount);
        OnHealthChanged?.Invoke(current, maxHealth);

        float healedAmount = current - previous;
        if (healedAmount > 0)
            OnHealed?.Invoke(healedAmount);
    }

    public void Die()
    {
        OnDeath?.Invoke();

        if (CompareTag("Player"))
        {
            Debug.Log("Player died.");
            return;
        }

        Debug.Log($"{gameObject.name} died and was destroyed.");
        Destroy(gameObject);
    }
}
