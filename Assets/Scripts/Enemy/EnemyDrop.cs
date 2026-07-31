using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropTable dropTable;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += SpawnDrop;
    }

    private void OnDisable()
    {
        health.OnDeath -= SpawnDrop;
    }

    private void SpawnDrop()
    {
        if (dropTable != null && dropTable.TryRoll(out DropTable.DropEntry drop))
            DropPoolManager.Instance.Spawn(drop, transform.position);

        UpgradeManager.Instance?.RegisterKill();
    }
}
