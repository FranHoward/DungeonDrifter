using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -8);
    [SerializeField] private float smooth = 5f;

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
    }
}