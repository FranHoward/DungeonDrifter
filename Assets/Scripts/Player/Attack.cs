using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DoAttack();
    }

    private void DoAttack()
    {
        Vector3 center = transform.position + transform.forward * range * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, range, enemyLayer);

        foreach(var hit in hits)
        {
            Debug.Log($"命中: {hit.name}");
            if (hit.TryGetComponent<Health>(out var health))
                health.TakeDamage(damage);
            if (hit.TryGetComponent<Renderer>(out var r))
                r.material.color = Color.red;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * range * 0.5f, range);
    }
}
