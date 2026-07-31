using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Room[] roomPrefabs;
    [SerializeField] private GameObject enemyPrefabs;
    [SerializeField] private int roomCount = 5;
    [SerializeField] private float playerHeightAboveGround = 1f;
    [SerializeField] private NavMeshSurface navMeshSurface;

    private void Start() => Generate();

    private void Generate()
    {
        ClearGeneratedRooms();

        Room previousRoom = null;
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        var generatedRooms = new List<Room>(roomCount);

        for (int i = 0; i < roomCount; i++)
        {
            var prefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            var room = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            room.DisableLocalNavMeshSurfaces();

            if (previousRoom != null)
                AlignEntranceToExit(room, previousRoom.Exit);
            else if (player != null)
                PlacePlayerInFirstRoom(player, room);

            generatedRooms.Add(room);
            previousRoom = room;
        }

        BuildNavMesh();

        foreach (var room in generatedRooms)
            SpawnEnemies(room);
    }

    private static void AlignEntranceToExit(Room room, Transform previousExit)
    {
        Quaternion rotationOffset =
            previousExit.rotation *
            Quaternion.Euler(0f, 180f, 0f) *
            Quaternion.Inverse(room.Entrance.rotation);

        room.transform.rotation = rotationOffset * room.transform.rotation;
        room.transform.position += previousExit.position - room.Entrance.position;
    }

    private void PlacePlayerInFirstRoom(Transform player, Room room)
    {
        player.position = GetRoomCenter(room) + Vector3.up * playerHeightAboveGround;

        if (player.TryGetComponent(out Rigidbody body))
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    private static Vector3 GetRoomCenter(Room room)
    {
        return Vector3.Lerp(room.Entrance.position, room.Exit.position, 0.5f);
    }

    private void BuildNavMesh()
    {
        if (navMeshSurface == null)
            navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == null)
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();

        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
    }

    private void SpawnEnemies(Room room)
    {
        int enemyNum = Random.Range(1, 4);

        for (int e = 0; e < enemyNum; e++)
        {
            Vector3 candidate = GetRoomCenter(room) +
                new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                Instantiate(enemyPrefabs, hit.position, Quaternion.identity, room.transform);
        }
    }

    private void ClearGeneratedRooms()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}
