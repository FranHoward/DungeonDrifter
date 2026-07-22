using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float current;

    public bool IsDead => current <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake() => current = maxHealth;

    public void TakeDamage(float amount)
    {
        if (current <= 0) return;
        current = Mathf.Max(0, current - amount);
        OnHealthChanged?.Invoke(current, maxHealth);
        if (current <= 0) Die();
    }

    public void Heal(float amount)
    {
        current = Mathf.Min(maxHealth, current + amount);
        OnHealthChanged?.Invoke(current, maxHealth);
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
