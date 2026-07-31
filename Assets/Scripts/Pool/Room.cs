using UnityEngine;
using Unity.AI.Navigation;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private Transform exit;

    public Transform Entrance => entrance;
    public Transform Exit => exit;

    public void DisableLocalNavMeshSurfaces()
    {
        foreach (var surface in GetComponentsInChildren<NavMeshSurface>(true))
        {
            surface.RemoveData();
            surface.enabled = false;
        }
    }
}
