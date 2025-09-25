using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public bool spawnOnStart = true;
    public bool continuousSpawning = true;
    
    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    public bool useRandomSpawnPoints = true;
    public float spawnRadius = 5f;
    public LayerMask groundLayer = 1;
    
    [Header("Spawn Area")]
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);
    public bool useSpawnArea = false;
    
    [Header("Enemy Settings")]
    public bool randomizeEnemyRotation = true;
    public float enemyYOffset = 0f;
    public bool addMovementScript = true;
    public bool useNavMesh = false;
    public float enemyMoveSpeed = 3f;
    public float enemyDetectionRange = 10f;
    public float enemyAttackRange = 2f;
    
    [Header("Wave Settings")]
    public bool useWaves = false;
    public int enemiesPerWave = 5;
    public float waveDelay = 5f;
    public int currentWave = 0;
    public int maxWaves = 10;
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnEnemySpawned;
    public UnityEngine.Events.UnityEvent OnWaveComplete;
    public UnityEngine.Events.UnityEvent OnAllWavesComplete;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private bool isSpawning = false;
    private int enemiesSpawnedThisWave = 0;
    
    void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }
    
    public void StartSpawning()
    {
        if (isSpawning) return;
        
        isSpawning = true;
        
        if (useWaves)
        {
            spawnCoroutine = StartCoroutine(SpawnWaves());
        }
        else
        {
            spawnCoroutine = StartCoroutine(ContinuousSpawning());
        }
        
        Debug.Log("Enemy spawning started!");
    }
    
    public void StopSpawning()
    {
        if (!isSpawning) return;
        
        isSpawning = false;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        Debug.Log("Enemy spawning stopped!");
    }
    
    IEnumerator ContinuousSpawning()
    {
        while (isSpawning)
        {
            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    IEnumerator SpawnWaves()
    {
        while (isSpawning && currentWave < maxWaves)
        {
            currentWave++;
            enemiesSpawnedThisWave = 0;
            
            Debug.Log($"Starting wave {currentWave}/{maxWaves}");
            
            // Spawn enemies for this wave
            while (enemiesSpawnedThisWave < enemiesPerWave && isSpawning)
            {
                if (activeEnemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                    enemiesSpawnedThisWave++;
                }
                
                yield return new WaitForSeconds(spawnInterval);
            }
            
            // Wait for wave delay
            yield return new WaitForSeconds(waveDelay);
        }
        
        if (currentWave >= maxWaves)
        {
            OnAllWavesComplete?.Invoke();
            Debug.Log("All waves completed!");
        }
    }
    
    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("No enemy prefab assigned!");
            return;
        }
        
        Vector3 spawnPosition = GetSpawnPosition();
        Quaternion spawnRotation = GetSpawnRotation();
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        
        // Add to active enemies list
        activeEnemies.Add(enemy);
        
        // Set up enemy death tracking
        SetupEnemyTracking(enemy);
        
        // Add movement script if enabled
        if (addMovementScript)
        {
            SetupEnemyMovement(enemy);
        }
        
        // Invoke spawn event
        OnEnemySpawned?.Invoke();
        
        Debug.Log($"Enemy spawned at {spawnPosition}. Active enemies: {activeEnemies.Count}");
    }
    
    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPos;
        
        if (spawnPoints.Length > 0 && useRandomSpawnPoints)
        {
            // Use random spawn point
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = randomPoint.position;
        }
        else if (useSpawnArea)
        {
            // Use spawn area
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                0f,
                Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );
            spawnPos = spawnAreaCenter + randomOffset;
        }
        else
        {
            // Use spawner position with radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            spawnPos = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
        
        // Add Y offset
        spawnPos.y += enemyYOffset;
        
        // Raycast to ground if ground layer is specified
        if (groundLayer != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out hit, 20f, groundLayer))
            {
                spawnPos.y = hit.point.y + enemyYOffset;
            }
        }
        
        return spawnPos;
    }
    
    Quaternion GetSpawnRotation()
    {
        if (randomizeEnemyRotation)
        {
            return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        else
        {
            return transform.rotation;
        }
    }
    
    void SetupEnemyTracking(GameObject enemy)
    {
        // Add death tracking to enemy
        SimpleEnemyHealth health = enemy.GetComponent<SimpleEnemyHealth>();
        if (health != null)
        {
            health.OnDeath.AddListener(() => OnEnemyDeath(enemy));
        }
        
        // Also check for regular Health component
        Health regularHealth = enemy.GetComponent<Health>();
        if (regularHealth != null)
        {
            regularHealth.OnDeath.AddListener(() => OnEnemyDeath(enemy));
        }
    }
    
    void OnEnemyDeath(GameObject enemy)
    {
        // Remove from active enemies list
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        
        // Notify kill counter
        KillCounter killCounter = FindObjectOfType<KillCounter>();
        if (killCounter != null)
        {
            killCounter.AddKill();
        }
        
        Debug.Log($"Enemy died. Active enemies: {activeEnemies.Count}");
    }
    
    void SetupEnemyMovement(GameObject enemy)
    {
        if (useNavMesh)
        {
            // Add NavMesh-based AI
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI == null)
            {
                enemyAI = enemy.AddComponent<EnemyAI>();
            }
            
            // Configure AI settings
            enemyAI.moveSpeed = enemyMoveSpeed;
            enemyAI.detectionRange = enemyDetectionRange;
            enemyAI.attackRange = enemyAttackRange;
            enemyAI.useNavMesh = true;
            enemyAI.alwaysChase = true;
        }
        else
        {
            // Add simple movement
            SimpleEnemyMovement movement = enemy.GetComponent<SimpleEnemyMovement>();
            if (movement == null)
            {
                movement = enemy.AddComponent<SimpleEnemyMovement>();
            }
            
            // Configure movement settings
            movement.moveSpeed = enemyMoveSpeed;
            movement.detectionRange = enemyDetectionRange;
            movement.attackRange = enemyAttackRange;
            movement.alwaysChase = true;
        }
        
        Debug.Log("Enemy movement script added and configured");
    }
    
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
        
        Debug.Log("All enemies cleared!");
    }
    
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }
    
    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }
    
    public void SetEnemyPrefab(GameObject prefab)
    {
        enemyPrefab = prefab;
    }
    
    // Public getters
    public int GetActiveEnemyCount() => activeEnemies.Count;
    public int GetCurrentWave() => currentWave;
    public int GetMaxWaves() => maxWaves;
    public bool IsSpawning() => isSpawning;
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        if (useSpawnArea)
        {
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
        
        // Draw spawn points
        Gizmos.color = Color.red;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 1f);
            }
        }
    }
}
