using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnDeadSukonbu : EditorWindow
{
    private GameObject sukobuPrefab;
    private int count = 30;
    private float spacing = 2f;

    // New tuning fields
    private float maxHillHeight = 4f;
    private float positionJitter = 0.15f;
    private double staggerInterval = 0.03; // seconds between enabling each ragdoll

    // Scheduler storage
    private class ScheduledEntry
    {
        public double dueTime;
        public GameObject instance;
        public bool hasActivator;
    }
    private static List<ScheduledEntry> scheduled = new List<ScheduledEntry>();
    private static bool scheduledHooked = false;

    [MenuItem("Tools/Spawn Dead Sukonbu")]
    public static void ShowWindow()
    {
        GetWindow<SpawnDeadSukonbu>("Spawn Dead Sukonbu");
    }

    void OnGUI()
    {
        GUILayout.Label("Spawn Dead Sukonbu for Screenshot", EditorStyles.boldLabel);

        sukobuPrefab = (GameObject)EditorGUILayout.ObjectField("Sukonbu Prefab", sukobuPrefab, typeof(GameObject), false);
        count = EditorGUILayout.IntField("Count", count);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        maxHillHeight = EditorGUILayout.FloatField("Max Hill Height", maxHillHeight);
        positionJitter = EditorGUILayout.FloatField("Position Jitter", positionJitter);
        staggerInterval = EditorGUILayout.DoubleField("Stagger Interval (s)", staggerInterval);

        if (GUILayout.Button("Spawn Dead Sukonbu"))
        {
            SpawnDeadSukonbuInstances();
        }

        if (GUILayout.Button("Clear All Dead Sukonbu"))
        {
            ClearAll();
        }
    }

    void SpawnDeadSukonbuInstances()
    {
        if (sukobuPrefab == null)
        {
            Debug.LogError("Please select a Sukonbu prefab!");
            return;
        }

        // Clear existing
        ClearAll();

        // Create parent
        GameObject parent = new GameObject("__DeadSukonbuGroup");

        // Grid dimensions
        int rows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int cols = rows;

        // Compute center and max distance for hill shaping
        Vector2 center = new Vector2((cols - 1) * 0.5f, (rows - 1) * 0.5f);
        float maxDist = Vector2.Distance(Vector2.zero, new Vector2(center.x, center.y));

        // Instantiate all with colliders off / kinematic to avoid instant physics resolution
        var spawnList = new List<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            Vector2 gridPos = new Vector2(col, row);
            float dist = Vector2.Distance(gridPos, center);

            // Height falloff: closer to center -> higher
            float t = (maxDist > 0f) ? (1f - (dist / maxDist)) : 1f;
            t = Mathf.Clamp01(t);
            // Smooth curve
            float height = maxHillHeight * (t * t);

            // Base position in world
            Vector3 pos = new Vector3(
                (col - center.x) * spacing + UnityEngine.Random.Range(-positionJitter, positionJitter),
                height,
                (row - center.y) * spacing + UnityEngine.Random.Range(-positionJitter, positionJitter)
            );

            GameObject sukobu = (GameObject)PrefabUtility.InstantiatePrefab(sukobuPrefab);
            sukobu.transform.position = pos;
            sukobu.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            sukobu.transform.SetParent(parent.transform);

            // Prepare physics: disable colliders and make rigidbodies kinematic so no solver impulses
            var colliders = sukobu.GetComponentsInChildren<Collider>();
            foreach (var colr in colliders)
            {
                colr.enabled = false;
            }

            var rbs = sukobu.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                // reset velocity if any
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Disable animator so it won't fight ragdoll activation
            var animator = sukobu.GetComponentInChildren<Animator>();
            if (animator != null)
                animator.enabled = false;

            spawnList.Add(sukobu);
        }

        // Schedule ragdoll enabling in a staggered fashion (so collisions resolve smoothly)
        double now = EditorApplication.timeSinceStartup;
        scheduled.Clear();
        for (int i = 0; i < spawnList.Count; i++)
        {
            var instance = spawnList[i];
            var activator = instance.GetComponentInChildren<RagdollActivator>();
            var entry = new ScheduledEntry
            {
                dueTime = now + i * staggerInterval,
                instance = instance,
                hasActivator = (activator != null)
            };
            scheduled.Add(entry);
        }
        HookSchedulerIfNeeded();

        Selection.activeGameObject = parent;
        Debug.Log($"Spawned {count} dead Sukonbu (mound placement, staggered ragdoll activation)");
    }

    void ClearAll()
    {
        GameObject group = GameObject.Find("__DeadSukonbuGroup");
        if (group != null)
        {
            DestroyImmediate(group);
        }

        // Clear scheduler if any
        scheduled.Clear();
        UnhookSchedulerIfNeeded();
    }

    static void HookSchedulerIfNeeded()
    {
        if (scheduledHooked) return;
        scheduledHooked = true;
        EditorApplication.update += ScheduledUpdate;
    }

    static void UnhookSchedulerIfNeeded()
    {
        if (!scheduledHooked) return;
        scheduledHooked = false;
        EditorApplication.update -= ScheduledUpdate;
    }

    static void ScheduledUpdate()
    {
        if (scheduled.Count == 0)
        {
            UnhookSchedulerIfNeeded();
            return;
        }

        double now = EditorApplication.timeSinceStartup;
        for (int i = scheduled.Count - 1; i >= 0; i--)
        {
            var entry = scheduled[i];
            if (now >= entry.dueTime)
            {
                EnableRagdollNow(entry.instance, entry.hasActivator);
                scheduled.RemoveAt(i);
            }
        }
    }

    static void EnableRagdollNow(GameObject instance, bool hasActivator)
    {
        if (instance == null) return;

        if (hasActivator)
        {
            var activator = instance.GetComponentInChildren<RagdollActivator>();
            if (activator != null)
            {
                try
                {
                    activator.SetRagdoll(true);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"RagdollActivator.SetRagdoll threw: {ex.Message}");
                    // fallback manual enable below
                    EnableRagdollManual(instance);
                }
            }
            else
            {
                EnableRagdollManual(instance);
            }
        }
        else
        {
            EnableRagdollManual(instance);
        }
    }

    static void EnableRagdollManual(GameObject instance)
    {
        if (instance == null) return;

        // Re-enable colliders and non-kinematic rigidbodies
        var colliders = instance.GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
        {
            c.enabled = true;
        }

        var rbs = instance.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // If there's an EnemyController that handles death animation/state, call Die() if present.
        var controller = instance.GetComponentInChildren<EnemyController>();
        if (controller != null)
        {
            // Make sure the controller's Die will not re-enable animator in a way that conflicts.
            try
            {
                controller.Die();
            }
            catch { /* ignore errors from editor-time calls */ }
        }
    }
}

