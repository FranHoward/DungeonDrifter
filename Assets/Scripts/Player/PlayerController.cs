using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody rb;
    private PlayerStats stats;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
    }

    public void OnMove(InputValue value) => input = value.Get<Vector2>();

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        float speedMultiplier = stats != null ? stats.SpeedMultiplier : 1f;
        rb.MovePosition(
            rb.position + dir * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
    }
}
