using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        health.OnHealthChanged += UpdateBar;
    }

    private void Start()
    {
        UpdateBar(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        slider.SetValueWithoutNotify(max > 0f ? current / max : 0f);
    }
}
