using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Room[] roomPrefabs;
    [FormerlySerializedAs("enemyPrefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [Header("Enemy balance profiles")]
    [SerializeField] private EnemyData scout;
    [SerializeField] private EnemyData raider;
    [SerializeField] private EnemyData soldier;
    [SerializeField] private EnemyData brute;
    [SerializeField] private EnemyData elite;
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

        for (int i = 0; i < generatedRooms.Count; i++)
            SpawnEnemies(generatedRooms[i], i);
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

    private void SpawnEnemies(Room room, int roomIndex)
    {
        int enemyNum = Random.Range(1, 4);

        for (int e = 0; e < enemyNum; e++)
        {
            Vector3 candidate = GetRoomCenter(room) +
                new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
            if (!NavMesh.SamplePosition(
                    candidate, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                continue;
            }

            GameObject enemy = Instantiate(
                enemyPrefab, hit.position, Quaternion.identity, room.transform);

            if (enemy.TryGetComponent(out EnemyAI enemyAI))
                enemyAI.Configure(ChooseEnemyProfile(roomIndex, e, enemyNum));
        }
    }

    private EnemyData ChooseEnemyProfile(
        int roomIndex,
        int enemyIndex,
        int enemyCount)
    {
        bool isFinalRoom = roomIndex == roomCount - 1;

        if (isFinalRoom)
        {
            if (enemyIndex == 0)
                return elite;

            // The final room has one Elite, at most one Brute, then Soldiers.
            return enemyIndex == 1 && enemyCount > 1 ? brute : soldier;
        }

        switch (roomIndex)
        {
            case 0:
                return scout;
            case 1:
                return Random.value < 0.5f ? scout : raider;
            case 2:
                return Random.value < 0.5f ? raider : soldier;
            default:
                // Room four has no more than one Brute.
                return enemyIndex == 0 ? brute : soldier;
        }
    }

    private void ClearGeneratedRooms()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}
