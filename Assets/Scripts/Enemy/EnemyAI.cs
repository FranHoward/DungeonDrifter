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
    [SerializeField] private float patrolDistancce = 1.0f;
    [SerializeField] private float patrolPointSpread = 1.5f;

    private static int nextPatrolSlot;
    private NavMeshAgent agent;
    private State state;
    private int currentIndex;
    private bool patrolDestinationSet;
    private Vector3 patrolOffset;

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
        }
    }

    private void Update()
    {
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
            case State.Attack: agent.SetDestination(transform.position); break;
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

        if (!agent.pathPending && agent.remainingDistance <= patrolDistancce)
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
}
