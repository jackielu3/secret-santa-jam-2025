using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public static PlatformSpawner Instance { get; private set; }

    [Header("Dotted Links")]
    [SerializeField] private Transform dotPrefab;
    [SerializeField] private float dottedLineWidth = 0.01f;
    [SerializeField] private float dottedSpacingMultiplier = 5f;

    private struct Anchor
    {
        public Vector3 pos;
        public Quaternion rot;

        public Vector3 forward => rot * Vector3.forward;
        public Vector3 right => rot * Vector3.right;
        public Vector3 up => Vector3.up;
    }

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
        if (origin == null)
        {
            Debug.LogWarning("PlatformSpawner: Missing origin.");
            return;
        }

        PlatformBehavior behavior = origin.Behavior;
        if (behavior == null)
        {
            Debug.LogWarning("RhythmPlatform has no behavior.");
            return;
        }

        if (behavior.nextBehavior == null && behavior.possibleNext.Count < 1)
        {
            Debug.LogWarning("No Possible Next.");
            return;
        }

        StartCoroutine(SpawnRoutine(origin, behavior));
    }

    private IEnumerator SpawnRoutine(RhythmPlatform originPlatform, PlatformBehavior behavior)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameRunning) yield break;

        int count = Mathf.Max(1, behavior.platformsToSpawn);

        float secondsBetween = 0f;
        if (BeatConductor.Instance != null)
        {
            secondsBetween = behavior.beatsBetweenSpawns * BeatConductor.Instance.BeatDuration;
        }

        Anchor lastAnchor = GetAnchor(originPlatform.transform, originPlatform.Behavior);

        for (int i = 0; i < count; i++)
        {
            PlatformBehavior next = null;

            if (behavior.nextBehavior != null)
            {
                next = behavior.nextBehavior;
            }
            else if (behavior.possibleNext != null && behavior.possibleNext.Count > 0)
            {
                int randomIndex = Random.Range(0, behavior.possibleNext.Count);
                next = behavior.possibleNext[randomIndex];
            }
            else
            {
                Debug.LogWarning("No Possible Next.");
                yield break;
            }

            if (next == null || next.platformPrefab == null)
            {
                Debug.LogWarning("Chosen next behavior has no platformPrefab.");
                yield break;
            }

            GameObject nextPrefab = next.platformPrefab;

            Vector3 offset = ComputeOffsetForIndex(i, behavior, lastAnchor);
            Vector3 spawnPos = lastAnchor.pos + offset;
            Vector3 away = (spawnPos - lastAnchor.pos);

            if (away.sqrMagnitude < 1e-6f) away = lastAnchor.forward;
            away.Normalize();
            Quaternion lookRot = Quaternion.LookRotation(away, Vector3.up);
            Vector3 euler = lookRot.eulerAngles;
            euler = Vector3.Scale(euler, behavior.rotationMult);
            Quaternion rotation = Quaternion.Euler(euler);

            GameObject newPlatObj = Instantiate(nextPrefab, spawnPos, rotation);
            var rhythmPlat = newPlatObj.GetComponent<RhythmPlatform>();

            CreateDottedDotsLink(
                originPlatform.transform, originPlatform.Behavior,
                newPlatObj.transform, next
            );

            if (rhythmPlat != null)
            {
                AssignBehavior(rhythmPlat, next);
                bool isLast = (i == count - 1);
                rhythmPlat.SetLastInChain(isLast);
                LoseBarrierManager.Instance.SetNextPlatform(rhythmPlat);
            }

            lastAnchor = GetAnchor(newPlatObj.transform, next);

            if (i < count - 1 && secondsBetween > 0f)
            {
                yield return new WaitForSeconds(secondsBetween);
            }
        }

        yield break;
    }
    private Vector3 ComputeOffsetForIndex(int i, PlatformBehavior behavior, Anchor origin)
    {
        Vector3 forward = origin.forward;
        Vector3 right = origin.right;
        Vector3 up = Vector3.up;

        if (behavior.relativePositions != null && behavior.relativePositions.Length > 0)
        {
            Vector3 patternOffset = behavior.relativePositions[Mathf.Min(i, behavior.relativePositions.Length - 1)];
            return right * patternOffset.x + up * patternOffset.y + forward * patternOffset.z;
        }

        if (behavior.useRandom)
        {
            float half = behavior.spawnArcDegrees * 0.5f;
            float angle = Random.Range(-half, half);

            Vector3 baseForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            if (baseForward.sqrMagnitude < 1e-6f) baseForward = Vector3.forward;

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseForward;

            float yOffset = 0;
            if (behavior.maxTopOffset != 0 || behavior.maxBotOffset != 0)
                yOffset = Random.Range(behavior.maxBotOffset, behavior.maxTopOffset);

            return dir * behavior.spawnRadius + Vector3.up * yOffset;
        }

        return forward * behavior.defaultForwardDistance
             + right * behavior.defaultSideOffset
             + up * behavior.defaultVerticalOffset;
    }

    private void AssignBehavior(RhythmPlatform platform, PlatformBehavior behavior)
    {
        platform.SetBehavior(behavior);
    }

    private void CreateDottedDotsLink(Transform fromRoot, PlatformBehavior fromBehavior, Transform toRoot, PlatformBehavior toBehavior)
    {
        if (!dotPrefab) return;

        Vector3 fromWorld = fromRoot.TransformPoint(fromBehavior != null ? fromBehavior.spawnAnchorLocal : Vector3.zero);
        Vector3 toWorld = toRoot.TransformPoint(toBehavior != null ? toBehavior.landingAnchorLocal : Vector3.zero);

        GameObject go = new GameObject("DottedLinkDots");
        var link = go.AddComponent<DottedLinkDots>();

        link.a = fromRoot;
        link.b = toRoot;

        link.useWorldOffsets = true;
        link.aWorldOffset = fromWorld - fromRoot.position;
        link.bWorldOffset = toWorld - toRoot.position;

        link.dotPrefab = dotPrefab;
        link.width = dottedLineWidth;
        link.spacingMultiplier = dottedSpacingMultiplier;
    }

    private Anchor GetAnchor(Transform platformRoot, PlatformBehavior behavior)
    {
        Vector3 local = behavior != null ? behavior.spawnAnchorLocal : Vector3.zero;
        return new Anchor
        {
            pos = platformRoot.TransformPoint(local),
            rot = platformRoot.rotation
        };
    }
}
