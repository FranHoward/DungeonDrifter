using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Attack }
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float patrolDistance = 1.0f;
    [SerializeField] private float patrolPointSpread = 1.5f;

    private static int nextPatrolSlot;
    private NavMeshAgent agent;
    private State state;
    private int currentIndex;
    private bool patrolDestinationSet;
    private Vector3 patrolOffset;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float damage = 10f;
    private Health playerHealth;
    private float nextAttackTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPatrolSlots()
    {
        nextPatrolSlot = 0;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (points != null && points.Length > 0)
        {
            int patrolSlot = nextPatrolSlot++;
            currentIndex = patrolSlot % points.Length;

            float angle = patrolSlot * 137.5f * Mathf.Deg2Rad;
            patrolOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle))
                * patrolPointSpread;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null || playerHealth.IsDead)
        {
            agent.SetDestination(transform.position);
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        state = dist <= attackRange ? State.Attack
            : dist <= chaseRange ? State.Chase
            : State.Patrol;

        if (state != State.Patrol)
        {
            patrolDestinationSet = false;
        }
        
        switch (state)
        {
            case State.Chase: agent.SetDestination(player.position); break;
            case State.Attack: Attack(); break;
            case State.Patrol: UpdatePatrol(); break;
        }
    }

    private void UpdatePatrol()
    {
        if (points == null || points.Length == 0)
        {
            return;
        }

        if (!patrolDestinationSet)
        {
            SetCurrentPatrolDestination();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= patrolDistance)
        {
            currentIndex = (currentIndex + 1) % points.Length;
            SetCurrentPatrolDestination();
        }
    }

    private void SetCurrentPatrolDestination()
    {
        Transform point = points[currentIndex];

        if (point == null)
        {
            return;
        }

        Vector3 destination = point.position + patrolOffset;

        if (NavMesh.SamplePosition(
            destination,
            out NavMeshHit hit,
            patrolPointSpread,
            agent.areaMask))
        {
            destination = hit.position;
        }

        agent.SetDestination(destination);
        patrolDestinationSet = true;
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        Debug.Log($"Player is attacked for {damage} damage.");
        playerHealth.TakeDamage(damage);
    }
}
