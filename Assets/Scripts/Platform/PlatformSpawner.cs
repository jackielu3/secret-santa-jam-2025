using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public static PlatformSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform playerTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void SpawnFromPlatform(RhythmPlatform origin, Transform player)
    {
        if (origin == null || platformPrefab == null)
        {
            Debug.LogWarning("PlatformSpawner: Missing origin or prefab.");
            return;
        }

        PlatformBehavior behavior = origin.Behavior;
        if (behavior == null)
        {
            Debug.LogWarning("RhythmPlatform has no behavior.");
            return;
        }

        StartCoroutine(SpawnRoutine(origin.transform, player, behavior));
    }

    private IEnumerator SpawnRoutine(Transform origin, Transform player, PlatformBehavior behavior)
    {
        int count = Mathf.Max(1, behavior.platformsToSpawn);



        yield break;
    }

}
