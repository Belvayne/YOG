using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Group Spawn Settings")]
    [SerializeField] private int initialGroupSize = 1;
    [SerializeField] private int maxGroupSize = 10;
    [SerializeField] private float groupSizeIncreaseInterval = 30f;
    [SerializeField] private int groupSizeIncrement = 1;

    private Coroutine spawnCoroutine;
    private int currentGroupSize;
    private float elapsedTime;

    void Start()
    {
        currentGroupSize = initialGroupSize;
        elapsedTime = 0f;

        if (spawnOnStart)
        {
            StartSpawning();
        }
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
            SpawnEnemyGroups();
            yield return new WaitForSeconds(spawnInterval);

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
                int prefabIndex = Random.Range(0, enemyPrefabs.Length);
                
                // Add slight random offset to prevent enemies from spawning exactly on top of each other
                Vector3 spawnPosition = spawnPoint.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0f,
                    Random.Range(-0.5f, 0.5f)
                );

                Instantiate(enemyPrefabs[prefabIndex], spawnPosition, spawnPoint.rotation);
            }
        }
    }
}
