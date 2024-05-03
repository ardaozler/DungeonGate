using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Transform Player;
    public int NumberOfEnemiesToSpawn = 5;
    public float MinSpawnDelay = 1f; // Minimum spawn delay
    public float MaxSpawnDelay = 2f; // Maximum spawn delay
    public List<Enemy> EnemyPrefabs = new List<Enemy>();
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

    private bool canSpawn = true; // Add this variable to control spawning

    private void Awake()
    {
        for (int i = 0; i < EnemyPrefabs.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], NumberOfEnemiesToSpawn));
        }
    }

    private void Start()
    {
        Triangulation = NavMesh.CalculateTriangulation();

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int SpawnedEnemies = 0;

        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if (canSpawn) // Check if spawning is allowed
            {
                if (EnemySpawnMethod == SpawnMethod.RoundRobin)
                {
                    SpawnRoundRobinEnemy(SpawnedEnemies);
                }
                else if (EnemySpawnMethod == SpawnMethod.Random)
                {
                    SpawnRandomEnemy();
                }

                SpawnedEnemies++;
            }

            float delay = Random.Range(MinSpawnDelay, MaxSpawnDelay); // Random delay between min and max
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnRoundRobinEnemy(int SpawnedEnemies)
    {
        int SpawnIndex = SpawnedEnemies % EnemyPrefabs.Count;

        DoSpawnEnemy(SpawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, EnemyPrefabs.Count));
    }

    private void DoSpawnEnemy(int SpawnIndex)
    {

        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();

            int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, -1))
            {
                enemy.Agent.Warp(Hit.position);

                enemy.Movement.player = Player;
                enemy.Agent.enabled = true;
                enemy.Movement.StartChasing();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {Triangulation.vertices[VertexIndex]}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
        }
    }

    // Implement your logic here to decide when monsters can spawn
    private void CheckSpawnCondition()
    {
        // Example condition: Allow spawning only if the player is within a certain distance
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        canSpawn = distanceToPlayer < 20f; // Adjust the distance as needed
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random
        // Diðer Spawnlamalar
    }
}