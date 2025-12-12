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

        float secondsBetween = 0f;
        if (BeatConductor.Instance != null)
        {
            secondsBetween = behavior.beatsBetweenSpawns * BeatConductor.Instance.BeatDuration;
        }

        Transform lastAnchor = origin;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = ComputeOffsetForIndex(i, behavior, lastAnchor, player);
            Vector3 spawnPos = lastAnchor.position + offset;

            GameObject newPlatObj = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
            var rhythmPlat = newPlatObj.GetComponent<RhythmPlatform>();

            PlatformBehavior next = behavior.nextBehavior ? behavior.nextBehavior : behavior;

            if (rhythmPlat != null)
            {
                AssignBehavior(rhythmPlat, next);   
            }

            lastAnchor = newPlatObj.transform;

            if (i < count - 1 && secondsBetween > 0f)
            {
                yield return new WaitForSeconds(secondsBetween);
            }
        }

        yield break;
    }

    private Vector3 ComputeOffsetForIndex(int i, PlatformBehavior behavior, Transform origin, Transform player)
    {
        if (behavior.relativePositions != null && behavior.relativePositions.Length > 0)
        {
            Vector3 patternOffset = behavior.relativePositions[Mathf.Min(i, behavior.relativePositions.Length - 1)];
            if (behavior.usePlayerForward && player != null)
            {
                Vector3 forward = player.forward;
                Vector3 right = player.right;
                Vector3 up = Vector3.up;


                return forward * patternOffset.z
                    + right * patternOffset.x
                    + up * patternOffset.y;
            }
            else
            {
                return patternOffset;
            }
        }

        if (player != null)
        {
            Vector3 forward = player.forward;
            Vector3 right = player.right;
            Vector3 up = Vector3.up;

            return forward * behavior.defaultForwardDistance
                + right * behavior.defaultSideOffset
                + up * behavior.defaultVerticalOffset;
        }

        return Vector3.forward * behavior.defaultForwardDistance;
    }

    private void AssignBehavior(RhythmPlatform platform, PlatformBehavior behavior)
    {
        platform.SetBehavior(behavior);
    }
}
