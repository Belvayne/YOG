using UnityEngine;

public class SpawnDeadEnemy : MonoBehaviour
{
    [Header("Prefab to spawn every second")]
    public GameObject prefabToSpawn;

    [Header("Spawn interval in seconds")]
    public float spawnInterval = 1f;

    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, transform.position, transform.rotation);
            }
            timer = 0f;
        }
    }
}
