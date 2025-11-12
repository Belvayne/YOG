using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int maxActiveEnemies = 10;

    [Header("Group Spawn Settings")]
    [SerializeField] private int initialGroupSize = 1;
    [SerializeField] private int maxGroupSize = 6;
    [SerializeField] private float groupSizeIncreaseInterval = 60f;
    [SerializeField] private int groupSizeIncrement = 1;

    private Coroutine spawnCoroutine;
    private int currentGroupSize;
    private float elapsedTime;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        currentGroupSize = initialGroupSize;
        elapsedTime = 0f;

        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        // Clean up null references from destroyed enemies
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemyGroups();

            elapsedTime += spawnInterval;
            if (elapsedTime >= groupSizeIncreaseInterval)
            {
                elapsedTime = 0f;
                currentGroupSize = Mathf.Min(currentGroupSize + groupSizeIncrement, maxGroupSize);
            }
        }
    }

    private void SpawnEnemyGroups()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            return;

        // Spawn a group at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < currentGroupSize; i++)
            {
                // Check if we've reached the maximum enemy limit
                if (activeEnemies.Count >= maxActiveEnemies)
                {
                    Debug.LogWarning($"EnemySpawner: Maximum enemy limit ({maxActiveEnemies}) reached. Skipping spawn.");
                    return;
                }

                int prefabIndex = Random.Range(0, enemyPrefabs.Length);
                
                // Add slight random offset to prevent enemies from spawning exactly on top of each other
                Vector3 spawnPosition = spawnPoint.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0f,
                    Random.Range(-0.5f, 0.5f)
                );

                GameObject spawnedEnemy = Instantiate(enemyPrefabs[prefabIndex], spawnPosition, spawnPoint.rotation);
                activeEnemies.Add(spawnedEnemy);
            }
        }
    }

    // Public method to get the current number of active enemies
    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    // Public method to manually remove an enemy from tracking (if needed)
    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    void OnDestroy()
    {
        // Clean up the list when the spawner is destroyed
        activeEnemies.Clear();
    }
}
