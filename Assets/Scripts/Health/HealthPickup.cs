using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healthAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out var hp))
        {
            hp.Heal(healthAmount);
            gameObject.SetActive(false);
        }
    }
}
