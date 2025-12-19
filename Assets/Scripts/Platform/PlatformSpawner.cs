using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public static PlatformSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject platformPrefab;

    [Header("Dotted Links")]
    [SerializeField] private Transform dotPrefab;
    [SerializeField] private float dottedLineWidth = 0.01f;
    [SerializeField] private float dottedSpacingMultiplier = 5f;


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

        StartCoroutine(SpawnRoutine(origin.transform, behavior));
    }

    private IEnumerator SpawnRoutine(Transform origin, PlatformBehavior behavior)
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
            Vector3 offset = ComputeOffsetForIndex(i, behavior, lastAnchor);
            Vector3 spawnPos = lastAnchor.position + offset;

            Vector3 away = (spawnPos - lastAnchor.position);
            if (away.sqrMagnitude < 1e-6f) away = lastAnchor.forward;
            away.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(away, Vector3.up);
            Vector3 euler = lookRot.eulerAngles;
            euler = Vector3.Scale(euler, behavior.rotationMult);
            Quaternion rotation = Quaternion.Euler(euler);

            GameObject newPlatObj = Instantiate(platformPrefab, spawnPos, rotation);
            var rhythmPlat = newPlatObj.GetComponent<RhythmPlatform>();

            PlatformBehavior next = behavior.nextBehavior ? behavior.nextBehavior : behavior;
            CreateDottedDotsLink(lastAnchor, newPlatObj.transform, next.dottedLineOffset);

            if (rhythmPlat != null)
            {
                AssignBehavior(rhythmPlat, next);
                bool isLast = (i == count - 1);
                rhythmPlat.SetLastInChain(isLast);
            }

            lastAnchor = newPlatObj.transform;

            if (i < count - 1 && secondsBetween > 0f)
            {
                yield return new WaitForSeconds(secondsBetween);
            }
        }

        yield break;
    }

    private Vector3 ComputeOffsetForIndex(int i, PlatformBehavior behavior, Transform origin)
    {
        Vector3 forward = origin.forward;
        Vector3 right = origin.right;
        Vector3 up = Vector3.up;

        if (behavior.relativePositions != null && behavior.relativePositions.Length > 0)
        {
            Vector3 patternOffset = behavior.relativePositions[Mathf.Min(i, behavior.relativePositions.Length - 1)];
            return right * patternOffset.x + up * patternOffset.y + forward * patternOffset.z;
        }

        if (behavior.useRandom != false)
        {
            float half = behavior.spawnArcDegrees * 0.5f;
            float angle = Random.Range(-half, half);

            Vector3 baseForward = Vector3.ProjectOnPlane(origin.forward, Vector3.up).normalized;
            if (baseForward.sqrMagnitude < 1e-6f) baseForward = Vector3.forward;

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseForward;

            float yOffset = 0;
            if (behavior.maxTopOffset != 0 || behavior.maxBotOffset != 0)
            {
                yOffset = Random.Range(behavior.maxBotOffset, behavior.maxTopOffset);
            }

            return dir * behavior.spawnRadius + Vector3.up * yOffset;
        }

        // Default offset (also relative to origin rotation now)
        return forward * behavior.defaultForwardDistance
             + right * behavior.defaultSideOffset
             + up * behavior.defaultVerticalOffset;
    }

    private void AssignBehavior(RhythmPlatform platform, PlatformBehavior behavior)
    {
        platform.SetBehavior(behavior);
    }

    private void CreateDottedDotsLink(Transform from, Transform to, Vector3 offsetLocal)
    {
        if (!dotPrefab) return;

        GameObject go = new GameObject("DottedLinkDots");

        var link = go.AddComponent<DottedLinkDots>();
        link.a = from;
        link.b = to;
        link.aLocalOffset = offsetLocal;
        link.bLocalOffset = offsetLocal;

        link.dotPrefab = dotPrefab;
        link.width = dottedLineWidth;
        link.spacingMultiplier = dottedSpacingMultiplier;
    }
}
